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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    /// Sia.PublicarPnt(9665,"MediosMagneticos");
    /// Sia.TabU(9665);
    public partial class MediosMagneticos : UserControl
    {

        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public MediosMagneticos(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            idemp = SiaWin._BusinessId;
            CargarEmpresas();
            LoadConfig();
        }

        public void CargarEmpresas()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select businessid, businesscode, businessname, Businessalias from business where (select Seg_AccProjectBusiness.Access from Seg_AccProjectBusiness where GroupId = " + SiaWin._UserGroup.ToString() + "  and ProjectId = " + SiaWin._ProyectId.ToString() + " and Access = 1 and Business.BusinessId = Seg_AccProjectBusiness.BusinessId)= 1");
            DataTable empresas = SiaWin.Func.SqlDT(sb.ToString(), "Empresas", 0);
            comboBoxEmpresas.ItemsSource = empresas.DefaultView;
        }

        public string nameBussines(string code)
        {
            string query = "select businessid, businesscode, businessname, Businessalias from business where businesscode='" + code + "' ";
            DataTable empresas = SiaWin.Func.SqlDT(query, "Empresas", 0);
            return empresas.Rows.Count > 0 ? empresas.Rows[0]["businessname"].ToString() : "Ninguna";
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Medios Magneticos";
                TabControl1.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private async void BtnEjecutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(tx_codigo.Text))
                {
                    MessageBox.Show("!debe de ingresar el codigo¡", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (comboBoxEmpresas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una o mas empresas", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                GridMM.ClearFilters();
                GridMM.ItemsSource = null;

                BtnEjecutar.IsEnabled = false;

                DateTime fec = Convert.ToDateTime(fec_ano.Value.ToString());
                string fechaCon = fec.Year.ToString();
                string codigo = tx_codigo.Text;
                string empresa = comboBoxEmpresas.SelectedValue.ToString();


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fechaCon, codigo, empresa, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result) == null)
                {
                    BtnEjecutar.IsEnabled = true;
                    tabitem.Progreso(false);
                    this.sfBusyIndicator.IsBusy = false;
                    GridConfiguracion.IsEnabled = true;
                    MessageBox.Show("no hay ningun dato para mostrar");
                    return;
                }

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    GridMM.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    tx_rows.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                    tx_empresa.Text = nameBussines(empresa).Trim();
                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;
                }


                BtnEjecutar.IsEnabled = true;
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la consulta:" + w);
            }

        }

        private DataSet LoadData(string anno, string codigo, string empresas, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpMediosMagneticos", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@_anno", anno);
                cmd.Parameters.AddWithValue("@tipo", codigo);
                cmd.Parameters.AddWithValue("@codEmpresa", empresas);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("error en la consulta:" + ex);
                return null;
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar("MmMae_Codigo", "cod_mm", "nom_mm", "cod_mm", "cod_mm", "Conceptos", cnEmp, true, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Height = 400;
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
                    e.Handled = true;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("Error al en codigo:" + w);
            }
        }

        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            if (e.ColumnName == "mcpo" || e.ColumnName == "mun" || e.ColumnName == "dpto")
            {                
                e.Range.Text = e.CellValue.ToString();
                e.Handled = true;
            }
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;
                var excelEngine = GridMM.ExportToExcel(GridMM.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;

                switch (tx_codigo.Text.Trim())
                {
                    case "1001":
                        workBook.ActiveSheet.Columns[9].NumberFormat = "00";
                        workBook.ActiveSheet.Columns[10].NumberFormat = "000";
                        break;
                    case "1003":
                        workBook.ActiveSheet.Columns[10].NumberFormat = "00";                        
                        workBook.ActiveSheet.Columns[11].NumberFormat = "000";                       
                        break;
                    case "1008":
                        workBook.ActiveSheet.Columns[10].NumberFormat = "00";
                        workBook.ActiveSheet.Columns[11].NumberFormat = "000";
                        break;
                    case "1009":
                        workBook.ActiveSheet.Columns[10].NumberFormat = "00";
                        workBook.ActiveSheet.Columns[11].NumberFormat = "000";
                        break;
                    case "1010":
                        workBook.ActiveSheet.Columns[9].NumberFormat = "00";
                        workBook.ActiveSheet.Columns[10].NumberFormat = "000";
                        break;
                    case "2276":
                        workBook.ActiveSheet.Columns[7].NumberFormat = "00";
                        workBook.ActiveSheet.Columns[8].NumberFormat = "000";
                        break;
                }




                //workBook.ActiveSheet.Columns[10].NumberFormat = "000";
                //workBook.ActiveSheet.Columns[11].NumberFormat = "000";



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
            catch (Exception w)
            {
                MessageBox.Show("error a exportar:" + w);
            }
        }






    }
}
