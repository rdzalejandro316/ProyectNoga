using CargarPedidoRemision;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9664, "CargarPedidoRemision");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9664, "CargarPedidoRemision");
    //ww.idemp=1;
    //ww.tercero="832005223";
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();


    public partial class CargarPedidoRemision : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public int idregcabReturn = -1;
        public string codtrn = string.Empty;
        public string numtrn = string.Empty;
        public string bodega = "";
        public string tercero = "";

        public DataTable tabla;

        DataTable tablaCuerpo = new DataTable();

        public Boolean bandera = false;

        public Boolean actualizaDoc = false;

        public int PntTip = 0;

        public Boolean addRow = false;

        public string tipoTransaccion = "";

        public string campoDescTip = "";
        public string campoDescLin = "";

        DataTable DtCuerpo = new DataTable();
        public DataTable temporal = new DataTable();

        DataTable dt_cargar = new DataTable();

        public CargarPedidoRemision()
        {
            InitializeComponent();

            dt_cargar.Columns.Add("idreg");
            dt_cargar.Columns.Add("num_trn");
            dt_cargar.Columns.Add("cod_cli");
        }

        public void loadColumns()
        {
            DtCuerpo.Columns.Add("idreg");
            DtCuerpo.Columns.Add("num_trn");
            DtCuerpo.Columns.Add("cod_ref");
            DtCuerpo.Columns.Add("nom_ref");
            DtCuerpo.Columns.Add("cantidad", typeof(decimal));
            DtCuerpo.Columns.Add("val_uni");
            DtCuerpo.Columns.Add("subtotal");
            DtCuerpo.Columns.Add("por_des");
            DtCuerpo.Columns.Add("tot_tot");
            DtCuerpo.Columns.Add("val_ref");
            DtCuerpo.Columns.Add("por_iva");
            DtCuerpo.Columns.Add("val_iva");
            DtCuerpo.Columns.Add("por_ret");
            DtCuerpo.Columns.Add("val_ret");
            DtCuerpo.Columns.Add("por_ica", typeof(decimal));
            DtCuerpo.Columns.Add("val_ica");
            DtCuerpo.Columns.Add("por_riva");
            DtCuerpo.Columns.Add("val_riva");
            DtCuerpo.Columns.Add("val_des");
            DtCuerpo.Columns.Add("cant_pend", typeof(decimal));
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            SiaWin = Application.Current.MainWindow;
            LoadConfig();
            pantalla();
            loadColumns();
            consultaCabeza();

            Tx_name.Text = "Pedidos - Remision : " + tercero;

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
                this.Title = "Pedidos y Cotizaciones - Empresa:" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("aqui-" + e.Message);
            }
        }
        public void pantalla()
        {
            //this.MinHeight = 650;
            //this.MaxHeight = 650;
            //this.MinWidth = 1200;
            //this.MaxWidth = 1200;
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.F5)
                {
                    if (dataGridCabeza.SelectedIndex >= 0)
                    {
                        BTNfacturar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("@888" + w);
            }
        }

        public async void consultaCabeza()
        {
            try
            {

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                dataGridCabeza.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                DtCuerpo.Clear();
                dataGridCabeza.ItemsSource = null;

                string where = "and cabeza.cod_cli='" + tercero + "' and cabeza.bod_tra='"+bodega+"' ";
                


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadInfo("505", cod_empresa, where, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    tabla = ((DataSet)slowTask.Result).Tables[0];
                    if (!tabla.Columns.Contains("pass"))
                        tabla.Columns.Add("pass", typeof(bool));
                    foreach (System.Data.DataRow dr in tabla.Rows) dr["pass"] = false;

                    //dataGridCabeza.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    dataGridCabeza.ItemsSource = tabla;

                    Tot_regis.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                }

                dataGridCabeza.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar el procedimiento almacenado" + w);
            }
        }

        public DataSet LoadInfo(string tag, string empresa, string where, CancellationToken cancellationToken)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                cmd = new SqlCommand("_EmpPvConsultaPedCot", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_trn", tag.ToString());
                cmd.Parameters.AddWithValue("@_codemp", empresa);
                cmd.Parameters.AddWithValue("@where", where);
                cmd.Parameters.AddWithValue("@fechaIni", DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy"));
                //cmd.Parameters.AddWithValue("@fechaIni", DateTime.Today.AddDays(-20).ToString("dd/MM/yyyy"));
                cmd.Parameters.AddWithValue("@fechaFin", DateTime.Now.ToString("dd/MM/yyyy"));
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();

                foreach (System.Data.DataRow item in ds.Tables[0].Rows)
                {
                    string num_trn = item["num_trn"].ToString().Trim();
                    string idreg = item["idreg"].ToString().Trim();

                    if (!string.IsNullOrWhiteSpace(num_trn) && !string.IsNullOrWhiteSpace(idreg))
                    {
                        SqlConnection con1 = new SqlConnection(SiaWin._cn);
                        SqlCommand cmd1 = new SqlCommand();
                        SqlDataAdapter da1 = new SqlDataAdapter();
                        DataTable ds1 = new DataTable();
                        cmd1 = new SqlCommand("_EmpPvCotizacion", con1);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@cod_trn", tag);
                        cmd1.Parameters.AddWithValue("@idreg", idreg);
                        cmd1.Parameters.AddWithValue("@num_trn", num_trn);
                        cmd1.Parameters.AddWithValue("@_codemp", empresa);
                        da1 = new SqlDataAdapter(cmd1);
                        da1.Fill(ds1);
                        con1.Close();
                        if (ds1.Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow dr_cu in ds1.Rows)
                            {
                                //MessageBox.Show(dr_cu["por_ica"].ToString());
                                //double val_uni = Convert.ToDouble(dr_cu["val_uni"]);
                                //double subtotal = Convert.ToDouble(dr_cu["subtotal"]);
                                //double tot_tot = Convert.ToDouble(dr_cu["tot_tot"]);

                                DtCuerpo.Rows.Add
                                    (
                                        dr_cu["idreg"].ToString(),
                                        dr_cu["num_trn"].ToString(),
                                        dr_cu["cod_ref"].ToString(),
                                        dr_cu["nom_ref"].ToString(),
                                        Convert.ToDecimal(dr_cu["cantidad"]),
                                        dr_cu["val_uni"].ToString(),
                                        dr_cu["subtotal"].ToString(),
                                        dr_cu["por_des"].ToString(),
                                        dr_cu["tot_tot"].ToString(),
                                        dr_cu["val_ref"].ToString(),
                                        dr_cu["por_iva"].ToString(),
                                        dr_cu["val_iva"].ToString(),
                                        dr_cu["por_ret"].ToString(),
                                        dr_cu["val_ret"].ToString(),
                                        dr_cu["por_ica"].ToString(),
                                        dr_cu["val_ica"].ToString(),
                                        dr_cu["por_riva"].ToString(),
                                        dr_cu["val_riva"].ToString(),
                                        dr_cu["val_des"].ToString(),
                                        Convert.ToDecimal(dr_cu["cant_pend"])
                                    );
                            }
                        }

                        bool bandera = factura_cuzada(num_trn);
                        item["facturado"] = bandera == true ? "SI" : "NO";
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("erro en la consulta" + w);
            }
            return ds;
        }


        private void dataGridCabeza_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                //cabeza();
                if (dataGridCabeza.SelectedIndex < 0)
                {
                    dataGridCabeza.ItemsSource = 0;
                    dataGridCuerpo.ItemsSource = 0;
                    return;
                }


                DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                string idreg = row["idreg"].ToString();
                string num_trn = row["num_trn"].ToString().Trim();
                string des_mov = row["des_mov"].ToString().Trim();

                Nota.Text = des_mov;

                #region temporal
                DataTable dt_temporal = new DataTable();
                dt_temporal.Columns.Add("idreg");
                dt_temporal.Columns.Add("num_trn");
                dt_temporal.Columns.Add("cod_ref");
                dt_temporal.Columns.Add("nom_ref");
                dt_temporal.Columns.Add("cantidad");
                dt_temporal.Columns.Add("val_uni");
                dt_temporal.Columns.Add("subtotal");
                dt_temporal.Columns.Add("por_des");
                dt_temporal.Columns.Add("tot_tot");
                dt_temporal.Columns.Add("val_ref");
                dt_temporal.Columns.Add("por_iva");
                dt_temporal.Columns.Add("val_iva");
                dt_temporal.Columns.Add("por_ret");
                dt_temporal.Columns.Add("val_ret");
                dt_temporal.Columns.Add("por_ica");
                dt_temporal.Columns.Add("val_ica");
                dt_temporal.Columns.Add("por_riva");
                dt_temporal.Columns.Add("val_riva");
                dt_temporal.Columns.Add("val_des");
                dt_temporal.Columns.Add("cant_pend");
                #endregion                

                bool flag = false;
                double sumCantidad = 0;
                double sumTotal = 0;

                double cntOut = 0;
                double cntPendiOut = 0;
                double totalOut = 0;

                foreach (System.Data.DataRow item in DtCuerpo.Rows)
                {
                    string num = item["num_trn"].ToString().Trim();

                    //double cnt =  Convert.ToDouble(item["cantidad"]);
                    double cnt = double.TryParse(item["cantidad"].ToString(), out cntOut) == true ? Convert.ToDouble(item["cantidad"]) : 0;
                    double cnt_pendi = double.TryParse(item["cant_pend"].ToString(), out cntPendiOut) == true ? Convert.ToDouble(item["cant_pend"]) : 0;
                    double total = double.TryParse(item["tot_tot"].ToString(), out totalOut) == true ? Convert.ToDouble(item["tot_tot"]) : 0;


                    //double cnt_pendi = Convert.ToDouble(item["cant_pend"]);                  
                    //double total = Convert.ToDouble(item["tot_tot"]);


                    if (cnt != cnt_pendi) flag = true;

                    if (num == num_trn)
                    {
                        sumCantidad += cnt;
                        sumTotal += total;

                        dt_temporal.Rows.Add
                            (
                                item["idreg"].ToString(),
                                  item["num_trn"].ToString(),
                                        item["cod_ref"].ToString(),
                                        item["nom_ref"].ToString(),
                                        Convert.ToDecimal(item["cantidad"]),
                                        item["val_uni"].ToString(),
                                        item["subtotal"].ToString(),
                                        item["por_des"].ToString(),
                                        item["tot_tot"].ToString(),
                                        item["val_ref"].ToString(),
                                        item["por_iva"].ToString(),
                                        item["val_iva"].ToString(),
                                        item["por_ret"].ToString(),
                                        item["val_ret"].ToString(),
                                        item["por_ica"].ToString(),
                                        item["val_ica"].ToString(),
                                        item["por_riva"].ToString(),
                                        item["val_riva"].ToString(),
                                        item["val_des"].ToString(),
                                        Convert.ToDecimal(item["cant_pend"])
                            );
                    }
                }

                temporal.Clear();
                temporal = dt_temporal;
                dataGridCuerpo.ItemsSource = dt_temporal.DefaultView;
                Tot_Cantid.Text = sumCantidad.ToString();
                Tot_Total.Text = sumTotal.ToString("C");
                Tot_saldo.Text = "-";

                Tot_RegCu.Text = temporal.Rows.Count.ToString();

                if (flag)
                {
                    factura(num_trn);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("change:" + w);
            }

        }



        public void factura(string pedido)
        {
            string query = "select idregcab,num_trn from InCue_doc where doc_cruc='" + pedido + "' and (cod_trn='005' or cod_trn='145')";
            DataTable DTcompra = SiaWin.Func.SqlDT(query, "Compra", idemp);
            if (DTcompra.Rows.Count > 0)
            {
                DocumentoCompra.Text = DTcompra.Rows[0]["num_trn"].ToString();
                idregCompra.Text = DTcompra.Rows[0]["idregcab"].ToString();
                BTNdetalle.Visibility = Visibility.Visible;
            }
            else
            {
                DocumentoCompra.Text = "Ninguno";
                BTNdetalle.Visibility = Visibility.Hidden;
            }

        }

        public bool factura_cuzada(string pedido)
        {
            string query = "select idregcab,num_trn from InCue_doc where doc_cruc='" + pedido + "' and (cod_trn='005' or cod_trn='145')";
            DataTable DTcompra = SiaWin.Func.SqlDT(query, "Compra", idemp);
            return DTcompra.Rows.Count > 0 ? true : false;
        }


        private void BTNfacturar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                bool flag = false;
                bool flag_compra = false;
                foreach (System.Data.DataRow dr in tabla.Rows)
                {
                    if (Convert.ToBoolean(dr["pass"]) == true) flag = true;
                    if (dr["facturado"].ToString().Trim() == "SI") flag_compra = true;
                }

                if (!flag)
                {
                    MessageBox.Show("seleccione almenos un documento", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (flag_compra)
                {
                    if (MessageBox.Show("Este documento ya tiene algunos items facturados desea facturarlo..?", "Guardar Traslado", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        return;
                }

                this.Close();
            }
            catch (Exception)
            {

                MessageBox.Show("Seleccione un Documento para Realizar la Facturacion");
            }
        }

        private void dataGridCuerpo_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridCuerpo.SelectedIndex < 0) return;


                DataRowView row = (DataRowView)dataGridCuerpo.SelectedItems[0];
                string referencia = row["cod_ref"].ToString().Trim();
                //MessageBox.Show("bodega:"+ bodega);

                decimal saldoin = SiaWin.Func.SaldoInv(referencia, bodega, cod_empresa);
                Tot_saldo.Text = saldoin.ToString();

            }
            catch (Exception w)
            {
                MessageBox.Show("error al seelcionar cuerpo");
            }

        }

        private void TextBxCB_consulta_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                consultaCabeza();
                Tot_Cantid.Text = "-";
                Tot_Total.Text = "-";
                Tot_saldo.Text = "-";
            }
            catch (Exception w)
            {
                MessageBox.Show("dropdown " + w);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (tipoTransaccion == "004")
            {
                campoDescTip = "des_mos";
                campoDescLin = "Por_des";
            }
            else
            {
                campoDescTip = "por_des";
                campoDescLin = "por_desc";
            }
            Tx_titleSal.Text = "saldo-" + bodega.Trim();
        }


        private void BTNsalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BTNdetalle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DetalleCompra ventana = new DetalleCompra();
                ventana.idreg = idregCompra.Text;
                ventana.num_trn = DocumentoCompra.Text;
                ventana.ShowInTaskbar = false;
                ventana.Owner = Application.Current.MainWindow;
                ventana.ShowDialog();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el detalles");
            }
        }


        private void BTNImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridCabeza.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                    int idreg = Convert.ToInt32(row["idreg"]);

                    DataTable dt = SiaWin.Func.SqlDT("select * from incue_doc where idregcab='" + idreg + "' ", "pedido", idemp);
                    if (dt.Rows.Count > 0)
                    {
                        double tot = Convert.ToDouble(dt.Compute("Sum(tot_tot)", ""));
                        ImprimePedidoCotiza(idreg, false, tot);
                    }
                    else
                    {
                        MessageBox.Show("no");
                    }

                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al imprimir:" + w);
            }
        }
        void ImprimePedidoCotiza(int iddocu, bool reimprimir = false, double totalFac = 0)
        {
            try
            {
                dynamic Pnt9461 = SiaWin.WindowExt(9461, "DocumentosReportes");  //carga desde sql
                Pnt9461.DocumentoIdCab = iddocu;
                Pnt9461.idEmp = idemp;

                Pnt9461.ReportPath = @"/Otros/FrmDocumentos/PvPedidoCotiza";

                Pnt9461.Copias = 1;
                Pnt9461.DirecPrinter = Convert.ToBoolean(false);
                Pnt9461.DirecPrinter = false;

                System.Text.StringBuilder _sqlcue = new System.Text.StringBuilder();
                _sqlcue.Append("select cue.idreg,cue.cod_bod,nom_bod,ref.cod_ref,ref.cod_ant,ref.cod_tip,tip.nom_tip,ref.cod_prv,ref.nom_ref,cue.cantidad,cue.val_uni,subtotal,val_iva,");
                _sqlcue.Append("cue.val_des,cue.por_des,cue.tot_tot,cue.cos_uni,cue.cos_tot,cue.val_riva,cue.val_ret,cue.val_ica from incue_doc as cue inner join incab_doc on incab_doc.idreg=cue.idregcab and incab_doc.idreg=" + iddocu.ToString());
                _sqlcue.Append("inner join inmae_ref as ref on ref.cod_ref=cue.cod_ref inner join inmae_tip as tip on tip.cod_tip=ref.cod_tip inner join inmae_bod as bod on bod.cod_bod=cue.cod_bod order by cod_prv ");

                System.Text.StringBuilder _sqlcab = new System.Text.StringBuilder();
                _sqlcab.Append(" SELECT trn.nom_trn, cab.fec_trn, cab.fec_ven, cab.cod_trn, cab.num_trn, cab.cod_ven, cab.ord_comp, mer.nom_mer, ter.nom_ter, ter.cod_ter, ter.ciudad, ter.dir,ter.dir_comer,ter.tel1, cab.for_pag, cab.val_ret, cab.val_riva, cab.val_rica, cab.fa_cufe, suc.cod_suc, nom_suc, suc.dir as dir_suc, dir_corres, suc.tel as tel_suc, fax, suc.cod_ven as cod_ven_suc, cod_rut, suc.cod_ciu as cod_ciu_suc, suc.estado as estado_suc, suc.cod_zona as cod_zona_suc,isnull(muni.nom_muni,'') as ciudad_suc ");
                _sqlcab.Append(" FROM InCab_doc AS cab left JOIN  InMae_mer AS mer ON mer.cod_mer = cab.cod_ven INNER JOIN InMae_trn AS trn ON trn.cod_trn = cab.cod_trn INNER JOIN Comae_ter AS ter ON ter.cod_ter = cab.cod_cli ");
                _sqlcab.Append(" left join inmae_suc as suc on suc.cod_ter = cab.cod_cli");
                _sqlcab.Append("  left join MmMae_muni as muni on muni.cod_depa=suc.cod_ciu ");

                _sqlcab.Append(" WHERE cab.idreg = " + iddocu.ToString());

                Pnt9461.Tag1 = _sqlcab.ToString();
                Pnt9461.Tag2 = _sqlcue.ToString();
                Pnt9461.Tag3 = "select * from inmae_bod where cod_bod='" + bodega + "'";
                Pnt9461.Tag4 = "select * from copventas where cod_pvt='" + bodega + "'";
                Pnt9461.Tag5 = SiaWin.Func.enletras(totalFac.ToString());  //valor en letra
                Pnt9461.usuario = SiaWin._UserAlias;
                Pnt9461.titlePie = reimprimir == false ? " " : "ORGINAL              R";

                //string nameprinterreport = Pventas.Rows[0]["nameprint"].ToString().Trim();
                //string nameprinterreport = "";
                //if (!string.IsNullOrEmpty(nameprinterreport)) Pnt9461.printName = nameprinterreport;


                Pnt9461.ShowInTaskbar = false;
                Pnt9461.Owner = Application.Current.MainWindow;
                Pnt9461.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                Pnt9461.ShowDialog();
                Pnt9461 = null;
            }
            catch (System.Exception _error)
            {
                MessageBox.Show("erorIMP" + _error);                                
            }
        }

        private void BTNview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dtview = new DataTable();
                dtview.Columns.Add("documento");

                foreach (System.Data.DataRow item in tabla.Rows)
                {
                    if (Convert.ToBoolean(item["pass"]))
                    {
                        System.Data.DataRow toInsert = dtview.NewRow();
                        toInsert["documento"] = item["num_trn"];
                        dtview.Rows.Add(toInsert);
                    }
                }

                if (dtview.Rows.Count > 0)
                {
                    SiaWin.Browse(dtview);
                }
                else
                {
                    MessageBox.Show("marque algun pedido para facturar", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("erro al ver los pedidos seleccionados:" + w);
            }
        }

        private void BtnSelAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (System.Data.DataRow dr in tabla.Rows)
                    dr["pass"] = true;
                dataGridCabeza.UpdateLayout();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al seleccionar todos:" + w);
            }
        }





    }
}

