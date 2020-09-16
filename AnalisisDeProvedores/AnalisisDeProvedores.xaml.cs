using Microsoft.Win32;
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
    /// Sia.PublicarPnt(9598,"AnalisisDeProvedores");
    /// Sia.TabU(9598);
    public partial class AnalisisDeProvedores : UserControl
    {

        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public AnalisisDeProvedores(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            CargarEmpresas();
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
                tabitem.Title = "Analisis de provedores";
                FecIni.Text = DateTime.Now.ToShortDateString();
                FecFin.Text = DateTime.Now.ToShortDateString();
                TabControl1.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void CargarEmpresas()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select businessid, businesscode, businessname, businessalias from business where (select Seg_AccProjectBusiness.Access from Seg_AccProjectBusiness where GroupId = " + SiaWin._UserGroup.ToString() + "  and ProjectId = " + SiaWin._ProyectId.ToString() + " and Access = 1 and Business.BusinessId = Seg_AccProjectBusiness.BusinessId)= 1");
            DataTable empresas = SiaWin.Func.SqlDT(sb.ToString(), "Empresas", 0);
            comboBoxEmpresas.ItemsSource = empresas.DefaultView;
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

        private async void BtnEjecutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
             
                if (string.IsNullOrEmpty(FecIni.Text) || string.IsNullOrEmpty(FecFin.Text))
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
                BtnEjecutar.IsEnabled = false;

                string fecha_Ini = FecIni.Text.ToString();
                string fecha_Fin = FecFin.Text.ToString();
                //string empresas = comboBoxEmpresas.SelectedValue.ToString();
                string empresas = returnEmpresas();

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fecha_Ini, fecha_Fin, empresas, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    tabItemExt2.IsSelected = true;

                    //tab 1
                    GridDocument.ItemsSource = ((DataSet)slowTask.Result).Tables[0].DefaultView;
                    Total_D1.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                    double Cnt_D2 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cantidad)", ""));
                    Total_D2.Text = Cnt_D2.ToString();
                    double CosTot_D3 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cos_tot)", ""));
                    Total_D3.Text = CosTot_D3.ToString("C");
                    double CosTot_D4 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cos_totn)", ""));
                    Total_D4.Text = CosTot_D4.ToString("C");

                    //tab 2
                    GridProduct.ItemsSource = ((DataSet)slowTask.Result).Tables[1].DefaultView;
                    Total_P1.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                    double Cnt_p2 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[1].Compute("Sum(cantidad)", ""));
                    Total_P2.Text = Cnt_D2.ToString();
                    double Cnt_p3 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[1].Compute("Sum(can_dev)", ""));
                    Total_P3.Text = Cnt_p3.ToString();
                    double CosTot_p4 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[1].Compute("Sum(total)", ""));
                    Total_P4.Text = CosTot_p4.ToString("C");

                    //tab 3
                    GridBodega.ItemsSource = ((DataSet)slowTask.Result).Tables[2].DefaultView;
                    Total_B1.Text = ((DataSet)slowTask.Result).Tables[2].Rows.Count.ToString();
                    double Cnt_B2 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[2].Compute("Sum(cantidad)", ""));
                    Total_B2.Text = Cnt_B2.ToString();
                    double Cnt_B3 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[2].Compute("Sum(can_dev)", ""));
                    Total_B3.Text = Cnt_B3.ToString();
                    double CosTot_B4 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[2].Compute("Sum(total)", ""));
                    Total_B4.Text = CosTot_B4.ToString("C");

                    //tab 4
                    GridLinea.ItemsSource = ((DataSet)slowTask.Result).Tables[3].DefaultView;
                    Total_L1.Text = ((DataSet)slowTask.Result).Tables[3].Rows.Count.ToString();
                    double Cnt_L2 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[3].Compute("Sum(cantidad)", ""));
                    Total_L2.Text = Cnt_L2.ToString();
                    double Cnt_L3 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[3].Compute("Sum(can_dev)", ""));
                    Total_L3.Text = Cnt_L3.ToString();
                    double CosTot_L4 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[3].Compute("Sum(total)", ""));
                    Total_L4.Text = CosTot_L4.ToString("C");

                    //tab 5
                    GridProvedor.ItemsSource = ((DataSet)slowTask.Result).Tables[4].DefaultView;
                    Total_PR1.Text = ((DataSet)slowTask.Result).Tables[4].Rows.Count.ToString();
                    double Cnt_PR2 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[4].Compute("Sum(cantidad)", ""));
                    Total_PR2.Text = Cnt_PR2.ToString();
                    double Cnt_PR3 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[4].Compute("Sum(can_dev)", ""));
                    Total_PR3.Text = Cnt_PR3.ToString();
                    double CosTot_PR4 = Convert.ToDouble(((DataSet)slowTask.Result).Tables[4].Compute("Sum(total)", ""));
                    Total_PR4.Text = CosTot_PR4.ToString("C");

                    GridProvedorDoc.ItemsSource = ((DataSet)slowTask.Result).Tables[5].DefaultView;
                }

                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                BtnEjecutar.IsEnabled = true;
            }
            catch (Exception ex)
            {
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                MessageBox.Show("error loaddata:" + ex.Message);
            }

        }

        private DataSet LoadData(string fechaIni, string fechaFin, string empresa, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpConsultaInAnalisisCompras_MultiEmpresa", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", fechaIni);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                cmd.Parameters.AddWithValue("@Where", " ");
                cmd.Parameters.AddWithValue("@codemp", empresa);
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



        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {

        }


        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {            e.Range.CellStyle.Font.Size = 10;
            e.Range.CellStyle.Font.FontName = "Segoe UI";
            if (e.ColumnName == "cantidad" || e.ColumnName == "cos_uni" || e.ColumnName == "cos_tot" || e.ColumnName == "cos_uni" || e.ColumnName == "cos_tot" || e.ColumnName == "cos_unin" || e.ColumnName == "cos_totn" || e.ColumnName == "val_des" || e.ColumnName == "val_iva" || e.ColumnName == "val_ret" || e.ColumnName == "val_ica" || e.ColumnName == "val_riva" || e.ColumnName == "total")
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
            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExportMode = ExportMode.Value;
            options.ExcelVersion = ExcelVersion.Excel2013;
            options.CellsExportingEventHandler = CellExportingHandler;            
            

            SfDataGrid sfdg = new SfDataGrid();
            if (((Button)sender).Tag.ToString() == "1") sfdg = GridDocument;
            if (((Button)sender).Tag.ToString() == "2") sfdg = GridProduct;
            if (((Button)sender).Tag.ToString() == "3") sfdg = GridBodega;
            if (((Button)sender).Tag.ToString() == "4") sfdg = GridLinea;
            if (((Button)sender).Tag.ToString() == "5") sfdg = GridProvedor;
            if (((Button)sender).Tag.ToString() == "6") sfdg = GridProvedorDoc;
            

            var excelEngine = sfdg.ExportToExcel(sfdg.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];

            if (((Button)sender).Tag.ToString() == "1")
            {
                workBook.ActiveSheet.Columns[1].NumberFormat = "00";
                workBook.ActiveSheet.Columns[2].NumberFormat = "000";
                workBook.ActiveSheet.Columns[9].NumberFormat = "000";
                workBook.ActiveSheet.Columns[26].NumberFormat = "000";

                workBook.ActiveSheet.Columns[13].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[14].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[15].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[16].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[17].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[18].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[19].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[20].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[21].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[22].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[22].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[23].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[24].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[25].NumberFormat = "0.0";
            }

            if (((Button)sender).Tag.ToString() == "2")
            {
                workBook.ActiveSheet.Columns[2].NumberFormat = "000";

                workBook.ActiveSheet.Columns[4].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[5].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[6].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[7].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[8].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[9].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[10].NumberFormat = "0.0";

            }


            if (((Button)sender).Tag.ToString() == "3")
            {
                workBook.ActiveSheet.Columns[0].NumberFormat = "000";

                workBook.ActiveSheet.Columns[3].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[4].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[5].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[6].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[7].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[8].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[9].NumberFormat = "0.0";                
            }


            if (((Button)sender).Tag.ToString() == "4")
            {
                workBook.ActiveSheet.Columns[0].NumberFormat = "000";

                workBook.ActiveSheet.Columns[2].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[3].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[4].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[5].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[6].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[7].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[8].NumberFormat = "0.0";                
            }


            if (((Button)sender).Tag.ToString() == "5")
            {
                workBook.ActiveSheet.Columns[2].NumberFormat = "000";

                workBook.ActiveSheet.Columns[6].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[7].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[8].NumberFormat = "0.0";                
                workBook.ActiveSheet.Columns[9].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[10].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[11].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[12].NumberFormat = "0.0";
            }


            if (((Button)sender).Tag.ToString() == "6")
            {
                workBook.ActiveSheet.Columns[2].NumberFormat = "000";
                workBook.ActiveSheet.Columns[6].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[7].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[8].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[9].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[10].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[11].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[12].NumberFormat = "0.0";
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

        private void GridClasific_FilterChanged(object sender, Syncfusion.UI.Xaml.Grid.GridFilterEventArgs e)
        {
            try
            {
                if ((sender as SfDataGrid).Name == "GridDocument")
                {
                    var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                    var records = (sender as SfDataGrid).View.Records;

                    double val1 = 0;
                    double val2 = 0;
                    double val3 = 0;

                    for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                    {
                        val1 += Convert.ToDouble(provider.GetValue(records[i].Data, "cantidad").ToString());
                        val2 += Convert.ToDouble(provider.GetValue(records[i].Data, "cos_tot").ToString());
                        val3 += Convert.ToDouble(provider.GetValue(records[i].Data, "cos_totn").ToString());
                    }
                    Total_D1.Text = (sender as SfDataGrid).View.Records.Count.ToString();
                    Total_D2.Text = val1.ToString();
                    Total_D3.Text = val2.ToString("C");
                    Total_D4.Text = val3.ToString("C");
                }

                if ((sender as SfDataGrid).Name == "GridProduct")
                {
                    var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                    var records = (sender as SfDataGrid).View.Records;

                    double val1 = 0;
                    double val2 = 0;
                    double val3 = 0;

                    for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                    {
                        val1 += Convert.ToDouble(provider.GetValue(records[i].Data, "cantidad").ToString());
                        val2 += Convert.ToDouble(provider.GetValue(records[i].Data, "can_dev").ToString());
                        val3 += Convert.ToDouble(provider.GetValue(records[i].Data, "total").ToString());
                    }
                    Total_P1.Text = (sender as SfDataGrid).View.Records.Count.ToString();
                    Total_P2.Text = val1.ToString();
                    Total_P3.Text = val2.ToString();
                    Total_P4.Text = val3.ToString("C");
                }

                if ((sender as SfDataGrid).Name == "GridBodega")
                {
                    var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                    var records = (sender as SfDataGrid).View.Records;

                    double val1 = 0;
                    double val2 = 0;
                    double val3 = 0;

                    for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                    {
                        val1 += Convert.ToDouble(provider.GetValue(records[i].Data, "cantidad").ToString());
                        val2 += Convert.ToDouble(provider.GetValue(records[i].Data, "can_dev").ToString());
                        val3 += Convert.ToDouble(provider.GetValue(records[i].Data, "total").ToString());
                    }
                    Total_B1.Text = (sender as SfDataGrid).View.Records.Count.ToString();
                    Total_B2.Text = val1.ToString();
                    Total_B3.Text = val2.ToString();
                    Total_B4.Text = val3.ToString("C");
                }

                if ((sender as SfDataGrid).Name == "GridLinea")
                {
                    var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                    var records = (sender as SfDataGrid).View.Records;

                    double val1 = 0;
                    double val2 = 0;
                    double val3 = 0;

                    for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                    {
                        val1 += Convert.ToDouble(provider.GetValue(records[i].Data, "cantidad").ToString());
                        val2 += Convert.ToDouble(provider.GetValue(records[i].Data, "can_dev").ToString());
                        val3 += Convert.ToDouble(provider.GetValue(records[i].Data, "total").ToString());
                    }
                    Total_L1.Text = (sender as SfDataGrid).View.Records.Count.ToString();
                    Total_L2.Text = val1.ToString();
                    Total_L3.Text = val2.ToString();
                    Total_L4.Text = val3.ToString("C");
                }

                if ((sender as SfDataGrid).Name == "GridProvedor")
                {
                    var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                    var records = (sender as SfDataGrid).View.Records;

                    double val1 = 0;
                    double val2 = 0;
                    double val3 = 0;

                    for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                    {
                        val1 += Convert.ToDouble(provider.GetValue(records[i].Data, "cantidad").ToString());
                        val2 += Convert.ToDouble(provider.GetValue(records[i].Data, "can_dev").ToString());
                        val3 += Convert.ToDouble(provider.GetValue(records[i].Data, "total").ToString());
                    }
                    Total_PR1.Text = (sender as SfDataGrid).View.Records.Count.ToString();
                    Total_PR2.Text = val1.ToString();
                    Total_PR3.Text = val2.ToString();
                    Total_PR4.Text = val3.ToString("C");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error-f" + w);
            }
        }



    }
}


