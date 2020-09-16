using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
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
    //    Sia.PublicarPnt(9615,"ContabilizacionFletes");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9615,"ContabilizacionFletes");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class ContabilizacionFletes : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        decimal total_flete = 0;
        decimal total_seguro = 0;
        decimal total_desc = 0;
        decimal total_con = 0;

        public DataTable dtcuerpo = new DataTable();

        public ContabilizacionFletes()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();

            dtcuerpo.Columns.Add("n_guia");
            dtcuerpo.Columns.Add("vr_flete");
            dtcuerpo.Columns.Add("vr_seguro");
            dtcuerpo.Columns.Add("cod_cli");
            dtcuerpo.Columns.Add("cod_cco");
            dtcuerpo.Columns.Add("check", typeof(Int32));
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
                this.Title = "Contabilizacion de fletes " + cod_empresa + "-" + nomempresa;
                Tx_fecha.Text = DateTime.Now.ToString();
                Tx_fec_ven.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string query = "select cod_prv,n_guia,n_fra,vr_flete,vr_seguro,fecha,0 as 'check'   from indet_fle where cod_prv='" + TX_trans.Text + "' and fecha>='" + Tx_fecha.Text + "' ";

                //antiguo con centro de costo
                //string query = "select *,0 as 'check' From infletes where cod_prv = '" + TX_trans.Text + "' and ind_acu = '0';";
                string query = "select n_guia,vr_flete,vr_seguro,cod_cli,isnull(Comae_cco.cod_cco,'') as cod_cco,0 as 'check' ";
                query += "From infletes ";
                query += "left join Comae_ter on Comae_ter.cod_ter = infletes.cod_cli ";
                query += "left join InMae_mer on Comae_ter.cod_ven = InMae_mer.cod_mer ";
                query += "left join Comae_cco on InMae_mer.cod_cco = Comae_cco.cod_cco ";
                query += "where cod_prv = '" + TX_trans.Text + "' and ind_acu = '0'; ";


                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                if (dt.Rows.Count > 0)
                {
                    //dataGrid1.ItemsSource = dt.DefaultView;
                    if (dtcuerpo.Rows.Count > 0) dtcuerpo.Clear();

                    foreach (System.Data.DataRow item in dt.Rows)
                    {
                        dtcuerpo.Rows.Add
                            (
                              item["n_guia"].ToString(),
                              item["vr_flete"].ToString(),
                              item["vr_seguro"].ToString(),
                              item["cod_cli"].ToString(),
                              item["cod_cco"].ToString(),
                               Convert.ToInt32(item["check"])
                            );
                    }

                    dataGrid1.ItemsSource = dtcuerpo.DefaultView;

                    TxRegis.Text = dt.Rows.Count.ToString();
                }
                else
                {
                    MessageBox.Show("no hay ningun flete registrado");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dtcuerpo.Rows.Count <= 0)
                {
                    MessageBox.Show("genere una consulta", "aleta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                bool flag = false;
                foreach (System.Data.DataRow item in dtcuerpo.Rows)
                {
                    int che = Convert.ToInt32(item["check"]);
                    if (che == 1) flag = true;                    
                }

                if (flag == false)
                {
                    MessageBox.Show("seleccione por lo menos una guia", "aleta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (MessageBox.Show("Usted desea guardar el documento..?", "Guardar Documento", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int id = DocuContable();
                    if (id > 0)
                    {
                        MessageBox.Show("documento contable generado exitosamente");
                        SiaWin.TabTrn(0, idemp, true, id, 1, WinModal: true);
                        dataGrid1.ItemsSource = null;
                        dtcuerpo.Clear();
                        TX_trans.Text = "";
                        TX_transName.Text = "";
                        Tx_factura.Text = "";
                        decimal val = 0;
                        Tx_fletes.Text = val.ToString("C");
                        Tx_seguros.Text = val.ToString("C");
                        Tx_totFact.Text = val.ToString("C");
                        TxRegis.Text = "0";
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar:" + w);
            }
        }

        public string GetIndCons()
        {
            System.Data.DataTable dt = SiaWin.Func.SqlDT("select ind_con from Comae_trn where cod_trn='05A'", "tabla", idemp);
            return dt.Rows.Count > 0 ? dt.Rows[0]["ind_con"].ToString().Trim() : "";
        }

        public int DocuContable()
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

                try
                {

                    string ind_con = GetIndCons().Trim();
                    string sqlConsecutivo = " ";

                    if (ind_con == "3")
                    {
                        DateTime fecha = Convert.ToDateTime(Tx_fecha.Text);
                        sqlConsecutivo += "declare @ini  char(8);declare @lonnum as int = 12;declare @iFolioHost int = 0;declare @iConsecutivo char(12) = '';";
                        sqlConsecutivo += "SELECT @iFolioHost = num_" + fecha.ToString("MM") + "+1,@lonnum=lon_num, ";
                        sqlConsecutivo += "@ini=rtrim(inicial)+'" + fecha.ToString("MM") + fecha.ToString("yyyy").Substring(2, 2) + "' FROM comae_trn  WHERE cod_trn = '05A'; ";
                        sqlConsecutivo += "UPDATE Comae_trn SET num_" + fecha.ToString("MM") + "= ISNULL(num_" + fecha.ToString("MM") + ", 0) + 1  WHERE cod_trn='05A';";
                        sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+REPLICATE('0', iif(@lonnum<3,12,@lonnum) - len(rtrim(@ini)) - len(rtrim(convert(varchar, @iFolioHost)))) + rtrim(convert(varchar, @iFolioHost)); ";
                    }
                    else
                    {
                        sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12) = '' ;";
                        sqlConsecutivo += "declare @iFolioHost int = 0;";
                        sqlConsecutivo += "UPDATE Comae_trn SET num_act= ISNULL(num_act, 0) + 1  WHERE cod_trn='05A';";
                        sqlConsecutivo += "SELECT @iFolioHost = num_act,@ini=rtrim(inicial) FROM Comae_trn  WHERE cod_trn='05A';";
                        sqlConsecutivo += "set @num=@iFolioHost;";
                        sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));";
                    }



                    string sqlcab = sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,fec_trn,num_trn,detalle,fec_ven) values ('05A','" + Tx_fecha.Text + "',@iConsecutivo,'Contabilizacion de Flete','" + Tx_fec_ven.Text + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                    string sql = "";

                    decimal tot_flete = 0;

                    foreach (System.Data.DataRow item in dtcuerpo.Rows)
                    {
                        int che = Convert.ToInt32(item["check"]);

                        if (che == 1)
                        {
                            string guia = item["n_guia"].ToString().Trim();
                            decimal fle = Convert.ToDecimal(item["vr_flete"]);
                            decimal seg = Convert.ToDecimal(item["vr_seguro"]);
                            string cod_cco = item["cod_cco"].ToString().Trim();
                            //string guiadesmov = item["n_guia"].ToString().Trim();

                            decimal suma = fle + seg;
                            tot_flete += suma;
                            sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,doc_ref,doc_cruc,bas_mov,deb_mov,cre_mov) values (@NewID,'05A',@iConsecutivo,'523550','" + TX_trans.Text.Trim() + "','" + cod_cco + "','GUIA: " + guia + "',@iConsecutivo,'',0," + suma.ToString("F", CultureInfo.InvariantCulture) + ",0);";

                            //actualizacion
                            sql = sql + "update infletes set n_cau ='" + Tx_factura.Text + "',ind_acu='1' where cod_prv = '" + TX_trans.Text + "' and n_guia = '" + guia + "';";
                        }
                    }

                    //contra
                    sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,doc_cruc,bas_mov,deb_mov,cre_mov) values (@NewID,'05A',@iConsecutivo,'233545','" + TX_trans.Text.Trim() + "','N FACT:" + Tx_factura.Text + "','',0,0," + tot_flete.ToString("F", CultureInfo.InvariantCulture) + ");";


                    command.CommandText = sqlcab + sql + @"select CAST(@NewId AS int);";
                    //MessageBox.Show("Documento Contable Generado:"+command.CommandText);
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    int id = Convert.ToInt32(r.ToString());
                    return id;
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                    return -1;
                }
                catch (Exception ex)
                {
                    errorMessages.Append("Error:" + ex.StackTrace + "-" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                    return -1;
                }
            }
        }


        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F8 || e.Key == Key.Enter)
            {
                try
                {
                    int idr = 0; string code = ""; string nombre = "";
                    dynamic xx = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "nom_ter", "idrow", "Maestra de clientes", cnEmp, false, "", idEmp: idemp);
                    xx.ShowInTaskbar = false;
                    xx.Owner = Application.Current.MainWindow;
                    xx.Height = 400;
                    xx.ShowDialog();
                    idr = xx.IdRowReturn;
                    code = xx.Codigo;
                    nombre = xx.Nombre;
                    xx = null;
                    if (idr > 0)
                    {
                        if ((sender as TextBox).Name == "TX_trans")
                        {
                            TX_trans.Text = code;
                            TX_transName.Text = nombre;
                        }
                    }
                }
                catch (Exception w)
                {
                    MessageBox.Show("error al buscar:" + w);
                }
            }
        }

        private void TX_trans_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text)) return;

            string query = "select * from comae_ter where cod_ter='" + (sender as TextBox).Text.Trim() + "'; ";

            System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);

            if (dt.Rows.Count > 0)
            {
                (sender as TextBox).Text = dt.Rows[0]["cod_ter"].ToString();

                if ((sender as TextBox).Name == "TX_trans")
                    TX_transName.Text = dt.Rows[0]["nom_ter"].ToString().Trim();
            }
            else
            {
                MessageBox.Show("el tercero ingresado no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                (sender as TextBox).Text = "";
                TX_transName.Text = "";
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }





        private void dataGrid1_CurrentCellValueChanged(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellValueChangedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGrid1.SelectedItems[0];
                int id = Convert.ToInt32(row["check"]);
                decimal flete = Convert.ToDecimal(row["vr_flete"]);
                decimal seguro = Convert.ToDecimal(row["vr_seguro"]);

                if (id == 1)
                {
                    total_flete += flete;
                    total_seguro += seguro;
                }
                else
                {
                    total_flete -= flete;
                    total_seguro -= seguro;
                }

                Tx_fletes.Text = total_flete.ToString("C");
                Tx_seguros.Text = total_seguro.ToString("C");

                Tx_totFact.Text = (total_flete + total_seguro).ToString("C");

            }
            catch (Exception)
            {

                throw;
            }
        }








    }
}

