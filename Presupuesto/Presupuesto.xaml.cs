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
    /// Sia.PublicarPnt(9676,"Presupuesto");
    /// Sia.TabU(9676);

    public partial class Presupuesto : UserControl
    {

        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public Presupuesto(dynamic tabitem1)
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
                tabitem.Title = "Presupuesto";                
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
                if (comboBoxEmpresas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una o mas empresas", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                GridOne.ClearFilters();
                GridOne.ItemsSource = null;


                BtnEjecutar.IsEnabled = false;                
                string empresas = comboBoxEmpresas.SelectedValue.ToString();
                decimal porcen = Convert.ToDecimal(tx_por.Value);
                DateTime fec = Convert.ToDateTime(Fec.Value.ToString());
                string fecha = fec.Year.ToString();
                string ven = returnVendedor();
                          
                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(empresas,fecha, ven,porcen,source.Token), source.Token);
                await slowTask;
                BtnEjecutar.IsEnabled = true;
                
                if (((DataSet)slowTask.Result) == null)
                {
                    BtnEjecutar.IsEnabled = true;
                    tabitem.Progreso(false);
                    this.sfBusyIndicator.IsBusy = false;
                    GridConfiguracion.IsEnabled = true;                    
                    return;
                }

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    GridOne.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Total1.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    double rows = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Rows.Count);
                    double val_pres_ano = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(val_pres_ano)", ""));
                    double cnt_pres_ano = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cnt_pres_ano)", ""));
                    double val_pres_mes = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(val_pres_mes)", ""));
                    double cnt_pres_mes = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cnt_pres_mes)", ""));

                    Total1.Text = rows.ToString();
                    Total2.Text = val_pres_ano.ToString("C");
                    Total3.Text = cnt_pres_ano.ToString();
                    Total4.Text = val_pres_mes.ToString("C");
                    Total5.Text = cnt_pres_mes.ToString();

                    ChartEmpVenAno.ItemsSource = ((DataSet)slowTask.Result).Tables[1];
                    ChartEmpVenMes.ItemsSource = ((DataSet)slowTask.Result).Tables[1];

                    GridTwo.ItemsSource = ((DataSet)slowTask.Result).Tables[2];
                    tx_vendedor.Text = ((DataSet)slowTask.Result).Tables[2].Rows.Count.ToString();

                    GridThree.ItemsSource = ((DataSet)slowTask.Result).Tables[3];
                    tx_bodega.Text = ((DataSet)slowTask.Result).Tables[3].Rows.Count.ToString();

                    GridFour.ItemsSource = ((DataSet)slowTask.Result).Tables[4];
                    tx_linea.Text = ((DataSet)slowTask.Result).Tables[4].Rows.Count.ToString();

                    GridFive.ItemsSource = ((DataSet)slowTask.Result).Tables[5];
                    tx_provedor.Text = ((DataSet)slowTask.Result).Tables[5].Rows.Count.ToString();

                    GridSix.ItemsSource = ((DataSet)slowTask.Result).Tables[6];
                    tx_ven.Text = ((DataSet)slowTask.Result).Tables[6].Rows.Count.ToString();

                    if (((DataSet)slowTask.Result).Tables[6].Rows.Count>0)
                    {
                        double val_pres_ano_v = Convert.ToDouble(((DataSet)slowTask.Result).Tables[6].Compute("Sum(val_pres_ano)", ""));
                        double cnt_pres_ano_v = Convert.ToDouble(((DataSet)slowTask.Result).Tables[6].Compute("Sum(cnt_pres_ano)", ""));
                        double val_pres_mes_v = Convert.ToDouble(((DataSet)slowTask.Result).Tables[6].Compute("Sum(val_pres_mes)", ""));
                        double cnt_pres_mes_v = Convert.ToDouble(((DataSet)slowTask.Result).Tables[6].Compute("Sum(cnt_pres_mes)", ""));
                        double rows_v = Convert.ToDouble(((DataSet)slowTask.Result).Tables[6].Rows.Count);

                        VTotal1.Text = rows_v.ToString();
                        VTotal2.Text = val_pres_ano.ToString("C");
                        VTotal3.Text = cnt_pres_ano.ToString();
                        VTotal4.Text = val_pres_mes.ToString("C");
                        VTotal5.Text = cnt_pres_mes.ToString();
                    }

                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;                    
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
                MessageBox.Show("error en la consulta"+ex, "BtnClick");

            }
        }


        private DataSet LoadData(string empresas, string ano,string codven, decimal por,CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();               
                cmd = new SqlCommand("_EmpSpPresupuesto", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@por", por);
                cmd.Parameters.AddWithValue("@ano", ano);
                cmd.Parameters.AddWithValue("@codven", codven);
                cmd.Parameters.AddWithValue("@codemp", empresas);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;

            }
            catch (Exception e)
            {             
                return null;
            }
        }

        public string returnVendedor()
        {
            string ven = "";            
            foreach (DataRowView ob in comboBoxVendedor.SelectedItems)
            {
                String valueCta = ob["cod_mer"].ToString().Trim();
                ven += valueCta + ",";
            }
            string ss = ven.Trim().Substring(ven.Trim().Length - 1);
            if (ss == ",") ven = ven.Substring(0, ven.Trim().Length - 1);            
            return ven;
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }


        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 10;
            e.Range.CellStyle.Font.FontName = "Segoe UI";
            if (e.ColumnName == "val_pres_ano" || e.ColumnName == "cnt_pres_ano" || e.ColumnName == "val_pres_mes" || e.ColumnName == "cnt_pres_mes")
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }

            if (e.ColumnName == "cod_emp" || e.ColumnName == "cod_tip" || e.ColumnName == "cod_bod" || e.ColumnName == "cod_ref" || e.ColumnName == "cod_prv" || e.ColumnName == "cod_ven")
            {
                string value = e.CellValue.ToString();
                e.Range.Text = value;
                e.Handled = true;
            }

        }



        private void btn_excel_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SfDataGrid sfdg = new SfDataGrid();
                if (((Button)sender).Tag.ToString() == "1") sfdg = GridOne;
                if (((Button)sender).Tag.ToString() == "2") sfdg = GridTwo;
                if (((Button)sender).Tag.ToString() == "3") sfdg = GridThree;
                if (((Button)sender).Tag.ToString() == "4") sfdg = GridFour;
                if (((Button)sender).Tag.ToString() == "5") sfdg = GridFive;
                if (((Button)sender).Tag.ToString() == "6") sfdg = GridSix;

                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExportMode = ExportMode.Value;
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;
                var excelEngine = sfdg.ExportToExcel(sfdg.View, options);
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
                    
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {                        
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

        private void comboBoxEmpresas_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (comboBoxEmpresas.SelectedIndex>=0)
                {
                    string cod_emp = comboBoxEmpresas.SelectedValue.ToString();

                    int id = 0;
                    foreach (DataRowView ob in comboBoxEmpresas.SelectedItems)
                        { id = Convert.ToInt16(ob["businessid"]);}

                    if (id>0)
                    {
                        DataTable vendedores = SiaWin.Func.SqlDT("select rtrim(cod_mer) as cod_mer,rtrim(cod_mer)+' - '+rtrim(nom_mer) as nom_mer from inmae_mer", "vendedores", id);
                        comboBoxVendedor.ItemsSource = vendedores.DefaultView;
                    }
                   
                }
                
                    
                

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar vendedore:"+w);
            }
        }


    }
}
