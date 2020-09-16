using Egreso;
using Microsoft.Reporting.WinForms;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9541,"Egreso");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9541, "Egreso");
    //ww.codpvta = "003";
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();    

    public partial class Egreso : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int moduloid = 0;

        public string codbod = "";
        public string codpvta = "";

        DataTable dtCue = new DataTable();
        DataTable dtBanco = new DataTable();

        DataTable dt_egreso = new DataTable();


        double valorCxC = 0;
        double valorCxCAnt = 0;
        double valorCxP = 0;
        double valorCxPAnt = 0;
        double saldoCxC = 0;
        double saldoCxCAnt = 0;
        double saldoCxP = 0;
        double saldoCxPAnt = 0;
        double abonoCxC = 0;
        double abonoCxCAnt = 0;
        double abonoCxP = 0;
        double abonoCxPAnt = 0;

        double Retefte = 0;
        double Reteica = 0;
        double Reteiva = 0;
        double Descuento = 0;

        double VlrSaldo = 0;



        public Egreso()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();

            ActivaDesactivaControles(0);
            BtbGrabar.Focus();
            loadEgreso();
            act(1);

            this.DataContext = this;


        }

        public void loadEgreso()
        {
            dt_egreso.Columns.Add("cod_cta");
            dt_egreso.Columns.Add("cod_ter");
            dt_egreso.Columns.Add("cod_cco");
            dt_egreso.Columns.Add("des_mov");
            dt_egreso.Columns.Add("doc_cruc");
            dt_egreso.Columns.Add("bas_mov", typeof(double));
            dt_egreso.Columns.Add("deb_mov", typeof(double));
            dt_egreso.Columns.Add("cre_mov", typeof(double));
            GridConfig.ItemsSource = dt_egreso.DefaultView;
        }


        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Egresos " + cod_empresa + "-" + nomempresa;
                TxtUser.Text = SiaWin._UserAlias;

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='CO'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());

                Fec_con.Text = DateTime.Now.ToString();
                GridConfig.SelectionController = new GridSelectionControllerExt(GridConfig);

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        public class GridSelectionControllerExt : GridSelectionController
        {
            private SfDataGrid grid;
            public GridSelectionControllerExt(SfDataGrid datagrid) : base(datagrid)
            {
                grid = datagrid;
            }
            protected override void ProcessKeyDown(KeyEventArgs args)
            {
                try
                {
                    var currentKey = args.Key;
                    var arguments = new KeyEventArgs(args.KeyboardDevice, args.InputSource, args.Timestamp, Key.Tab)
                    {
                        RoutedEvent = args.RoutedEvent
                    };
                    if (currentKey == Key.Enter)
                    {
                        if (grid.IsReadOnly == false && grid.CurrentColumn is GridTextColumn) { }
                        base.ProcessKeyDown(arguments);
                        args.Handled = arguments.Handled;
                        return;
                    }

                    if (currentKey == Key.Up)
                    {
                        //MessageBox.Show("a1");
                        if (grid.View.IsAddingNew == true || grid.View.IsCurrentBeforeFirst == true)
                        {
                            //MessageBox.Show("a2");
                            grid.View.CancelEdit();
                            grid.View.CancelNew();
                        }
                        //MessageBox.Show("a3");
                        grid.UpdateLayout();
                    }


                    base.ProcessKeyDown(args);
                }
                catch (Exception w)
                {
                    //MessageBox.Show("errro:::" + w);
                }
            }
        }

        void MoveToNextUIElement(KeyEventArgs e)
        {
            try
            {
                FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;
                TraversalRequest request = new TraversalRequest(focusDirection);
                UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
                if (elementWithFocus != null)
                    if (elementWithFocus.MoveFocus(request)) e.Handled = true;
            }
            catch (Exception w)
            {
                MessageBox.Show("error :" + w);
            }

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (BtbGrabar.Content.ToString().Trim() == "Nuevo") return;



            if (e.Key == Key.F5 && Tab1.IsSelected == true)
            {
                if (BtbGrabar.Content.ToString().Trim() == "Grabar")
                {
                    BtbGrabar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    return;
                }
            }
            else
            {
                if (e.Key == Key.F5 && Btn_Save.Content.ToString().Trim() == "Guardar")
                {
                    Btn_Save.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    return;
                }
            }

            if (e.Key == Key.F9)
            {
                if (dtCue.Rows.Count > 0)
                {
                    if (MessageBox.Show("Usted desea cruzar todos los documentos ?", "Cruzar pagos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No) return;
                    foreach (System.Data.DataRow dr in dtCue.Rows)
                    {
                        double _saldo = Convert.ToDouble(dr["saldo"].ToString());
                        dr.BeginEdit();
                        dr["abono"] = _saldo;
                        dr.EndEdit();
                    }
                    dataGrid.UpdateLayout();
                    sumaAbonos();
                    dataGrid.Focus();
                    dataGrid.SelectedIndex = 0;
                    //    dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[8]);
                }
            }
            if (e.Key == Key.F6)
            {
                if (dtCue.Rows.Count > 0)
                {
                    if (MessageBox.Show("Usted desea cancelar abonos .... ?", "Cancela Cruces de pagos", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.No) return;
                    foreach (System.Data.DataRow dr in dtCue.Rows)
                    {
                        dr.BeginEdit();
                        dr["abono"] = 0;
                        dr.EndEdit();
                    }
                    dataGrid.UpdateLayout();
                    sumaAbonos();
                    dataGrid.Focus();
                    dataGrid.SelectedIndex = 0;
                    //      dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[8]);
                }
            }
            if (e.Key == Key.Escape)
            {

                if (Tab1.IsSelected == true)
                {
                    if (BtbGrabar.Content.ToString().Trim() == "Grabar")
                    {
                        BtbCancelar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        e.Handled = false;
                        return;
                    }
                }


                if (Tab2.IsSelected == true)
                {
                    if (Btn_Save.Content.ToString().Trim() == "Guardar")
                    {
                        Btn_Cancel.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        e.Handled = false;
                        return;
                    }
                }
            }

        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            getCombBanc();

        }

        public void getCombBanc()
        {
            try
            {
                dtBanco = SiaWin.Func.SqlDT("select * from comae_ban", "bancos", idemp);
                CbBanco.ItemsSource = dtBanco.DefaultView;
                CbBanco.DisplayMemberPath = "nom_ban";
                CbBanco.SelectedValuePath = "cod_ban";
                CbBanco.SelectedIndex = 1;

                ComBo_Banco.ItemsSource = dtBanco.DefaultView;
                ComBo_Banco.DisplayMemberPath = "nom_ban";
                ComBo_Banco.SelectedValuePath = "cod_ban";
                ComBo_Banco.SelectedIndex = 1;

            }
            catch (Exception w)
            {
                MessageBox.Show(w.Message);
            }
        }


        public void ActivaDesactivaControles(int estado)
        {
            if (estado == 0)
            {
                TextCodeCliente.Text = string.Empty;
                TextNomCliente.Text = string.Empty;
                TextNumeroDoc.Text = string.Empty;
                CbTrans.IsEnabled = false;
                TxtCheque.Text = "";
                TXotroTer.Text = "";
                TextNota.Text = "";
                BtbGrabar.Content = "Nuevo";
                BtbCancelar.Content = "Salir";
                dataGrid.AllowEditing = true;
                dtCue.Clear();

                TextReteIva.Text = "0,00";
                TextRetefte.Text = "0,00";
                TextIca.Text = "0,00";
                txDes.Text = "0,00";
                TextVlrRecibido.Text = "0,00";

                Cta_ref.Text = "";
                Cta_Riva.Text = "";
                Cta_rivaDT.Text = "";
                CtaRica.Text = "";


                TextCodeCliente.Focusable = false;
                TextNomCliente.Focusable = false;
                TextNota.Focusable = false;
                TXotroTer.Focusable = false;
                CbBanco.Focusable = false;
                CbTrans.Focusable = false;
                DtFec.Focusable = false;
                TxtCheque.Focusable = false;

                valorCxC = 0;
                valorCxCAnt = 0;
                valorCxP = 0;
                valorCxPAnt = 0;
                saldoCxC = 0;
                saldoCxCAnt = 0;
                saldoCxP = 0;
                saldoCxPAnt = 0;
                abonoCxC = 0;
                abonoCxCAnt = 0;
                abonoCxP = 0;
                abonoCxPAnt = 0;
                Retefte = 0;
                Reteica = 0;
                Reteiva = 0;
                Descuento = 0;
                VlrSaldo = 0;
                Descuento = 0;

                TextVlrRecibido.Value = 0;
                TextRetefte.Value = 0;
                TextReteIva.Value = 0;
                TextIca.Value = 0;
                txDes.Value = 0;


                TextCxC.Text = "0,00";
                TextCxCAnt.Text = "0,00";
                TextCxP.Text = "0,00";
                TextCxPAnt.Text = "0,00";
                TotalCxc.Text = "0,00";
                TextCxCAbono.Text = "0,00";
                TextCxCAntAbono.Text = "0,00";
                TextCxPAbono.Text = "0,00";
                TextCxPAntAbono.Text = "0,00";
                TotalAbono.Text = "0,00";
                TextCxCSaldo.Text = "0,00";
                TextCxCAntSaldo.Text = "0,00";
                TextCxPSaldo.Text = "0,00";
                TextCxPAntSaldo.Text = "0,00";
                TotalSaldo.Text = "0,00";
                TotalRecaudo.Text = "0,00";


            }
            if (estado == 1) //creando
            {
                TextCodeCliente.Text = string.Empty;
                TextNomCliente.Text = string.Empty;
                TextNumeroDoc.Text = "";
                CbTrans.IsEnabled = true;
                DtFec.Text = DateTime.Now.ToString();

                BtbGrabar.Content = "Grabar";
                BtbCancelar.Content = "Cancelar";
                dataGrid.AllowEditing = false;
                dtCue.Clear();
                dataGrid.UpdateLayout();
                TextCodeCliente.Focusable = true;


                TextCodeCliente.Focusable = true;
                TextNomCliente.Focusable = true;
                TextNota.Focusable = true;
                TXotroTer.Focusable = true;
                CbBanco.Focusable = true;
                CbTrans.Focusable = true;
                DtFec.Focusable = true;
                TxtCheque.Focusable = true;


                TextNumeroDoc.Text = consecutivo();

                TextCodeCliente.Focusable = true;
                TextRetefte.Text = "0,00";
                TextIca.Text = "0,00";
                TextVlrRecibido.Text = "0,00";

                TextNota.Text = "";
                TXotroTer.Text = "";
                TxtCheque.Text = "";

                Cta_ref.Text = "";
                Cta_Riva.Text = "";
                Cta_rivaDT.Text = "";
                CtaRica.Text = "";

                TextCodeCliente.Focus();

            }
        }

        public string consecutivo()
        {
            string con = "";
            try
            {
                string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate(); ";
                sqlConsecutivo += "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);  ";
                sqlConsecutivo += "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0; ";
                sqlConsecutivo += "SELECT @iFolioHost= isnull(num_act,0)+1,@ini=rtrim(inicial) FROM Comae_trn WHERE cod_trn='02'; ";
                sqlConsecutivo += "set @num=@iFolioHost ";
                sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+REPLICATE ('0',12-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num)); ";
                sqlConsecutivo += "select @iConsecutivo as consecutivo;  ";

                DataTable dt = SiaWin.DB.SqlDT(sqlConsecutivo, "cons", idemp);

                if (dt.Rows.Count > 0)
                {
                    con = dt.Rows[0]["consecutivo"].ToString();
                }



            }
            catch (Exception w)
            {
                MessageBox.Show("error en el consecutivo:" + w);
                con = "***";
            }

            return con;
        }

        private void Tx_ter_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tercero = (sender as TextBox);
            validTer(tercero);

        }

        public void validTer(TextBox ter)
        {
            try
            {
                if (ter.Text.Length > 0)
                {
                    var tp = getTercero(ter.Text);
                    if (tp.Item1 == false)
                    {
                        MessageBox.Show("el tercero ingresado no existe ingrese uno nuevamente");
                        TextCodeCliente.Text = "";
                        int idr = 0; string code = ""; string nombre = "";
                        dynamic xx = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "nom_ter", "idrow", "Maestra de clientes", cnEmp, false, "", idEmp: idemp);
                        xx.ShowInTaskbar = false;
                        xx.Owner = Application.Current.MainWindow;
                        xx.Height = 400;
                        xx.ShowDialog();
                        idr = xx.IdRowReturn;
                        code = xx.Codigo;
                        nombre = xx.Nombre;
                        xx = null;
                        if (idr > 0)
                        {
                            if (ter.Name == "TextCodeCliente") { TextCodeCliente.Text = code; TextNomCliente.Text = nombre; ConsultaSaldoCartera(); }
                            else { tx_Clie.Text = code; Tx_NomCli.Text = nombre; }
                        }

                    }
                    else
                    {
                        if (tp.Item1 == true && ter.Name == "TextCodeCliente") ConsultaSaldoCartera();

                        if (ter.Name == "TextCodeCliente") TextNomCliente.Text = tp.Item2;
                        else Tx_NomCli.Text = tp.Item2;
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error ww" + w);
            }
        }


        public Tuple<bool, string> getTercero(string ter)
        {
            bool flag = false;
            string select = "select * from comae_ter where cod_ter='" + ter + "'";
            DataTable dt = SiaWin.Func.SqlDT(select, "tercero", SiaWin._BusinessId);
            if (dt.Rows.Count > 0) flag = true;
            string nombre = dt.Rows.Count > 0 ? dt.Rows[0]["nom_ter"].ToString() : "";
            var tuple = new Tuple<bool, string>(flag, nombre);
            return tuple;
        }



        public async void ConsultaSaldoCartera()
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;
                dataGrid.ItemsSource = null;


                string tercero = TextCodeCliente.Text;
                string empresa = cod_empresa;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(tercero, empresa, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGrid.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    dtCue = ((DataSet)slowTask.Result).Tables[0];
                }

                this.sfBusyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                this.sfBusyIndicator.IsBusy = false;
                MessageBox.Show(ex.Message);
            }
        }

        private DataSet LoadData(string tercero, string empresa, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_empSpCoAnalisisCxc", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ter", tercero);
                cmd.Parameters.AddWithValue("@Cta", "");
                cmd.Parameters.AddWithValue("@TipoApli", -1);
                cmd.Parameters.AddWithValue("@Resumen", 1);
                cmd.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString());
                cmd.Parameters.AddWithValue("@TrnCo", "");
                cmd.Parameters.AddWithValue("@NumCo", "");
                cmd.Parameters.AddWithValue("@Cco", "");
                cmd.Parameters.AddWithValue("@Ven", "");
                cmd.Parameters.AddWithValue("@codemp", empresa);
                //dtCue.Clear();                
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                this.sfBusyIndicator.IsBusy = false;
                MessageBox.Show(e.Message);
                return null;
            }
        }



        //private void ConsultaSaldoCartera()
        //{
        //    try
        //    {
        //        SqlConnection con = new SqlConnection(SiaWin._cn);
        //        SqlCommand cmd = new SqlCommand();
        //        SqlDataAdapter da = new SqlDataAdapter();
        //        DataSet ds = new DataSet();

        //        cmd = new SqlCommand("_empSpCoAnalisisCxc", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Ter", TextCodeCliente.Text.Trim());
        //        cmd.Parameters.AddWithValue("@Cta", "");
        //        cmd.Parameters.AddWithValue("@TipoApli", -1);
        //        cmd.Parameters.AddWithValue("@Resumen", 1);
        //        cmd.Parameters.AddWithValue("@Fecha", DateTime.Now.ToString());
        //        cmd.Parameters.AddWithValue("@TrnCo", "");
        //        cmd.Parameters.AddWithValue("@NumCo", "");
        //        cmd.Parameters.AddWithValue("@Cco", "");
        //        cmd.Parameters.AddWithValue("@Ven", "");
        //        cmd.Parameters.AddWithValue("@codemp", cod_empresa);
        //        dtCue.Clear();

        //        //JESUS
        //        da = new SqlDataAdapter(cmd);
        //        da.Fill(dtCue);
        //        con.Close();
        //        //SiaWin.Browse(dtCue);

        //        if (dtCue.Rows.Count == 0)
        //        {
        //            MessageBox.Show("Sin informacion de cartera");
        //            dataGrid.ItemsSource = null;
        //            TextCodeCliente.Text = "";
        //            TextNomCliente.Text = "";
        //        }

        //        dataGrid.ItemsSource = dtCue.DefaultView;
        //    }
        //    catch (Exception W)
        //    {
        //        MessageBox.Show("Actualiza Grid www:" + W);
        //    }
        //}



        private void sumaAbonos()
        {
            try
            {
                if (string.IsNullOrEmpty(TextCodeCliente.Text)) return;

                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=3").ToString(), out abonoCxC);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=4").ToString(), out abonoCxCAnt);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=1").ToString(), out abonoCxP);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=2").ToString(), out abonoCxPAnt);

                double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=3").ToString(), out saldoCxC);
                double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=4").ToString(), out saldoCxCAnt);
                double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=1").ToString(), out saldoCxP);
                double.TryParse(dtCue.Compute("Sum(saldo)", "tip_apli=2").ToString(), out saldoCxPAnt);

                TextCxC.Text = saldoCxC.ToString("C");
                TextCxCAnt.Text = saldoCxCAnt.ToString("C");
                TextCxP.Text = saldoCxP.ToString("C");
                TextCxPAnt.Text = saldoCxPAnt.ToString("C");

                TextCxCAbono.Text = abonoCxC.ToString("C");
                TextCxCAntAbono.Text = abonoCxCAnt.ToString("C");
                TextCxPAbono.Text = abonoCxP.ToString("C");
                TextCxPAntAbono.Text = abonoCxPAnt.ToString("C");
                TextCxCSaldo.Text = (saldoCxC - abonoCxC).ToString("C");

                TextCxCAntSaldo.Text = (saldoCxCAnt - abonoCxCAnt).ToString("C");
                TextCxPSaldo.Text = (saldoCxP - abonoCxP).ToString("C");
                TextCxPAntSaldo.Text = (saldoCxPAnt - abonoCxPAnt).ToString("C");
                TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                TotalAbono.Text = (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt).ToString("C");
                TotalSaldo.Text = ((valorCxC - valorCxCAnt - valorCxP + valorCxPAnt) - (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt)).ToString("C"); ;



                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=3").ToString(), out abonoCxC);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=4").ToString(), out abonoCxCAnt);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=1").ToString(), out abonoCxP);
                double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=2").ToString(), out abonoCxPAnt);

                Reteica = Convert.ToDouble(TextIca.Value);
                Reteiva = Convert.ToDouble(TextReteIva.Value);
                Retefte = Convert.ToDouble(TextRetefte.Value);
                Descuento = Convert.ToDouble(txDes.Value);

                #region totales otros
                TextCxCAbono.Text = abonoCxC.ToString("C");
                TextCxCAntAbono.Text = abonoCxCAnt.ToString("C");
                TextCxPAbono.Text = abonoCxP.ToString("C");
                TextCxPAntAbono.Text = abonoCxPAnt.ToString("C");

                TextCxCSaldo.Text = (saldoCxC - abonoCxC).ToString("C");

                TextCxCAntSaldo.Text = (saldoCxCAnt - abonoCxCAnt).ToString("C");
                TextCxPSaldo.Text = (saldoCxP - abonoCxP).ToString("C");
                TextCxPAntSaldo.Text = (saldoCxPAnt - abonoCxPAnt).ToString("C");
                TotalCxc.Text = (valorCxC - valorCxCAnt - valorCxP + valorCxPAnt).ToString("C");
                TotalAbono.Text = (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt).ToString("C");
                TotalSaldo.Text = ((valorCxC - valorCxCAnt - valorCxP + valorCxPAnt) - (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt)).ToString("C");
                //TotalRecaudo.Text = (abonoCxC - abonoCxCAnt - abonoCxP + abonoCxPAnt - Retefte - Reteica - Reteiva - Descuento).ToString("C");
                #endregion

                double ret = Retefte + Reteica + Descuento;
                //double operation = (cxpSum + cxcantSum - cxcSum - cxpantSum) - (ret);
                double operation = (abonoCxP + abonoCxCAnt - abonoCxC - abonoCxPAnt) - (ret);

                TotalRecaudo.Text = operation.ToString("C");
            }
            catch (Exception W)
            {
                MessageBox.Show("sUMA DE ABONOS www:" + W);
            }
        }

        private void Tx_ter_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.F8 || e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty((sender as TextBox).Text))
                {
                    int idr = 0; string code = ""; string nombre = "";
                    dynamic xx = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "nom_ter", "idrow", "Maestra de clientes", cnEmp, false, "", idEmp: idemp);
                    xx.ShowInTaskbar = false;
                    xx.Owner = Application.Current.MainWindow;
                    xx.Height = 400;
                    xx.ShowDialog();
                    idr = xx.IdRowReturn;
                    code = xx.Codigo;
                    nombre = xx.Nombre;
                    xx = null;
                    if (idr > 0)
                    {
                        if ((sender as TextBox).Name == "TextCodeCliente") { TextCodeCliente.Text = code; TextNomCliente.Text = nombre; ConsultaSaldoCartera(); }
                        else { tx_Clie.Text = code; Tx_NomCli.Text = nombre; }
                    }
                }
            }
        }

        private void DataGrid_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {
                GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;




                if (Colum.MappingName == "abono")
                {
                    System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                    decimal _saldo = Convert.ToDecimal(dr["saldo"].ToString());
                    decimal _abono = Convert.ToDecimal(dr["abono"].ToString());
                    if (_abono > _saldo)
                    {
                        MessageBox.Show("El valor abonado es mayor al saldo...");
                        dr.BeginEdit();
                        dr["abono"] = 0;
                        dr.EndEdit();
                    }
                    dataGrid.UpdateLayout();
                    sumaAbonos();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("22:" + w);
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8)
                {
                    GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
                    if (Colum.MappingName == "abono")
                    {
                        System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                        dr.BeginEdit();
                        //double reduccion = (Retefte + Reteiva + Reteica + Descuento);
                        double saldo = Convert.ToDouble(dr["saldo"].ToString());
                        VlrSaldo = saldo;
                        dr["abono"] = VlrSaldo;

                        dr.EndEdit();
                        e.Handled = true;
                    }
                    dataGrid.UpdateLayout();

                    sumaAbonos();
                }
                if (e.Key == Key.F3)
                {
                    GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
                    if (Colum.MappingName == "abono")
                    {
                        System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                        dr.BeginEdit();
                        double reduccion = (Retefte + Reteiva + Reteica + Descuento);
                        double saldo = Convert.ToDouble(dr["saldo"].ToString());
                        VlrSaldo = saldo - reduccion;

                        string tipo = dr["abono"].ToString();
                        MessageBox.Show("tipo:" + tipo);


                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("11: F8" + w);
            }

        }

        private void Cta_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8)
                {
                    int idr = 0; string code = ""; string nombre = "";
                    dynamic xx = SiaWin.WindowBuscar("Comae_cta", "cod_cta", "nom_cta", "nom_cta", "idrow", "Maestra de cuentas", cnEmp, false, " tip_cta='A' ", idEmp: idemp);
                    xx.ShowInTaskbar = false;
                    xx.Owner = Application.Current.MainWindow;
                    xx.Height = 400;
                    xx.ShowDialog();
                    idr = xx.IdRowReturn;
                    code = xx.Codigo;
                    nombre = xx.Nombre;
                    xx = null;
                    if (idr > 0)
                        (sender as TextBox).Text = code;

                    if (string.IsNullOrEmpty(code)) e.Handled = false;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir cuentas:" + w);
            }
        }

        public bool whileCuent()
        {
            bool flag = true;

            double val_rfte = Convert.ToDouble(TextRetefte.Value);
            if (val_rfte > 0) if (string.IsNullOrEmpty(Cta_ref.Text)) flag = false;

            double val_riva = Convert.ToDouble(TextReteIva.Value);
            if (val_riva > 0) if (string.IsNullOrEmpty(Cta_Riva.Text) || string.IsNullOrEmpty(Cta_rivaDT.Text)) flag = false;

            double val_rica = Convert.ToDouble(TextIca.Value);
            if (val_rica > 0) if (string.IsNullOrEmpty(CtaRica.Text)) flag = false;

            return flag;
        }

        private void BtbGrabar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BtbGrabar.Content.ToString() == "Nuevo")
                {
                    ActivaDesactivaControles(1);
                }
                else
                {

                    string _CodeCliente = TextCodeCliente.Text;
                    if (string.IsNullOrEmpty(_CodeCliente))
                    {
                        MessageBox.Show("Falta Nit/cc del cliente..");
                        TextCodeCliente.Focus();
                        return;
                    }
                    if (CbBanco.SelectedIndex < 0)
                    {
                        MessageBox.Show("Seleccione Vendedor.....");
                        CbBanco.Focus();
                        return;
                    }
                    if (CbTrans.SelectedIndex < 0)
                    {
                        MessageBox.Show("Seleccione SI o NO en transferencia.....");
                        CbBanco.Focus();
                        return;
                    }
                    if (dtCue.Rows.Count == 0)
                    {
                        MessageBox.Show("No hay registros en el cuerpo de documentos...");
                        TextCodeCliente.Focus();
                        return;
                    }

                    var valor = TotalRecaudo.Text;
                    decimal TotalPag = decimal.Parse(valor, NumberStyles.Currency);
                    if (TotalPag <= 0)
                    {
                        MessageBox.Show("el total a pagar tiene que ser positivo");
                        return;
                    }



                    if (whileCuent() == false)
                    {
                        MessageBox.Show("llene todos los campo de las cuentas respectivamente");
                        return;
                    }


                    if (MessageBox.Show("Usted desea guardar el documento..?", "Guardar Recibo de Caja", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            int iddocumento = 0;

                            iddocumento = ExecuteSqlTransaction(_CodeCliente.ToString());

                            if (iddocumento <= 0) return;
                            if (iddocumento > 0)
                            {
                                MessageBox.Show("Egreso Generado");

                                imprimirPrograEgreso(CbBanco.SelectedValue.ToString().Trim(), iddocumento.ToString().Trim());
                            }

                            ActivaDesactivaControles(0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        dataGrid.Focus();
                    }
                }
            }
            catch (Exception exx)
            {
                MessageBox.Show(exx.Message);
            }
        }


        public void imprimirPrograEgreso(string code_banc, string idreg)
        {
            try
            {
                string formato = "";
                string direccion_formato = "";

                string query = "select * from comae_ban where cod_ban='" + code_banc + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "bancos", idemp);
                if (dt.Rows.Count > 0) formato = dt.Rows[0]["formato"].ToString().Trim();


                if (!string.IsNullOrWhiteSpace(formato))
                {
                    switch (formato)
                    {
                        case "A":
                            direccion_formato = @"/Contabilidad/Egresos/Egresos_BancBogota"; break;
                        case "B":
                            direccion_formato = @"/Contabilidad/Egresos/Egresos_BancAgrario"; break;
                    }
                }
                else
                {
                    direccion_formato = @"/Contabilidad/Egresos/Egresos_BancBogota";
                }


                //MessageBox.Show(direccion_formato);

                List<ReportParameter> parameters = new List<ReportParameter>();

                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(cod_empresa);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);

                ReportParameter paramcodtrn = new ReportParameter();
                paramcodtrn.Values.Add(idreg);
                paramcodtrn.Name = "idreg";
                parameters.Add(paramcodtrn);

                ReportParameter paramTipo = new ReportParameter();
                paramTipo.Values.Add("1");
                paramTipo.Name = "condition";
                parameters.Add(paramTipo);


                string sqlcheque = @"select num_chq from Cocue_doc where idregcab='" + idreg + "'";
                DataTable dtCheuque = SiaWin.DB.SqlDT(sqlcheque, "tmp", idemp);
                string cheque = "";
                foreach (System.Data.DataRow dr in dtCheuque.Rows)
                {
                    if (!string.IsNullOrEmpty(dr["num_chq"].ToString().Trim())) cheque = dr["num_chq"].ToString().Trim();
                }

                ReportParameter paramCheque = new ReportParameter();
                paramCheque.Values.Add(cheque);
                paramCheque.Name = "cheque";
                parameters.Add(paramCheque);


                ReportParameter paramBanco = new ReportParameter();
                paramBanco.Values.Add(code_banc);
                paramBanco.Name = "cod_banco";
                parameters.Add(paramBanco);

                //string sqltexttotal = @"select cast(sum(deb_mov) as decimal(12,0)) as total from Cocue_doc where idregcab='" + idreg + "'";

                string sqltexttotal = @"select cast(sum(cre_mov) as decimal(12, 2)) as total from Cocue_doc where cre_mov>= 0 and (SUBSTRING(rtrim(Cocue_doc.cod_cta),1,4)= '1110' or Cocue_doc.cod_cta = '11050504' or  Cocue_doc.cod_cta = '11100505' or SUBSTRING(rtrim(Cocue_doc.cod_cta),1,4)= '2105') and idregcab='" + idreg + "'";
                //MessageBox.Show(sqltexttotal);

                DataTable dtTotal = SiaWin.DB.SqlDT(sqltexttotal, "tmp", idemp);
                decimal totalfac = 0;
                if (dtTotal.Rows.Count > 0)
                {
                    totalfac = (decimal)dtTotal.Rows[0]["total"];

                }
                //MessageBox.Show(totalfac.ToString());
                string enletras = SiaWin.Func.enletras(totalfac.ToString());  //valor en letra
                ReportParameter paramLetra = new ReportParameter();
                paramLetra.Values.Add(enletras);
                paramLetra.Name = "enletra";
                parameters.Add(paramLetra);

                ReportParameter paramValor = new ReportParameter();
                paramValor.Values.Add(totalfac.ToString());
                paramValor.Name = "valorEgreso";
                parameters.Add(paramValor);


                string TituloReport = "titulo desde c#";

                SiaWin.Reportes(parameters, direccion_formato, TituloReporte: TituloReport, Modal: true, idemp: idemp, ZoomPercent: 50);
            }
            catch (Exception w)
            {
                MessageBox.Show("error en #imprimirPrograEgreso#:" + w);
            }
        }



        private int ExecuteSqlTransaction(string codter)
        {

            string TipoConsecutivo = "num_act";
            string codtrn = "02";
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                connection.Open();
                StringBuilder errorMessages = new StringBuilder();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                transaction = connection.BeginTransaction("Transaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    double bas_mov = Convert.ToDouble(TextVlrRecibido.Value);

                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                    sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12) = '' ;";
                    sqlConsecutivo += "declare @iFolioHost int = 0;";
                    sqlConsecutivo += "UPDATE Comae_trn SET " + TipoConsecutivo + " = ISNULL(" + TipoConsecutivo + ", 0) + 1  WHERE cod_trn='" + codtrn + "';";
                    sqlConsecutivo += "SELECT @iFolioHost = " + TipoConsecutivo + ",@ini=rtrim(inicial) FROM Comae_trn  WHERE cod_trn='" + codtrn + "';";
                    sqlConsecutivo += "set @num=@iFolioHost;";
                    sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+REPLICATE ('0',12-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));";

                    string sqlcab = sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,fec_trn,num_trn,detalle,otro_ter,fec_posf,cod_ban,UserId) values ('" + codtrn + "',@fecdoc,@iConsecutivo,'" + TextNota.Text.Trim() + "','" + TXotroTer.Text + "','" + DtFec.Text + "','" + CbBanco.SelectedValue + "'," + SiaWin._UserId + ");DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                    string sql = "";

                    foreach (System.Data.DataRow item in dtCue.Rows)
                    {

                        double abono = Convert.ToDouble(item["abono"].ToString());

                        if (abono > 0)
                        {
                            double saldo = Convert.ToDouble(item["saldo"].ToString());

                            int tipapli = Convert.ToInt32(item["tip_apli"].ToString());
                            //tipapli = 1-- cxp,//tipapli = 2 -- cxpant,//tipapli = 3 -- cxc,//tipapli = 4 -- cxcant

                            if (tipapli == 2 || tipapli == 3)
                            {
                                sql += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,bas_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + item["cod_cta"].ToString() + "','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Pago/Abono credito Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + abono.ToString("F", CultureInfo.InvariantCulture) + ");";
                            }
                            if (tipapli == 1 || tipapli == 4)
                            {
                                sql += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,bas_mov,deb_mov,doc_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + item["cod_cta"].ToString() + "','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Pago/Abono debito Doc:" + item["factura"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + ", " + abono.ToString("F", CultureInfo.InvariantCulture) + ",'" + item["factura"].ToString() + "' );";
                            }
                        }
                    }

                    if (Retefte > 0)
                    {
                        string cntRetefte = string.IsNullOrEmpty(Cta_ref.Text) ? "236540" : Cta_ref.Text;
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,bas_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cntRetefte + "','','" + codter.Trim() + "','ReteFte:" + codter + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + Retefte.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Reteica > 0)
                    {
                        string cntReteica = string.IsNullOrEmpty(CtaRica.Text) ? "237807" : CtaRica.Text;
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,bas_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cntReteica + "','','" + codter.Trim() + "','ReteIca" + codter + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + Reteica.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Reteiva > 0)
                    {
                        string cntReteivaDeb = string.IsNullOrEmpty(Cta_Riva.Text) ? "237715" : Cta_Riva.Text;
                        string cntReteivaCre = string.IsNullOrEmpty(Cta_rivaDT.Text) ? "237715" : Cta_rivaDT.Text;

                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,bas_mov,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cntReteivaDeb + "','','" + codter.Trim() + "','ReteIva DEB:" + codter + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + Reteiva.ToString("F", CultureInfo.InvariantCulture) + ");";
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,bas_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cntReteivaCre + "','','" + codter.Trim() + "','ReteIva CRE:" + codter + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + Reteiva.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }


                    if (Descuento > 0)
                    {
                        string cntDescuento = "421040";
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,bas_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cntDescuento + "','','" + codter.Trim() + "','Descto:" + codter + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + "," + Descuento.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }


                    string sqlban = "";

                    string slect = "select * from comae_ban where cod_ban='" + CbBanco.SelectedValue + "' ";

                    DataTable dt = SiaWin.Func.SqlDT(slect, "bancos", idemp);


                    if (dt.Rows.Count > 0)
                    {


                        string cta = dt.Rows[0]["cod_cta"].ToString();

                        double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=3").ToString(), out abonoCxC);
                        double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=4").ToString(), out abonoCxCAnt);
                        double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=1").ToString(), out abonoCxP);
                        double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=2").ToString(), out abonoCxPAnt);


                        double ret = Retefte + Reteica + Descuento;
                        double tot_pagar = (abonoCxP + abonoCxCAnt - abonoCxC - abonoCxPAnt) - (ret);

                        string chequeTra = string.IsNullOrEmpty(TxtCheque.Text) ? "TRANS" : TxtCheque.Text;

                        sqlban = sqlban + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,cre_mov,num_chq,bas_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cta + "','','" + codter.Trim() + "','Pago/Abono:" + codter + "'," + tot_pagar.ToString("F", CultureInfo.InvariantCulture) + ",'" + chequeTra + "'," + bas_mov.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }

                    string valor = (CbTrans.SelectedItem as ComboBoxItem).Content.ToString();
                    if (valor == "No")
                    {
                        sqlban = sqlban + @"UPDATE comae_ban SET  num_act=ISNULL(num_act, 0)+1  WHERE cod_ban='" + CbBanco.SelectedValue + "';";
                    }

                    command.CommandText = sqlcab + sql + sqlban + @"select CAST(@NewId AS int);";

                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    return Convert.ToInt32(r.ToString());
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                    return -1;
                }
                catch (Exception ex)
                {
                    errorMessages.Append("Error:" + ex.StackTrace + "-" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                    return -1;
                }
            }
        }

        private void BtbCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BtbCancelar.Content.ToString() == "Cancelar")
                {
                    if (dtCue.Rows.Count > 0)
                    {
                        if (MessageBox.Show("Usted desea aaa ........?", "Cancelar Recibo de Caja", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                        {
                            e.Handled = true;
                            return;
                        }
                    }
                    ActivaDesactivaControles(0);
                    BtbGrabar.Focus();
                    e.Handled = true;
                    return;
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ActualizaTotal(object sender, RoutedEventArgs e)
        {
            sumaAbonos();
        }

        private void CbTrans_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string valor = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();
            ComboBox cb = (sender as ComboBox);
            ComboBox cb_bancos = new ComboBox();

            cb_bancos = cb.Name == "CbTrans" ? (ComboBox)this.FindName("CbBanco") : (ComboBox)this.FindName("ComBo_Banco");
            if (valor == "No")
            {
                if (cb_bancos.SelectedIndex >= 0)
                {
                    string consecutivo = numero_cheque(cb_bancos.SelectedValue.ToString().Trim());
                    if (cb.Name == "CbTrans") TxtCheque.Text = consecutivo;
                    else Tx_Cheque.Text = consecutivo;
                }
            }
            else
                if (cb.Name == "CbTrans") TxtCheque.Text = ""; else Tx_Cheque.Text = "";

        }

        public string numero_cheque(string banco)
        {
            string con = "";
            string select = "select ISNULL(num_act,0)+1 as consecutivo from comae_ban where cod_ban = '" + banco + "'";
            DataTable dt = SiaWin.Func.SqlDT(select, "consecutivo", idemp);
            if (dt.Rows.Count > 0) con = dt.Rows[0]["consecutivo"].ToString();
            return con;
        }

        private void BtnGetDocument_Click(object sender, RoutedEventArgs e)
        {
            DataRowView GridCab = (DataRowView)dataGrid.SelectedItems[0];
            string num_trn = GridCab["num_trn"].ToString();

            ViewDocument view = new ViewDocument();
            view.document = num_trn;
            view.ShowInTaskbar = false;
            view.Owner = Application.Current.MainWindow;
            view.ShowDialog();
        }

        private void cuenta_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string cnt = (sender as TextBox).Text.Trim();
                if (string.IsNullOrEmpty(cnt) || cnt == "") return;

                bool ban = GetCuentas(cnt);

                if (ban == false)
                {
                    MessageBox.Show("la cuenta ingresada no existe");
                    (sender as TextBox).Text = "";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el lost focus:" + w);
            }
        }

        public bool GetCuentas(string cnt)
        {
            bool flag = false;
            DataTable dt = SiaWin.Func.SqlDT("select * from Comae_cta where cod_cta='" + cnt + "'", "cuenta", idemp);
            if (dt.Rows.Count > 0) flag = true;
            return flag;
        }
        // ----------------- tab 2 ----------------------------------------------------------------------

        //public double sum_deb { get; set; }
        //public double sum_cre { get; set; }
        //public double diferencia { get; set; }



        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (Btn_Save.Content.ToString() == "Nuevo")
            {

                act(2);
                if (dt_egreso.Rows.Count == 0)
                {
                    dt_egreso.Rows.Add("", "", "", "Ninguno", "", 0, 0, 0);
                }
                tx_Clie.Focus();
            }
            else
            {

                if (val_cam() == false)
                {
                    MessageBox.Show("llene todos los campos ingresados");
                    return;
                }

                if (valDebCre() == false)
                {
                    MessageBox.Show("la sumatoria total de los debitos debe ser mayor a las sumatoria total de los creditos");
                    return;
                }

                if (MessageBox.Show("usted desea generar el egreso sin causacion", "generar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int id_doc = DocumentEgeCaus();
                    if (id_doc > 0)
                    {
                        MessageBox.Show("egreso sin causacion generado exitosamente", "transaccion exitosa", MessageBoxButton.OK, MessageBoxImage.None);
                        imprimirPrograEgreso(ComBo_Banco.SelectedValue.ToString(), id_doc.ToString());

                        act(1);
                        act(1);
                    }

                }

            }
        }





        private int DocumentEgeCaus()
        {

            string TipoConsecutivo = "num_act";
            string codtrn = "02";
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                connection.Open();
                StringBuilder errorMessages = new StringBuilder();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                transaction = connection.BeginTransaction("Transaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {



                    string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                    sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12) = '' ;";
                    sqlConsecutivo += "declare @iFolioHost int = 0;";
                    sqlConsecutivo += "UPDATE Comae_trn SET " + TipoConsecutivo + " = ISNULL(" + TipoConsecutivo + ", 0) + 1  WHERE cod_trn='" + codtrn + "';";
                    sqlConsecutivo += "SELECT @iFolioHost = " + TipoConsecutivo + ",@ini=rtrim(inicial) FROM Comae_trn  WHERE cod_trn='" + codtrn + "';";
                    sqlConsecutivo += "set @num=@iFolioHost;";
                    sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+REPLICATE ('0',12-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));";

                    string sqlcab = sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,fec_trn,num_trn,detalle,otro_ter,fec_posf,cod_ban,UserId) values ('" + codtrn + "',@fecdoc,@iConsecutivo,'" + Tx_Nota.Text.Trim() + "','" + TX_ot_Ter.Text + "','" + tx_Fec_pos.Text + "','" + ComBo_Banco.SelectedValue + "'," + SiaWin._UserId + ");DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                    string sql = "";
                    string sqlban = "";

                    double debito = 0;
                    double credito = 0;

                    foreach (System.Data.DataRow item in dt_egreso.Rows)
                    {
                        debito += item.IsNull("deb_mov") ? 0 : Convert.ToDouble(item["deb_mov"]);
                        credito += item.IsNull("cre_mov") ? 0 : Convert.ToDouble(item["cre_mov"]);

                        string cod_cta = item.IsNull("cod_cta") ? "" : item["cod_cta"].ToString();
                        string cod_cco = item.IsNull("cod_cco") ? "" : item["cod_cco"].ToString();
                        string cod_ter = item.IsNull("cod_ter") ? "" : item["cod_ter"].ToString();
                        string des_mov = item.IsNull("des_mov") ? "" : item["des_mov"].ToString();
                        string doc_cruc = item.IsNull("doc_cruc") ? "" : item["doc_cruc"].ToString();
                        decimal bas_mov = item.IsNull("bas_mov") ? 0 : Convert.ToDecimal(item["bas_mov"]);
                        decimal deb_mov = item.IsNull("deb_mov") ? 0 : Convert.ToDecimal(item["deb_mov"]);
                        decimal cre_mov = item.IsNull("cre_mov") ? 0 : Convert.ToDecimal(item["cre_mov"]);

                        if (!string.IsNullOrEmpty(cod_cta))
                        {
                            sql += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,bas_mov,deb_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cod_cta + "','" + cod_cco + "','" + cod_ter + "','" + des_mov + "','" + doc_cruc + "'," + bas_mov + "," + deb_mov + "," + cre_mov + ");";
                        }

                    }

                    double contraBanc = debito - credito;

                    if (contraBanc > 0)
                    {
                        DataTable dt = dtBanco.Select("cod_ban='" + ComBo_Banco.SelectedValue + "'").CopyToDataTable();
                        //SiaWin.Browse(dt);                     
                        string cnt = dt.Rows[0]["cod_cta"].ToString().Trim();
                        string chequeTra = string.IsNullOrEmpty(Tx_Cheque.Text) ? "TRANS" : Tx_Cheque.Text;

                        sql += @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,cre_mov,num_chq) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cnt + "','','" + tx_Clie.Text + "','BANCO',''," + contraBanc + ",'" + chequeTra + "');";
                    }

                    string valor = (Cb_Trans.SelectedItem as ComboBoxItem).Content.ToString();
                    if (valor == "No")
                        sqlban = sqlban + @"UPDATE comae_ban SET  num_act=ISNULL(num_act, 0)+1  WHERE cod_ban='" + ComBo_Banco.SelectedValue + "';";


                    command.CommandText = sqlcab + sql + sqlban + @"select CAST(@NewId AS int);";
                    //MessageBox.Show(command.CommandText);

                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    return Convert.ToInt32(r.ToString());
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                    return -1;
                }
                catch (Exception ex)
                {
                    errorMessages.Append("Error:" + ex.StackTrace + "-" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                    return -1;
                }
            }
        }


        public bool val_cam()
        {
            bool flag = true;
            if (string.IsNullOrEmpty(tx_Clie.Text)) flag = false;
            if (string.IsNullOrEmpty(Tx_NomCli.Text)) flag = false;
            if (ComBo_Banco.SelectedIndex < 0) flag = false;
            //if (string.IsNullOrEmpty(Tx_Nota.Text)) flag = false;
            //if (string.IsNullOrEmpty(TX_ot_Ter.Text)) flag = false;
            if (Cb_Trans.SelectedIndex < 0) flag = false;
            if (string.IsNullOrEmpty(tx_Fec_pos.Text)) flag = false;
            //if (string.IsNullOrEmpty(Tx_Cheque.Text)) flag = false;
            return flag;
        }



        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (Btn_Cancel.Content.ToString() == "Salir")
            {
                this.Close();
            }
            else
            {
                act(1);
            }
        }

        public void act(int val)
        {
            if (val == 1)
            {
                dt_egreso.Clear();
                Btn_Save.Content = "Nuevo";
                Btn_Cancel.Content = "Salir";
                Txt_usaurio.Text = "---";

                Text_Ndoc.Text = "---";

                tx_Clie.Focusable = false;
                tx_Clie.Text = "";
                Tx_NomCli.Focusable = false;
                Tx_NomCli.Text = "";
                ComBo_Banco.IsEnabled = false;
                Tx_Nota.Focusable = false;
                Tx_Nota.Text = "";
                TX_ot_Ter.Focusable = false;
                TX_ot_Ter.Text = "";
                Cb_Trans.IsEnabled = false;
                tx_Fec_pos.Focusable = false;
                tx_Fec_pos.Text = "";
                Tx_Cheque.IsEnabled = false;
                Tx_Cheque.Text = "";
            }
            if (val == 2)
            {

                Btn_Save.Content = "Guardar";
                Btn_Cancel.Content = "Cancelar";
                Txt_usaurio.Text = SiaWin._UserAlias;
                Text_Ndoc.Text = consecutivo();

                tx_Clie.Focusable = true;
                Tx_NomCli.Focusable = false;
                ComBo_Banco.IsEnabled = true;
                Tx_Nota.Focusable = true;
                TX_ot_Ter.Focusable = true;
                Cb_Trans.IsEnabled = true;
                tx_Fec_pos.Focusable = true;
                tx_Fec_pos.Text = DateTime.Now.ToString();
                Tx_Cheque.Focusable = true;
            }

        }

        private void GridConfig_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {


                GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;

                var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                var rowData = GridConfig.GetRecordAtRowIndex(columnIndex);

                string tabla = ""; string codigo = ""; string nombre = ""; string title = ""; string where = "";


                if ((sender as SfDataGrid).SelectedIndex == -1) return;

                string t = getTabla(Colum);
                if (string.IsNullOrEmpty(t)) return;


                if (e.Key == Key.F8)
                {
                    if (Colum.MappingName == "cod_cta")
                    {
                        tabla = "comae_cta"; codigo = "cod_cta"; nombre = "nom_cta"; title = "Maestra de cuentas";
                        where = " tip_cta='A' ";
                    }
                    if (Colum.MappingName == "cod_ter")
                    {
                        tabla = "comae_ter"; codigo = "cod_ter"; nombre = "nom_ter"; title = "Maestra de tercero";
                    }
                    if (Colum.MappingName == "cod_cco")
                    {
                        tabla = "comae_cco"; codigo = "cod_cco"; nombre = "nom_cco"; title = "Maestra de Centro de costos";
                    }

                    if (GridConfig.SelectedIndex == -1)
                        this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();

                    if (Colum.MappingName == "cod_ter" || Colum.MappingName == "cod_cco" || Colum.MappingName == "cod_cta")
                    {
                        int idr = 0; string codi = ""; string nom = "";
                        dynamic xx = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", title, SiaWin.Func.DatosEmp(idemp), false, where, idEmp: idemp);
                        xx.ShowInTaskbar = false;
                        xx.Owner = Application.Current.MainWindow;
                        xx.Height = 500;
                        xx.ShowDialog();
                        idr = xx.IdRowReturn;
                        codi = xx.Codigo;
                        nom = xx.Nombre;

                        reflector.SetValue(rowData, Colum.MappingName, codi);

                        GridConfig.UpdateDataRow(columnIndex);
                        GridConfig.UpdateLayout();
                        GridConfig.Columns[Colum.MappingName].AllowEditing = true;
                    }

                    if (Colum.MappingName == "doc_cruc")
                    {
                        dynamic ww = SiaWin.WindowExt(9381, "TrnDocumentoCruce");  //carga desde sql
                        ww.codcliente = tx_Clie.Text;
                        ww.nomter = Tx_NomCli.Text;
                        //ww.codcta = cod_cta;
                        ww.fechacorte = DateTime.Now;
                        ww.idemp = idemp;
                        //ww.FilasRegistros = _trn.dsDoc.Tables["Cue"].Select("cod_cta='" + cod_cta.Trim() + "' and cod_ter='" + cod_cli + "' and doc_cruc<>''");
                        ww.ShowInTaskbar = false;
                        ww.Owner = Application.Current.MainWindow;
                        ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        ww.ShowDialog();

                    }



                }
            }
            catch (Exception w)
            {
                MessageBox.Show("****" + w);
            }
        }

        public string getTabla(GridColumn col)
        {
            string map = col.MappingName.ToString();
            string tabla = "";
            switch (map)
            {
                case "cod_cta": tabla = "comae_cta"; break;
                case "cod_ter": tabla = "comae_ter"; break;
                case "cod_cco": tabla = "comae_cco"; break;
                    //case "doc_cruc": tabla = "tabla"; break;
            }
            return tabla;
        }

        private void GridConfig_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            try
            {
                //MessageBox.Show("a1");
                GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;
                //MessageBox.Show("a2");

                var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                //MessageBox.Show("a3");
                var rowData = GridConfig.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex);
                //MessageBox.Show("a4:"+ e.RowColumnIndex.RowIndex);



                string valor = DBNull.Value.Equals(reflector.GetValue(rowData, Colum.MappingName)) ? "" : reflector.GetValue(rowData, Colum.MappingName).ToString();

                //string valor = reflector.GetValue(rowData, Colum.MappingName).ToString();


                //MessageBox.Show("a5");

                string tabla = getTabla(Colum);

                if (string.IsNullOrEmpty(getTabla(Colum)) || string.IsNullOrEmpty(valor)) return;

                if (validar(tabla, valor) == true)
                {
                    reflector.SetValue(rowData, Colum.MappingName, valor).ToString();
                    GridConfig.UpdateDataRow(e.RowColumnIndex.RowIndex);
                    GridConfig.UpdateLayout();
                    GridConfig.Columns[Colum.MappingName].AllowEditing = true;

                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_cta"))) reflector.SetValue(rowData, "cod_cta", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_ter"))) reflector.SetValue(rowData, "cod_ter", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_ter"))) reflector.SetValue(rowData, "cod_ter", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_cco"))) reflector.SetValue(rowData, "cod_cco", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "des_mov"))) reflector.SetValue(rowData, "des_mov", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "doc_cruc"))) reflector.SetValue(rowData, "doc_cruc", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "bas_mov"))) reflector.SetValue(rowData, "bas_mov", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "deb_mov"))) reflector.SetValue(rowData, "deb_mov", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cre_mov"))) reflector.SetValue(rowData, "cre_mov", 0);
                }
                else
                {
                    MessageBox.Show("el codigo ingresado no existe");
                    reflector.SetValue(rowData, Colum.MappingName, "").ToString();

                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_cta"))) reflector.SetValue(rowData, "cod_cta", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_ter"))) reflector.SetValue(rowData, "cod_ter", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_cco"))) reflector.SetValue(rowData, "cod_cco", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "des_mov"))) reflector.SetValue(rowData, "des_mov", "");
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "doc_cruc"))) reflector.SetValue(rowData, "doc_cruc", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "bas_mov"))) reflector.SetValue(rowData, "bas_mov", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "deb_mov"))) reflector.SetValue(rowData, "deb_mov", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cre_mov"))) reflector.SetValue(rowData, "cre_mov", 0);

                    GridConfig.UpdateDataRow(e.RowColumnIndex.RowIndex);
                    GridConfig.UpdateLayout();
                    GridConfig.Columns[Colum.MappingName].AllowEditing = true;
                }
                updTot();
            }
            catch (Exception w)
            {
                // MessageBox.Show("error al editar:" + w);
            }

        }


        public void updTot()
        {
            if (dt_egreso.Rows.Count > 0)
            {
                double deb = Convert.ToDouble(dt_egreso.Compute("Sum(deb_mov)", ""));
                double cred = Convert.ToDouble(dt_egreso.Compute("Sum(cre_mov)", ""));
                double dif = deb - cred;
                Tot_Deb.Text = deb.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
                Tot_Cre.Text = cred.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
                Tot_Dif.Text = dif.ToString("N", CultureInfo.CreateSpecificCulture("es-ES"));
            }
        }

        public bool valDebCre()
        {
            bool flag = false;
            if (dt_egreso.Rows.Count > 0)
            {
                double deb = Convert.ToDouble(dt_egreso.Compute("Sum(deb_mov)", ""));
                double cred = Convert.ToDouble(dt_egreso.Compute("Sum(cre_mov)", ""));
                if (deb > cred) flag = true;
            }
            return flag;
        }


        public bool validar(string table, string value)
        {
            bool flag = false;
            string campo = "";
            string where = "";
            switch (table)
            {
                case "comae_cta": campo = "cod_cta"; where = "and tip_cta='A' "; break;
                case "comae_ter": campo = "cod_ter"; where = ""; break;
                case "comae_cco": campo = "cod_cco"; where = ""; break;
            }


            string select = "select * from " + table + " where " + campo + "='" + value + "' " + where + "; ";
            DataTable dt = SiaWin.Func.SqlDT(select, "table", idemp);
            if (dt.Rows.Count > 0) flag = true;


            return flag;
        }

        private void GridConfig_CurrentCellActivating(object sender, CurrentCellActivatingEventArgs e)
        {

            if (e.CurrentRowColumnIndex.ColumnIndex == 1 || e.CurrentRowColumnIndex.ColumnIndex == 8)
                GridConfig.AddNewRowPosition = AddNewRowPosition.Bottom;
            else
                GridConfig.AddNewRowPosition = AddNewRowPosition.None;
            GridConfig.UpdateLayout();
            updTot();
        }

        private void ComBo_Banco_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Cb_Trans.SelectedIndex = 0;
        }

        private void Tx_Fec_pos_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            MoveToNextUIElement(e);
            MoveToNextUIElement(e);
            MoveToNextUIElement(e);
        }

        private void GridConfig_CurrentCellActivated(object sender, CurrentCellActivatedEventArgs e)
        {
            try
            {
                bool t = this.GridConfig.View.IsAddingNew;
                if (!t)
                {
                    if ((e.CurrentRowColumnIndex.RowIndex) > GridConfig.View.Records.Count)
                    {
                        if (e.CurrentRowColumnIndex.ColumnIndex > 0)
                            this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();
                    }
                    else
                    {
                        GridConfig.UpdateLayout();
                        var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                        int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                        var rowData = GridConfig.GetRecordAtRowIndex(columnIndex);
                        string cod_cta = reflector.GetValue(rowData, "cod_cta").ToString().Trim();
                        if (string.IsNullOrEmpty(cod_cta))
                        {
                            this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();
                            //return;
                        }

                        string cod_ter = reflector.GetValue(rowData, "cod_ter").ToString().Trim();
                        if (string.IsNullOrEmpty(cod_ter))
                        {
                            this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();
                            //return;
                        }
                    }
                }

                if (Keyboard.IsKeyDown(Key.Tab) || Keyboard.IsKeyDown(Key.Right) || Keyboard.IsKeyDown(Key.Return))
                {
                    //MessageBox.Show("A1");
                    var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                    int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                    var rowData = GridConfig.GetRecordAtRowIndex(columnIndex);
                    //MessageBox.Show("A2"+ e.OriginalSender);
                    //MessageBox.Show("A3" + e.PreviousRowColumnIndex.ColumnIndex);
                    //MessageBox.Show("A4" + e.ActivationTrigger);


                    GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;
                    string tabla = ""; string codigo = ""; string nombre = ""; string title = ""; string where = "";
                    //MessageBox.Show("A3");

                    if (e.PreviousRowColumnIndex.ColumnIndex == 1)
                    {
                        if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_cta")))
                        {
                            MessageBox.Show("nullo");
                            tabla = "comae_cta"; codigo = "cod_cta"; nombre = "nom_cta"; title = "Maestra de cuentas";
                            where = " tip_cta='A' ";
                            int idr = 0; string codi = ""; string nom = "";
                            dynamic xx = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", title, SiaWin.Func.DatosEmp(idemp), false, where, idEmp: idemp);
                            xx.ShowInTaskbar = false;
                            xx.Owner = Application.Current.MainWindow;
                            xx.Height = 500;
                            xx.ShowDialog();
                            idr = xx.IdRowReturn;
                            codi = xx.Codigo;
                            nom = xx.Nombre;
                            //GridConfig.MoveCurrentCell(e.PreviousRowColumnIndex, false);
                            reflector.SetValue(rowData, "cod_cta", codi);
                            GridConfig.UpdateDataRow(columnIndex);
                            GridConfig.UpdateLayout();
                            GridConfig.Columns["cod_cta"].AllowEditing = true;
                            return;
                        }

                        string cod_cta = reflector.GetValue(rowData, "cod_cta").ToString().Trim();
                        if (string.IsNullOrEmpty(cod_cta))
                        {

                            //GridConfig.MoveCurrentCell(e.PreviousRowColumnIndex, false);

                            tabla = "comae_cta"; codigo = "cod_cta"; nombre = "nom_cta"; title = "Maestra de cuentas"; where = " tip_cta='A'";
                            int idr = 0; string codi = ""; string nom = "";
                            dynamic xx = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", title, SiaWin.Func.DatosEmp(idemp), false, where, idEmp: idemp);
                            xx.ShowInTaskbar = false;
                            xx.Owner = Application.Current.MainWindow;
                            xx.Height = 500;
                            xx.ShowDialog();
                            idr = xx.IdRowReturn;
                            codi = xx.Codigo;
                            nom = xx.Nombre;
                            //GridConfig.MoveCurrentCell(e.PreviousRowColumnIndex, false);
                            reflector.SetValue(rowData, "cod_cta", codi);
                            GridConfig.UpdateDataRow(columnIndex);
                            GridConfig.UpdateLayout();
                            GridConfig.Columns["cod_cta"].AllowEditing = true;
                            return;
                        }
                    }

                    if (e.PreviousRowColumnIndex.ColumnIndex == 2)
                    {
                        if (DBNull.Value.Equals(reflector.GetValue(rowData, "cod_ter")))
                        {
                            //MessageBox.Show("tercero nullo");
                            //GridConfig.MoveCurrentCell(e.PreviousRowColumnIndex, false);
                            tabla = "comae_ter"; codigo = "cod_ter"; nombre = "nom_ter"; title = "Maestra de tercero";
                            int idr = 0; string codi = ""; string nom = "";
                            dynamic xx = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", title, SiaWin.Func.DatosEmp(idemp), false, where, idEmp: idemp);
                            xx.ShowInTaskbar = false;
                            xx.Owner = Application.Current.MainWindow;
                            xx.Height = 500;
                            xx.ShowDialog();
                            idr = xx.IdRowReturn;
                            codi = xx.Codigo;
                            nom = xx.Nombre;
                            reflector.SetValue(rowData, "cod_ter", codi);
                            GridConfig.UpdateDataRow(columnIndex);
                            GridConfig.UpdateLayout();
                            GridConfig.Columns["cod_ter"].AllowEditing = true;
                            return;
                        }

                        string cod_ter = reflector.GetValue(rowData, "cod_ter").ToString().Trim();
                        if (string.IsNullOrEmpty(cod_ter))
                        {
                            //MessageBox.Show("tercero vacio");
                            //GridConfig.MoveCurrentCell(e.PreviousRowColumnIndex, false);
                            tabla = "comae_ter"; codigo = "cod_ter"; nombre = "nom_ter"; title = "Maestra de tercero";
                            int idr = 0; string codi = ""; string nom = "";
                            dynamic xx = SiaWin.WindowBuscar(tabla, codigo, nombre, codigo, "idrow", title, SiaWin.Func.DatosEmp(idemp), false, where, idEmp: idemp);
                            xx.ShowInTaskbar = false;
                            xx.Owner = Application.Current.MainWindow;
                            xx.Height = 500;
                            xx.ShowDialog();
                            idr = xx.IdRowReturn;
                            codi = xx.Codigo;
                            nom = xx.Nombre;
                            reflector.SetValue(rowData, "cod_ter", codi);
                            GridConfig.UpdateDataRow(columnIndex);
                            GridConfig.UpdateLayout();
                            GridConfig.Columns["cod_ter"].AllowEditing = true;
                            return;
                        }
                    }


                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error ??" + w);
            }
        }

        private void BtnConsulta_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string where = string.IsNullOrWhiteSpace(Tx_document.Text) ? "" : " and cab.num_trn='" + Tx_document.Text + "' ";

                //string query = "select * from CoCab_doc where cod_trn='02' and fec_trn>='" + Fec_con.Text + "' ";
                string query = "select cab.idreg,cab.cod_trn,cab.num_trn,cab.fec_trn,cab.detalle,cab.fec_posf,cab.fec_ven,cab.otro_ter,cab.cod_ban,ban.nom_ban from CoCab_doc as cab ";
                query += "left join comae_ban as ban on cab.cod_ban = ban.cod_ban ";
                query += "where cod_trn = '02' and fec_trn>= '" + Fec_con.Text + "' " + where;

                DataTable dt = SiaWin.Func.SqlDT(query, "bancos", idemp);
                if (dt.Rows.Count > 0)
                {
                    dataGridConsulta.ItemsSource = dt.DefaultView;
                    Txt_TotalReg.Text = dt.Rows.Count.ToString();
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(Tx_document.Text))
                    {
                        dataGridConsulta.ItemsSource = null;
                        MessageBox.Show("no hay niguno egreso apartir de la fecha:" + Fec_con.Text);
                    }
                    else
                    {
                        dataGridConsulta.ItemsSource = null;
                        MessageBox.Show("no existe ningun registro con este documento:" + Tx_document.Text);
                    }
                }
            }
            catch (Exception w)
            {

                throw;
            }
        }


        private void NtmImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridConsulta.SelectedItems[0];
                string idreg = row["idreg"].ToString();
                string banco = row["cod_ban"].ToString();

                imprimirPrograEgreso(banco, idreg);


            }
            catch (Exception w)
            {
                MessageBox.Show("error al imprimir:" + w);
            }
        }

        private void BtnGetDocument_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridConsulta.SelectedItems[0];
                int idreg = Convert.ToInt32(row["idreg"]);

                SiaWin.TabTrn(0, idemp, true, idreg, 1, WinModal: true);

            }
            catch (Exception w)
            {
                MessageBox.Show("opcion no disponible se esta trabajando en ello");
            }
        }


    }
}

