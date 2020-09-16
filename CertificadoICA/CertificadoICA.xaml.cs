using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Spreadsheet;
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

    //Sia.PublicarPnt(9660, "CertificadoICA");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9660, "CertificadoICA");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class CertificadoICA : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int cosn = 0;
        public DataTable DTserver;
        public CertificadoICA()
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
                this.Title = "Certificado " + cod_empresa + "-" + nomempresa;
                CargarEmpresas();
                DTserver = cargarDatosSerividor();

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        public void CargarEmpresas()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select businessid, businesscode, businessname, Businessalias from business where (select Seg_AccProjectBusiness.Access from Seg_AccProjectBusiness where GroupId = " + SiaWin._UserGroup.ToString() + "  and ProjectId = " + SiaWin._ProyectId.ToString() + " and Access = 1 and Business.BusinessId = Seg_AccProjectBusiness.BusinessId)= 1");
            DataTable empresas = SiaWin.Func.SqlDT(sb.ToString(), "Empresas", 0);
            comboBoxEmpresas.ItemsSource = empresas.DefaultView;
            CbBussines.ItemsSource = empresas.DefaultView;
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
                    MessageBox.Show("seleccione una empresa");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Tx_tercero.Text))
                {
                    MessageBox.Show("el campo de tercero e obligatorio");
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
                    TabItemExt tabItemExt1 = new TabItemExt();
                    tabItemExt1.Header = "Consulta General - " + cosn.ToString();

                    string cod = comboBoxEmpresas.SelectedValue.ToString();
                    string ter = Tx_tercero.Text;
                    DateTime fec = Convert.ToDateTime(fec_ano.Value.ToString());
                    string ano = fec.Year.ToString();

                    DateTime pi = Convert.ToDateTime(per_ini.Value.ToString());
                    string per_i = pi.Month.ToString();
                    DateTime pf = Convert.ToDateTime(per_fin.Value.ToString());
                    string per_f = pf.Month.ToString();

                    parameters.Add(new ReportParameter("perini", per_i));
                    parameters.Add(new ReportParameter("perfin", per_f));
                    parameters.Add(new ReportParameter("anno", ano));
                    parameters.Add(new ReportParameter("ter", ter));
                    parameters.Add(new ReportParameter("codemp", cod_empresa));

                    WindowsFormsHost winFormsHost = new WindowsFormsHost();
                    ReportViewer viewer = new ReportViewer();
                    viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");

                    viewer.ServerReport.ReportPath = "/Contabilidad/Certificados/Certificado_ica";

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
                CancellationToken token = source.Token;

                sfBusyIndicator.IsBusy = true;

                dataGridAutomatico.ItemsSource = null;

                string emp = comboBoxEmpresas.SelectedValue.ToString();
                string ter = Tx_tercero.Text;
                DateTime fec = Convert.ToDateTime(fec_ano.Value.ToString());
                string ano = fec.Year.ToString();

                DateTime pi = Convert.ToDateTime(per_ini.Value.ToString());
                string per_i = pi.Month.ToString();
                DateTime pf = Convert.ToDateTime(per_fin.Value.ToString());
                string per_f = pf.Month.ToString();

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ter, ano, per_i, per_f, emp, source.Token), source.Token);
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
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show("erro2:" + ex);
                this.Opacity = 1;
            }
        }


        private DataSet LoadData(string ter, string ano, string perini, string perfin, string CodEmp, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_Empcertica", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ter", ter);
                cmd.Parameters.AddWithValue("@anno", ano);
                cmd.Parameters.AddWithValue("@perini", perini);
                cmd.Parameters.AddWithValue("@perfin", perfin);
                cmd.Parameters.AddWithValue("@codemp", CodEmp);
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


        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SiaWin._UserId == 21)
                {
                    var options = new ExcelExportingOptions();
                    var excelEngine = dataGridAutomatico.ExportToExcel(dataGridAutomatico.View, options);
                    var workBook = excelEngine.Excel.Workbooks[0];
                    workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;

                    //dataGridAutomatico.Tag = tipo.Trim() == "3" ? "A" : "B";
                    //if (dataGridAutomatico.Tag.ToString().Trim() == "A")
                    //{
                    //    workBook.ActiveSheet.Columns[0].NumberFormat = "@";//formato texto
                    //    workBook.ActiveSheet.Columns[2].NumberFormat = "@";
                    //    workBook.ActiveSheet.Columns[13].NumberFormat = "@";
                    //    workBook.ActiveSheet.Columns[14].NumberFormat = "@";
                    //}
                    //else
                    //{
                    //    workBook.ActiveSheet.Columns[0].NumberFormat = "000";//formato numero
                    //    workBook.ActiveSheet.Columns[2].NumberFormat = "000";
                    //    workBook.ActiveSheet.Columns[9].NumberFormat = "000";
                    //}


                    Window window1 = new Window();
                    SfSpreadsheet spreadsheet = new SfSpreadsheet();
                    spreadsheet.Open(workBook);
                    window1.Content = spreadsheet;
                    window1.ShowInTaskbar = false;
                    window1.Owner = Application.Current.MainWindow;
                    window1.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window1.Show();
                }
                else
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

                        //Message box confirmation to view the created workbook.
                        if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",
                                            MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                            System.Diagnostics.Process.Start(sfd.FileName);
                        }
                    }
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al exportar");
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.F8 || e.Key == System.Windows.Input.Key.Enter)
                {
                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "cod_ter", "cod_ter", "maestra de terceros", cnEmp, false, "", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.ShowDialog();

                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (idr > 0)
                    {
                        (sender as TextBox).Text = code;
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;
                }
                if (e.Key == Key.Enter)
                {
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL BUSCAR E TERCERO:" + w);
            }
        }

        private void BtnGrilla_Click(object sender, RoutedEventArgs e)
        {
            tabItemExt4.IsSelected = true;
            consultaMM();            
        }

        public async void consultaMM()
        {
            try
            {

                if (CbBussines.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una empresa");
                    return;
                }


                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                sfBusyIndicator.IsBusy = true;

                dataGridAutomatico.ItemsSource = null;

                string emp = CbBussines.SelectedValue.ToString();

                DateTime fec = Convert.ToDateTime(fec_anoMM.Value.ToString());
                string ano = fec.Year.ToString();

                DateTime pi = Convert.ToDateTime(per_iniMM.Value.ToString());
                string per_i = pi.Month <= 9 ? "0" + pi.Month : pi.Month.ToString();                

                DateTime pf = Convert.ToDateTime(per_finMM.Value.ToString());                
                string per_f = pf.Month <= 9 ? "0" + pf.Month : pf.Month.ToString();

               


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadDataMM(ano, per_i, per_f.ToString(), emp, source.Token), source.Token);
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
            }
        }

        private DataSet LoadDataMM(string ano, string perini, string perfin, string CodEmp, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpCoMediosDistritales", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codemp", CodEmp);
                cmd.Parameters.AddWithValue("@anno", ano);
                cmd.Parameters.AddWithValue("@perini", perini);
                cmd.Parameters.AddWithValue("@perfin", perfin);                
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








    }
}
