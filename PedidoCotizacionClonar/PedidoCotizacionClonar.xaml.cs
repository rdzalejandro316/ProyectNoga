using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9697,"PedidoCotizacionClonar");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9697,"PedidoCotizacionClonar");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();    

    public partial class PedidoCotizacionClonar : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        DataTable dt_cab = new DataTable();
        DataTable dt_cue = new DataTable();

        public PedidoCotizacionClonar()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId;          
        }
   

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                if (idemp <= 0) idemp = SiaWin._BusinessId;
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Clonar Pedido-Cotizacion -" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int index = (sender as ComboBox).SelectedIndex;

                switch (index)
                {
                    case 0: tx_tipo.Text = "Pedido"; Tx_Documento.Text = ""; break;
                    case 1: tx_tipo.Text = "Cotizacion"; Tx_Documento.Text = ""; break;
                }

                if (index >= 0)
                {
                    if (dataGridCue.View.Records.Count > 0) dataGridCue.ItemsSource = null;
                    if (dataGridCab.View.Records.Count > 0) dataGridCab.ItemsSource = null;
                    TxRegistro.Text = "0";
                    Tx_Documento.Text = "";
                }

            }
            catch (Exception w)
            {
                //MessageBox.Show("error en el combobox:" + w);
            }
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt_cab.Rows.Count <= 0 || dt_cue.Rows.Count <= 0)
                {
                    MessageBox.Show("no hay ninguno documento para clonar por favor ingrese el documento", "alert", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                string documnt = CbTipo.SelectedIndex == 0 ? "COTIZACION" : "PEDIDO";
                if (MessageBox.Show("Usted desea clonar el documento " + Tx_Documento.Text.Trim() + " a " + documnt + " ", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    int id = GenerarDocumento();
                    if (id > 0) SiaWin.TabTrn(0, idemp, true, id, 2, WinModal: true);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar:" + w);
            }
        }

        public void asa()
        {
            string idreg = "";
            string num_trn = "";

            if (string.IsNullOrEmpty(idreg))
            {
                MessageBox.Show("debe de seleccionar un documento ya creado");
            }
            else
            {                                
                DataTable dt_info = ((Inicio)Application.Current.MainWindow).Func.SqlDT("select * from incab_doc where num_trn='" + num_trn + "' ", "cabeza", _trn.BusinessId);
                if (dt_info.Rows.Count>0)
                {
                    int id = dt_info.Rows[0]["idreg"];
                    if (id > 0) SiaWin.TabTrn(0, idemp, true, id, 2, WinModal: true);
                }
            }
        }

        public int GenerarDocumento()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cnEmp))
                {
                    connection.Open();
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;
                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;


                    string TipoConsecutivo = CbTipo.SelectedIndex == 0 ? "cotizaciones" : "pedidos";
                    string codtrn = CbTipo.SelectedIndex == 0 ? "011" : "505";


                    string codpvta = dt_cab.Rows[0]["bod_tra"].ToString();
                    string cod_p = "";
                    switch (codpvta)
                    {
                        case "011": cod_p = "010"; break;
                        case "014": cod_p = "012"; break;
                        case "015": cod_p = "050"; break;
                        case "081": cod_p = "080"; break;
                        default: cod_p = codpvta; break;
                    }



                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                    sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12) = '' ; declare @iFolioHost int = 0;";
                    sqlConsecutivo += "UPDATE COpventas SET " + TipoConsecutivo + " = ISNULL(" + TipoConsecutivo + ", 0) + 1  WHERE cod_pvt='" + cod_p + "';";
                    sqlConsecutivo += "declare  @nomcmp as char(12)='" + TipoConsecutivo + "';SELECT @iFolioHost = " + TipoConsecutivo + ",@ini=CASE @nomcmp  WHEN 'fac_contado' THEN inicial   WHEN 'fac_credito' THEN ini_cred WHEN 'nc_fe' THEN ini_ncfe  ELSE '" + cod_p + "'   END  FROM Copventas  WHERE cod_pvt='" + cod_p + "';";
                    sqlConsecutivo += "set @num=@iFolioHost;";
                    sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+rtrim(convert(varchar,@num));";

                    string des_mov = dt_cab.Rows[0]["des_mov"].ToString();


                    DateTime dat;

                    string cod_cli = dt_cab.Rows[0]["cod_cli"].ToString();
                    string cod_ven = dt_cab.Rows[0]["cod_ven"].ToString();
                    string dia_pla = dt_cab.Rows[0]["dia_pla"].ToString();
                    DateTime fec_ven = Convert.ToDateTime(
                        dt_cab.Rows[0]["fec_ven"] == DBNull.Value || DateTime.TryParse(dt_cab.Rows[0]["fec_ven"].ToString(), out dat) == false ?
                        DateTime.Now : dt_cab.Rows[0]["fec_ven"]);
                    string suc_cli = dt_cab.Rows[0]["suc_cli"].ToString();
                    string cod_cco = dt_cab.Rows[0]["cod_cco"].ToString();

                    string sqlcab = sqlConsecutivo + @"INSERT INTO incab_doc (cod_trn,fec_trn,cod_cli,suc_cli,cod_ven,num_trn,doc_ref,dia_pla,fec_ven,cod_cco,des_mov,bod_tra,UserId) values ('" + codtrn + "',@fecdoc,'" + cod_cli + "','" + suc_cli + "','" + cod_ven + "',@iConsecutivo,@iConsecutivo," + dia_pla + ",'" + fec_ven.ToString("dd/MM/yyyy") + "', '" + cod_cco + "','" + des_mov + "','" + codpvta + "'," + SiaWin._UserId + ");DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                    string sqlcue = "";
                    foreach (DataRow dr in dt_cue.Rows)
                    {
                        string cod_ref = dr["cod_ref"].ToString();
                        string cod_bod = dr["cod_bod"].ToString();
                        decimal cantidad = Convert.ToDecimal(dr["cantidad"].ToString());
                        decimal val_uni = Convert.ToDecimal(dr["val_uni"].ToString());
                        decimal subtotal = Convert.ToDecimal(dr["subtotal"].ToString());
                        decimal por_des = Convert.ToDecimal(dr["por_des"].ToString());
                        decimal val_des = Convert.ToDecimal(dr["val_des"].ToString());
                        decimal por_iva = Convert.ToDecimal(dr["por_iva"].ToString());
                        string cod_tiva = dr["cod_tiva"].ToString();
                        decimal val_iva = Convert.ToDecimal(dr["val_iva"].ToString());
                        decimal tot_tot = Convert.ToDecimal(dr["tot_tot"].ToString());
                        string cod_sub = dr["cod_sub"].ToString();
                        decimal val_ica = Convert.ToDecimal(dr["val_ica"].ToString());
                        decimal val_ret = Convert.ToDecimal(dr["val_ret"].ToString());
                        decimal val_riva = Convert.ToDecimal(dr["val_riva"].ToString());
                        decimal por_ica = Convert.ToDecimal(dr["por_ica"].ToString());
                        decimal por_ret = Convert.ToDecimal(dr["por_ret"].ToString());
                        decimal por_riva = Convert.ToDecimal(dr["por_riva"].ToString());

                        sqlcue += @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cantidad,val_uni,subtotal,por_des,val_des,por_iva,cod_tiva,val_iva,tot_tot,cod_sub,val_ica,val_ret,val_riva,por_ica,por_ret,por_riva) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cod_ref + "','" + cod_bod + "'," + cantidad.ToString("F", CultureInfo.InvariantCulture) + "," + val_uni.ToString("F", CultureInfo.InvariantCulture) + "," + subtotal.ToString("F", CultureInfo.InvariantCulture) + "," + por_des.ToString("F", CultureInfo.InvariantCulture) + "," + val_des.ToString("F", CultureInfo.InvariantCulture) + "," + por_iva.ToString("F", CultureInfo.InvariantCulture) + ",'" + cod_tiva.ToString() + "'," + val_iva.ToString("F", CultureInfo.InvariantCulture) + "," + tot_tot.ToString("F", CultureInfo.InvariantCulture) + ",'" + cod_sub + "'," + val_ica.ToString("F", CultureInfo.InvariantCulture) + "," + val_ret.ToString("F", CultureInfo.InvariantCulture) + "," + val_riva.ToString("F", CultureInfo.InvariantCulture) + "," + por_ica.ToString("F", CultureInfo.InvariantCulture) + "," + por_ret.ToString("F", CultureInfo.InvariantCulture) + "," + por_riva.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }

                    //MessageBox.Show("query:" + sqlcab + sqlcue);
                    command.CommandText = sqlcab + sqlcue + @"select CAST(@NewId AS int);";
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    return Convert.ToInt32(r.ToString());
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar documento:" + w);
                return 0;
            }
        }



        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string where = CbTipo.SelectedIndex == 0 ? " cod_trn='505'" : "cod_trn='011'";
                string title = CbTipo.SelectedIndex == 0 ? "-Pedido " : "-Cotizacion";
                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar("incab_doc", "cod_trn", "num_trn", "num_trn", "idreg", "Documentos" + title, cnEmp, false, where, idEmp: idemp);
                xx.ShowInTaskbar = false;
                xx.Owner = Application.Current.MainWindow;
                xx.Height = 300;
                xx.Width = 400;
                xx.ShowDialog();
                idr = xx.IdRowReturn;
                code = xx.Codigo;
                nombre = xx.Nombre;
                if (idr > 0)
                {
                    Tx_Documento.Text = nombre;
                    CargarDoc(idr.ToString());
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al busxar:" + w);
            }
        }

        public void CargarDoc(string idreg)
        {
            try
            {
                string query_cab = "select cab.cod_trn,cab.num_trn,cab.cod_cli,ter.nom_ter,cab.des_mov,cab.bod_tra,cab.fec_trn,cab.cod_ven,cab.dia_pla,cab.fec_ven,cab.suc_cli,cab.cod_cco,cab.UserId ";
                query_cab += "from InCab_doc cab ";
                query_cab += "inner join Comae_ter ter on ter.cod_ter = cab.cod_cli ";
                query_cab += "where idreg='" + idreg + "' ";
                dt_cab = SiaWin.Func.SqlDT(query_cab, "cabeza", idemp);
                if (dt_cab.Rows.Count > 0) dataGridCab.ItemsSource = dt_cab.DefaultView;

                string query_cue = "select cue.cod_ref,ref.nom_ref,ref.cod_ant,cue.cod_bod,cue.cantidad,cue.val_uni,cue.subtotal,cue.por_des,cue.val_des,cue.por_iva,cue.cod_tiva,cue.val_iva,cue.tot_tot,cue.cod_sub,cue.val_ica,cue.val_ret,cue.val_riva,cue.por_ica,cue.por_ret,cue.por_riva ";
                query_cue += "from InCue_doc cue ";
                query_cue += "inner join InMae_ref ref on ref.cod_ref = cue.cod_ref ";
                query_cue += "where idregcab='" + idreg + "' ";
                dt_cue = SiaWin.Func.SqlDT(query_cue, "cuerpo", idemp);
                if (dt_cue.Rows.Count > 0) dataGridCue.ItemsSource = dt_cue.DefaultView;
                TxRegistro.Text = dt_cue.Rows.Count.ToString();

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar documento:" + w);
            }
        }


    }
}

