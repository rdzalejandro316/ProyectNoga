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

    /// Sia.PublicarPnt(9590,"AnalisisCostoR18");
    /// Sia.TabU(9590);
    public partial class AnalisisCostoR18 : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string sqlerror = "";

        public AnalisisCostoR18(dynamic tabitem1)
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
            sb.Append("select businessid, businesscode, businessname, Businessalias from business where (select Seg_AccProjectBusiness.Access from Seg_AccProjectBusiness where GroupId = "+SiaWin._UserGroup.ToString()+"  and ProjectId = " + SiaWin._ProyectId.ToString()+" and Access = 1 and Business.BusinessId = Seg_AccProjectBusiness.BusinessId)= 1");
            //DataTable empresas = SiaWin.Func.SqlDT("select businesscode,businessname,businessnit from Business where BusinessStatus='1' ", "Empresas", 0);
            DataTable empresas = SiaWin.Func.SqlDT(sb.ToString(), "Empresas", 0);
            comboBoxEmpresas.ItemsSource = empresas.DefaultView;
        }

        public int TraeIdEmpresa(string code)
        {
            int idreturn = -1;
            try
            {
               
                StringBuilder sb = new StringBuilder();
                sb.Append("select BusinessId from business where  Business.BusinessCode ='" + code + "'");
                //DataTable empresas = SiaWin.Func.SqlDT("select businesscode,businessname,businessnit from Business where BusinessStatus='1' ", "Empresas", 0);
                DataTable empresas = SiaWin.Func.SqlDT(sb.ToString(), "Empresas", 0);
                if (empresas.Rows.Count > 0) idreturn = Convert.ToInt32(empresas.Rows[0]["BusinessId"].ToString());
            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return idreturn;
            
        }


        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                //cnEmp = foundRow["BusinessCn"].ToString().Trim();
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Analisis Costo R18";
                FecIni.Text = DateTime.Now.ToShortDateString();
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
                VentasPorProducto.ClearFilters();
                VentasPorProducto.ItemsSource = null;


                BtnEjecutar.IsEnabled = false;
                //source.CancelAfter(TimeSpan.FromSeconds(1));
                string fechaCon = FecIni.Text.ToString();

                string empresas = returnEmpresas();


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fechaCon, empresas, source.Token), source.Token);
                await slowTask;
                
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                //MessageBox.Show(((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString());

                if (((DataSet)slowTask.Result) == null)
                {
                    BtnEjecutar.IsEnabled = true;
                    tabitem.Progreso(false);
                    this.sfBusyIndicator.IsBusy = false;
                    GridConfiguracion.IsEnabled = true;
                    if (sqlerror == "") MessageBox.Show("Error al cargar datos ó Periodo sin información:" + sqlerror);
                    if (sqlerror != "") MessageBox.Show(sqlerror,"Task");
                    return;

                }

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    VentasPorProducto.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;


                    double rows = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Rows.Count);
                    double CantNeto = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(saldo_fin)", ""));
                    double CantCosTot = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cos_tot)", ""));
                    double CantCosTotN = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cos_totn)", ""));

                    Total1.Text = rows.ToString();
                    Total2.Text = CantNeto.ToString();
                    Total3.Text = CantCosTot.ToString("C");
                    Total4.Text = CantCosTotN.ToString("C");


                    System.Data.DataTable AgruEmpres = new System.Data.DataTable();
                    System.Data.DataTable dtEmpresa = ((DataSet)slowTask.Result).Tables[0];

                    if (dtEmpresa.Rows.Count > 0)
                    {
                        AgruEmpres = dtEmpresa.AsEnumerable()
                            .GroupBy(a => a["nom_emp"].ToString().Trim())
                            .Select(c =>
                            {
                                var row = ((DataSet)slowTask.Result).Tables[0].NewRow();
                                row["nom_emp"] = c.Key;
                                row["saldo_fin"] = c.Sum(a => a.Field<decimal>("saldo_fin"));
                                return row;
                            }).CopyToDataTable();
                    }
                    //SiaWin.Browse(AgruEmpres);
                    ChartCircle.ItemsSource = AgruEmpres;


                    System.Data.DataTable AgruEmpresCosto = new System.Data.DataTable();

                    if (dtEmpresa.Rows.Count > 0)
                    {
                        AgruEmpresCosto = dtEmpresa.AsEnumerable()
                            .GroupBy(a => a["nom_emp"].ToString().Trim())
                            .Select(c =>
                            {
                                var row = ((DataSet)slowTask.Result).Tables[0].NewRow();
                                row["nom_emp"] = c.Key;
                                row["cos_tot"] = c.Sum(a => a.Field<decimal>("cos_tot"));
                                return row;
                            }).CopyToDataTable();
                    }

                    //SiaWin.Browse(AgruEmpres);
                    chartCostos.ItemsSource = AgruEmpresCosto;

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
                MessageBox.Show(ex.Message,"BtnClick");

            }
        }
        private DataSet LoadData(string fechaCon, string empresas, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                //cmd = new SqlCommand("SpConsultaInAnalisisDeVentas", con);
                cmd = new SqlCommand("_EmpSaldosInventariosPorBodegaLineaEmpresasConsulta", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Fecha", fechaCon);
                cmd.Parameters.AddWithValue("@codemp", empresas);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;

            }
            catch (SqlException ex)
            {
                //this.Opacity = 1;
                //BtnEjecutar.IsEnabled = true;
                //tabitem.Progreso(false);
                //this.sfBusyIndicator.IsBusy = false;
                //GridConfiguracion.IsEnabled = true;
                sqlerror = ex.Message;
                //MessageBox.Show(ex.Message);
                return null;
            }

            catch (Exception e)
            {
                sqlerror = e.Message;
                //MessageBox.Show(e.Message);
                return null;
            }
        }


        public string returnEmpresas()
        {
            string empresas = "";
            //if (comboBoxEmpresas.SelectedIndex > 0)
            //{
            foreach (DataRowView ob in comboBoxEmpresas.SelectedItems)
            {
                String valueCta = ob["BusinessCode"].ToString();
                empresas += valueCta + ",";
            }
            string ss = empresas.Trim().Substring(empresas.Trim().Length - 1);
            if (ss == ",") empresas = empresas.Substring(0, empresas.Trim().Length - 1);
            //}
            return empresas;
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }


        private void dataGrid_FilterChanged(object sender, GridFilterEventArgs e)
        {
            try
            {                
                var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                var records = (sender as SfDataGrid).View.Records;
                
                double CantNeto = 0;
                double CantCosUni = 0;
                double CantCosTot = 0;
                double costotn = 0;

                for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                {                    
                    CantNeto += Convert.ToDouble(provider.GetValue(records[i].Data, "saldo_fin").ToString());
                    CantCosTot += Convert.ToDouble(provider.GetValue(records[i].Data, "cos_tot").ToString());
                    costotn += Convert.ToDouble(provider.GetValue(records[i].Data, "cos_totn").ToString());
                }                

                Total1.Text = VentasPorProducto.View.Records.Count.ToString();
                Total2.Text = CantNeto.ToString();
                Total3.Text = CantCosTot.ToString("C");
                Total4.Text = costotn.ToString("C");                             

                
            }
            catch (Exception w)
            {
                MessageBox.Show("error-f" + w);
            }
        }





        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;
            e.Range.CellStyle.Font.FontName = "Segoe UI";
            if (e.ColumnName == "saldo_fin" || e.ColumnName == "cos_uni" || e.ColumnName == "cos_tot" || e.ColumnName == "cos_unin" || e.ColumnName == "cos_totn")
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }
            if (e.ColumnName == "cod_emp" || e.ColumnName == "cod_tip" || e.ColumnName == "cod_bod" || e.ColumnName == "cod_ref" )
            {
                string value = e.CellValue.ToString();
               
                e.Range.Text = value;
                e.Handled = true;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExportMode = ExportMode.Value;
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;
                var excelEngine = VentasPorProducto.ExportToExcel(VentasPorProducto.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.ActiveSheet.Columns[0].NumberFormat = "000";
                workBook.ActiveSheet.Columns[2].NumberFormat = "000";
                workBook.ActiveSheet.Columns[4].NumberFormat = "000";

                workBook.ActiveSheet.Columns[8].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[9].NumberFormat = "0.0";
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
            catch (Exception ex)
            {
                MessageBox.Show("erro2:" + ex);
                this.Opacity = 1;
            }
        }

        private void BtnHidden_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString().Trim();
            if (tag == "0")
            {
                BtnHidden.Content = "Mostrar Graficos";
                (sender as Button).Tag = "1";

                Chart1.Visibility = Visibility.Hidden;
                Chart2.Visibility = Visibility.Hidden;

                //Grid Grid_1 = new Grid();
                //Grid.SetRow(Grid_1, 0);
                //ColumnDefinition colm1 = new ColumnDefinition() { Width = new GridLength(350, GridUnitType.Star) };

                Grid.SetRowSpan(GridSpan, 2);
                Grid.SetColumnSpan(VentasPorProducto, 2);

            }
            else
            {
                BtnHidden.Content = "Ocultar Graficos";
                (sender as Button).Tag = "0";

                Chart1.Visibility = Visibility.Visible;
                Chart2.Visibility = Visibility.Visible;
                Grid.SetRowSpan(GridSpan, 1);
                Grid.SetColumnSpan(VentasPorProducto, 1);
            }
        }

        private void BtnKardex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)VentasPorProducto.SelectedItems[0];
                dynamic w = SiaWin.WindowExt(9466, "Kardex");
                w.ShowInTaskbar = false;
                w.Owner = Application.Current.MainWindow;
                w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                w.codref = row["cod_ref"].ToString();                
                w.codbod = row["cod_bod"].ToString();
                w.codemp = row["cod_emp"].ToString();
                w.fechacorte = FecIni.SelectedDate.Value;
                int idempresa = TraeIdEmpresa(w.codemp);
                if(idempresa<0)
                {
                    MessageBox.Show("No existe idEmpresa con el codigo .:" + w.codemp);
                    return;
                }
                w.idemp = idempresa;

                w.ShowDialog();

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el kardex:"+w);
            }
        }
    }
}
