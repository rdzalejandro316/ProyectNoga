using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SiasoftAppExt
{
    /// Sia.PublicarPnt(9626,"Costeo");
    /// Sia.TabU(9626);
    public partial class Costeo : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string sqlerror = "";
        string nitEmp = "";

        public Costeo(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            idemp = SiaWin._BusinessId;
            CargarEmpresas();
            LoadConfig();
            Total1.Text = "0";
            Total2.Text = "0.00";
            Total2a.Text = "0.00";
            Total3.Text = "0";
            Total4.Text = "0";
            Total5.Text = "0";
            Total6.Text = "0";
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
                nitEmp = foundRow["BusinessNit"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Costeo";
                Fec.Value = DateTime.Now.ToShortDateString();
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
                if (string.IsNullOrEmpty(Fec.Value.ToString()))
                {
                    MessageBox.Show("llene los campos de las fecha", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                if (comboBoxEmpresas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una empresa", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                GridCosteo.ClearFilters();
                GridCosteo.ItemsSource = null;
                GridConta.ClearFilters();
                GridConta.ItemsSource = null;
                BtnEjecutar.IsEnabled = false;
                DateTime fec = Convert.ToDateTime(Fec.Value.ToString());
                int fecha = fec.Year;
                DateTime per = Convert.ToDateTime(Periodo.Value);
                int periodo = per.Month;
                sqlerror = "";
                bool contabiliza = CheckContabilisa.IsChecked == true ? true : false;
                bool actualizaCostos = CheckActualizaCosto.IsChecked == true ? true : false;
                string codemp = comboBoxEmpresas.SelectedValue.ToString();
                tabitem.Progreso(true);
                SiaWin.Auditor(0, "Ejecuto Costeo Año:"+fecha.ToString()+" Periodo:"+periodo.ToString()+" Empresa:"+codemp+" Actualizar costos="+actualizaCostos.ToString()+" Generar Documento contable:"+contabiliza.ToString() , 2, 194);
                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fecha, periodo, contabiliza,actualizaCostos, codemp, source.Token), source.Token);
                await slowTask;
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                //MessageBox.Show(((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString());
                if(((DataSet)slowTask.Result)==null)
                {
                    BtnEjecutar.IsEnabled = true;
                    tabitem.Progreso(false);
                    this.sfBusyIndicator.IsBusy = false;
                    GridConfiguracion.IsEnabled = true;
                    if(sqlerror=="") MessageBox.Show("Error al cargar datos ó Periodo sin información:"+ sqlerror);
                    if (sqlerror != "") MessageBox.Show( sqlerror);
                    return;
                }
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    GridCosteo.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    int errores = ((DataSet)slowTask.Result).Tables[1].Rows.Count;
                    //MessageBox.Show("errores:" + errores.ToString());
                    GridCosteoErrores.ItemsSource = ((DataSet)slowTask.Result).Tables[1];
                    if (errores == 0)
                    {
                        //rowerrors.Height. = HeightMode.Auto;
                        GridCosteoErrores.Height = 0;
                        GridCosteoErrores.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        GridCosteoErrores.Height =200;
                        //rowerrors.Height = 140;
                        GridCosteoErrores.Visibility = Visibility.Visible;
                    }
                    //TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;
                    double rows = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Rows.Count);
                    double Cant = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cantidad)", "tip_trn=1"));
                    double Cants = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cantidad)", "tip_trn=2"));
                    double cos_tot = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cos_tot)", ""));
                    double cose_tot = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cose_tot)", ""));
                    double coss_tot = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(coss_tot)", ""));
                    double cos_totn = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cos_totn)", ""));
                    Total1.Text = rows.ToString();
                    Total2.Text = Cant.ToString("N2");
                    Total2a.Text = Cants.ToString("N2");
                    Total3.Text = cos_tot.ToString("C");
                    Total4.Text = cose_tot.ToString("C");
                    Total5.Text = coss_tot.ToString("C");
                    Total6.Text = cos_totn.ToString("C");
                    if (((DataSet)slowTask.Result).Tables[2].Rows.Count > 0) SiaWin.Browse((((DataSet)slowTask.Result).Tables[2]));
                    RegErrores.Text = ((DataSet)slowTask.Result).Tables[1].Rows.Count.ToString();
                    if(contabiliza==true)  GridConta.ItemsSource = ((DataSet)slowTask.Result).Tables[3];
                    if(contabiliza == true) GridContaN.ItemsSource = ((DataSet)slowTask.Result).Tables[4];
                    BussineName.Text = returnEmpresas();
                }
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (SqlException ex)
            {
                //this.Opacity = 1;
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                MessageBox.Show(ex.Message);
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
        public string returnEmpresas()
        {
            DataRowView oDataRowView = comboBoxEmpresas.SelectedItem as DataRowView;
            string sValue = "inicial";

            if (oDataRowView != null)
            {
                sValue = oDataRowView.Row["businessname"] as string;
            }
            return sValue;
        }
        private DataSet LoadData(int fecha, int periodo, bool contabiliza,bool actualizacosto, string empresas, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpCostos", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ano", fecha);
                cmd.Parameters.AddWithValue("@Per", periodo);
                cmd.Parameters.AddWithValue("@Cnt", contabiliza);
                cmd.Parameters.AddWithValue("@Actualiza", actualizacosto);                
                cmd.Parameters.AddWithValue("@codemp", empresas);
                cmd.Parameters.AddWithValue("@tipoError", "1");
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (SqlException ex)
            {
                sqlerror = ex.Message;
                return null;
            }
        }
        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            tabitem.Cerrar(0);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                SfDataGrid sfdg = new SfDataGrid();
                if (((Button)sender).Tag.ToString() == "1") sfdg = GridCosteo;
                if (((Button)sender).Tag.ToString() == "2") sfdg = GridCosteoErrores;

                var excelEngine = sfdg.ExportToExcel(sfdg.View, options);
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
        private void dataGrid_FilterChanged(object sender, Syncfusion.UI.Xaml.Grid.GridFilterEventArgs e)
        {
            try
            {
                var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                var records = (sender as SfDataGrid).View.Records;

                double cantidad = 0;
                double cos_tot = 0;
                double cose_tot = 0;
                double coss_tot = 0;
                double cos_totn = 0;

                double val;

                for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                {
                    cantidad += Convert.ToDouble(                         
                        provider.GetValue(records[i].Data, "cantidad") == DBNull.Value || double.TryParse(provider.GetValue(records[i].Data, "cantidad").ToString(), out val) == false ? 
                            0 : provider.GetValue(records[i].Data, "cantidad")
                        );

                    cos_tot += Convert.ToDouble(
                            provider.GetValue(records[i].Data, "cos_tot") == DBNull.Value || double.TryParse(provider.GetValue(records[i].Data, "cos_tot").ToString(), out val) == false ?
                            0 : provider.GetValue(records[i].Data, "cos_tot")                        
                        );

                    cose_tot += Convert.ToDouble(
                            provider.GetValue(records[i].Data, "cose_tot") == DBNull.Value || double.TryParse(provider.GetValue(records[i].Data, "cose_tot").ToString(), out val) == false ?
                            0 : provider.GetValue(records[i].Data, "cose_tot")   
                        );

                    coss_tot += Convert.ToDouble(
                            provider.GetValue(records[i].Data, "coss_tot") == DBNull.Value || double.TryParse(provider.GetValue(records[i].Data, "coss_tot").ToString(), out val) == false ?
                            0 : provider.GetValue(records[i].Data, "coss_tot")                        
                        );

                    cos_totn += Convert.ToDouble(
                            provider.GetValue(records[i].Data, "cos_totn") == DBNull.Value || double.TryParse(provider.GetValue(records[i].Data, "cos_totn").ToString(), out val) == false ?
                            0 : provider.GetValue(records[i].Data, "cos_totn")                        
                        );

                }

                Total1.Text = GridCosteo.View.Records.Count.ToString();
                Total2.Text = cantidad.ToString();
                Total3.Text = cos_tot.ToString("C");
                Total4.Text = cose_tot.ToString("C");
                Total5.Text = coss_tot.ToString("C");
                Total6.Text = cos_totn.ToString("C");
            }
            catch (Exception w)
            {
                MessageBox.Show("error-f" + w);
            }
        }

        private void BtnConciliar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime fec = Convert.ToDateTime(Fec.Value.ToString());
                int fecha = fec.Year;

                DateTime per = Convert.ToDateTime(Periodo.Value);
                int periodo = per.Month;
                sqlerror = "";
                bool contabiliza = CheckContabilisa.IsChecked == true ? true : false;
                bool actualizaCostos = CheckActualizaCosto.IsChecked == true ? true : false;
                string codemp = comboBoxEmpresas.SelectedValue.ToString();

                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpCostosCovsIn", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ano", fecha);
                cmd.Parameters.AddWithValue("@Periodo", periodo);
                cmd.Parameters.AddWithValue("@codemp", codemp);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();

                if(ds.Tables[0].Rows.Count>0) SiaWin.Browse(ds.Tables[0]);
                if (ds.Tables[1].Rows.Count> 0) SiaWin.Browse(ds.Tables[1]);
            }
            catch (Exception w)
            {
                MessageBox.Show("error-f" + w.Message);
            }
        }

        private void dataGridConsulta_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {

                DataRowView row = (DataRowView)GridCosteo.SelectedItems[0];
                if (row == null)
                {
                    MessageBox.Show("Registro sin datos");
                    return;
                }
                

                string cod_trn = row["cod_trn"].ToString().Trim();
                string num_trn = row["num_trn"].ToString().Trim();
                
                DataTable dt = SiaWin.Func.SqlDT("select * from incab_doc where num_trn='" + num_trn + "' and cod_trn='" + cod_trn+"' ", "referencia", IdEmprSel());
                
                if (dt.Rows.Count>0)
                {
                    int id = Convert.ToInt32(dt.Rows[0]["idreg"]);
                    SiaWin.TabTrn(0, IdEmprSel(), true, id, 2, WinModal: true);
                }              
            }
            catch (Exception w)
            {
               MessageBox.Show("aa:"+w);
            }
        }

        public int IdEmprSel()
        {
            string codemp = comboBoxEmpresas.SelectedValue.ToString();
            DataTable dt = SiaWin.Func.SqlDT("select BusinessId from Business where BusinessCode='"+codemp+"'", "referencia", 0);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["BusinessId"]) : 0;
        } 

        private void BtnKardex_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (GridCosteo.SelectedIndex>=0)
                {
                    DataRowView row = (DataRowView)GridCosteo.SelectedItems[0];
                    if (row == null)
                    {
                        MessageBox.Show("Registro sin datos");
                        return;
                    }

                    string cod_bod = row["cod_bod"].ToString().Trim();
                    string cod_ref = row["cod_ref"].ToString().Trim();
                    string codemp = comboBoxEmpresas.SelectedValue.ToString();
                    dynamic w = SiaWin.WindowExt(9466, "Kardex");
                    w.ShowInTaskbar = false;
                    w.Owner = Application.Current.MainWindow;
                    w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    w.codref = cod_ref;            
                    w.codbod = cod_bod;
                    w.codemp = codemp;
                    w.ShowDialog();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("errror al abrir kardex:"+w);
            }
        }



    }
}
