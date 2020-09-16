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

    //Sia.PublicarPnt(9642, "ConsultaDevolucion");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9642, "ConsultaDevolucion");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class ConsultaDevolucion : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int cosn = 0;
        public DataTable DTserver;
        public ConsultaDevolucion()
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
                this.Title = "Devolcion de facturas" + cod_empresa + "-" + nomempresa;

                Fec_ini.Text = DateTime.Now.AddMonths(-1).ToString();
                Fec_fin.Text = DateTime.Now.ToString("dd/MM/yyyy");

                DataTable dt = SiaWin.Func.SqlDT("select cod_bod,RTRIM(nom_bod)+'-'+RTRIM(cod_bod) as nom_bod from inmae_bod where cod_emp='" + cod_empresa + "'; ", "inmae_bod", idemp);
                comboBoxBodegas.ItemsSource = dt.DefaultView;
                comboBoxBodegas.DisplayMemberPath = "nom_bod";
                comboBoxBodegas.SelectedValuePath = "cod_bod";


                Fec_ini_det.Text = DateTime.Now.AddMonths(-1).ToString();
                Fec_fin_det.Text = DateTime.Now.ToString("dd/MM/yyyy");

                CmbBodDet.ItemsSource = dt.DefaultView;
                CmbBodDet.DisplayMemberPath = "nom_bod";
                CmbBodDet.SelectedValuePath = "cod_bod";


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


        public string returnTipBod()
        {

            string tipos = "";
            if (comboBoxBodegas.SelectedIndex >= 0)
            {
                foreach (DataRowView ob in comboBoxBodegas.SelectedItems)
                {
                    String valueCta = ob["cod_bod"].ToString();
                    //tipos += "'" + valueCta + "'" + ",";
                    tipos += valueCta + ",";
                }
                string ss = tipos.Trim().Substring(tipos.Trim().Length - 1);
                if (ss == ",") tipos = tipos.Substring(0, tipos.Trim().Length - 1);
            }
            //MessageBox.Show("A2");
            return tipos;
        }

        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (comboBoxBodegas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una bodega");
                    return;
                }


                if (CheGridGeneral.IsChecked == true)
                {
                    tabItemExt4.IsSelected = true;
                    consulta("1");
                }
                else
                {
                    cosn++;
                    List<ReportParameter> parameters = new List<ReportParameter>();
                    TabItemExt tabItemExt1 = new TabItemExt();
                    tabItemExt1.Header = "Consulta General - " + cosn.ToString();

                    string bod = returnTipBod();
                    parameters.Add(new ReportParameter("bod", bod));
                    parameters.Add(new ReportParameter("fec_ini", Fec_ini.Text));
                    parameters.Add(new ReportParameter("fec_fin", Fec_fin.Text));
                    parameters.Add(new ReportParameter("codemp", cod_empresa));

                    WindowsFormsHost winFormsHost = new WindowsFormsHost();
                    ReportViewer viewer = new ReportViewer();
                    viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");

                    viewer.ServerReport.ReportPath = "/Inventarios/Devoluciones/DevolucionGeneral";

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


        public string returnTipBodDet()
        {

            string tipos = "";
            if (CmbBodDet.SelectedIndex >= 0)
            {                
                foreach (DataRowView ob in CmbBodDet.SelectedItems)
                {
                    String valueCta = ob["cod_bod"].ToString();
                    //tipos += "'" + valueCta + "'" + ",";
                    tipos += valueCta + ",";
                }
                string ss = tipos.Trim().Substring(tipos.Trim().Length - 1);
                if (ss == ",") tipos = tipos.Substring(0, tipos.Trim().Length - 1);
            }
            //MessageBox.Show("A2");
            return tipos;
        }

        private void BtnConsultarDetallada_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (CmbBodDet.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una bodega");
                    return;
                }

                if (CheGrid.IsChecked == true)
                {
                    tabItemExt4.IsSelected = true;
                    consulta("3");
                }
                else
                {
                    cosn++;
                    List<ReportParameter> parameters = new List<ReportParameter>();
                    TabItemExt tabItemExt1 = new TabItemExt();
                    tabItemExt1.Header = "Consulta General - " + cosn.ToString();

                    string bod = returnTipBodDet();
                    parameters.Add(new ReportParameter("bod", bod));
                    parameters.Add(new ReportParameter("fec_ini", Fec_ini.Text));
                    parameters.Add(new ReportParameter("fec_fin", Fec_fin.Text));
                    parameters.Add(new ReportParameter("codemp", cod_empresa));

                    WindowsFormsHost winFormsHost = new WindowsFormsHost();
                    ReportViewer viewer = new ReportViewer();
                    viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");

                    viewer.ServerReport.ReportPath = "/Inventarios/Devoluciones/DevolucionDetallada";

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


        public async void consulta(string tipo)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                sfBusyIndicator.IsBusy = true;

                dataGridAutomatico.ItemsSource = null;

                string ffi = "";
                string fff = "";
                string bod = "";
                if (tipo.Trim() == "3")
                {
                    ffi = Fec_ini_det.Text.ToString();
                    fff = Fec_fin_det.Text.ToString();
                    bod = returnTipBodDet();
                }
                else
                {
                    ffi = Fec_ini.Text.ToString();
                    fff = Fec_fin.Text.ToString();
                    bod = returnTipBod();
                }


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ffi, fff, bod, tipo, cod_empresa, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridAutomatico.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Txtotal.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                    dataGridAutomatico.Tag = tipo.Trim() == "3" ? "A" : "B";
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



        private DataSet LoadData(string FechaIN, string FechaFI, string bodega, string tipo, string CodEmp, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpFacDevolucionCon", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@fec_ini", FechaIN);
                cmd.Parameters.AddWithValue("@fec_fin", FechaFI);
                cmd.Parameters.AddWithValue("@bod", bodega);
                //cmd.Parameters.AddWithValue("@detalla", "3");
                cmd.Parameters.AddWithValue("@detalla", tipo);
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

        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;
            e.Range.CellStyle.Font.FontName = "Segoe UI";
            if (e.ColumnName == "val_uni" || e.ColumnName == "cantidad" || e.ColumnName == "subtotal")
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }
            if (e.ColumnName == "cod_dev" || e.ColumnName == "cod_tip" || e.ColumnName == "cod_bod" || e.ColumnName == "cod_ref" || e.ColumnName == "cod_trn")
            {
                string value = e.CellValue.ToString();

                e.Range.Text = value;
                e.Handled = true;
            }

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
                    if (dataGridAutomatico.Tag.ToString().Trim() == "A")
                    {
                        workBook.ActiveSheet.Columns[0].NumberFormat = "000";
                        workBook.ActiveSheet.Columns[2].NumberFormat = "000";
                        workBook.ActiveSheet.Columns[13].NumberFormat = "000";
                        workBook.ActiveSheet.Columns[14].NumberFormat = "000";
                    }
                    else
                    {
                        workBook.ActiveSheet.Columns[0].NumberFormat = "000";//formato numero
                        workBook.ActiveSheet.Columns[2].NumberFormat = "000";
                        workBook.ActiveSheet.Columns[9].NumberFormat = "000";
                    }
                    

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
                    options.CellsExportingEventHandler = CellExportingHandler;
                    var excelEngine = dataGridAutomatico.ExportToExcel(dataGridAutomatico.View, options);
                    var workBook = excelEngine.Excel.Workbooks[0];                    
                    workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;
                    
                    if (dataGridAutomatico.Tag.ToString().Trim() == "A")
                    {
                        
                        workBook.ActiveSheet.Columns[0].NumberFormat = "000";
                        workBook.ActiveSheet.Columns[2].NumberFormat = "000";
                        workBook.ActiveSheet.Columns[13].NumberFormat = "000";
                        workBook.ActiveSheet.Columns[14].NumberFormat = "000";
                    }
                    else
                    {                       
                        workBook.ActiveSheet.Columns[0].NumberFormat = "000";
                        workBook.ActiveSheet.Columns[2].NumberFormat = "000";
                        workBook.ActiveSheet.Columns[9].NumberFormat = "000";
                    }


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






    }
}
