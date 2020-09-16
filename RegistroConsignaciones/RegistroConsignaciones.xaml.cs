using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
    //    Sia.PublicarPnt(9631,"RegistroConsignaciones");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9631,"RegistroConsignaciones");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class RegistroConsignaciones : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int moduloid = 0;

        double total_Consi = 0;
        public RegistroConsignaciones()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Registro Consignaciones " + cod_empresa + "-" + nomempresa;

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='CO'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());


                //Tx_fec.Text = DateTime.Now.ToString();
                loadDate();

                TX_totcon.Text = total_Consi.ToString("C");
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        public void clean()
        {
            dataGridCons.ItemsSource = null;
            TX_totcon.Text = "0";
            Tx_cunta.Text = "";
            Tx_name.Text = "";
            Tx_rows.Text = "0";
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private async void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;
                dataGridCons.ClearFilters();
                dataGridCons.ItemsSource = null;

                string fec_con = Tx_fec.Text;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fec_con, cod_empresa, source.Token), source.Token);
                await slowTask;
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridCons.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Tx_rows.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                }
                else
                {
                    dataGridCons.ItemsSource = null;
                    Tx_rows.Text = "0";
                }


                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al consutar");
            }
        }

        private DataSet LoadData(string fechaCon, string empresa, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_Empconsignaciones", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@feconsig", fechaCon);
                cmd.Parameters.AddWithValue("@codemp", empresa);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        public void loadDate()
        {
            try
            {
                string query = "select fec_rc,fec_cons,con_cie from Co_confi";
                DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
                if (dt.Rows.Count > 0) Tx_fec.Text = dt.Rows[0]["fec_cons"].ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar las fechas");
            }
        }


        private void BtnEjecutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones

                if (Tx_rows.Text == "0")
                {
                    MessageBox.Show("fije la fecha y consulte", "alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
               
                if (string.IsNullOrEmpty(Tx_cunta.Text))
                {
                    MessageBox.Show("llene el campo de cuenta", "alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                #endregion

                int doc = Documento();

                if (doc > 0)
                {
                    SiaWin.TabTrn(0, idemp, true, doc, moduloid, WinModal: true);
                    clean();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al procesar:" + w);
            }
        }


        public int Documento()
        {
            try
            {
                int idreg = 0;

                if (MessageBox.Show("Usted desea guardar el documento..?", "Guardar Traslado", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    string codtrn = "09";
                    DateTime fechaActual = DateTime.Today;

                    using (SqlConnection connection = new SqlConnection(cnEmp))
                    {

                        connection.Open();
                        StringBuilder errorMessages = new StringBuilder();
                        SqlCommand command = connection.CreateCommand();
                        SqlTransaction transaction;
                        //Start a local transaction.
                        transaction = connection.BeginTransaction("Transaction");
                        command.Connection = connection;
                        command.Transaction = transaction;


                        string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                        sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12) = '' ;";
                        sqlConsecutivo += "declare @iFolioHost int = 0;";
                        sqlConsecutivo += "UPDATE Comae_trn SET num_act= ISNULL(num_act, 0) + 1  WHERE cod_trn='09';";
                        sqlConsecutivo += "SELECT @iFolioHost = num_act,@ini=rtrim(inicial) FROM Comae_trn  WHERE cod_trn='09';";
                        sqlConsecutivo += "set @num=@iFolioHost;";
                        sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));";


                        string sqlcab = sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn)
                        values ('" + codtrn + "',@iConsecutivo,'"+ Tx_fec.Text+ "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";


                        string sqlcue = "";
                        var reflector = this.dataGridCons.View.GetPropertyAccessProvider(); int a = 1;

                        decimal tot_con = 0;

                        foreach (var row in dataGridCons.View.Records)
                        {
                            var rowData = dataGridCons.GetRecordAtRowIndex(a);
                            bool check = Convert.ToBoolean(reflector.GetValue(rowData, "ind_consig"));

                            string cta = reflector.GetValue(rowData, "cod_cta").ToString();
                            string cter = reflector.GetValue(rowData, "cod_ter").ToString();
                            string num_trn = reflector.GetValue(rowData, "num_trn").ToString();
                            string num_chq = reflector.GetValue(rowData, "num_chq").ToString();
                            string cod_banc = reflector.GetValue(rowData, "cod_banc").ToString();

                            decimal credito = Convert.ToDecimal(reflector.GetValue(rowData, "saldo"));
                            if (check)
                            {
                                tot_con += credito;
                                sqlcue = sqlcue + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,doc_ref,doc_cruc,bas_mov,deb_mov,cre_mov,num_chq,cod_banc) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cta + "','" + cter + "','consigancion','" + num_trn + "','" + num_trn + "',0,0," + credito.ToString("F", CultureInfo.InvariantCulture) + ",'" + num_chq + "','" + cod_banc + "');";
                            }

                            a = a + 1;
                        }

                        //si no selecciono ningun cheque
                        if (string.IsNullOrEmpty(sqlcue))
                        {
                            sqlcue = sqlcue + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,des_mov,bas_mov,deb_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + Tx_cunta.Text + "','efectivo',0," + TextVlrEfc.Value + ",0);";
                        }

                        decimal efec = Convert.ToDecimal(TextVlrEfc.Value);
                        decimal total = efec + tot_con;


                        if (TextVlrEfc.Value>0)
                        {
                            sqlcue = sqlcue + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,bas_mov,deb_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'11050501','','consigancion',0,0," + TextVlrEfc.Value + ");";
                        }
                        

                        if (tot_con > 0)
                        {
                            sqlcue = sqlcue + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,bas_mov,deb_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + Tx_cunta.Text + "','','consigancion',0," + total + ",0);";
                        }



                        command.CommandText = sqlcab + sqlcue + "select CAST(@NewId AS int);";
                        //MessageBox.Show(command.CommandText.ToString());
                        var r = new object();
                        r = command.ExecuteScalar();
                        transaction.Commit();
                        connection.Close();
                        MessageBox.Show("documento generado");
                        idreg = Convert.ToInt32(r.ToString());
                    }

                    return idreg;
                }
                else
                {
                    MessageBox.Show("no se genero el Documento");
                    return 0;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el documento:" + w);
                return 0;
            }
        }

        private void dataGridCons_CurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {
            actualizaTot();
        }


        public void actualizaTot()
        {
            var reflector = this.dataGridCons.View.GetPropertyAccessProvider();
            decimal valor = 0;
            int a = 1;
            foreach (var row in dataGridCons.View.Records)
            {
                var row_col = dataGridCons.GetRecordAtRowIndex(a);
                bool flag = Convert.ToBoolean(reflector.GetValue(row_col, "ind_consig"));
                if (flag) valor += Convert.ToDecimal(reflector.GetValue(row_col, "saldo"));
                a = a + 1;
            }

            decimal val = valor + Convert.ToDecimal(TextVlrEfc.Value);
            TX_totcon.Text = val.ToString("C");
        }


        private void dataGridCons_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            if (dataGridCons.SelectedIndex >= 0)
            {
                DataRowView row = (DataRowView)dataGridCons.SelectedItems[0];
                string ter = row["nom_ter"].ToString();
                Tx_name.Text = ter;
            }
        }

        private void Tx_cunta_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter || e.Key == Key.F8)
                {
                    int idr = 0; string code = ""; string nombre = "";
                    dynamic xx = SiaWin.WindowBuscar("comae_cta", "cod_cta", "nom_cta", "nom_cta", "idrow", "Maestra de cuentass", cnEmp, false, "", idEmp: idemp);
                    xx.ShowInTaskbar = false;
                    xx.Owner = Application.Current.MainWindow;
                    xx.Height = 300;
                    xx.Width = 400;
                    xx.ShowDialog();
                    idr = xx.IdRowReturn;
                    code = xx.Codigo;
                    nombre = xx.Nombre;
                    xx = null;
                    if (idr > 0)
                    {
                        Tx_cunta.Text = code;
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar cuenta");
            }
        }

        private void TextVlrEfc_LostFocus(object sender, RoutedEventArgs e)
        {
            actualizaTot();
        }



    }
}
