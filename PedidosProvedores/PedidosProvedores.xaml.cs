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
    /// Sia.PublicarPnt(9600,"PedidosProvedores");
    /// Sia.TabU(9600);
    public partial class PedidosProvedores : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public PedidosProvedores(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            idemp = SiaWin._BusinessId;            
            LoadConfig();
            CargarEmpresas();
        }

        public void CargarEmpresas()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select businessid, businesscode, businessname, businessalias from business where (select Seg_AccProjectBusiness.Access from Seg_AccProjectBusiness where GroupId = " + SiaWin._UserGroup.ToString() + "  and ProjectId = " + SiaWin._ProyectId.ToString() + " and Access = 1 and Business.BusinessId = Seg_AccProjectBusiness.BusinessId)= 1");           
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
                tabitem.Title = "Pedidos Provedores";                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
                DataGridSf.ClearFilters();
                DataGridSf.ItemsSource = null;

                BtnEjecutar.IsEnabled = false;
                string dias = TextBox_dias.Value.ToString();
                string empresas = returnEmpresas();

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(dias, empresas, source.Token), source.Token);
                await slowTask;
                                
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    DataGridSf.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;


                    double rows = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Rows.Count);
                    double cnt_ped = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cant_ped)", ""));
                    double cnt_ent = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cant_recib)", ""));
                    double cnt_pend = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cant_pend)", ""));

                    Total1.Text = rows.ToString();
                    Total2.Text = cnt_ped.ToString();
                    Total3.Text = cnt_ent.ToString();
                    Total4.Text = cnt_pend.ToString();

                    System.Data.DataTable AgruEmpresPedido = new System.Data.DataTable();
                    System.Data.DataTable dtEmpresa = ((DataSet)slowTask.Result).Tables[0];

                    if (dtEmpresa.Rows.Count > 0)
                    {
                        AgruEmpresPedido = dtEmpresa.AsEnumerable()
                            .GroupBy(a => a["nom_emp"].ToString().Trim())
                            .Select(c =>
                            {
                                var row = ((DataSet)slowTask.Result).Tables[0].NewRow();
                                row["nom_emp"] = c.Key;
                                row["cant_ped"] = c.Sum(a => a.Field<decimal>("cant_ped"));
                                return row;
                            }).CopyToDataTable();
                    }
                    Chart1.ItemsSource = AgruEmpresPedido;


                    System.Data.DataTable AgruEmpresReibido = new System.Data.DataTable();
                    if (dtEmpresa.Rows.Count > 0)
                    {
                        AgruEmpresReibido = dtEmpresa.AsEnumerable()
                            .GroupBy(a => a["nom_emp"].ToString().Trim())
                            .Select(c =>
                            {
                                var row = ((DataSet)slowTask.Result).Tables[0].NewRow();
                                row["nom_emp"] = c.Key;
                                row["cant_recib"] = c.Sum(a => a.Field<decimal>("cant_recib"));
                                return row;
                            }).CopyToDataTable();
                    }
                    Chart2.ItemsSource = AgruEmpresReibido;


                    System.Data.DataTable AgruEmpresPendiente = new System.Data.DataTable();
                    if (dtEmpresa.Rows.Count > 0)
                    {
                        AgruEmpresPendiente = dtEmpresa.AsEnumerable()
                            .GroupBy(a => a["nom_emp"].ToString().Trim())
                            .Select(c =>
                            {
                                var row = ((DataSet)slowTask.Result).Tables[0].NewRow();
                                row["nom_emp"] = c.Key;
                                row["cant_pend"] = c.Sum(a => a.Field<decimal>("cant_pend"));
                                return row;
                            }).CopyToDataTable();
                    }
                    Chart3.ItemsSource = AgruEmpresPendiente;

                }

                BtnEjecutar.IsEnabled = true;
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (Exception ex)
            {                
                BtnEjecutar.IsEnabled = true;             
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                MessageBox.Show(ex.Message);

            }
        }


        private DataSet LoadData(string dias, string empresas, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();                
                cmd = new SqlCommand("_EmpAnalisPedidoProvede_MultiEmpresa", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Fecha_dias", dias);
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
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                MessageBox.Show(e.Message);
                return null;
            }
        }



        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;

                var excelEngine = DataGridSf.ExportToExcel(DataGridSf.View, options);
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
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:"+w);
            }
        }

        private void BtnHidden_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString().Trim();
            if (tag == "0")
            {
                BtnHidden.Content = "Mostrar Graficos";
                (sender as Button).Tag = "1";

                Grafico1.Visibility = Visibility.Hidden;
                Grafico2.Visibility = Visibility.Hidden;
                Grafico3.Visibility = Visibility.Hidden;
                Grid.SetRowSpan(GridSpan, 2);                
            }
            else
            {
                BtnHidden.Content = "Ocultar Graficos";
                (sender as Button).Tag = "0";

                Grafico1.Visibility = Visibility.Visible;
                Grafico2.Visibility = Visibility.Visible;
                Grafico3.Visibility = Visibility.Visible;
                Grid.SetRowSpan(GridSpan, 1);                
            }
        }

        private void VentasPorProducto_FilterChanged(object sender, Syncfusion.UI.Xaml.Grid.GridFilterEventArgs e)
        {
            try
            {
                var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                var records = (sender as SfDataGrid).View.Records;

                double cnt_ped = 0;
                double cnt_ent = 0;
                double cnt_pend = 0;

                for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                {
                    cnt_ped += Convert.ToDouble(provider.GetValue(records[i].Data, "cant_ped").ToString());
                    cnt_ent += Convert.ToDouble(provider.GetValue(records[i].Data, "cant_recib").ToString());
                    cnt_pend += Convert.ToDouble(provider.GetValue(records[i].Data, "cant_pend").ToString());
                }

                Total1.Text = DataGridSf.View.Records.Count.ToString();
                Total2.Text = cnt_ped.ToString();
                Total3.Text = cnt_ent.ToString();
                Total4.Text = cnt_pend.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error-f" + w);
            }
        }







    }
}
