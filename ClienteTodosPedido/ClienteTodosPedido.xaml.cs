using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //    Sia.PublicarPnt(9654,"ClienteTodosPedido");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9654,"ClienteTodosPedido");
    //    ww.tercero = "";
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class ClienteTodosPedido : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public string tercero = "";
        public bool flag = false;
        public DataTable dt;
        public DataTable dt_repetidas;
        public DataTable dt_temp;


        public ClienteTodosPedido()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            dt_temp = new DataTable();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            cargar();
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
                this.Title = "Pedido-Cliente" + cod_empresa + "-" + nomempresa;

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        public async void cargar()
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                sfBusyIndicator.IsBusy = true;
                dataGridPedidos.ItemsSource = null;
                source.CancelAfter(TimeSpan.FromSeconds(1));

                string ter = tercero;
                string emp = cod_empresa;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ter, emp, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dt_temp = ((DataSet)slowTask.Result).Tables[0];

                    dataGridPedidos.ItemsSource = dt_temp.DefaultView;
                    Tx_total.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    if (((DataSet)slowTask.Result).Tables[1].Rows.Count > 0)
                        tx_name.Text = ((DataSet)slowTask.Result).Tables[1].Rows[0]["nom_ter"].ToString().Trim();

                    if (((DataSet)slowTask.Result).Tables[2].Rows.Count > 0)
                    {
                        dt_repetidas = new DataTable();
                        dt_repetidas = ((DataSet)slowTask.Result).Tables[2];
                    }

                }
                else
                {
                    MessageBox.Show("No contiene pedidos pendientes");
                }


                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }

        private DataSet LoadData(string ter, string emp, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpPedidosFaltantesCliente", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codter", ter);
                cmd.Parameters.AddWithValue("@codemp", emp);
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

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridPedidos.ExportToExcel(dataGridPedidos.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
                };

                if (sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        if (sfd.FilterIndex == 1)
                            workBook.Version = ExcelVersion.Excel97to2003;
                        else if (sfd.FilterIndex == 2)
                            workBook.Version = ExcelVersion.Excel2010;
                        else
                            workBook.Version = ExcelVersion.Excel2013;
                        workBook.SaveAs(stream);
                    }

                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:" + w);
            }
        }

        private void BtnPedido_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridPedidos.SelectedItems[0];
                if (row == null) return;
                int id = Convert.ToInt32(row["idreg"]);
                SiaWin.TabTrn(0, idemp, true, id, 2, WinModal: true);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir pedido:" + w);
            }
        }


        public bool detectRepetidos()
        {
            try
            {
                bool flag = false;

                #region antetior algoritmo


                var duplicates = dt_temp.AsEnumerable()
               .Select(dr => dr.Field<string>("cod_ref"))
               .GroupBy(x => x)
               .Where(g => g.Count() > 1)
               .Select(g => g.Key)
               .ToList();

                foreach (var item in duplicates.ToList()) flag = true;



                #endregion


                return flag;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la validacion:" + w);
                return false;
            }
        }


        private void BtnFacturar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt_temp.Rows.Count > 0)
                {
                    #region tabla

                    dt = new DataTable();
                    dt.Columns.Add("idrow", typeof(int));
                    dt.Columns.Add("cod_ref");
                    dt.Columns.Add("cod_ant");
                    dt.Columns.Add("nom_prv");
                    dt.Columns.Add("p_pendiente", typeof(double));
                    dt.Columns.Add("val_uni", typeof(double));
                    dt.Columns.Add("val_iva", typeof(double));
                    dt.Columns.Add("val_ica", typeof(double));
                    dt.Columns.Add("val_ret", typeof(double));
                    dt.Columns.Add("val_riva", typeof(double));
                    dt.Columns.Add("por_iva", typeof(double));
                    dt.Columns.Add("por_ret", typeof(double));
                    dt.Columns.Add("por_ica", typeof(double));
                    dt.Columns.Add("por_riva", typeof(double));
                    dt.Columns.Add("por_des", typeof(double));
                    dt.Columns.Add("subtotal", typeof(double));
                    dt.Columns.Add("tot_tot", typeof(double));
                    dt.Columns.Add("cod_tiva");
                    dt.Columns.Add("num_trn");



                    #endregion

                    bool v = detectRepetidos();

                    if (v)
                    {
                        string message = "(SI)-'el cliente " + tx_name.Text.Trim() + " tiene referencias repetidas ustede desea anular todas las referencias repetidas y solo traer la referencia mas actuales' \n";
                        message += "(NO)- 'facturar las referencias seleccionadas' ";

                        MessageBoxResult messa = MessageBox.Show(message, "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Information);

                        if (messa == MessageBoxResult.Yes)
                        {

                            foreach (DataRow item in dt_temp.Rows) item["estado"] = false;

                            foreach (DataRow item in dt_temp.Rows)
                            {
                                string idreg = item["idreg"].ToString().Trim();
                                string cod_ref = item["cod_ref"].ToString().Trim();

                                DataRow[] result = dt_repetidas.Select("idreg='" + idreg + "' and cod_ref='" + cod_ref + "' ");

                                if (result.Length > 0)
                                {
                                    string valor = "";
                                    foreach (DataRow val in result) valor = val["idreg"].ToString().Trim();

                                    if (idreg == valor)
                                    {
                                        item["estado"] = true;
                                    }
                                }
                            }

                            if (MessageBox.Show("Usted desea anular las referencias no seleccionadas", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                            {
                                GridAnular();
                            }

                            GridLLenar();
                        }

                        if (messa == MessageBoxResult.No)
                        {
                            GridLLenar();
                        }

                        if (MessageBox.Show("Usted desea pasar a factura", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            this.Close();
                        }
                    }
                    else
                    {                        
                        GridLLenar();

                        if (MessageBox.Show("Usted desea pasar a factura", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al facturar:" + w);
            }
        }

        public void GridAnular()
        {
            string query = "";
            foreach (DataRow dr in dt_temp.Rows)
            {
                if (Convert.ToBoolean(dr["estado"]) == false)
                {
                    string idreg = dr["idreg"].ToString();
                    string cod_ref = dr["cod_ref"].ToString();
                    query += "update incue_doc set cod_anu='07',est_anu='A' where idregcab='" + idreg + "' and cod_ref='" + cod_ref + "';";
                }
            }
            if (!string.IsNullOrEmpty(query)) if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("error al actualizar"); }
        }

        public void GridLLenar()
        {
            foreach (DataRow dr in dt_temp.Rows)
            {
                if (Convert.ToBoolean(dr["estado"]) == true)
                {
                    DataRow drow = dt.NewRow();
                    drow["idrow"] = Convert.ToInt32(dr["idrow"]);
                    drow["cod_ref"] = dr["cod_ref"].ToString();
                    drow["cod_ant"] = dr["cod_ref"].ToString();
                    drow["cod_ant"] = dr["cod_ant"].ToString();
                    drow["nom_prv"] = dr["nom_prv"].ToString();
                    drow["p_pendiente"] = Convert.ToDouble(dr["p_pendiente"]);//cantidad
                    drow["val_uni"] = Convert.ToDouble(dr["val_uni"]);
                    drow["val_iva"] = Convert.ToDouble(dr["val_iva"]);
                    drow["val_ica"] = Convert.ToDouble(dr["val_ica"]);
                    drow["val_ret"] = Convert.ToDouble(dr["val_ret"]);
                    drow["val_riva"] = Convert.ToDouble(dr["val_riva"]);
                    drow["por_iva"] = Convert.ToDouble(dr["por_iva"]);
                    drow["por_ret"] = Convert.ToDouble(dr["por_ret"]);
                    drow["por_ica"] = Convert.ToDouble(dr["por_ica"]);
                    drow["por_riva"] = Convert.ToDouble(dr["por_riva"]);
                    drow["por_des"] = Convert.ToDouble(dr["por_des"]);
                    drow["subtotal"] = Convert.ToDouble(dr["subtotal"]);
                    drow["tot_tot"] = Convert.ToDouble(dr["tot_tot"]);
                    drow["cod_tiva"] = dr["cod_tiva"].ToString();
                    drow["num_trn"] = dr["num_trn"].ToString();
                    dt.Rows.Add(drow);
                }
            }
            flag = true;
            //this.Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                BtnFacturar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {

                foreach (DataRow item in dt_temp.Rows)
                {
                    item["estado"] = true;
                };

            }
            catch (Exception w)
            {
                MessageBox.Show("erro al seleccionar las casilla de estado:" + w);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (DataRow item in dt_temp.Rows)
            {
                item["estado"] = false;
            };
        }


    }
}
