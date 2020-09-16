
using GeneracionPedidosProvedores;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SiasoftAppExt
{
    #region 
    //Sia.PublicarPnt(9457,"GeneracionPedidosProvedores");
    //Sia.TabU(9457);

    //pantalla de pruebas
    //Sia.PublicarPnt(9683,"GeneracionPedidosProvedores");
    //Sia.TabU(9683);

    //Sia.PublicarPnt(9461,"DocumentosReportes");
    //dynamic Pnt9461 = ((Inicio)Application.Current.MainWindow).WindowExt(9461, "DocumentosReportes");  
    //Pnt9461.TituloReporte = "titulo";                
    //Pnt9461.DocumentoIdCab = 164103;
    //Pnt9461.idEmp = 010;
    //Pnt9461.ReportPath = @"/Otros/FrmDocumentos/PvCotizacion010";
    //Pnt9461.Copias = 010;                
    //Pnt9461.DirecPrinter = false;          
    //Pnt9461.ShowInTaskbar = false;
    //Pnt9461.Owner = Application.Current.MainWindow;
    //Pnt9461.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //Pnt9461.ShowDialog();
    #endregion

    public partial class GeneracionPedidosProvedores : UserControl
    {
        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string nitPRV = "";
        string BodegConsg = "";


        string cadenaWhere = "";
        Boolean banderaSelect = false;

        public string ProvedorExt = "";
        int moduloid = 0;

        DataTable dtConfigu = new DataTable();

        public string cod_ant = "doctor";

        public GeneracionPedidosProvedores(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.MultiTab = true;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            if (tabitem.idemp <= 0) idemp = SiaWin._BusinessId;

            tabitem.Title = "generacion de pedidos";
            tabitem.Logo(9, ".png");
            //idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                //idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "generacion de pedidos (" + aliasemp + ")";

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());

                FechaConsul.Text = DateTime.Now.ToString();
                FechaBack.Text = DateTime.Now.AddMonths(-1).ToString();
                FechaEntre.Text = DateTime.Now.ToString();
                Fec_pedido.Text = DateTime.Now.AddMonths(-2).ToString();

                if (SiaWin._UserId == 200 || SiaWin._UserId == 21)
                {
                    // MessageBox.Show("siii");
                    dtConfigu = SiaWin.Func.SqlDT("select * from configPntPedidosProv  where UserId='200' ", "config", 0);
                }
                else
                {
                    BTNConfig.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string tag = ((TextBox)sender).Tag.ToString();

                if (e.Key == System.Windows.Input.Key.Delete || e.Key == System.Windows.Input.Key.Back)
                {
                    if (tag == "inmae_bod")
                    {
                        TextCod_bod.Text = "";
                    }
                    if (tag == "inmae_prv")
                    {
                        TextCod_Pro.Text = "";
                    }
                    if (tag == "inmae_tip")
                    {
                        TextCod_Lin.Text = "";
                    }
                    return;
                }

                if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.F8)
                {
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = false; string cmpwhere = "";
                    if (string.IsNullOrEmpty(tag)) return;

                    if (tag == "inmae_bod")
                    {
                        cmptabla = tag; cmpcodigo = "cod_bod"; cmpnombre = "nom_bod"; cmporden = "cod_bod"; cmpidrow = "idrow"; cmptitulo = "Maestra de bodegas"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";//tipo_bod='4'
                    }
                    if (tag == "inmae_prv")
                    {
                        cmptabla = tag; cmpcodigo = "cod_prv"; cmpnombre = "nom_prv"; cmporden = "cod_prv"; cmpidrow = "idrow"; cmptitulo = "Maestra de provedores"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                    }
                    if (tag == "inmae_tip")
                    {
                        cmptabla = tag; cmpcodigo = "cod_tip"; cmpnombre = "nom_tip"; cmporden = "cod_tip"; cmpidrow = "idrow"; cmptitulo = "Maestra de linea"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                    }

                    int idr = 0; string code = ""; string nom = "";

                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Width = 500;
                    winb.Height = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (idr > 0)
                    {
                        if (tag == "inmae_bod")
                        {
                            TextCod_bod.Text = code;
                            loadBodConsg(code);
                        }
                        if (tag == "inmae_prv")
                        {
                            TextCod_Pro.Text = code;
                            loadNitPrv(code);

                            ProvedorExterior(code);
                        }
                        if (tag == "inmae_tip")
                        {
                            TextCod_Lin.Text = code;
                        }

                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;
                    if (e.Key == Key.Enter)
                    {
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("error p" + ex);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {


            string tag = ((TextBox)sender).Tag.ToString();
            string text = ((TextBox)sender).Text;

            if (string.IsNullOrEmpty(text)) return;

            //MessageBox.Show("text:"+ text);
            if (BuscarCodigo(tag, text) == false)
            {
                MessageBox.Show("El codigo que ingreso no existe");
                ((TextBox)sender).Text = "";
            }
            else
            {

                if ((sender as TextBox).Name == "TextCod_Pro")
                {
                    loadNitPrv(text);
                }

                if ((sender as TextBox).Name == "TextCod_bod")
                {
                    loadBodConsg(text);
                }
            }

        }

        public Boolean BuscarCodigo(string tag, string codigo)
        {
            Boolean bandera = false;
            string campo = "";
            switch (tag)
            {
                case "inmae_bod":
                    campo = "cod_bod";
                    break;
                case "inmae_prv":
                    campo = "cod_prv";
                    break;
                case "inmae_tip":
                    campo = "cod_tip";
                    break;
            }


            string cadena = "select * from " + tag + " where " + campo + "='" + codigo + "'  ";
            DataTable tabla = SiaWin.Func.SqlDT(cadena, "Buscar", idemp);
            if (tabla.Rows.Count > 0) bandera = true;

            return bandera;
        }

        public void loadNitPrv(string codigoPRV)
        {
            try
            {
                string cadena = "select nit_prv from InMae_prv where cod_prv='" + codigoPRV + "'; ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "nit", idemp);
                nitPRV = dt.Rows[0]["nit_prv"].ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar nit:" + w);
            }
        }

        public void loadBodConsg(string cod_bod)
        {
            try
            {
                string cadena = "select bod_cons from InMae_bod where cod_bod='" + cod_bod + "';";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "bodegas", idemp);
                if (dt.Rows.Count > 0)
                {
                    string lista = dt.Rows[0]["bod_cons"].ToString();
                    List<string> list = new List<string>(lista.Split(','));

                    foreach (var item in list)
                    {
                        BodegConsg += "'" + item.Trim() + "',";
                    }                    //BodegConsg = 
                }

                BodegConsg += "'" + cod_bod + "'";
                //  MessageBox.Show(BodegConsg);
                // BodegConsg = dt.Rows.Count> 0 ? dt.Rows[0]["bod_cons"].ToString() : cod_bod;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar nit:" + w);
            }
        }

        public void ProvedorExterior(string codigo)
        {
            try
            {
                string cadena = "select ind_imp from InMae_prv where cod_prv='" + codigo + "' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "bodega", idemp);

                if (dt.Rows[0]["ind_imp"].ToString() == "1")
                {
                    ProvedorExt = "Exterior";
                }
                else
                {
                    ProvedorExt = "Nacional";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("error en el codigo de el provedor");
            }

        }

        public void where(string linea)
        {
            if (TextCod_Lin.Text.Length > 0)
            {
                cadenaWhere = "AND ref.cod_tip='" + linea + "'  ";
            }
            else
            {
                cadenaWhere = " ";
            }
        }

        public void clearValuesBod()
        {
            Bod1.Text = "";
            alcance1.Text = "";
            Bod3_4.Text = "";
            alcance3_4.Text = "";
            Bod10.Text = "";
            alcance10.Text = "";
            Bod12_13.Text = "";
            alcance12_13.Text = "";
            Bod5.Text = "";
            alcance5.Text = "";
            Bod7_9.Text = "";
            alcance7_9.Text = "";
            Bod17_19.Text = "";
            alcance17_19.Text = "";
            Bod8.Text = "";
            alcance8.Text = "";
            Bod50_52.Text = "";
            alcance50_52.Text = "";
        }

        private async void Consultar(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FechaConsul.Text.Length <= 0)
                {
                    MessageBox.Show("debe de ingresar una fecha para la consulta");
                    return;
                }
                if (String.IsNullOrEmpty(TextCod_bod.Text))
                {
                    MessageBox.Show("debe de ingresar una bodega para la consulta");
                    return;
                }
                if (String.IsNullOrEmpty(TextCod_Pro.Text))
                {
                    MessageBox.Show("debe de ingresar una provedor para la consulta");
                    return;
                }




                dataGridCxC.ItemsSource = 0;
                TXtotal.Text = "";
                banderaSelect = false;
                dataGridCxC.SelectedItems.Clear();
                clearValuesBod();
                limpiarValoresSuma();

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;
                Panel.Opacity = 0.3;

                //dataGridCxC.ItemsSource = null;
                //CharGrid.DataContext = null;
                //AreaLineaTotal.ItemsSource = null;
                BTNconsultar.IsEnabled = false;

                source.CancelAfter(TimeSpan.FromSeconds(1));
                //tabitem.Progreso(true);


                DateTime fechaConsulta = Convert.ToDateTime(FechaConsul.Text);
                int monthFechaCon = fechaConsulta.Month;
                int mesIni = Int32.Parse(TextBox_Meses.Value.ToString());
                DateTime _mesini = fechaConsulta.AddMonths(-mesIni);
                int monthMesCon = _mesini.Month;
                int prom = monthFechaCon - monthMesCon;

                string queryFecha = ""; int _mes = 1; int _aum_con = 0; int _aum_mesi = 1; int _dia_um = 1;
                string armaCAmpos = "";
                //string cadenaUpdate = "update #tempora set mes1=ISNULL(mes1,0),mes2=ISNULL(mes2,0),mes3=ISNULL(mes3,0),mes4=ISNULL(mes4,0),mes5=ISNULL(mes5,0);";
                string cadenaUpdate = "update #tempora set ";

                //string[] columnas = new string[12];
                List<string> columnas = new List<string>();

                string feciniSum = "";

                for (int i = 0; i < mesIni; i++)
                {
                    DateTime _fec_con = _mesini.AddMonths(_aum_con);//07-
                    DateTime _fec_con_day = _fec_con.AddDays(_dia_um);
                    var f = _fec_con_day.ToString("dd/MM/yyyy");

                    if (i == 0) feciniSum = f;

                    DateTime _fec_mes = _mesini.AddMonths(_aum_mesi);// 01/06/2018 - 01/07/2018
                    var m = _fec_mes.ToString("dd/MM/yyyy");

                    queryFecha += "sum(IIF(convert(date,cab.fec_trn,103) BETWEEN '" + f + "' and '" + m + "' , IIF(cab.cod_trn BETWEEN '004' and '005', cantidad, -cantidad), 00000000000.00)) as [mes" + _mes + "], ";
                    armaCAmpos += "mes" + _mes + " numeric(12,2),";
                    cadenaUpdate += "mes" + _mes + "=ISNULL(mes" + _mes + ",0),";

                    columnas.Add("mes" + _mes);

                    _mes++; _aum_con++; _aum_mesi++;
                }

                cadenaUpdate += "total=ISNULL(total,0);";

                //alcance1.Text = queryFecha;

                where(TextCod_Lin.Text);

                string v_bodega = TextCod_bod.Text;
                string v_provedor = TextCod_Pro.Text;
                string v_armarFecha = queryFecha;
                string v_mesini = _mesini.ToString("dd/MM/yyyy");
                string v_fechConsu = FechaConsul.Text;
                string v_armaCampos = armaCAmpos;
                string v_armaWhere = cadenaWhere;
                string v_fechBack = FechaBack.Text;
                string v_codEmp = cod_empresa;
                string v_costo_unitario = "";
                string updateTotales = cadenaUpdate;
                v_costo_unitario = ProvedorExt == "Exterior" ? "cos_usd" : "vrunc";
                string bodConsignacion = BodegConsg.Trim();
                string fec_pedido = Fec_pedido.Text;
                string feinisuma = feciniSum;

                //MessageBox.Show("!pantalla en mantenimineto por favor espere!");
                if (SiaWin._UserId == 21)
                {
                    //MessageBox.Show("feinisuma:" + feinisuma);
                    //MessageBox.Show("v_bodega:" + v_bodega);
                    //MessageBox.Show("v_bodega:" + v_bodega);
                    //MessageBox.Show("v_provedor:" + v_provedor);
                    //MessageBox.Show("v_armarFecha:" + v_armarFecha);
                    //MessageBox.Show("v_mesini:" + v_mesini);
                    //MessageBox.Show("v_fechConsu:" + v_fechConsu);
                    //MessageBox.Show("v_armaCampos:" + v_armaCampos);
                    //MessageBox.Show("v_armaWhere:" + v_armaWhere);
                    //MessageBox.Show("v_fechBack:" + v_fechBack);
                    //MessageBox.Show("v_codEmp:" + v_codEmp);
                    //MessageBox.Show("v_costo_unitario:" + v_costo_unitario);
                    //MessageBox.Show("//@updateTotales:" + updateTotales);
                    //MessageBox.Show("@BodegConsg:" + bodConsignacion);
                    //MessageBox.Show("@feinisuma:" + feinisuma);
                    //MessageBox.Show("@v_costo_unitario:" + v_costo_unitario);
                }



                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(v_bodega, v_provedor, v_armarFecha, v_mesini, v_fechConsu, v_armaCampos, v_armaWhere, v_fechBack, v_costo_unitario, updateTotales, bodConsignacion, fec_pedido, v_codEmp, feinisuma, source.Token), source.Token);
                await slowTask;
                BTNconsultar.IsEnabled = true;
                //tabitem.Progreso(false);
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {

                    DataTable tableTemp = ((DataSet)slowTask.Result).Tables[0];
                    decimal total_sugerido = 0;

                    foreach (System.Data.DataRow row in tableTemp.Rows)
                    {
                        foreach (var item in columnas)
                        {
                            if (row[item] == DBNull.Value || string.IsNullOrEmpty(row[item].ToString().Trim()))
                            {
                                //if (row[item].ToString().Trim() == item.ToString().Trim())
                                //{
                                    row[item] = 0;
                                //}
                            }
                        }


                        #region operaciones


                        //decimal total = 0;

                        switch (v_bodega)
                        {                            
                            case "003":
                                row["saldoinv"] = Convert.ToDecimal(row["saldoinv"]) + Convert.ToDecimal(row["saldob4"]);                                
                                break;
                            case "004":
                                row["saldoinv"] = Convert.ToDecimal(row["saldoinv"]) + Convert.ToDecimal(row["saldob3"]);                                
                                break;                            
                            case "012":
                                row["saldoinv"] = Convert.ToDecimal(row["saldoinv"]) + Convert.ToDecimal(row["saldob13"]);                                
                                break;
                            case "013":
                                row["saldoinv"] = Convert.ToDecimal(row["saldoinv"]) + Convert.ToDecimal(row["saldob12"]);                                
                                break;                            
                            case "007":
                                row["saldoinv"] = Convert.ToDecimal(row["saldoinv"]) + Convert.ToDecimal(row["saldob9"]);                                
                                break;                            
                            case "009":
                                row["saldoinv"] = Convert.ToDecimal(row["saldoinv"]) + Convert.ToDecimal(row["saldob7"]);                                
                                break;
                            case "017":
                                row["saldoinv"] = Convert.ToDecimal(row["saldoinv"]) + Convert.ToDecimal(row["saldob19"]);
                                break;
                            case "019":
                                row["saldoinv"] = Convert.ToDecimal(row["saldoinv"]) + Convert.ToDecimal(row["saldob17"]);                                
                                break;
                            case "050":
                                row["saldoinv"] = Convert.ToDecimal(row["saldoinv"]) + Convert.ToDecimal(row["saldob52"]);                                
                                break;
                            case "052":
                                row["saldoinv"] = Convert.ToDecimal(row["saldoinv"]) + Convert.ToDecimal(row["saldob50"]);                                
                                break;
                        }


                        decimal total = Convert.ToDecimal(row["total"].ToString());                        

                        decimal meses = Convert.ToDecimal(TextBox_Meses.Value.ToString());
                        int promedio = Convert.ToInt32(total / meses);
                        row["promedio"] = promedio;

                        if (Convert.ToDecimal(row["promedio"]) != 0)
                        {                            
                            row["alcance"] = (Convert.ToDecimal(row["saldoinv"]) * 30) / Convert.ToDecimal(row["promedio"]);
                        }
                        else
                        {
                            row["alcance"] = 0;
                        }

                        decimal xmin = Convert.ToDecimal(TextBox_Minimo.Value.ToString()) * Convert.ToDecimal(row["promedio"]);
                        decimal pp = (xmin / 2) + xmin;
                        decimal maxi = Convert.ToDecimal(TextBox_Maximo.Value.ToString()) * Convert.ToDecimal(row["promedio"]);

                        //if (row["backorder"].ToString() == "") { row["backorder"] = 0.00; }
                        //if (row["bod900"].ToString() == "") { row["bod900"] = 0.00; }
                        //if (row["ped_pen"].ToString() == "") { row["ped_pen"] = 0.00; }

                        decimal suge = maxi - xmin + pp - Convert.ToDecimal(row["saldoinv"]) - Convert.ToDecimal(row["backorder"]) - Convert.ToDecimal(row["bod900"]) + Convert.ToDecimal(row["ped_pen"]);
                        row["sugerido"] = suge > 0 ? row["sugerido"] = suge : row["sugerido"] = "0.00";


                        row["cantidad_ped"] = "0.00";
                        //    row["subt_ped"] = "0.00";



                        decimal n = 0;
                        if (decimal.TryParse(row["sugerido"].ToString(), out n) == true)
                        {
                            decimal cos_uni = Convert.ToDecimal(row["cost_uni_ped"]);
                            decimal tot_sug = Convert.ToDecimal(row["sugerido"]);
                            total_sugerido += cos_uni * tot_sug;
                        }
                    }


                    TotalSugerTotal.Text = total_sugerido.ToString("C");


                    dataGridCxC.ItemsSource = tableTemp;


                    #endregion

                    foreach (string prime in columnas)
                    {
                        dataGridCxC.Columns[prime].Width = 60;
                    }

                    //foreach (var item in columnas)
                    //{
                    //    dataGridCxC.Columns[item].ShowHeaderToolTip = true;
                    //    DataTemplate template = new DataTemplate();
                    //    TextBlock texblo = new TextBlock(){ Text= "hola1"};
                    //    FrameworkElementFactory FEF = new FrameworkElementFactory(typeof(TextBlock));                        
                    //    FEF.SetValue(TextBlock.TextProperty, "aaa");                        
                    //    template.VisualTree = FEF;
                    //    dataGridCxC.Columns[item].HeaderToolTipTemplate = template;
                    //}



                    dataGridCxC.Columns["cod_tiva"].IsHidden = true;
                    dataGridCxC.Columns["por_iva"].IsHidden = true;

                    dataGridCxC.Columns["alc_001"].IsHidden = true;
                    dataGridCxC.Columns["alc_003"].IsHidden = true;
                    dataGridCxC.Columns["alc_004"].IsHidden = true;
                    dataGridCxC.Columns["alc_010"].IsHidden = true;
                    dataGridCxC.Columns["alc_012"].IsHidden = true;
                    dataGridCxC.Columns["alc_013"].IsHidden = true;
                    dataGridCxC.Columns["alc_005"].IsHidden = true;
                    dataGridCxC.Columns["alc_007"].IsHidden = true;
                    dataGridCxC.Columns["alc_009"].IsHidden = true;
                    dataGridCxC.Columns["alc_017"].IsHidden = true;
                    dataGridCxC.Columns["alc_019"].IsHidden = true;
                    dataGridCxC.Columns["alc_008"].IsHidden = true;

                    dataGridCxC.Columns["alc_050"].IsHidden = true;
                    dataGridCxC.Columns["alc_052"].IsHidden = true;


                    if (dtConfigu.Rows.Count > 0)
                    {
                        dataGridCxC.Columns["peso"].Width = Convert.ToInt32(dtConfigu.Rows[0]["col_peso_width"]);
                        dataGridCxC.Columns["peso"].IsHidden = Convert.ToInt32(dtConfigu.Rows[0]["col_peso"]) == 0 ? true : false;

                        dataGridCxC.Columns["total"].Width = Convert.ToInt32(dtConfigu.Rows[0]["col_total_width"]);
                        dataGridCxC.Columns["total"].IsHidden = Convert.ToInt32(dtConfigu.Rows[0]["col_total"]) == 0 ? true : false;

                        dataGridCxC.Columns["ped_pen"].Width = Convert.ToInt32(dtConfigu.Rows[0]["col_ped_pen_width"]);
                        dataGridCxC.Columns["ped_pen"].IsHidden = Convert.ToInt32(dtConfigu.Rows[0]["col_ped_pen"]) == 0 ? true : false;

                        dataGridCxC.Columns["saldoinv"].Width = Convert.ToInt32(dtConfigu.Rows[0]["col_saldoinv_width"]);
                        dataGridCxC.Columns["saldoinv"].IsHidden = Convert.ToInt32(dtConfigu.Rows[0]["col_saldoinv"]) == 0 ? true : false;

                        dataGridCxC.Columns["bod900"].Width = Convert.ToInt32(dtConfigu.Rows[0]["col_bod900_width"]);
                        dataGridCxC.Columns["bod900"].IsHidden = Convert.ToInt32(dtConfigu.Rows[0]["col_bod900"]) == 0 ? true : false;

                        dataGridCxC.Columns["promedio"].Width = Convert.ToInt32(dtConfigu.Rows[0]["col_promedio_width"]);
                        dataGridCxC.Columns["promedio"].IsHidden = Convert.ToInt32(dtConfigu.Rows[0]["col_promedio"]) == 0 ? true : false;

                        dataGridCxC.Columns["backorder"].Width = Convert.ToInt32(dtConfigu.Rows[0]["col_backorder_width"]);
                        dataGridCxC.Columns["backorder"].IsHidden = Convert.ToInt32(dtConfigu.Rows[0]["col_backorder"]) == 0 ? true : false;

                        dataGridCxC.Columns["alcance"].Width = Convert.ToInt32(dtConfigu.Rows[0]["col_alcance_width"]);
                        dataGridCxC.Columns["alcance"].IsHidden = Convert.ToInt32(dtConfigu.Rows[0]["col_alcance"]) == 0 ? true : false;

                        dataGridCxC.Columns["sugerido"].Width = Convert.ToInt32(dtConfigu.Rows[0]["col_sugerido_width"]);
                        dataGridCxC.Columns["sugerido"].IsHidden = Convert.ToInt32(dtConfigu.Rows[0]["col_sugerido"]) == 0 ? true : false;

                        dataGridCxC.FontSize = Convert.ToInt32(dtConfigu.Rows[0]["fuente"]);
                    }
                    else
                    {
                        dataGridCxC.Columns["peso"].Width = 60;
                        dataGridCxC.Columns["total"].Width = 60;
                        dataGridCxC.Columns["ped_pen"].Width = 60;
                        dataGridCxC.Columns["saldoinv"].Width = 60;
                        dataGridCxC.Columns["bod900"].Width = 60;
                        dataGridCxC.Columns["promedio"].Width = 60;
                        dataGridCxC.Columns["backorder"].Width = 60;
                        dataGridCxC.Columns["alcance"].Width = 60;
                        dataGridCxC.Columns["sugerido"].Width = 60;
                        dataGridCxC.Columns["cantidad_ped"].Width = 70;
                        dataGridCxC.Columns["cost_uni_ped"].Width = 80;
                        dataGridCxC.Columns["subt_ped"].Width = 80;
                    }

                    //dataGridCxC.Columns["cod_ref"].ShowHeaderToolTip = true;



                    dataGridCxC.Columns["cantidad_ped"].Width = 70;
                    dataGridCxC.Columns["cost_uni_ped"].Width = 80;
                    dataGridCxC.Columns["subt_ped"].Width = 80;

                    dataGridCxC.Columns["cantidad_ped"].AllowEditing = true;
                    dataGridCxC.Columns["cantidad_ped"].CellStyle = (Style)FindResource("edit");
                    dataGridCxC.Columns["cantidad_ped"].HeaderText = "cantidad a Pedir";

                    dataGridCxC.Columns["backorder"].CellStyle = (Style)FindResource("edit");
                    dataGridCxC.Columns["total"].CellStyle = (Style)FindResource("edit");
                    dataGridCxC.Columns["saldoinv"].CellStyle = (Style)FindResource("edit");

                    dataGridCxC.Columns["ped_pen"].HeaderText = "p pendiente";
                    dataGridCxC.Columns["cost_uni_ped"].HeaderText = "costo unitario";
                    dataGridCxC.Columns["subt_ped"].HeaderText = "subtotal";

                    TXtotal.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                    ////CharGrid.DataContext = ((DataSet)slowTask.Result).Tables[1];
                    ////AreaLineaTotal.ItemsSource = ((DataSet)slowTask.Result).Tables[1];

                    banderaSelect = true;
                }
                else
                {
                    TXtotal.Text = "0";
                }

                Panel.Opacity = 1;
                this.sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("error111-" + w.ToString());
            }

        }


        private DataSet LoadData(string bodega, string provedor, string armarFecha, string mesini, string fechConsu, string armaCampos, string armaWhere, string fechBack,
            string costo_unitario, string update, string bodConsignacion, string fec_pedido, string codEmp, string fecsuma, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("GeneracionPedidosProvedores", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_bod", bodega);
                cmd.Parameters.AddWithValue("@cod_prv", provedor);
                cmd.Parameters.AddWithValue("@ArmaFech", armarFecha);
                cmd.Parameters.AddWithValue("@mesIni", mesini);
                cmd.Parameters.AddWithValue("@fechaConsulta", fechConsu);
                cmd.Parameters.AddWithValue("@armarCampos", armaCampos);
                cmd.Parameters.AddWithValue("@armarWhere", armaWhere);
                cmd.Parameters.AddWithValue("@fec_back", fechBack);
                cmd.Parameters.AddWithValue("@campo_costoUni", costo_unitario);
                cmd.Parameters.AddWithValue("@updateTotales", update);
                cmd.Parameters.AddWithValue("@Bodegaconsigna", bodConsignacion);
                cmd.Parameters.AddWithValue("@fec_pedido_compra", fec_pedido);
                cmd.Parameters.AddWithValue("@cod_empresa", cod_empresa);
                cmd.Parameters.AddWithValue("@fecsumaini", fecsuma);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show("error");
                return null;
            }
        }

        public void alcanceRed(TextBox alcance)
        {
            int valor = Convert.ToInt32(alcance.Text);
            if (valor >= 180)
            {
                alcance.Foreground = Brushes.Red;
            }
        }

        private void dataGridCxC_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            try
            {
                if (banderaSelect == true)
                {
                    DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                    saldoBodegas();
                    alcanceBodegas(row["cod_ref"].ToString());
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("erro en  alcance:" + w);
            }

        }

        public void saldoBodegas()
        {
            try
            {

                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];

                Bod1.Text = row["saldob1"].ToString();

                decimal bod3 = Convert.ToDecimal(row["saldob3"].ToString());
                decimal bod4 = Convert.ToDecimal(row["saldob4"].ToString());
                Bod3_4.Text = (bod3 + bod4).ToString();

                Bod10.Text = row["saldob10"].ToString();

                decimal bod12 = Convert.ToDecimal(row["saldob12"].ToString());
                decimal bod13 = Convert.ToDecimal(row["saldob13"].ToString());
                Bod12_13.Text = (bod12 + bod13).ToString();

                Bod5.Text = row["saldob5"].ToString();

                decimal bod7 = Convert.ToDecimal(row["saldob7"].ToString());
                decimal bod9 = Convert.ToDecimal(row["saldob9"].ToString());
                Bod7_9.Text = (bod7 + bod9).ToString();

                decimal bod17 = Convert.ToDecimal(row["saldob17"].ToString());
                decimal bod19 = Convert.ToDecimal(row["saldob19"].ToString());
                Bod17_19.Text = (bod17 + bod19).ToString();

                Bod8.Text = row["saldob8"].ToString();

                decimal bod50 = Convert.ToDecimal(row["saldob50"].ToString());
                decimal bod52 = Convert.ToDecimal(row["saldob52"].ToString());
                Bod50_52.Text = (bod50 + bod52).ToString();
                UpdateLayout();

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar saldos: " + w);
            }


        }

        public void alcanceBodegas(string referencia)
        {
            try
            {
                //int mesIni = Int32.Parse(TextBox_Meses.Value.ToString());
                //DateTime fechaConsulta = Convert.ToDateTime(FechaConsul.Text);
                //DateTime _mesini = fechaConsulta.AddMonths(-mesIni);

                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];

                //bod001                
                FormularAlcance(Convert.ToDecimal(row["alc_001"]), Bod1, alcance1);
                //bod003+4                
                FormularAlcance(Convert.ToDecimal(row["alc_003"]), Bod3_4, alcance3_4);
                //bod010
                FormularAlcance(Convert.ToDecimal(row["alc_010"]), Bod10, alcance10);
                //bod012_13                
                FormularAlcance(Convert.ToDecimal(row["alc_012"]), Bod12_13, alcance12_13);
                //bod005
                FormularAlcance(Convert.ToDecimal(row["alc_005"]), Bod5, alcance5);
                //bod007+9                
                FormularAlcance(Convert.ToDecimal(row["alc_007"]), Bod7_9, alcance7_9);
                //bod0017+19                
                FormularAlcance(Convert.ToDecimal(row["alc_017"]), Bod17_19, alcance17_19);
                //bod008
                FormularAlcance(Convert.ToDecimal(row["alc_008"]), Bod8, alcance8);
                //bod0050+052
                FormularAlcance(Convert.ToDecimal(row["alc_050"]), Bod50_52, alcance50_52);

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el procedimiento para obtener el alcance_:" + w);
            }
        }

        private void AssociatedObject_CopyGridCellContent(object sender, GridCopyPasteCellEventArgs e)
        {
            //Skip to copy contents for all the inactive cells from the selected row 
            SfDataGrid grid = e.OriginalSender is DetailsViewDataGrid ? (SfDataGrid)e.OriginalSender : (SfDataGrid)sender;
            if (grid != null && grid.SelectionController != null
                && grid.SelectionController.CurrentCellManager != null
                && grid.SelectionController.CurrentCellManager.CurrentCell != null
                && e.Column.MappingName != grid.SelectionController.CurrentCellManager.CurrentCell.GridColumn.MappingName)
            {
                e.Handled = true;
            }
        }

        public void FormularAlcance(decimal totalBod, TextBox saldo, TextBox destino)
        {
            try
            {

                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];                                
                decimal total = destino.Tag.ToString().Trim() == TextCod_bod.Text.Trim() ?
                    Convert.ToDecimal(row["total"]):totalBod;

                decimal meses = Convert.ToDecimal(TextBox_Meses.Value.ToString());                
                int promedio = Convert.ToInt32(total / meses);
                
                decimal alcance = 0;

                if (Convert.ToInt32(promedio) != 0) {
                    alcance = (Convert.ToDecimal(saldo.Text) * 30) / Convert.ToDecimal(promedio);                    
                }

               destino.Foreground = alcance >= 180 ? Brushes.Red : Brushes.Black;
               destino.Text = alcance.ToString();

            }
            catch (Exception)
            {
                MessageBox.Show("erro en la formula");
            }
        }

        private void BTNdetalle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Detalle ventana = new Detalle();
                //Sia.PublicarPnt(9670,"DetallePedidosProvedor");
                dynamic ventana = SiaWin.WindowExt(9670, "DetallePedidosProvedor");

                ventana.idemp = idemp;
                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];


                DateTime fechaConsulta = Convert.ToDateTime(FechaConsul.Text);
                int mesIni = Int32.Parse(TextBox_Meses.Value.ToString());
                DateTime _mesini = fechaConsulta.AddMonths(-mesIni);

                DateTime fechaBack = Convert.ToDateTime(FechaBack.Text);

                ventana.referencia = row["cod_ref"].ToString();
                ventana.bodega = TextCod_bod.Text;
                ventana.mesini = _mesini.ToString("dd/MM/yyyy");
                ventana.backorder = fechaBack.ToString("dd/MM/yyyy");
                ventana.fec_con = fechaConsulta.ToString("dd/MM/yyyy");
                ventana.fec_pedido = Fec_pedido.Text;

                ventana.empresa = cod_empresa;

                ventana.Owner = Application.Current.MainWindow;
                ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ventana.ShowInTaskbar = false;
                ventana.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("error al cargar pedidos y bakorder");
            }
        }

        public Boolean ValidarCampos(int codigo)
        {
            Boolean bandera = false;

            //validar fecha de entrega
            if (codigo == 1)
            {
                if (FechaEntre.Text.Length > 0)
                {
                    bandera = true;
                }
                else
                {
                    MessageBox.Show("ingrese la fecha de entrega");
                }
            }

            return bandera;
        }

        public Boolean validarRegitros()
        {
            if (dataGridCxC.ItemsSource == null) return false;

            Boolean bandera = false;
            int a = 1;
            var reflector = this.dataGridCxC.View.GetPropertyAccessProvider();
            foreach (var row in dataGridCxC.View.Records)
            {
                var rowData = dataGridCxC.GetRecordAtRowIndex(a);
                var cantidad = reflector.GetValue(rowData, "cantidad_ped");
                if (cantidad.ToString() != "0.00" && cantidad.ToString() != "0")
                {
                    bandera = true;
                    break;
                }

                a = a + 1;
            }

            return bandera;
        }

        private void dataGridCxC_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            try
            {

                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                decimal cantidad = Convert.ToDecimal(row["cantidad_ped"]);



                decimal costo = Convert.ToDecimal(row["cost_uni_ped"]);
                row["subt_ped"] = cantidad * costo;

                if (cantidad.ToString() != "0.00" && cantidad.ToString() != "0")
                {
                    decimal cantidadSugerido = Convert.ToDecimal(row["sugerido"]);
                    row["subt_ped_sugerido"] = cantidadSugerido * costo;
                }
                else
                {
                    row["subt_ped_sugerido"] = "0.00";
                }
                if (cantidad.ToString() != "0.00" && cantidad.ToString() != "0")
                {
                    row["subt_kg"] = cantidad * Convert.ToDecimal(row["peso"]);
                }
                else
                {
                    row["subt_kg"] = "0.00";
                }



                sumarValores();
            }
            catch (Exception)
            {
                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                row["cantidad_ped"] = "0.00";
                row["subt_ped_sugerido"] = "0.00";
                row["subt_kg"] = "0.00";
                row["subt_ped"] = "0.00";
                sumarValores();
            }

        }

        public void validaredicion(string cantidad)
        {

            if (cantidad == null || string.IsNullOrWhiteSpace(cantidad) || string.IsNullOrEmpty(cantidad))
            {
                MessageBox.Show("vacio");
            }
            else
            {
                MessageBox.Show("lleno");
            }

        }

        public void sumarValores()
        {
            int a = 1;
            decimal suma = 0;
            decimal sumaSugerido = 0;
            decimal sumakg = 0;
            var reflector = this.dataGridCxC.View.GetPropertyAccessProvider();
            foreach (var row in dataGridCxC.View.Records)
            {

                var rowData = dataGridCxC.GetRecordAtRowIndex(a);
                var cantidad = reflector.GetValue(rowData, "cantidad_ped");
                var total = reflector.GetValue(rowData, "subt_ped");
                var total_sugerido = reflector.GetValue(rowData, "subt_ped_sugerido");
                var peso = reflector.GetValue(rowData, "subt_kg");
                if (cantidad.ToString() != "0.00" && cantidad.ToString() != "0")
                {
                    suma = suma + Convert.ToDecimal(total);
                    sumaSugerido = sumaSugerido + Convert.ToDecimal(total_sugerido);
                    sumakg = sumakg + Convert.ToDecimal(peso);
                }

                a = a + 1;
            }

            TotPedi.Text = suma.ToString("C");
            TotalSuger.Text = sumaSugerido.ToString("C");
            Totalkg.Text = sumakg.ToString() + " Kg";
        }

        public void limpiarValoresSuma()
        {
            TotPedi.Text = "$ 0";
            TotalSuger.Text = "$ 0";
            Totalkg.Text = "0 Kg";
        }

        private void BTNdocument_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (ValidarCampos(1) == false) return;
                if (validarRegitros() == false)
                {
                    MessageBox.Show("no se puede generar el documento por que no ha registrado ningun cantidad a pedir");
                    return;
                }

                int idreg = Documento();
                if (idreg > 0)
                    //SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 5, 42, 0, "Ingreso a:ReimprimirFacturasyNotasCredito Empresa:" + nomemp, "");
                    //SiaWin.Auditor(idreg, "Factura Electronica:" + response.codigo.ToString() + " " + response.mensaje, _ModuloId, _AccesoId);

                    SiaWin.TabTrn(0, idemp, true, idreg, moduloid, WinModal: true);

            }
            catch (Exception w)
            {
                MessageBox.Show("error en la generacion:" + w);
            }

        }

        public int Documento()
        {
            try
            {
                int idreg = 0;
                if (MessageBox.Show("Usted desea guardar el documento..?", "Guardar Traslado", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    string codtrn = "500";
                    string TipoConsecutivo = "pedidos";
                    string cod_bod = TextCod_bod.Text;
                    DateTime fechaActual = DateTime.Today;

                    using (SqlConnection connection = new SqlConnection(cnEmp))
                    {

                        connection.Open();
                        StringBuilder errorMessages = new StringBuilder();
                        SqlCommand command = connection.CreateCommand();
                        SqlTransaction transaction;
                        //Start a local transaction.
                        transaction = connection.BeginTransaction("Transaction");
                        command.Connection = connection;
                        command.Transaction = transaction;


                        string sqlConsecutivo = @"declare @fecdoc as datetime;
                        set @fecdoc = getdate();declare @ini as char(4);
                        declare @num as varchar(12);declare @iConsecutivo char(12) = '' ;
                        declare @iFolioHost int = 0;" +
                        "SELECT @iFolioHost = " + TipoConsecutivo + ",@ini=rtrim(cod_pvt) FROM Copventas  WHERE cod_pvt='" + cod_bod + "';" +
                        "set @num=@iFolioHost;";
                        sqlConsecutivo += "select @iConsecutivo = rtrim(@ini) + '-' + rtrim(convert(varchar, @num))";
                        sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num)); ";


                        string sqlcab = sqlConsecutivo + @"INSERT INTO incab_doc (ano_doc,per_doc,cod_trn,num_trn,fec_trn,cod_prv,suc_rem,fec_ven)
                        values ('" + DateTime.Now.Year.ToString() + "','" + fechaActual.ToString("MM") + "','" + codtrn + "',@iConsecutivo,@fecdoc,'" + nitPRV + "','" + cod_bod + "','" + FechaEntre.Text + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";


                        string sqlcue = "";
                        var reflector = this.dataGridCxC.View.GetPropertyAccessProvider();
                        int a = 1;
                        foreach (var row in dataGridCxC.View.Records)
                        {
                            foreach (var column in dataGridCxC.Columns)
                            {
                                if (column.MappingName == "cantidad_ped")
                                {
                                    var rowData = dataGridCxC.GetRecordAtRowIndex(a);
                                    var cantidad = reflector.GetValue(rowData, "cantidad_ped");
                                    var referencias = reflector.GetValue(rowData, "cod_ref");
                                    decimal cost_uni = Convert.ToDecimal(reflector.GetValue(rowData, "cost_uni_ped"));
                                    string subtotal = reflector.GetValue(rowData, "subt_ped").ToString();

                                    decimal por_iva = Convert.ToDecimal(reflector.GetValue(rowData, "por_iva"));
                                    var cod_tiva = reflector.GetValue(rowData, "cod_tiva");
                                    decimal val_iva = (cost_uni * por_iva) / 100;


                                    if (cantidad.ToString() != "0.00" && cantidad.ToString() != "0")
                                    {
                                        sqlcue = sqlcue + @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_sub,cod_ref,cod_bod,cantidad,cos_uni,cos_tot,cod_tiva,por_iva,val_iva) values (@NewID,'" + codtrn + "',@iConsecutivo,'050','" + referencias + "','" + cod_bod + "'," + cantidad + "," + cost_uni.ToString("F", CultureInfo.InvariantCulture) + "," + subtotal.Replace(',', '.') + ",'" + cod_tiva + "'," + por_iva + "," + val_iva + ");";
                                    }

                                    break;
                                }
                            }
                            a = a + 1;
                        }

                        //MessageBox.Show("a1");
                        string actualzaConsecu = "UPDATE COpventas SET " + TipoConsecutivo + " = ISNULL(" + TipoConsecutivo + ", 0) + 1  WHERE cod_pvt='" + cod_bod + "';";
                        command.CommandText = sqlcab + sqlcue + actualzaConsecu + @"select CAST(@NewId AS int);";
                        //MessageBox.Show(command.CommandText.ToString());
                        var r = new object();
                        r = command.ExecuteScalar();
                        transaction.Commit();
                        connection.Close();
                        MessageBox.Show("documento generado");
                        idreg = Convert.ToInt32(r.ToString());
                    }

                    return idreg;
                }
                else
                {
                    MessageBox.Show("no se genero el Documento");
                    return 0;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el documento:" + w);
                return 0;
            }
        }


        private void Export_excel(object sender, RoutedEventArgs e)
        {

            var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            options.ExcelVersion = ExcelVersion.Excel2013;
            var excelEngine = dataGridCxC.ExportToExcel(dataGridCxC.View, options);
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

                if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
            }
        }

        private void DataGridCxC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F8 || e.Key == Key.F6)
            {
                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar("InMae_ref", "cod_ref", "nom_ref", "idrow", "idrow", "Maestra de referencia", SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                winb.Height = 400;
                winb.Width = 500;
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                winb = null;

                try
                {
                    if (string.IsNullOrEmpty(code)) return;

                    if (recorrer(code.Trim()) == true)
                    {
                        dataGridCxC.SearchHelper.SearchHighlightBrush = Brushes.Transparent;
                        this.dataGridCxC.SearchHelper.ClearSearch();
                        //this.dataGridTabla.SearchHelper.FindNext(code);                        
                        this.dataGridCxC.SearchHelper.FindNext(code);
                        this.dataGridCxC.SelectionController.MoveCurrentCell(this.dataGridCxC.SearchHelper.CurrentRowColumnIndex);
                        dataGridCxC.SearchHelper.SearchHighlightBrush = Brushes.Transparent;                        
                        DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                        saldoBodegas();
                        alcanceBodegas(row["cod_ref"].ToString());
                    }
                    else
                    {
                        MessageBox.Show("no se encuentra la referencia");
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("error al recorrer");
                }
            }
        }

        public Boolean recorrer(string WinRef)
        {
            Boolean bandera = false;
            var reflector = this.dataGridCxC.View.GetPropertyAccessProvider();
            int a = 1;
            foreach (var row in dataGridCxC.View.Records)
            {
                foreach (var column in dataGridCxC.Columns)
                {
                    if (column.MappingName == "cod_ref")
                    {
                        var rowData = dataGridCxC.GetRecordAtRowIndex(a);

                        var referencias = reflector.GetValue(rowData, "cod_ref");

                        if (referencias.ToString().Trim() == WinRef)
                        {
                            bandera = true;
                        }
                        break;
                    }
                }
                a = a + 1;
            }
            return bandera;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tag = ((Button)sender).Tag.ToString();
                //if (SiaWin._UserId == 21)
                //{

                //    MessageBox.Show("tag:"+tag);
                //    MessageBox.Show("GetEmpresa(tag):" + GetEmpresa(tag));


                //}

                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                dynamic w = SiaWin.WindowExt(9466, "Kardex");
                w.ShowInTaskbar = false;
                w.Owner = Application.Current.MainWindow;
                w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                w.idemp = idemp;
                w.codref = row["cod_ref"].ToString();
                //string tag = ((Button)sender).Tag.ToString();
                w.codbod = tag;
                w.codemp = GetEmpresa(tag);
                w.ShowDialog();
            }
            catch (Exception w)
            {
                MessageBox.Show("selecione la referencia:" + w);
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

        private void BTNConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Configuracion ventana = new Configuracion();
                ventana.Owner = Application.Current.MainWindow;
                ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ventana.ShowInTaskbar = false;
                ventana.ShowDialog();

                if (ventana.flag == true)
                {
                    dtConfigu.Clear();
                    dtConfigu = SiaWin.Func.SqlDT("select * from configPntPedidosProv  where UserId='200' ", "config", 0);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir:" + w);
            }
        }

        private void TxVentas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridCxC.SelectedIndex >= 0)
                {

                    //AnalisisDeVenta ventana = new AnalisisDeVenta(idemp);
                    dynamic ventana = SiaWin.WindowExt(9671, "AnalisisVentaWindows");
                    ventana.idemp = idemp;

                    DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                    DateTime fechaConsulta = Convert.ToDateTime(FechaConsul.Text);
                    int mesIni = Int32.Parse(TextBox_Meses.Value.ToString());
                    DateTime _mesini = fechaConsulta.AddMonths(-mesIni);
                    ventana.FecIni.Text = _mesini.ToString("dd/MM/yyyy");
                    ventana.FecFin.Text = fechaConsulta.ToString("dd/MM/yyyy");
                    ventana.TextBoxRefI.Text = row["cod_ref"].ToString();
                    ventana.TextBoxRefF.Text = row["cod_ref"].ToString();
                    ventana.Owner = Application.Current.MainWindow;
                    ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    ventana.ShowInTaskbar = false;
                    ventana.ShowDialog();
                }
                else
                {
                    MessageBox.Show("seleccione una referencia de la grilla");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir:" + w);
            }
        }

        private void TxSia_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region armar query
                
                DateTime fechaConsulta = Convert.ToDateTime(FechaConsul.Text);
                int monthFechaCon = fechaConsulta.Month;
                int mesIni = Int32.Parse(TextBox_Meses.Value.ToString());
                DateTime _mesini = fechaConsulta.AddMonths(-mesIni);
                int monthMesCon = _mesini.Month;
                int prom = monthFechaCon - monthMesCon;

                string queryFecha = ""; int _mes = 1; int _aum_con = 0; int _aum_mesi = 1; int _dia_um = 1;
                string armaCAmpos = "";                
                string cadenaUpdate = "update #tempora set ";
                
                List<string> columnas = new List<string>();

                string feciniSum = "";

                for (int i = 0; i < mesIni; i++)
                {
                    DateTime _fec_con = _mesini.AddMonths(_aum_con);//07-
                    DateTime _fec_con_day = _fec_con.AddDays(_dia_um);
                    var f = _fec_con_day.ToString("dd/MM/yyyy");

                    if (i == 0) feciniSum = f;

                    DateTime _fec_mes = _mesini.AddMonths(_aum_mesi);// 01/06/2018 - 01/07/2018
                    var m = _fec_mes.ToString("dd/MM/yyyy");

                    queryFecha += "sum(IIF(convert(date,cab.fec_trn,103) BETWEEN '" + f + "' and '" + m + "' , IIF(cab.cod_trn BETWEEN '004' and '005', cantidad, -cantidad), 00000000000.00)) as [mes" + _mes + "], ";
                    armaCAmpos += "mes" + _mes + " numeric(12,2),";
                    cadenaUpdate += "mes" + _mes + "=ISNULL(mes" + _mes + ",0),";

                    columnas.Add("mes" + _mes);

                    _mes++; _aum_con++; _aum_mesi++;
                }

                cadenaUpdate += "total=ISNULL(total,0);";

                #endregion

                where(TextCod_Lin.Text);

                string v_bodega = TextCod_bod.Text;
                string v_provedor = TextCod_Pro.Text;
                string v_armarFecha = queryFecha;
                string v_mesini = _mesini.ToString("dd/MM/yyyy");
                string v_fechConsu = FechaConsul.Text;
                string v_armaCampos = armaCAmpos;
                string v_armaWhere = cadenaWhere;
                string v_fechBack = FechaBack.Text;
                string v_codEmp = cod_empresa;
                string v_costo_unitario = "";
                string updateTotales = cadenaUpdate;
                v_costo_unitario = ProvedorExt == "Exterior" ? "cos_usd" : "vrunc";
                string bodConsignacion = BodegConsg.Trim();
                string fec_pedido = Fec_pedido.Text;
                string feinisuma = feciniSum;

                //MessageBox.Show("FechaConsul:" + FechaConsul.Text);
                //MessageBox.Show("feinisuma:" + feinisuma);
                //MessageBox.Show("v_bodega:" + v_bodega);
                //MessageBox.Show("v_bodega:" + v_bodega);
                //MessageBox.Show("v_provedor:" + v_provedor);
                //MessageBox.Show("v_armarFecha:" + v_armarFecha);
                //MessageBox.Show("v_mesini:" + v_mesini);
                //MessageBox.Show("v_fechConsu:" + v_fechConsu);
                //MessageBox.Show("v_armaCampos:" + v_armaCampos);
                //MessageBox.Show("v_armaWhere:" + v_armaWhere);
                //MessageBox.Show("v_fechBack:" + v_fechBack);
                //MessageBox.Show("v_codEmp:" + v_codEmp);
                //MessageBox.Show("v_costo_unitario:" + v_costo_unitario);
                //MessageBox.Show("//@updateTotales:" + updateTotales);
                //MessageBox.Show("@BodegConsg:" + bodConsignacion);
                //MessageBox.Show("@feinisuma:" + feinisuma);
                //MessageBox.Show("@v_costo_unitario:" + v_costo_unitario);
                //MessageBox.Show("lo seleccionado row");

                //DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                
                //MessageBox.Show("row[saldob1].ToString():" + row["saldob1"].ToString());

                //decimal bod3 = Convert.ToDecimal(row["saldob3"].ToString());
                //decimal bod4 = Convert.ToDecimal(row["saldob4"].ToString());
                //MessageBox.Show("row[saldob3].ToString():" + row["saldob3"].ToString());
                //MessageBox.Show("row[saldob4].ToString():" + row["saldob4"].ToString());

                //MessageBox.Show("row[saldob10].ToString():" + row["saldob10"].ToString());                

                //decimal bod12 = Convert.ToDecimal(row["saldob12"].ToString());
                //decimal bod13 = Convert.ToDecimal(row["saldob13"].ToString());
                //MessageBox.Show("row[saldob12].ToString():" + row["saldob12"].ToString());
                //MessageBox.Show("row[saldob13].ToString():" + row["saldob13"].ToString());

                //MessageBox.Show("row[saldob5].ToString():" + row["saldob5"].ToString());

                //decimal bod7 = Convert.ToDecimal(row["saldob7"].ToString());
                //decimal bod9 = Convert.ToDecimal(row["saldob9"].ToString());
                //MessageBox.Show("row[saldob7].ToString():" + row["saldob7"].ToString());
                //MessageBox.Show("row[saldob9].ToString():" + row["saldob9"].ToString());

                //decimal bod17 = Convert.ToDecimal(row["saldob17"].ToString());
                //decimal bod19 = Convert.ToDecimal(row["saldob19"].ToString());
                //MessageBox.Show("row[saldob17].ToString():" + row["saldob17"].ToString());
                //MessageBox.Show("row[saldob19].ToString():" + row["saldob19"].ToString());


                //MessageBox.Show("row[saldob8].ToString():" + row["saldob8"].ToString());

                //decimal bod50 = Convert.ToDecimal(row["saldob50"].ToString());
                //decimal bod52 = Convert.ToDecimal(row["saldob52"].ToString());
                //MessageBox.Show("row[saldob50].ToString():" + row["saldob50"].ToString());
                //MessageBox.Show("row[saldob52].ToString():" + row["saldob52"].ToString());

                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                DataTable dt = row.DataView.ToTable();
                //popo
                SiaWin.Browse(dt);

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el button sia:" + w);
            }
        }




        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        //SiaWin.Auditor(idrowcab, "Factura Electronica:" + response.codigo.ToString() + " " + response.mensaje, _ModuloId, _AccesoId);
        //        //SiaWin.Auditor(0, "Factura Electronica:" , , 3);
        //        decimal saldoin = SiaWin.Func.SaldoInv("4515IN", "004", "010");
        //        MessageBox.Show("saldoin:"+ saldoin);
        //    }
        //    catch (Exception w)
        //    {
        //        MessageBox.Show("error auditoria:" + w);
        //    }
        //}





    }
}
