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
    /// Sia.PublicarPnt(9706,"AnalisisDeVentaMultiEmpresa");
    /// Sia.TabU(9706);
    public partial class AnalisisDeVentaMultiEmpresa : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        int moduloid = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public AnalisisDeVentaMultiEmpresa(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.MultiTab = true;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            if (tabitem.idemp <= 0) idemp = SiaWin._BusinessId;
            LoadConfig();
            CargarEmpresas();
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
                //cnEmp = foundRow["BusinessCn"].ToString().Trim();
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Analisis de Venta(" + aliasemp + ")";

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                FecIni.Text = DateTime.Now.ToShortDateString();
                FecFin.Text = DateTime.Now.ToShortDateString();

                TabControl1.SelectedIndex = 0;
                int grupo = SiaWin._UserGroup;
                string cod_grupo = "";

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show("aqui88");


            }
        }



        private string ArmaWhere()
        {
            string cadenawhere = null;
            string RefI = TextBoxRefI.Text.Trim();
            string TipI = TextBoxTipI.Text.Trim();
            string GruI = TextBoxGrpI.Text.Trim();


            if (!string.IsNullOrEmpty(RefI))
            {
                cadenawhere += " and  cue.cod_ref='" + RefI + "' ";
            }
            if (!string.IsNullOrEmpty(TipI))
            {
                cadenawhere += " and  ref.cod_tip='" + TipI + "'";
            }
            if (!string.IsNullOrEmpty(GruI))
            {
                cadenawhere += " and  ref.cod_gru='" + GruI + "'";
            }

            return cadenawhere;
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

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (comboBoxEmpresas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una o mas empresas", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                string where = ArmaWhere();

                if (string.IsNullOrEmpty(where)) where = " ";

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                VentasPorProducto.ItemsSource = null;
                VentaPorBodega.ItemsSource = null;
                VentasPorCliente.ItemsSource = null;
                VentasPorLinea.ItemsSource = null;                

                CharVentasBodega.DataContext = null;
                AreaSeriesVta.ItemsSource = null;

                BtnEjecutar.IsEnabled = false;

                string empresas = returnEmpresas();

                string ffi = FecIni.Text.ToString();
                string fff = FecFin.Text.ToString();
                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ffi, fff, where, empresas, source.Token), source.Token);
                await slowTask;

                BtnEjecutar.IsEnabled = true;                
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    VentasPorProducto.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Total1.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    VentaPorBodega.ItemsSource = ((DataSet)slowTask.Result).Tables[1];
                    Total2.Text = ((DataSet)slowTask.Result).Tables[1].Rows.Count.ToString();

                    CharVentasBodega.DataContext = ((DataSet)slowTask.Result).Tables[1];
                    AreaSeriesVta.ItemsSource = ((DataSet)slowTask.Result).Tables[1];

                    VentasPorCliente.ItemsSource = ((DataSet)slowTask.Result).Tables[2];
                    Total3.Text = ((DataSet)slowTask.Result).Tables[2].Rows.Count.ToString();

                    VentasPorVendedor.ItemsSource = ((DataSet)slowTask.Result).Tables[3];
                    Total4.Text = ((DataSet)slowTask.Result).Tables[3].Rows.Count.ToString();

                    VentasPorLinea.ItemsSource = ((DataSet)slowTask.Result).Tables[4];
                    Total5.Text = ((DataSet)slowTask.Result).Tables[4].Rows.Count.ToString();
                    
                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;

                    //TABLA 0
                    double CantNeto = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(neto)", "").ToString());
                    double sub = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(subtotal)", "").ToString());
                    double descto = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(val_des)", "").ToString());
                    double iva = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(val_iva)", "").ToString());
                    double total = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(total)", "").ToString());
                    double costo = Convert.ToDouble(((DataSet)slowTask.Result).Tables[0].Compute("Sum(cos_tot)", "").ToString());

                    llenarTotales(sub, descto, iva, total, CantNeto, costo);

                }

                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (Exception ex)
            {
                this.Opacity = 1;
                MessageBox.Show("aqui 2.1" + ex);

            }
        }

        public void llenarTotales(double p1, double p2, double p3, double p4, double ca, double costo)
        {


            TextCantidad1.Text = ca.ToString();
            TextSubtotal1.Text = p1.ToString("C");
            TextDescuento1.Text = p2.ToString("C");
            TextIva1.Text = p3.ToString("C");
            TextTotal1.Text = p4.ToString("C");

            TextCantidad2.Text = ca.ToString();
            TextSubtotal2.Text = p1.ToString("C");
            TextDescuento2.Text = p2.ToString("C");
            TextIva2.Text = p3.ToString("C");
            TextTotal2.Text = p4.ToString("C");

            TextCantidad3.Text = ca.ToString();
            TextSubtotal3.Text = p1.ToString("C");
            TextDescuento3.Text = p2.ToString("C");
            TextIva3.Text = p3.ToString("C");
            TextTotal3.Text = p4.ToString("C");

            TextCantidad4.Text = ca.ToString();
            TextSubtotal4.Text = p1.ToString("C");
            TextDescuento4.Text = p2.ToString("C");
            TextIva4.Text = p3.ToString("C");
            TextTotal4.Text = p4.ToString("C");
            TextTotalCosto.Text = costo.ToString("C");


            TextCantidad5.Text = ca.ToString();
            TextSubtotal5.Text = p1.ToString("C");
            TextDescuento5.Text = p2.ToString("C");
            TextIva5.Text = p3.ToString("C");
            TextTotal5.Text = p4.ToString("C");

            
        }

       
        private DataSet LoadData(string Fi, string Ff, string where, string empresas, CancellationToken cancellationToken)
        {

            try
            {
                
                SqlConnection con1 = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();                
                cmd = new SqlCommand("_EmpSpConsultaInAnalisisDeVentasTipo_MultiEmpresa", con1);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", Fi);
                cmd.Parameters.AddWithValue("@FechaFin", Ff);
                cmd.Parameters.AddWithValue("@Where", where);
                cmd.Parameters.AddWithValue("@codemp", empresas);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con1.Close();                
                return ds;                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);                
                return null;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;                
                SfDataGrid sfdg = new SfDataGrid();
                if (((Button)sender).Tag.ToString() == "1") sfdg = VentasPorProducto;
                if (((Button)sender).Tag.ToString() == "2") sfdg = VentaPorBodega;
                if (((Button)sender).Tag.ToString() == "3") sfdg = VentasPorCliente;
                if (((Button)sender).Tag.ToString() == "4") sfdg = VentasPorVendedor;
                if (((Button)sender).Tag.ToString() == "5") sfdg = VentasPorLinea;                
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
                MessageBox.Show(ex.Message);
            }
        }


        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.F8)
                {
                    string tag = ((TextBox)sender).Tag.ToString();

                    if (string.IsNullOrEmpty(tag)) return;
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                    if (tag == "inmae_ref")
                    {
                        cmptabla = tag; cmpcodigo = "cod_ref"; cmpnombre = "nom_ref"; cmporden = "nom_ref"; cmpidrow = "idrow"; cmptitulo = "Maestra de productos"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "estado=1";
                    }
                    if (tag == "inmae_bod")
                    {
                        cmptabla = tag; cmpcodigo = "cod_bod"; cmpnombre = "nom_bod"; cmporden = "cod_bod"; cmpidrow = "idrow"; cmptitulo = "Maestra de bodegas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "estado=1 and ind_vta=1";
                    }
                    if (tag == "comae_ter")
                    {
                        cmptabla = tag; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "nom_ter"; cmpidrow = "idrow"; cmptitulo = "Maestra de terceros"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                    }
                    if (tag == "inmae_mer")
                    {
                        cmptabla = tag; cmpcodigo = "cod_mer"; cmpnombre = "nom_mer"; cmporden = "cod_mer"; cmpidrow = "idrow"; cmptitulo = "Maestra de vendedores"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                    }
                    if (tag == "inmae_tip")
                    {
                        cmptabla = tag; cmpcodigo = "cod_tip"; cmpnombre = "nom_tip"; cmporden = "cod_tip"; cmpidrow = "idrow"; cmptitulo = "Maestra de lineas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                    }
                    if (tag == "inmae_gru")
                    {
                        cmptabla = tag; cmpcodigo = "cod_gru"; cmpnombre = "nom_gru"; cmporden = "cod_gru"; cmpidrow = "idrow"; cmptitulo = "Maestra de grupo"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                    }

                    //MessageBox.Show(cmptabla + "-" + cmpcodigo + "-" + cmpnombre + "-" + cmporden + "-" + cmpidrow + "-" + cmptitulo + "-" + cmpconexion + "-" + cmpwhere);
                    int idr = 0; string code = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    winb = null;
                    if (idr > 0)
                    {
                        ((TextBox)sender).Text = code;
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                MessageBox.Show("aqui45");
            }

        }

        private void TextBoxRefI_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            MessageBox.Show(e.Key.ToString());
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {

            tabitem.Cerrar(0);
        }


        //*****************************************************************



        //private void BTNdetalle_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        string tag = ((Button)sender).Tag.ToString();
        //        Detalle Windows_Detalle = new Detalle();

        //        if (tag == "1")
        //        {
        //            DataRowView row = (DataRowView)VentasPorProducto.SelectedItems[0];
        //            Windows_Detalle.fecha_ini = FecIni.Text;
        //            Windows_Detalle.fecha_fin = FecFin.Text;
        //            Windows_Detalle.codigo = row["cod_ref"].ToString();
        //            Windows_Detalle.nombre = row["nom_ref"].ToString();
        //            Windows_Detalle.cnEmpExt = cnEmp;
        //        }
        //        if (tag == "2")
        //        {
        //            DataRowView row = (DataRowView)VentaPorBodega.SelectedItems[0];
        //            Windows_Detalle.fecha_ini = FecIni.Text;
        //            Windows_Detalle.fecha_fin = FecFin.Text;
        //            Windows_Detalle.codigo = row["cod_bod"].ToString();
        //            Windows_Detalle.nombre = row["nom_bod"].ToString();
        //            Windows_Detalle.cnEmpExt = cnEmp;
        //        }
        //        if (tag == "3")
        //        {
        //            DataRowView row = (DataRowView)VentasPorCliente.SelectedItems[0];
        //            Windows_Detalle.fecha_ini = FecIni.Text;
        //            Windows_Detalle.fecha_fin = FecFin.Text;
        //            Windows_Detalle.codigo = row["cod_cli"].ToString();
        //            Windows_Detalle.nombre = row["nom_cli"].ToString();
        //            Windows_Detalle.cnEmpExt = cnEmp;

        //        }
        //        if (tag == "4")
        //        {
        //            DataRowView row = (DataRowView)VentasPorLinea.SelectedItems[0];
        //            Windows_Detalle.fecha_ini = FecIni.Text;
        //            Windows_Detalle.fecha_fin = FecFin.Text;
        //            Windows_Detalle.codigo = row["cod_tip"].ToString();
        //            Windows_Detalle.nombre = row["nom_tip"].ToString();
        //            Windows_Detalle.cnEmpExt = cnEmp;
        //        }
        //        if (tag == "5")
        //        {
        //            DataRowView row = (DataRowView)VentasPorGrupo.SelectedItems[0];
        //            Windows_Detalle.fecha_ini = FecIni.Text;
        //            Windows_Detalle.fecha_fin = FecFin.Text;
        //            Windows_Detalle.codigo = row["cod_gru"].ToString();
        //            Windows_Detalle.nombre = row["nom_gru"].ToString();
        //            Windows_Detalle.cnEmpExt = cnEmp;
        //        }
        //        if (tag == "6")
        //        {
        //            DataRowView row = (DataRowView)VentasPorFPago.SelectedItems[0];
        //            Windows_Detalle.fecha_ini = FecIni.Text;
        //            Windows_Detalle.fecha_fin = FecFin.Text;
        //            Windows_Detalle.codigo = row["cod_fpag"].ToString();
        //            Windows_Detalle.nombre = row["nom_pag"].ToString();
        //            Windows_Detalle.cnEmpExt = cnEmp;
        //        }
        //        if (tag == "7")
        //        {
        //            DataRowView row = (DataRowView)VentasPorVendedor.SelectedItems[0];
        //            Windows_Detalle.fecha_ini = FecIni.Text;
        //            Windows_Detalle.fecha_fin = FecFin.Text;
        //            Windows_Detalle.codigo = row["cod_ven"].ToString();
        //            Windows_Detalle.nombre = row["nom_ven"].ToString();
        //            Windows_Detalle.cnEmpExt = cnEmp;
        //        }


        //        Windows_Detalle.tagBTN = tag;
        //        Windows_Detalle.ShowInTaskbar = false;
        //        Windows_Detalle.Owner = Application.Current.MainWindow;
        //        Windows_Detalle.ShowDialog();

        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Selecione una casilla del Grid");
        //    }
        //}



        private void dataGrid_FilterChanged(object sender, GridFilterEventArgs e)
        {
            try
            {
                string tag = ((SfDataGrid)sender).Tag.ToString();

                var provider = (sender as SfDataGrid).View.GetPropertyAccessProvider();
                var records = (sender as SfDataGrid).View.Records;

                double cantidadX = 0;
                double subtotalX = 0;
                double descuentoX = 0;
                double ivaX = 0;
                double totalX = 0;

                for (int i = 0; i < (sender as SfDataGrid).View.Records.Count; i++)
                {

                    cantidadX += Convert.ToDouble(provider.GetValue(records[i].Data, tag == "9" ? "cantidad" : "neto").ToString());
                    subtotalX += Convert.ToDouble(provider.GetValue(records[i].Data, "subtotal").ToString());
                    descuentoX += Convert.ToDouble(provider.GetValue(records[i].Data, "val_des").ToString());
                    ivaX += Convert.ToDouble(provider.GetValue(records[i].Data, "val_iva").ToString());
                    totalX += Convert.ToDouble(provider.GetValue(records[i].Data, "total").ToString());
                }

                if (tag == "1")
                {
                    TextCantidad1.Text = cantidadX.ToString();
                    TextSubtotal1.Text = subtotalX.ToString("C");
                    TextDescuento1.Text = descuentoX.ToString("C");
                    TextIva1.Text = ivaX.ToString("C");
                    TextTotal1.Text = totalX.ToString("C");
                    Total1.Text = VentasPorProducto.View.Records.Count.ToString();
                }
                if (tag == "2")
                {
                    TextCantidad2.Text = cantidadX.ToString();
                    TextSubtotal2.Text = subtotalX.ToString("C");
                    TextDescuento2.Text = descuentoX.ToString("C");
                    TextIva2.Text = ivaX.ToString("C");
                    TextTotal2.Text = totalX.ToString("C");
                    Total2.Text = VentaPorBodega.View.Records.Count.ToString();
                }
                if (tag == "3")
                {
                    TextCantidad3.Text = cantidadX.ToString();
                    TextSubtotal3.Text = subtotalX.ToString("C");
                    TextDescuento3.Text = descuentoX.ToString("C");
                    TextIva3.Text = ivaX.ToString("C");
                    TextTotal3.Text = totalX.ToString("C");
                    Total3.Text = VentasPorCliente.View.Records.Count.ToString();
                }
                if (tag == "4")
                {
                    TextCantidad4.Text = cantidadX.ToString();
                    TextSubtotal4.Text = subtotalX.ToString("C");
                    TextDescuento4.Text = descuentoX.ToString("C");
                    TextIva4.Text = ivaX.ToString("C");
                    TextTotal4.Text = totalX.ToString("C");
                    Total4.Text = VentasPorVendedor.View.Records.Count.ToString();
                }
                if (tag == "5")
                {
                    TextCantidad5.Text = cantidadX.ToString();
                    TextSubtotal5.Text = subtotalX.ToString("C");
                    TextDescuento5.Text = descuentoX.ToString("C");
                    TextIva5.Text = ivaX.ToString("C");
                    TextTotal5.Text = totalX.ToString("C");
                    Total5.Text = VentasPorLinea.View.Records.Count.ToString();
                }                

            }
            catch (Exception w)
            {
                MessageBox.Show("error-f" + w);
            }
            
        }



 

    }

}
