using Microsoft.Win32;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
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
    /// Sia.PublicarPnt(9591,"ClasificacionABC");
    /// Sia.TabU(9591);
    public partial class ClasificacionABC : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public ClasificacionABC(dynamic tabitem1)
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
            sb.Append("select businessid, businesscode, businessname, BusinessAlias from business where (select Seg_AccProjectBusiness.Access from Seg_AccProjectBusiness where GroupId = " + SiaWin._UserGroup.ToString() + "  and ProjectId = " + SiaWin._ProyectId.ToString() + " and Access = 1 and Business.BusinessId = Seg_AccProjectBusiness.BusinessId)= 1");
            //DataTable empresas = SiaWin.Func.SqlDT("select businesscode,businessname,businessnit from Business where BusinessStatus='1' ", "Empresas", 0);
            DataTable empresas = SiaWin.Func.SqlDT(sb.ToString(), "Empresas", 0);

            //            DataTable empresas = SiaWin.Func.SqlDT("select businesscode,businessname,businessnit from Business where BusinessStatus='1' ", "Empresas", 0);
            comboBoxEmpresas.ItemsSource = empresas.DefaultView;

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
                tabitem.Title = "Clasificacion ABC";
                FecIni.Text = DateTime.Now.AddMonths(-1).ToShortDateString();
                FecFin.Text = DateTime.Now.ToShortDateString();
                TabControl1.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void VentasPorProducto_FilterChanged(object sender, Syncfusion.UI.Xaml.Grid.GridFilterEventArgs e)
        {

        }

        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;
            e.Range.CellStyle.Font.FontName = "Segoe UI";
            if (e.ColumnName == "vta_cnt" || e.ColumnName == "vtacntpar" || e.ColumnName == "acumcntporc" || e.ColumnName == "vta_subtotal" || e.ColumnName == "vtasubpart" || e.ColumnName == "saldo_fin" || e.ColumnName == "cos_tot" || e.ColumnName == "acumcostporc")
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExportMode = ExportMode.Value;
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;
                var excelEngine = GridClasific.ExportToExcel(GridClasific.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.ActiveSheet.Columns[3].NumberFormat = "000";

                workBook.ActiveSheet.Columns[6].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[7].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[8].NumberFormat = "0.0";

                workBook.ActiveSheet.Columns[10].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[11].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[12].NumberFormat = "0.0";


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
                MessageBox.Show("error al exportar:" + w);
            }
        }





        private void BtnHidden_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString().Trim();
            if (tag == "0")
            {
                BtnHidden.Content = "Mostrar Graficos";
                (sender as Button).Tag = "1";
                Chart1_clasific.Visibility = Visibility.Hidden;
                Chart2_clasific.Visibility = Visibility.Hidden;
                Chart3_clasific.Visibility = Visibility.Hidden;
                Grid.SetRowSpan(GridSpan, 2);
            }
            else
            {
                BtnHidden.Content = "Ocultar Graficos";
                (sender as Button).Tag = "0";
                Chart1_clasific.Visibility = Visibility.Visible;
                Chart2_clasific.Visibility = Visibility.Visible;
                Chart3_clasific.Visibility = Visibility.Visible;
                Grid.SetRowSpan(GridSpan, 1);
            }
        }

        private async void BtnEjecutar_Click(object sender, RoutedEventArgs e)
        {
             try
            {
                if (string.IsNullOrEmpty(FecIni.Text))
                {
                    MessageBox.Show("llene los campos de las fecha", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
                GridClasific.ClearFilters();
                GridClasific.ItemsSource = null;

                BtnEjecutar.IsEnabled = false;

                string fecha_Ini = FecIni.Text.ToString();
                string fecha_Fin = FecFin.Text.ToString();


                string empresas = returnEmpresas();
                int excluir = 1;


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fecha_Ini, fecha_Fin, excluir, empresas, source.Token), source.Token);
                await slowTask;


                BtnEjecutar.IsEnabled = true;
                //tabitem.Progreso(false);
                //MessageBox.Show(((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString());
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {


                    #region tab 1

                    GridClasific.ItemsSource = ((DataSet)slowTask.Result).Tables[1];
                    double rows_tb1 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[1].Rows.Count);
                    double vta_cnt = Convert.ToDouble(((DataSet)slowTask.Result).Tables[1].Compute("Sum(vta_cnt)", ""));
                    Total1.Text = rows_tb1.ToString();
                    Total2.Text = vta_cnt.ToString();


                    System.Data.DataTable DtEmpCnt_detallado_cricle1 = new System.Data.DataTable();
                    System.Data.DataTable DtEmpCnt_detallado_cricle2 = new System.Data.DataTable();
                    System.Data.DataTable DtEmpCnt_detallado_cricle3 = new System.Data.DataTable();
                    System.Data.DataTable dtEmpresa_detallado = ((DataSet)slowTask.Result).Tables[1];

                    if (dtEmpresa_detallado.Rows.Count > 0)
                    {
                        DtEmpCnt_detallado_cricle1 = dtEmpresa_detallado.AsEnumerable()
                            .GroupBy(a => a["califcnt"].ToString().Trim())
                            .Select(c =>
                            {
                                var row = ((DataSet)slowTask.Result).Tables[1].NewRow();
                                row["califcnt"] = c.Key;
                                row["acumcntporc"] = c.Sum(a => a.Field<decimal>("acumcntporc"));
                                return row;
                            }).CopyToDataTable();
                    }

                    if (dtEmpresa_detallado.Rows.Count > 0)
                    {
                        DtEmpCnt_detallado_cricle2 = dtEmpresa_detallado.AsEnumerable()
                            .GroupBy(a => a["califsub"].ToString().Trim())
                            .Select(c =>
                            {
                                var row = ((DataSet)slowTask.Result).Tables[1].NewRow();
                                row["califsub"] = c.Key;
                                row["acumsubporc"] = c.Sum(a => a.Field<decimal>("acumsubporc"));
                                return row;
                            }).CopyToDataTable();
                    }

                    if (dtEmpresa_detallado.Rows.Count > 0)
                    {
                        DtEmpCnt_detallado_cricle3 = dtEmpresa_detallado.AsEnumerable()
                            .GroupBy(a => a["califcost"].ToString().Trim())
                            .Select(c =>
                            {
                                var row = ((DataSet)slowTask.Result).Tables[1].NewRow();
                                row["califcost"] = c.Key;
                                row["acumcostporc"] = c.Sum(a => a.Field<decimal>("acumcostporc"));
                                return row;
                            }).CopyToDataTable();
                    }



                    ChartCircle1.ItemsSource = DtEmpCnt_detallado_cricle1;
                    ChartCircle2.ItemsSource = DtEmpCnt_detallado_cricle2;
                    ChartCircle3.ItemsSource = DtEmpCnt_detallado_cricle3;



                    #endregion


                    #region tab 2

                    Tb2_GridClasific.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;

                    System.Data.DataTable DtEmpCnt = new System.Data.DataTable();
                    System.Data.DataTable dtEmpresa = ((DataSet)slowTask.Result).Tables[0];

                    if (dtEmpresa.Rows.Count > 0)
                    {
                        DtEmpCnt = dtEmpresa.AsEnumerable()
                            .GroupBy(a => a["nom_emp"].ToString().Trim())
                            .Select(c =>
                            {
                                var row = ((DataSet)slowTask.Result).Tables[0].NewRow();
                                row["nom_emp"] = c.Key;
                                row["totcntemp"] = c.Sum(a => a.Field<decimal>("totcntemp"));
                                return row;
                            }).CopyToDataTable();
                    }

                    Tb2_chartEmpCnt.ItemsSource = DtEmpCnt;


                    double rows = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Rows.Count);
                    double totcntemp = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(totcntemp)", ""));
                    double vta_subtotal = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(vta_subtotal)", ""));
                    double cos_tot = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cos_tot)", ""));

                    Tb2_Total1.Text = rows.ToString();
                    Tb2_Total2.Text = totcntemp.ToString();
                    Tb2_Total3.Text = vta_subtotal.ToString("C");
                    Tb2_Total4.Text = cos_tot.ToString("C");

                    #endregion



                    //GridDetalle.ItemsSource = ((DataSet)slowTask.Result).Tables[1];
                }

                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (Exception ex)
            {
                //this.Opacity = 1;
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                MessageBox.Show(ex.Message);

            }
        }

        private DataSet LoadData(string fechaIni, string fechaFin, int excluir, string empresas, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpClasificacionABC1", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", fechaIni);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                //cmd.Parameters.AddWithValue("@FechaIni", "01/01/2019");
                //cmd.Parameters.AddWithValue("@FechaFin", "31/08/2019");
                cmd.Parameters.AddWithValue("@excluirempresas", excluir);
                cmd.Parameters.AddWithValue("@codemp", empresas);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();

                return ds;

            }
            catch (Exception e)
            {
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                MessageBox.Show("error loaddata:" + e.Message);
                return null;
            }
        }

        public string returnEmpresas()
        {
            string empresas = "";

            foreach (DataRowView ob in comboBoxEmpresas.SelectedItems)
            {
                String valueCta = ob["BusinessCode"].ToString();
                empresas += valueCta + ",";
            }
            string ss = empresas.Trim().Substring(empresas.Trim().Length - 1);
            if (ss == ",") empresas = empresas.Substring(0, empresas.Trim().Length - 1);
            return empresas;
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }

        private void Tb2_BtnHidden_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString().Trim();
            if (tag == "0")
            {
                BtnHidden.Content = "Mostrar Graficos";
                (sender as Button).Tag = "1";
                Chart2.Visibility = Visibility.Hidden;
                Grid.SetRowSpan(Tb2_GridSpan, 2);
            }
            else
            {
                BtnHidden.Content = "Ocultar Graficos";
                (sender as Button).Tag = "0";
                Chart2.Visibility = Visibility.Visible;
                Grid.SetRowSpan(Tb2_GridSpan, 1);
            }
        }

        private static void CellExportingHandlerGridTwo(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;
            e.Range.CellStyle.Font.FontName = "Segoe UI";
            if (e.ColumnName == "totcntgrup" || e.ColumnName == "totcntemp" || e.ColumnName == "totcntgrupref" || e.ColumnName == "totcntempref" || e.ColumnName == "totcntemprefbod" || e.ColumnName == "vta_cntparempref" || e.ColumnName == "vta_cntparemprefbod" || e.ColumnName == "totsubemp")
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }
        }


        private void Tb2_BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExportMode = ExportMode.Value;
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandlerGridTwo;
                var excelEngine = Tb2_GridClasific.ExportToExcel(Tb2_GridClasific.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];


                workBook.ActiveSheet.Columns[0].NumberFormat = "000";
                workBook.ActiveSheet.Columns[2].NumberFormat = "000";
                workBook.ActiveSheet.Columns[4].NumberFormat = "000";

                
                workBook.ActiveSheet.Columns[8].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[9].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[10].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[11].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[12].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[13].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[14].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[15].NumberFormat = "0.0";


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
                MessageBox.Show("error al eexportar:"+w);
            }
        }

        //grid rows wrap -------------------------------------------------------------------------------------------------


        GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();

        //To get the calculated height from the GetAutoRowHeight method.
        double autoHeight;

        public void dataGrid_QueryRowHeight(object sender, QueryRowHeightEventArgs e)
        {
            //checked whether the row index is header or not.

            if (this.GridClasific.GetHeaderIndex() == e.RowIndex)
            {
                if (this.GridClasific.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out autoHeight))
                {
                    if (autoHeight > 24)
                    {
                        e.Height = autoHeight;
                        e.Handled = true;
                    }
                }
            }
        }

      
    }
}

