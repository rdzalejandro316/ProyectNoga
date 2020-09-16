using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.Windows.Tools.Controls;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9698,"ConsultaFletes");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9698,"ConsultaFletes");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //ww.ShowDialog();

    public partial class ConsultaFletes : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int cosn = 0;
        public DataTable DTserver;

        public ConsultaFletes()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
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
                this.Title = "Consulta Fletes";

                Fec_ini.Text = DateTime.Now.AddMonths(-1).ToString();
                Fec_fin.Text = DateTime.Now.ToString("dd/MM/yyyy");

                DataTable dt = SiaWin.Func.SqlDT("select businessid,BusinessCode as businesscode,BusinessName as businessname from Business ", "empresa", 0);
                comboBoxEmpresas.ItemsSource = dt.DefaultView;


                DTserver = cargarDatosSerividor();

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public DataTable cargarDatosSerividor()
        {
            DataTable dt = SiaWin.Func.SqlDT("select ServerIP, UserServer, UserServerPassword, UserSql, UserSqlPassword from ReportServer", "Empresas", 0);
            return dt;
        }

        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (comboBoxEmpresas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una empresa", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (CheGridGeneral.IsChecked == true)
                {
                    tabItemExt4.IsSelected = true;
                    consulta();
                }
                else
                {
                    cosn++;
                    List<ReportParameter> parameters = new List<ReportParameter>();
                    string ffi = Fec_ini.Text.ToString();
                    string fff = Fec_fin.Text.ToString();
                    string emp = comboBoxEmpresas.SelectedValue.ToString();
                    string trans = Tx_Tercer.Text;
                    string clie = Tx_cli.Text;
                    string ven = Tx_ven.Text;
                    parameters.Add(new ReportParameter("codemp", emp));
                    parameters.Add(new ReportParameter("fecini", ffi));
                    parameters.Add(new ReportParameter("fecfin", fff));
                    parameters.Add(new ReportParameter("codprv", trans));
                    parameters.Add(new ReportParameter("codcli", clie));
                    parameters.Add(new ReportParameter("codven", ven));


                    TabItemExt tabItemExt1 = new TabItemExt();
                    tabItemExt1.Header = "Informe - " + cosn.ToString();


                    WindowsFormsHost winFormsHost = new WindowsFormsHost();
                    ReportViewer viewer = new ReportViewer();
                    viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");

                    viewer.ServerReport.ReportPath = "/Contabilidad/ConsultaFletes";

                    viewer.ProcessingMode = ProcessingMode.Remote;
                    ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                    rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(DTserver.Rows[0]["UserServer"].ToString(), DTserver.Rows[0]["UserServerPassword"].ToString());
                    List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();

                    foreach (var dataSource in viewer.ServerReport.GetDataSources())
                    {
                        DataSourceCredentials credn = new DataSourceCredentials();
                        credn.Name = dataSource.Name;
                        System.Windows.MessageBox.Show(dataSource.Name);
                        credn.UserId = DTserver.Rows[0]["UserSql"].ToString();
                        credn.Password = DTserver.Rows[0]["UserSqlPassword"].ToString();
                        crdentials.Add(credn);
                    }

                    viewer.ServerReport.SetDataSourceCredentials(crdentials);
                    viewer.ServerReport.SetParameters(parameters);
                    viewer.RefreshReport();

                    winFormsHost.Child = viewer;
                    tabItemExt1.Content = winFormsHost;
                    TabControl1.Items.Add(tabItemExt1);
                    UpdateLayout();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }

        public async void consulta()
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                sfBusyIndicator.IsBusy = true;
                dataGridAutomatico.ItemsSource = null;
                string ffi = Fec_ini.Text.ToString();
                string fff = Fec_fin.Text.ToString();
                string emp = comboBoxEmpresas.SelectedValue.ToString();
                string trans = Tx_Tercer.Text;
                string clie = Tx_cli.Text;
                string ven = Tx_ven.Text;


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ffi, fff, emp, trans, clie, ven, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridAutomatico.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Txtotal.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                }

                this.sfBusyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("erro2:" + ex);
                this.Opacity = 1;
            }
        }

        private DataSet LoadData(string FechaIN, string FechaFI, string emp, string trans, string cli, string ven, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpInFletes", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fecini", FechaIN);
                cmd.Parameters.AddWithValue("@fecfin", FechaFI);
                cmd.Parameters.AddWithValue("@codprv", trans);
                cmd.Parameters.AddWithValue("@codcli", cli);
                cmd.Parameters.AddWithValue("@codven", ven);
                cmd.Parameters.AddWithValue("@codEmpresa", emp);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show("en la consulta:" + e.Message);
                return null;
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Tx_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox tx = (sender as TextBox);

                if (comboBoxEmpresas.SelectedIndex >= 0)
                {
                    string query = "";

                    switch (tx.Name.Trim())
                    {
                        case "Tx_Tercer":
                            query += "select * from comae_ter where cod_ter='" + tx.Text + "' ";
                            break;
                        case "Tx_cli":
                            query += "select * from comae_ter where cod_ter='" + tx.Text + "' ";
                            break;
                        case "Tx_ven":
                            query += "select * from inmae_mer where cod_mer='" + tx.Text + "' ";
                            break;
                    }

                    DataTable dt = SiaWin.Func.SqlDT(query, "empresa", IdEmprSel());
                    if (dt.Rows.Count <= 0)
                    {
                        MessageBox.Show("no existe el codigo " + tx.Text + " ingresado", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        tx.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("seleccione un empresa", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    tx.Text = "";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al valdiar:" + w);
            }
        }

        public int IdEmprSel()
        {
            string codemp = comboBoxEmpresas.SelectedValue.ToString();
            DataTable dt = SiaWin.Func.SqlDT("select BusinessId from Business where BusinessCode='" + codemp + "'", "referencia", 0);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["BusinessId"]) : 0;
        }

        public string CnEmprConection()
        {
            string codemp = comboBoxEmpresas.SelectedValue.ToString();
            string query = "select BusinessCn from Business where BusinessCode='" + codemp + "'";            
            DataTable dt = SiaWin.Func.SqlDT(query, "empresa", 0);
            return dt.Rows.Count > 0 ? dt.Rows[0]["BusinessCn"].ToString() : "";
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridAutomatico.ExportToExcel(dataGridAutomatico.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;

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

                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archio", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al exportar");
            }
        }

        private void ButtonWin_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (comboBoxEmpresas.SelectedIndex >= 0)
                {

                    Button btn = (sender as Button);
                    string tag = btn.Tag.ToString();


                    if (string.IsNullOrEmpty(tag)) return;
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; bool mostrartodo = true; string cmpwhere = "";
                    switch (btn.Tag.ToString().Trim())
                    {
                        case "transportador":
                            cmptabla = "comae_ter"; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "nom_ter"; cmpidrow = "idrow"; cmptitulo = "Maestra de terceros "; mostrartodo = false; cmpwhere = " ";
                            break;
                        case "cliente":
                            cmptabla = "comae_ter"; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "nom_ter"; cmpidrow = "idrow"; cmptitulo = "Maestra de terceros"; mostrartodo = false; cmpwhere = " ";
                            break;
                        case "vendedor":
                            cmptabla = "inmae_mer"; cmpcodigo = "cod_mer"; cmpnombre = "nom_mer"; cmporden = "nom_mer"; cmpidrow = "idrow"; cmptitulo = "Maestra de vendedores";  mostrartodo = false; cmpwhere = " ";
                            break;
                    }

                    int idr = 0; string code = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, CnEmprConection(), mostrartodo, cmpwhere, IdEmprSel());
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 300;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    winb = null;
                    if (idr > 0)
                    {                        
                        if (tag == "transportador") Tx_Tercer.Text = code;
                        if (tag == "cliente") Tx_cli.Text = code;
                        if (tag == "vendedor") Tx_ven.Text = code;
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;

                }
                else
                {
                    MessageBox.Show("seleccione un empresa", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir maestra:" + w);
            }
        }






    }
}
