using ConsultaPedidos;
using Microsoft.Win32;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SiasoftAppExt
{


    //Sia.PublicarPnt(9476,"ConsultaPedidos");
    //Sia.TabU(9476);
    public partial class ConsultaPedidos : UserControl
    {


        dynamic SiaWin;
        dynamic tabitem;
        public int idemp = 0;
        public string cnEmp = "";
        string cod_empresa = "";

        public DataTable drTraslados = new DataTable();

        public ConsultaPedidos(dynamic tabitem1)
        {
            InitializeComponent();

            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.CerrarConEscape = false;
            //MessageBox.Show("tabitem.idemp:"+ tabitem.idemp);
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            if (tabitem.idemp <= 0) idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Consulta Pedidos (" + aliasemp + ")";
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();

                DateTime fechatemp = DateTime.Today;
                fechatemp = new DateTime(fechatemp.Year, fechatemp.Month, 1);

                fecha_ini.Text = fechatemp.ToString();
                fecha_fin.Text = DateTime.Now.ToString();
                fecha_compra.Text = fechatemp.ToString();

                CargarBodegas(cod_empresa);
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show(e.Message);
            }
        }

        public void CargarBodegas(string cod_emp)
        {
            DataTable dtBod = SiaWin.Func.SqlDT("select cod_bod,rtrim(cod_bod)+'-'+nom_bod as nom_bod from inmae_bod where cod_emp='" + cod_emp + "';", "Bodegas", idemp);
            comboBoxBodegas.ItemsSource = dtBod.DefaultView;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                //MessageBox.Show("cnEmp:" + cnEmp);
                string idTab = ((TextBox)sender).Tag.ToString();

                if (e.Key == Key.Enter || e.Key == Key.F8)
                {
                    if (idTab.Length > 0)
                    {
                        string tag = ((TextBox)sender).Tag.ToString();
                        string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                        if (string.IsNullOrEmpty(tag)) return;

                        if (tag == "inmae_tip")
                        {
                            cmptabla = tag; cmpcodigo = "cod_tip"; cmpnombre = "nom_tip"; cmporden = "cod_tip"; cmpidrow = "cod_tip"; cmptitulo = "Maestra de Lineas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        }
                        if (tag == "inmae_bod")
                        {
                            cmptabla = tag; cmpcodigo = "cod_bod"; cmpnombre = "nom_bod"; cmporden = "cod_bod"; cmpidrow = "cod_bod"; cmptitulo = "Maestra de Bodegas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        }
                        if (tag == "comae_ter")
                        {
                            cmptabla = tag; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "cod_ter"; cmpidrow = "cod_ter"; cmptitulo = "Maestra de Terceros"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                        }
                        if (tag == "inmae_mer")
                        {
                            cmptabla = tag; cmpcodigo = "cod_mer"; cmpnombre = "nom_mer"; cmporden = "cod_mer"; cmpidrow = "idrow"; cmptitulo = "Maestra de Vendedores"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                        }
                        if (tag == "inmae_prv")
                        {
                            cmptabla = tag; cmpcodigo = "cod_prv"; cmpnombre = "nom_prv"; cmporden = "cod_prv"; cmpidrow = "idrow"; cmptitulo = "Maestra de Provedor"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "";
                        }


                        string code = ""; string nom = "";
                        dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);

                        winb.ShowInTaskbar = false;
                        winb.Owner = Application.Current.MainWindow;
                        winb.ShowDialog();
                        //idr = winb.IdRowReturn;
                        code = winb.Codigo;
                        nom = winb.Nombre;
                        winb = null;

                        if (!string.IsNullOrWhiteSpace(code))
                        {
                            if (tag == "inmae_tip")
                            {
                                TX_linea.Text = code.Trim();
                                TxBox_linea.Text = nom;
                            }
                            if (tag == "comae_ter")
                            {
                                TX_cliente.Text = code.Trim();
                                TxBox_cliente.Text = nom;
                            }
                            if (tag == "inmae_mer")
                            {
                                TX_vendedor.Text = code.Trim();
                                TxBox_vendedor.Text = nom;
                            }
                            if (tag == "inmae_prv")
                            {
                                TX_provedor.Text = code.Trim();
                                TxBox_provedor.Text = nom;
                            }

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

            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message.ToString());
            }
        }


        private void TX_LostFocus(object sender, RoutedEventArgs e)
        {

            string idTag = ((TextBox)sender).Tag.ToString();
            string codigo = ""; string nombre = ""; TextBox campoNombre = new TextBox();
            switch (idTag)
            {
                case "inmae_tip":
                    codigo = "cod_tip"; nombre = "nom_tip"; campoNombre = (TextBox)this.FindName("TxBox_linea");
                    break;
                case "inmae_bod":
                    codigo = "cod_bod"; nombre = "nom_bod"; campoNombre = (TextBox)this.FindName("TxBox_bodega");
                    break;
                case "comae_ter":
                    codigo = "cod_ter"; nombre = "nom_ter"; campoNombre = (TextBox)this.FindName("TxBox_cliente");
                    break;
                case "inmae_mer":
                    codigo = "cod_mer"; nombre = "nom_mer"; campoNombre = (TextBox)this.FindName("TxBox_vendedor");
                    break;
                case "inmae_prv":
                    codigo = "cod_prv"; nombre = "nom_prv"; campoNombre = (TextBox)this.FindName("TxBox_provedor");
                    break;
            }


            if (string.IsNullOrEmpty(((TextBox)sender).Text)) { campoNombre.Text = ""; return; }

            string cadena = "select * from " + idTag + "  where  " + codigo + "='" + ((TextBox)sender).Text.ToString() + "'  ";
            DataTable tabla = SiaWin.Func.SqlDT(cadena, "Buscar", idemp);
            if (tabla.Rows.Count > 0)
            {
                ((TextBox)sender).Text = tabla.Rows[0][codigo].ToString();
                campoNombre.Text = tabla.Rows[0][nombre].ToString();
            }
            else
            {
                MessageBox.Show("el codigo ingresado no existe");
                ((TextBox)sender).Text = "";
                campoNombre.Text = "";
            }


        }


        public string returnTipBod()
        {

            string tipos = "";
            if (comboBoxBodegas.SelectedIndex >= 0)
            {
                foreach (DataRowView ob in comboBoxBodegas.SelectedItems)
                {
                    String valueCta = ob["cod_bod"].ToString();
                    tipos += "'" + valueCta + "'" + ",";
                }
                string ss = tipos.Trim().Substring(tipos.Trim().Length - 1);
                if (ss == ",") tipos = tipos.Substring(0, tipos.Trim().Length - 1);
            }
            //MessageBox.Show("A2");
            return tipos;
        }



        public string returnTipBodSinComa()
        {

            string tipos = "";
            if (comboBoxBodegas.SelectedIndex >= 0)
            {
                foreach (DataRowView ob in comboBoxBodegas.SelectedItems)
                {
                    String valueCta = ob["cod_bod"].ToString();
                    tipos += valueCta + ",";
                }
                string ss = tipos.Trim().Substring(tipos.Trim().Length - 1);
                if (ss == ",") tipos = tipos.Substring(0, tipos.Trim().Length - 1);
            }
            //MessageBox.Show("A2");
            return tipos;
        }



        private string ArmaWhere()
        {
            string cadenawhere = " ";
            string Linea = TX_linea.Text.Trim();
            string Cliente = TX_cliente.Text.Trim();
            string Vendedor = TX_vendedor.Text.Trim();
            string Provedor = TX_provedor.Text.Trim();

            string bodegas = returnTipBod();

            if (!string.IsNullOrWhiteSpace(bodegas))
                cadenawhere += " and cuerpo.cod_bod in(" + bodegas + ") ";
            if (!string.IsNullOrEmpty(Linea))
                cadenawhere += " and referencia.cod_tip='" + Linea + "'  ";
            if (!string.IsNullOrEmpty(Cliente))
                cadenawhere += " and cabeza.cod_cli='" + Cliente + "'  ";
            if (!string.IsNullOrEmpty(Vendedor))
                cadenawhere += " and cabeza.cod_ven='" + Vendedor + "' ";
            if (!string.IsNullOrEmpty(Provedor))
                cadenawhere += " and referencia.cod_prv='" + Provedor + "' ";

            return cadenawhere;
        }


        private void dataGridCxC_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            try
            {

                if (dataGridConsulta.SelectedIndex >= 0)
                {
                    #region saldos con funcion

                    //DataRowView row = (DataRowView)dataGridConsulta.SelectedItems[0];
                    //string referencia = row["p_cod_ref"].ToString();

                    //decimal saldoBod001 = SiaWin.Func.SaldoInv(referencia, "001", "010");
                    //Bod1.Text = saldoBod001.ToString();

                    //decimal saldoBod003 = SiaWin.Func.SaldoInv(referencia, "003", "010");
                    //decimal saldoBod004 = SiaWin.Func.SaldoInv(referencia, "004", "010");
                    //decimal saldoB3B4 = saldoBod003 + saldoBod004;
                    //Bod3_4.Text = saldoB3B4.ToString();

                    //decimal saldoBod010 = SiaWin.Func.SaldoInv(referencia, "010", "020");
                    //Bod10.Text = saldoBod010.ToString();

                    //decimal saldoBod012 = SiaWin.Func.SaldoInv(referencia, "012", "020");
                    //decimal saldoBod013 = SiaWin.Func.SaldoInv(referencia, "013", "020");
                    //decimal saldoB12B13 = saldoBod012 + saldoBod013;
                    //Bod12_13.Text = saldoB12B13.ToString();

                    //decimal saldoBod005 = SiaWin.Func.SaldoInv(referencia, "005", "030");
                    //Bod5.Text = saldoBod005.ToString();

                    //decimal saldoBod007 = SiaWin.Func.SaldoInv(referencia, "007", "030");
                    //decimal saldoBod009 = SiaWin.Func.SaldoInv(referencia, "009", "030");
                    //decimal saldoB7B9 = saldoBod007 + saldoBod009;
                    //Bod7_9.Text = saldoB7B9.ToString();

                    //decimal saldoBod017 = SiaWin.Func.SaldoInv(referencia, "017", "040");
                    //decimal saldoBod019 = SiaWin.Func.SaldoInv(referencia, "019", "040");
                    //decimal saldoB17B19 = saldoBod017 + saldoBod019;
                    //Bod17_19.Text = saldoB17B19.ToString();

                    //decimal saldoBod008 = SiaWin.Func.SaldoInv(referencia, "008", "040");
                    //Bod8.Text = saldoBod008.ToString();

                    //decimal saldoBod050 = SiaWin.Func.SaldoInv(referencia, "050", "050");
                    //decimal saldoBod052 = SiaWin.Func.SaldoInv(referencia, "052", "050");
                    //decimal saldoB50B52 = saldoBod050 + saldoBod052;
                    //Bod50_52.Text = saldoB50B52.ToString();
                    #endregion

                    DataRowView row = (DataRowView)dataGridConsulta.SelectedItems[0];
                    string referencia = row["p_cod_ref"].ToString();

                    decimal saldo_001 = Convert.ToDecimal(row["saldo_001"]);
                    Bod1.Text = saldo_001.ToString();
                    decimal saldo_003 = Convert.ToDecimal(row["saldo_003"]);
                    decimal saldo_004 = Convert.ToDecimal(row["saldo_004"]);
                    Bod3_4.Text = (saldo_003 + saldo_004).ToString();


                    decimal saldo_010 = Convert.ToDecimal(row["saldo_010"]);
                    Bod10.Text = saldo_010.ToString();

                    decimal saldo_012 = Convert.ToDecimal(row["saldo_012"]);
                    decimal saldo_013 = Convert.ToDecimal(row["saldo_013"]);
                    Bod12_13.Text = (saldo_012 + saldo_013).ToString();

                    decimal saldo_005 = Convert.ToDecimal(row["saldo_005"]);
                    Bod5.Text = saldo_005.ToString();

                    decimal saldo_007 = Convert.ToDecimal(row["saldo_007"]);
                    decimal saldo_009 = Convert.ToDecimal(row["saldo_009"]);
                    Bod7_9.Text = (saldo_007 + saldo_009).ToString();

                    decimal saldo_008 = Convert.ToDecimal(row["saldo_008"]);
                    Bod8.Text = saldo_008.ToString();

                    decimal saldo_017 = Convert.ToDecimal(row["saldo_017"]);
                    decimal saldo_019 = Convert.ToDecimal(row["saldo_019"]);
                    Bod17_19.Text = (saldo_017 + saldo_019).ToString();

                    decimal saldo_050 = Convert.ToDecimal(row["saldo_050"]);
                    decimal saldo_052 = Convert.ToDecimal(row["saldo_052"]);
                    Bod50_52.Text = (saldo_050 + saldo_052).ToString();



                    doc_pedido.Text = row["p_num_trn"].ToString();
                    decimal ctn = Convert.ToDecimal(drTraslados.Compute("Sum(p_pendiente)", "p_num_trn='" + row["p_num_trn"].ToString() + "' ").ToString());
                    doc_items.Text = ctn.ToString();

                    decimal tot = Math.Round(Convert.ToDecimal(drTraslados.Compute("Sum(valor_pedido)", "p_num_trn='" + row["p_num_trn"].ToString() + "' ")), 2);
                    doc_total.Text = tot.ToString("C");

                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("erro en la seleccion:" + w);
            }

        }



        private async void BTNconsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (comboBoxBodegas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una o mas bodega", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                ConfigGrid.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;

                dataGridConsulta.ItemsSource = null;

                string ffi = fecha_ini.Text.ToString();
                string fff = fecha_fin.Text.ToString();
                string where = ArmaWhere();
                string ffc = fecha_compra.Text.ToString();
                string whereAnular = TipoAnul.Text == "No" ? " and pedidos.est_anu<>'A' " : " ";
                string bodegas = returnTipBodSinComa();
                //if (SiaWin._UserId == 21)
                //{
                //    MessageBox.Show("ffi:"+ ffi);
                //    MessageBox.Show("ffF:" + fff);
                //    MessageBox.Show("where:" + where);
                //    MessageBox.Show("ffc:" + ffc);
                //    MessageBox.Show("ffc:" + whereAnular);
                //    MessageBox.Show("ffc:" + whereAnular);
                //MessageBox.Show("bodegas:" + bodegas);
                //}


                if (string.IsNullOrEmpty(where)) where = " ";


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ffi, fff, ffc, where, bodegas, whereAnular, cod_empresa, source.Token), source.Token);
                await slowTask;


                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    dataGridConsulta.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Total.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();


                    if (drTraslados.Rows.Count > 0) drTraslados.Clear();
                    drTraslados = ((DataSet)slowTask.Result).Tables[0];
                    dataGridTraslados.ItemsSource = drTraslados;
                    Tx_totTrans.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();

                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;
                }

                ConfigGrid.IsEnabled = true;
                this.sfBusyIndicator.IsBusy = false;
                //GridConfiguracion.IsEnabled = true;
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show("erro2:" + ex);
                this.Opacity = 1;
            }


        }



        private DataSet LoadData(string FechaIN, string FechaFI, string FechaCompra, string Where, string bodegas, string Anulacion, string CodEmp, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                //cmd = new SqlCommand("_EmpSpConsultaPedidos", con);
                cmd = new SqlCommand("_EmpSpConsultaPedidosTemporal", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaIni", FechaIN);
                cmd.Parameters.AddWithValue("@FechaFin", FechaFI);
                cmd.Parameters.AddWithValue("@Where", Where);
                cmd.Parameters.AddWithValue("@bodegas", bodegas);
                cmd.Parameters.AddWithValue("@fech_Compra", FechaCompra);
                cmd.Parameters.AddWithValue("@Anulacion", Anulacion);
                cmd.Parameters.AddWithValue("@codEmpresa", CodEmp);
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

        private void BTNcancelar_Click(object sender, RoutedEventArgs e)
        {
            TX_linea.Text = "";
            TxBox_linea.Text = "";
            TX_cliente.Text = "";
            TxBox_cliente.Text = "";
            TX_provedor.Text = "";
            TxBox_provedor.Text = "";

        }
        
        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            e.Range.CellStyle.Font.Size = 12;
            e.Range.CellStyle.Font.FontName = "Segoe UI";

            if (e.ColumnName == "cantidad" || e.ColumnName == "can_compra" || e.ColumnName == "p_pendiente" || e.ColumnName == "valor_pedido" || e.ColumnName == "valor_unitario" || e.ColumnName == "cnd" || e.ColumnName == "pv" || e.ColumnName == "saldo_001" || e.ColumnName == "saldo_005" || e.ColumnName == "saldo_008" || e.ColumnName == "saldo_010")
            {
                double value = 0;
                if (double.TryParse(e.CellValue.ToString(), out value))
                {
                    e.Range.Number = value;
                }
                e.Handled = true;
            }

            //if (e.ColumnName == "p_num_trn")
            //{
            //    string value = "";
            //    if (string.TryParse(e.CellValue.ToString(), out value))
            //    {
            //        e.Range.Number = value;
            //    }
            //    e.Handled = true;
            //}

        }

        private void ExportEXCEL_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExportMode = ExportMode.Value;
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;
                var excelEngine = dataGridConsulta.ExportToExcel(dataGridConsulta.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                //workBook.ActiveSheet.Columns[16].NumberFormat = "#.#";
                workBook.ActiveSheet.Columns[12].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[13].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[14].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[15].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[16].NumberFormat = "0.0";

                workBook.ActiveSheet.Columns[17].NumberFormat = "###";

                workBook.ActiveSheet.Columns[18].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[19].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[20].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[21].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[22].NumberFormat = "0.0";
                workBook.ActiveSheet.Columns[23].NumberFormat = "0.0";

                workBook.ActiveSheet.Columns[25].NumberFormat = "0.0";


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
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al exportar");
            }
        }

        private void BTNProceso_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                //SiaWin.Browse(drTraslados);
                foreach (System.Data.DataRow item in drTraslados.Rows)
                {
                    DateTime f_001 = item["fec_max_001"].ToString().Trim() == "-" ?
                        DateTime.Now : Convert.ToDateTime(item["fec_max_001"]);

                    DateTime f_005 = item["fec_max_005"].ToString().Trim() == "-" ?
                        DateTime.Now : Convert.ToDateTime(item["fec_max_005"]);

                    DateTime f_008 = item["fec_max_008"].ToString().Trim() == "-" ?
                        DateTime.Now : Convert.ToDateTime(item["fec_max_008"]);

                    DateTime f_010 = item["fec_max_010"].ToString().Trim() == "-" ?
                        DateTime.Now : Convert.ToDateTime(item["fec_max_010"]);

                    decimal cnt_ped = Convert.ToDecimal(item["p_pendiente"]);

                    decimal cnt_ped_sinoperacion = Convert.ToDecimal(item["p_pendiente"]);

                    decimal sal_001 = Convert.ToDecimal(item["saldo_001"]);
                    decimal sal_005 = Convert.ToDecimal(item["saldo_005"]);
                    decimal sal_008 = Convert.ToDecimal(item["saldo_008"]);
                    decimal sal_010 = Convert.ToDecimal(item["saldo_010"]);

                    decimal cnd = Convert.ToDecimal(item["cnd"]);

                    decimal cnt = cnt_ped;
                    //-1 mas vieja datetime.compare()
                    //0 igual
                    //1 es mayor

                    DataTable dt = getDateOld(f_001, f_005, f_008, f_010, sal_001, sal_005, sal_008, sal_010);

                    DataTable temp = dt.AsEnumerable()
                         .OrderBy(r => r.Field<DateTime>("fecha")).ThenByDescending(c => c.Field<decimal>("saldo"))
                         .CopyToDataTable();

                    bool flag = true;

                    bool CNDone = false;

                    bool alcCND = false;


                    foreach (System.Data.DataRow fec in temp.Rows)
                    {
                        string bod = fec["bodega"].ToString().Trim();

                        //cuando no alcansa en cnd

                        if (cnd > 0 && CNDone == false)
                        {
                            decimal c = cnt_ped - cnd;
                            if (c <= 0)
                            {
                                item["trn_cnd"] = cnt_ped;
                                alcCND = true;
                                CNDone = true;
                            }
                            else
                            {
                                item["trn_cnd"] = cnd;
                                cnt_ped -= cnd;
                                cnt = cnt_ped;
                                //alcCND = true;
                                CNDone = true;
                            }
                        }



                        if (alcCND == false)
                        {
                            switch (bod)
                            {
                                case "001":
                                    cnt -= sal_001;
                                    if (cnt <= 0)
                                    {
                                        if (flag == true)
                                        {
                                            item["trn_001"] = cnt_ped;
                                            flag = false;
                                        }
                                    }
                                    else
                                    {
                                        if (sal_001 > 0)
                                        {
                                            cnt_ped -= sal_001;
                                            item["trn_001"] = sal_001;
                                        }
                                    }
                                    break;
                                case "005":
                                    cnt -= sal_005;
                                    if (cnt <= 0)
                                    {
                                        if (flag == true)
                                        {
                                            item["trn_005"] = cnt_ped;
                                            flag = false;
                                        }
                                    }
                                    else
                                    {
                                        if (sal_005 > 0)
                                        {
                                            cnt_ped -= sal_005;
                                            item["trn_005"] = sal_005;
                                        }
                                    }
                                    break;
                                case "008":
                                    cnt -= sal_008;
                                    if (cnt <= 0)
                                    {
                                        if (flag == true)
                                        {
                                            item["trn_008"] = cnt_ped;
                                            flag = false;
                                        }
                                    }
                                    else
                                    {
                                        if (sal_008 > 0)
                                        {
                                            cnt_ped -= sal_008;
                                            item["trn_008"] = sal_008;
                                        }
                                    }
                                    break;
                                case "010":
                                    cnt -= sal_010;
                                    if (cnt <= 0)
                                    {
                                        if (flag == true)
                                        {
                                            item["trn_010"] = cnt_ped;
                                            flag = false;
                                        }
                                    }
                                    else
                                    {
                                        if (sal_010 > 0)
                                        {
                                            cnt_ped -= sal_010;
                                            item["trn_010"] = sal_010;
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    decimal trn_cnd = item["trn_cnd"].ToString().Trim() == "-" ? 0 : Convert.ToDecimal(item["trn_cnd"]);
                    decimal trn_001 = item["trn_001"].ToString().Trim() == "-" ? 0 : Convert.ToDecimal(item["trn_001"]);
                    decimal trn_005 = item["trn_005"].ToString().Trim() == "-" ? 0 : Convert.ToDecimal(item["trn_005"]);
                    decimal trn_008 = item["trn_008"].ToString().Trim() == "-" ? 0 : Convert.ToDecimal(item["trn_008"]);
                    decimal trn_010 = item["trn_010"].ToString().Trim() == "-" ? 0 : Convert.ToDecimal(item["trn_010"]);
                    decimal suma = trn_cnd + trn_001 + trn_005 + trn_008 + trn_010;

                    decimal dif = cnt_ped_sinoperacion - suma;
                    item["faltante"] = dif.ToString();

                }


            }
            catch (Exception w)
            {
                MessageBox.Show("errror al ejecutar el proceso:" + w);
            }
        }

        public DataTable getDateOld(DateTime f1, DateTime f5, DateTime f8, DateTime f10, decimal s1, decimal s5, decimal s8, decimal s10)
        {
            DataTable dt_fehas = new DataTable();
            dt_fehas.Columns.Add("bodega");
            dt_fehas.Columns.Add("fecha", typeof(DateTime));
            dt_fehas.Columns.Add("saldo", typeof(decimal));
            dt_fehas.Rows.Add("001", f1, s1);
            dt_fehas.Rows.Add("005", f5, s5);
            dt_fehas.Rows.Add("008", f8, s8);
            dt_fehas.Rows.Add("010", f10, s10);
            dt_fehas.DefaultView.Sort = "fecha asc";

            return dt_fehas;
        }

        private void ExportEXCELTrans_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridTraslados.ExportToExcel(dataGridTraslados.View, options);
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
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al exportar");
            }
        }

        private void dataGridConsulta_FilterChanged(object sender, GridFilterEventArgs e)
        {
            try
            {
                Total.Text = dataGridConsulta.View.Records.Count.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el filtro:" + w);
            }
        }

        private void BtnDetBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridConsulta.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridConsulta.SelectedItems[0];
                    DetalleBackorder w = new DetalleBackorder();
                    w.bodega = returnTipBodSinComa();
                    w.referencia = row["p_cod_ref"].ToString();
                    w.fecha = fecha_compra.Text;
                    w.idemp = idemp;
                    w.Owner = Application.Current.MainWindow;
                    w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    w.ShowInTaskbar = false;
                    w.ShowDialog();
                }
                else
                {
                    MessageBox.Show("seleccione un item para ver el Backorder");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir pnt de backorder:" + w);
            }
        }

        public string GetEmpresa(string tagEmp)
        {
            string empresa = "";
            switch (tagEmp)
            {
                case "001":
                    empresa = "010";
                    break;
                case "003":
                    empresa = "010";
                    break;
                case "010":
                    empresa = "020";
                    break;
                case "012":
                    empresa = "020";
                    break;
                case "005":
                    empresa = "030";
                    break;
                case "007":
                    empresa = "030";
                    break;
                case "017":
                    empresa = "040";
                    break;
                case "008":
                    empresa = "030";
                    break;
                case "050":
                    empresa = "050";
                    break;
            }

            return empresa;
        }

        private void BtnKardex_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (dataGridConsulta.SelectedIndex<0)
                {
                    MessageBox.Show("seleccione un item de la consulta","alerta",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                    return;
                }

                string tag = ((Button)sender).Tag.ToString();
                DataRowView row = (DataRowView)dataGridConsulta.SelectedItems[0];
                dynamic w = SiaWin.WindowExt(9466, "Kardex");
                w.ShowInTaskbar = false;
                w.Owner = Application.Current.MainWindow;
                w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                w.idemp = idemp;
                w.codref = row["p_cod_ref"].ToString();
                //string tag = ((Button)sender).Tag.ToString();
                w.codbod = tag;
                w.codemp = GetEmpresa(tag);
                w.ShowDialog();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar kardex:"+w);
            }
        }

        private void BtnDetPedi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridConsulta.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridConsulta.SelectedItems[0];
                    DetallePedidoVenta w = new DetallePedidoVenta();
                    w.n_pedido = row["p_num_trn"].ToString();
                    w.bodega = returnTipBodSinComa();
                    w.referencia = row["p_cod_ref"].ToString();
                    w.fecha = fecha_compra.Text;
                    w.idemp = idemp;
                    w.Owner = Application.Current.MainWindow;
                    w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    w.ShowInTaskbar = false;
                    w.ShowDialog();
                }
                else
                {
                    MessageBox.Show("seleccione un item para ver el Backorder");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir pnt de backorder:" + w);
            }
        }



    }
}


