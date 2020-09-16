using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
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
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using RecaudoCredicontado;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.Windows.Tools.Controls;
using Syncfusion.XlsIO;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9540,"RecaudoCredicontado");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9540,"RecaudoCredicontado");  
    //ww.codpvta = "003";
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class RecaudoCredicontado : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int moduloid = 0;

        DataTable dtCue = new DataTable();
        DataTable dtVen = new DataTable();
        DataTable fPago = new DataTable();

        public string codbod = "";
        public string codpvta = "";


        string nompvta = "";

        double Descto = 0;
        double Retefte = 0;
        double Reteica = 0;
        double Mayorvlr = 0;
        double Menorvlr = 0;
        double Anticipo = 0;

        double VlrRecibido = 0;

        double VlrAbonado = 0;

        public RecaudoCredicontado()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; 
            LoadConfig();

            ActivaDesactivaControles(0);
            BtbGrabar.Focus();

            TX_fecIni.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TX_fecFin.Text = DateTime.Now.ToString("dd/MM/yyyy");
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
                this.Title = "Recaudo Credicontado " + cod_empresa + "-" + nomempresa;

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());

                TextFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtEmpresa.Text = SiaWin._BusinessName.ToString().Trim();
                TxtPVenta.Text = codpvta;

                if (codpvta == string.Empty)
                {
                    MessageBox.Show("El usuario no tiene asignado un punto de venta, Pantalla Bloqueada");
                    this.IsEnabled = false;
                }
                else
                {
                    nompvta = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "nom_pvt", codpvta, idemp);
                    TxtPVenta.Text = codpvta + "-" + nompvta;
                    codbod = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_bod", codpvta, idemp);
                    if (string.IsNullOrEmpty(codbod))
                    {
                        MessageBox.Show("El punto de venta Asignado no tiene bodega , Pantalla Bloqueada");
                    }
                    TxtBod.Text = codbod;
                }

                dtVen = SiaWin.Func.SqlDT("select cod_mer as cod_ven,cod_mer+'-'+nom_mer as nom_ven,estado as estado from inmae_mer where estado=1  order by cod_mer", "inmae_mer", idemp);
                dtVen.PrimaryKey = new System.Data.DataColumn[] { dtVen.Columns["cod_mer"] };

                // establecer paths
                CmbVen.ItemsSource = dtVen.DefaultView;
                CmbVen.DisplayMemberPath = "nom_ven";
                CmbVen.SelectedValuePath = "cod_ven";

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show(w.Message);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (BtbGrabar.Content.ToString().Trim() == "Nuevo") return;
            if (e.Key == Key.F5 && BtbGrabar.Content.ToString().Trim() == "Grabar")
            {
                if (e.Key == System.Windows.Input.Key.F5)
                {
                    BtbGrabar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
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
                if (BtbGrabar.Content.ToString().Trim() == "Grabar")
                {
                    BtbCancelar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    e.Handled = false;
                    return;
                }
            }
        }

        void MoveToNextUIElement(KeyEventArgs e)
        {
            FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;
            TraversalRequest request = new TraversalRequest(focusDirection);
            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
            if (elementWithFocus != null)
                if (elementWithFocus.MoveFocus(request)) e.Handled = true;
        }

        private void TextCodeCliente_PreviewKeyDown(object sender, KeyEventArgs e)
        {


            if (e.Key == Key.Enter && !string.IsNullOrEmpty(TextNomCliente.Text))
            {
                MoveToNextUIElement(e);
            }

            if (e.Key == Key.F8 || e.Key == Key.Enter)
            {
                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "nom_ter", "idrow", "Maestra de clientes", cnEmp, false, "", idEmp: idemp);
                xx.ShowInTaskbar = false;
                xx.Owner = Application.Current.MainWindow;
                xx.Width = 400;
                xx.Height = 400;
                xx.ShowDialog();
                idr = xx.IdRowReturn;
                code = xx.Codigo;
                nombre = xx.Nombre;
                xx = null;
                if (idr > 0)
                {
                    TextCodeCliente.Text = code;
                    TextNomCliente.Text = nombre;
                }
                if (string.IsNullOrEmpty(code)) e.Handled = false;
                if (!string.IsNullOrEmpty(TextCodeCliente.Text.Trim())) TextCodeCliente.Focusable = false;
                if (string.IsNullOrEmpty(code)) return;


                string valida = cuentaValidacion();
                if (!string.IsNullOrEmpty(valida))
                {
                    MessageBox.Show(valida, "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                //ConsultaSaldoCartera();
            }

            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                return;
            }
            if ((e.Key == Key.Enter || e.Key == Key.Return || e.Key == Key.Tab))
            {
                TextBox s = e.Source as TextBox;
                if (s != null)
                {
                    s.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
        }

        public string cuentaValidacion()
        {
            string valor = string.Empty;

            DataTable dt = SiaWin.Func.SqlDT("select  ind_mod from Comae_cta where cod_cta='11050506' ", "Bodega", idemp);
            if (dt.Rows.Count >= 0)
            {
                int ind = Convert.ToInt32(dt.Rows[0]["ind_mod"]);
                if (ind != 1)
                    valor = "la cuenta no esta parametrisada para mostrar en el modulo cxc";
            }
            else
                valor = "la cuenta 11050506 no existe";

            return valor;
        }


        private void TextCode_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                if (BtbCancelar.Content.ToString().Trim() == "Salir") return;

                TextBox textbox = ((TextBox)sender);

                if (!string.IsNullOrEmpty(textbox.Text))
                {

                    if (!ActualizaCampos(textbox.Text.Trim()))
                    {
                        MessageBox.Show("El codigo de tercereo:" + textbox.Text.Trim() + " no existe");
                        textbox.Text = "";
                    }
                    else
                    {

                        string valida = cuentaValidacion();
                        if (!string.IsNullOrEmpty(valida))
                        {
                            MessageBox.Show(valida, "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return;
                        }
                        ConsultaSaldoCartera();
                        if (!string.IsNullOrEmpty(TextCodeCliente.Text.Trim())) TextCodeCliente.Focusable = false;
                    }
                }

                if (TextCodeCliente.Text.Trim().Length == 0)
                {
                    textbox.Dispatcher.BeginInvoke((Action)(() => { textbox.Focus(); }));
                    //e.Handled = true;
                    return;
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("ERROR LOSFOCUSTERCERO:" + w);
            }
        }


        private bool ActualizaCampos(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id)) return false;
                SqlDataReader dr = SiaWin.Func.SqlDR("SELECT idrow,cod_ter,nom_ter,dir1,tel1,observ FROM comae_ter where cod_ter='" + Id.ToString() + "' ", idemp);
                int idrow = 0;
                //string codter = "";
                string nomter = "";
                while (dr.Read())
                {
                    idrow = Convert.ToInt32(dr["idrow"]);
                    //codter = dr["cod_ter"].ToString();
                    nomter = dr["nom_ter"].ToString();
                    //dirter = dr["dir1"].ToString();
                    //telter = dr["tel1"].ToString();
                    TextNomCliente.Text = nomter;
                }
                dr.Close();
                if (idrow == 0) return false;
                if (idrow > 0) return true;
            }
            catch (System.Exception _error)
            {
                SiaWin.Func.SiaExeptionGobal(_error);
                MessageBox.Show(_error.Message);
            }
            return false;
        }


        public void ActivaDesactivaControles(int estado)
        {
            if (estado == 0)
            {
                TextCodeCliente.Text = string.Empty;
                TextNomCliente.Text = string.Empty;
                TextNumeroDoc.Text = string.Empty;
                CmbVen.SelectedIndex = -1;
                //CmbVen.IsEnabled = false;
                CbMes.IsEnabled = false;
                BtbGrabar.Content = "Nuevo";
                BtbCancelar.Content = "Salir";
                dataGrid.AllowEditing = true;
                dtCue.Clear();
                TextRetefte.Text = "0,00";
                TextIca.Text = "0,00";
                TextVlrRecibido.Text = "0,00";
                TextMayorVlr.Text = "0,00";
                TextMenorVlr.Text = "0,00";
                TextAnticipo.Text = "0,00";
                TotalAbono.Text = "0,00";
                TotalRecaudo.Text = "0,00";
                TextRProv.Text = "";


                Descto = 0;
                Retefte = 0;
                Reteica = 0;
                Mayorvlr = 0;
                Menorvlr = 0;
                Anticipo = 0;

                TextDescto.Value = 0;
                TextRetefte.Value = 0;
                TextIca.Value = 0;
                TextMayorVlr.Value = 0;
                TextMenorVlr.Value = 0;
                TextAnticipo.Value = 0;

                VlrAbonado = 0;
                Anticipo = 0;
                Mayorvlr = 0;
                Retefte = 0;
                Reteica = 0;
                Menorvlr = 0;
                Descto = 0;

                TextCodeCliente.Focusable = false;

            }
            if (estado == 1) //creando
            {
                TextCodeCliente.Text = string.Empty;
                TextNomCliente.Text = string.Empty;
                CmbVen.SelectedIndex = -1;
                //CmbVen.IsEnabled = true;
                CbMes.IsEnabled = true;
                TextNumeroDoc.Text = "";
                BtbGrabar.Content = "Grabar";
                BtbCancelar.Content = "Cancelar";
                dataGrid.AllowEditing = false;
                dtCue.Clear();
                dataGrid.UpdateLayout();
                TextCodeCliente.Focusable = true;

                TextNumeroDoc.Text = consecutivo();
                TextCodeCliente.Focusable = true;
                TextRetefte.Text = "0,00";
                TextIca.Text = "0,00";
                TextVlrRecibido.Text = "0,00";
                TextMayorVlr.Text = "0,00";
                TextMenorVlr.Text = "0,00";
                TextAnticipo.Text = "0,00";
                TextRProv.Text = "";


                Descto = 0;
                Retefte = 0;
                Reteica = 0;
                Mayorvlr = 0;
                Menorvlr = 0;
                Anticipo = 0;

                TextDescto.Value = 0;
                TextRetefte.Value = 0;
                TextIca.Value = 0;
                TextMayorVlr.Value = 0;
                TextMenorVlr.Value = 0;
                TextAnticipo.Value = 0;

                VlrAbonado = 0;
                Anticipo = 0;
                Mayorvlr = 0;
                Retefte = 0;
                Reteica = 0;
                Menorvlr = 0;
                Descto = 0;

                TextCodeCliente.Focus();


            }
        }


        public string consecutivo()
        {
            string con = "";
            try
            {
                string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                sqlConsecutivo += "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);  ";
                sqlConsecutivo += "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0; ";
                sqlConsecutivo += "SELECT @iFolioHost= isnull(rcaj,0)+1,@ini=rtrim('" + codpvta + "') FROM Copventas WHERE  cod_pvt='" + codpvta + "'; ";
                sqlConsecutivo += "set @num=@iFolioHost; ";
                sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));  ";
                sqlConsecutivo += "select @iConsecutivo as consecutivo; ";

                DataTable dt = SiaWin.DB.SqlDT(sqlConsecutivo, "cons", idemp);

                if (dt.Rows.Count > 0)
                {
                    con = dt.Rows[0]["consecutivo"].ToString();
                }


            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error en el consecutivo:" + w);
                con = "***";
            }

            return con;
        }


        private async void ConsultaSaldoCartera()
        {
            try
            {
                dataGrid.ItemsSource = 0;

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;

                string tercero = TextCodeCliente.Text.Trim();


                string fecha = TextFecha.Text;

                var slowTask = Task<DataTable>.Factory.StartNew(() => load(tercero, fecha, cod_empresa, source.Token), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    //SiaWin.Browse(((DataTable)slowTask.Result));

                    if (((DataTable)slowTask.Result).Rows.Count == 0)
                    {
                        MessageBox.Show("Sin informacion de cartera");
                        dataGrid.ItemsSource = null;
                        TextCodeCliente.Text = "";
                        TextNomCliente.Text = "";
                    }

                    dataGrid.ItemsSource = ((DataTable)slowTask.Result).DefaultView;

                }
                sfBusyIndicator.IsBusy = false;

            }
            catch (Exception W)
            {
                SiaWin.Func.SiaExeptionGobal(W);
                MessageBox.Show("Actualiza Grid www:" + W);
            }
        }


        public DataTable load(string ter, string fecha, string empre, CancellationToken cancellationToken)
        {
            SqlConnection con = new SqlConnection(SiaWin._cn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds1 = new DataSet();
            cmd = new SqlCommand("_empSpCoAnalisisCxc", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Ter", ter);
            cmd.Parameters.AddWithValue("@Cta", "11050506");
            cmd.Parameters.AddWithValue("@TipoApli", 1);
            cmd.Parameters.AddWithValue("@Resumen", 1);
            cmd.Parameters.AddWithValue("@Fecha", fecha);
            cmd.Parameters.AddWithValue("@TrnCo", "");
            cmd.Parameters.AddWithValue("@NumCo", "");
            cmd.Parameters.AddWithValue("@Cco", "");
            cmd.Parameters.AddWithValue("@Ven", "");
            cmd.Parameters.AddWithValue("@codemp", empre);
            cmd.Parameters.AddWithValue("@TipoReporte", 0);
            dtCue.Clear();
            da = new SqlDataAdapter(cmd);
            da.Fill(dtCue);
            con.Close();
            //SiaWin.Browse(dtCue);
            return dtCue;
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

                    var tuples = validacion();

                    if (tuples.Item1 == true)
                    {
                        if (tuples.Item2 == "TextFecha")
                        {
                            MessageBox.Show("el campo de la fecha esta vacio");
                            return;
                        }
                        if (tuples.Item2 == "TextCodeCliente")
                        {
                            MessageBox.Show("seleccione un cliente ");
                            return;
                        }
                        if (tuples.Item2 == "CbMes")
                        {
                            MessageBox.Show("seleccione si es hecho por mensajero");
                            return;
                        }
                        if (tuples.Item2 == "CmbVen")
                        {
                            MessageBox.Show("seleccione el vendedor");
                            return;
                        }
                        if (tuples.Item2 == "TextRProv")
                        {
                            MessageBox.Show("llene el campo de recibo");
                            return;
                        }


                    }

                    if (validarReciboProvi(TextRProv.Text) == false)
                    {
                        MessageBox.Show("complete el campo de recibo provisional");
                        TextRProv.Dispatcher.BeginInvoke((Action)(() => { TextRProv.Focus(); }));
                        return;
                    }

                    string _CodeCliente = TextCodeCliente.Text;

                    decimal ValorRecibido = Convert.ToDecimal(TextVlrRecibido.Value);
                    decimal totalRecibido = Math.Truncate(ValorRecibido);

                    var valor = TotalRecaudo.Text;
                    decimal TotalRec = decimal.Parse(valor, NumberStyles.Currency);

                    if (totalRecibido != TotalRec)
                    {
                        MessageBox.Show("el valor recibido no es igual al total de recaudo");
                        return;
                    }

                    double _abono = VlrAbonado;
                    if (_abono < 0)
                    {
                        MessageBox.Show("Valor Abono no puede ser menor a 0");
                        dataGrid.Focus();
                        dataGrid.SelectedIndex = 0;
                        //dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[8]);
                        return;
                    }
                    double abono = Convert.ToDouble(dtCue.Compute("Sum(abono)", "").ToString());
                    if (abono <= 0)
                    {
                        MessageBox.Show("No hay Abonos...");
                        dataGrid.Focus();
                        dataGrid.SelectedIndex = 0;
                        return;
                    }


                    if (MessageBox.Show("Usted desea realizar el recaudo?", "Guardar Recaudo Credicontado", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            int iddocumento = 0;
                            Retefte = Convert.ToDouble(TextRetefte.Value);
                            Reteica = Convert.ToDouble(TextIca.Value);
                            Mayorvlr = Convert.ToDouble(TextMayorVlr.Value);
                            Menorvlr = Convert.ToDouble(TextMenorVlr.Value);
                            Anticipo = Convert.ToDouble(TextAnticipo.Value);
                            Descto = Convert.ToDouble(TextDescto.Value);


                            double _abonototal = (VlrAbonado + Anticipo + Mayorvlr - Retefte - Reteica - Menorvlr - Descto);
                            double valorPasar = Math.Round(_abonototal);
                            SiaWin.ValReturn = valorPasar;

                            FormasDePago wFpago = new FormasDePago();

                            //wFpago.recibo_prov = is_reciboProv == true ? TextRProv.Text.Trim() : "";
                            //wFpago.vendedor = is_reciboProv == true ? CmbVen.SelectedValue.ToString() : "";

                            wFpago.ShowInTaskbar = false;
                            wFpago.Owner = Application.Current.MainWindow;
                            wFpago.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            wFpago.ShowDialog();
                            wFpago = null;
                            if (SiaWin.ValReturn == null) return; // cancelo forma de pago
                            fPago = (DataTable)SiaWin.ValReturn;
                            //SiaWin.Browse(fPago);                            
                            if (MessageBox.Show("Usted desea guardar el recaudo..?", "Guardar Recaudo", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                iddocumento = ExecuteSqlTransaction(_CodeCliente.ToString());

                                if (iddocumento <= 0)
                                    return;
                                else
                                {
                                    SiaWin.seguridad.Auditor(1, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, "GENERO RECIBO CREDICONTADO:" + iddocumento, "");
                                    MessageBox.Show("recaudo hecho por mensajero exitoso");                                    
                                    ImprimeRC(iddocumento);                                    

                                }
                                //ImprimeDocumento(iddocumento, TextCodeCliente.Text.Trim());                            
                                ActivaDesactivaControles(0);
                            }
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
                SiaWin.Func.SiaExeptionGobal(exx);
                MessageBox.Show(exx.Message);
            }
        }


        private int ExecuteSqlTransaction(string codter)
        {
            if (string.IsNullOrEmpty(cnEmp))
            {
                MessageBox.Show("Error - Cadena de Conexion nulla");
                return -1;
            }
            string TipoConsecutivo = "rcaj";
            string codtrn = "01B";
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
                    sqlConsecutivo += "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                    sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);  ";
                    sqlConsecutivo += "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0; ";
                    sqlConsecutivo += "SELECT @iFolioHost= isnull(rcaj,0)+1,@ini=rtrim('" + codpvta + "') FROM Copventas WHERE  cod_pvt='" + codpvta + "'; ";
                    sqlConsecutivo += "set @num=@iFolioHost; ";
                    sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));  ";
                    //sqlConsecutivo += "select @iConsecutivo as consecutivo; ";
                    sqlConsecutivo += "UPDATE COpventas SET " + TipoConsecutivo + " = ISNULL(" + TipoConsecutivo + ", 0) + 1  WHERE cod_pvt='" + codpvta + "'; ";


                    string sqlcab = sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,fec_trn,num_trn,detalle,cod_ven,rc_prov,ven_com,pun_ven) values ('" + codtrn + "',@fecdoc,@iConsecutivo,'Recaudo Credicontado','" + CmbVen.SelectedValue + "','" + TextRProv.Text.Trim() + "','" + CmbVen.SelectedValue + "','" + codpvta + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                    string sql = "";
                    foreach (System.Data.DataRow item in dtCue.Rows)
                    {
                        double abono = Convert.ToDouble(item["abono"].ToString());
                        if (abono > 0)
                        {
                            int tipapli = Convert.ToInt32(item["tip_apli"].ToString());
                            if (tipapli == 2 || tipapli == 3) sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + item["cod_cta"].ToString() + "','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Pago/Abono Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + abono.ToString("F", CultureInfo.InvariantCulture) + ");";
                            if (tipapli == 1 || tipapli == 4) sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,doc_cruc,doc_ref,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + item["cod_cta"].ToString() + "','" + item["cod_cco"].ToString() + "','" + item["cod_ter"].ToString() + "','Pago/Abono Doc:" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "','" + item["num_trn"].ToString() + "'," + abono.ToString("F", CultureInfo.InvariantCulture) + ");";
                        }
                    }

                    if (Retefte > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'13551505','','" + codter.Trim() + "','ReteFte:" + TextCodeCliente.Text.Trim() + "'," + Retefte.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Descto > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'530535','','" + codter.Trim() + "','Descto:" + TextCodeCliente.Text.Trim() + "'," + Descto.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }

                    if (Reteica > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'135518','','" + codter.Trim() + "','ReteIca" + TextCodeCliente.Text.Trim() + "'," + Reteica.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Mayorvlr > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'429505','','" + codter.Trim() + "','Mayor Vr Recibido:" + TextCodeCliente.Text.Trim() + "'," + Mayorvlr.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Menorvlr > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'530535','','" + codter.Trim() + "','Menor Vr Recibido:" + TextCodeCliente.Text.Trim() + "'," + Menorvlr.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }
                    if (Anticipo > 0)
                    {
                        sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,cre_mov) values (@NewID,'" + codtrn + "',@iConsecutivo,'280505','','" + codter.Trim() + "','Anticipo:" + TextCodeCliente.Text.Trim() + "'," + Anticipo.ToString("F", CultureInfo.InvariantCulture) + ");";
                    }


                    string sqlban = "";
                    foreach (System.Data.DataRow item1 in fPago.Rows)
                    {
                        string value = item1["valor"].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            double abono = Convert.ToDouble(item1["valor"].ToString());
                            if (abono > 0)
                            {
                                string _cta = item1["cod_cta"].ToString().Trim();
                                string cod_ban = item1["cod_ban"].ToString().Trim();
                                string fec_venc = item1["fec_venc"].ToString().Trim();
                                string fec_con = item1["fec_con"].ToString().Trim();
                                string documento = item1["documento"].ToString().Trim();
                                string cod_banco = item1["cod_banco"].ToString().Trim();

                                //if (cod_ban == "45" || cod_ban == "50")                                
                                //if (cod_ban == "01" || cod_ban == "90")
                                sqlban = sqlban + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov,fec_venc,num_chq,cod_banc,cod_pag) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + _cta.Trim() + "','','" + codter.Trim() + "','Pago/Abono:" + TextCodeCliente.Text.Trim() + "'," + abono.ToString("F", CultureInfo.InvariantCulture) + ",'" + fec_venc + "','" + documento + "','" + cod_banco + "','" + cod_ban + "');";
                                //else
                                //  sqlban = sqlban + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_cco,cod_ter,des_mov,deb_mov,fec_con,cod_pag) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + _cta.Trim() + "','','" + codter.Trim() + "','Pago/Abono:" + codter + "'," + abono.ToString("F", CultureInfo.InvariantCulture) + ",'" + fec_con + "','" + cod_ban + "');";


                            }
                        }
                    }
                    //mierda
                    
                    command.CommandText = sqlcab + sql + sqlban + @"select CAST(@NewId AS int);";
                    //MessageBox.Show(command.CommandText);
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    return Convert.ToInt32(r.ToString());
                }
                catch (Exception ex)
                {
                    //SiaWin.Func.SiaExeptionGobal(ex);
                    //errorMessages.Append("Error sasas :" + ex.StackTrace + "-" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show("error comuniquese con el administador:"+ex);
                    return -1;
                }
            }
        }

        public Tuple<bool, string> validacion()
        {
            bool flag = false;
            string cadena = "";

            if (string.IsNullOrEmpty(TextFecha.Text) || TextFecha.Text == "") { flag = true; cadena = "TextFecha"; }
            if (string.IsNullOrEmpty(TextCodeCliente.Text) || TextCodeCliente.Text == "") { flag = true; cadena = "TextCodeCliente"; }
            if (string.IsNullOrEmpty(TextNomCliente.Text) || TextNomCliente.Text == "") { flag = true; cadena = "TextNomCliente"; }
            if (CbMes.SelectedIndex < 0) { flag = true; cadena = "CbMes"; }
            if (CmbVen.SelectedIndex < 0) { flag = true; cadena = "CmbVen"; }
            if (TextVlrRecibido.Value < 0) { flag = true; cadena = "TextVlrRecibido"; }


            var tuple = new Tuple<bool, string>(flag, cadena);


            return tuple;
        }


        private void dataGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            sumaAbonos();
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8)
                {
                    GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
                    if (Colum.MappingName == "abono" || Colum.MappingName == "dto_imal" || Colum.MappingName == "dto_incol" || Colum.MappingName == "dto_tmk" || Colum.MappingName == "dto_gab" || Colum.MappingName == "dto_vcd" || Colum.MappingName == "dto_sic" || Colum.MappingName == "dto_ot")
                    {
                        System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                        dr.BeginEdit();
                        VlrRecibido = Convert.ToDouble(TextVlrRecibido.Value);
                        double vrRecaudo = (VlrAbonado + Anticipo + Mayorvlr - Retefte - Reteica - Menorvlr - Descto);
                        VlrRecibido = VlrRecibido - vrRecaudo;

                        double _cnt = Convert.ToDouble(dr["saldo"].ToString());
                        if (VlrRecibido >= _cnt)
                            dr["abono"] = _cnt;
                        else
                            dr["abono"] = VlrRecibido;



                        dr.EndEdit();
                        e.Handled = true;
                    }
                    dataGrid.UpdateLayout();

                    sumaAbonos();
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("11: F8" + w);
            }
        }

        private void sumaAbonos()
        {
            try
            {
                if (string.IsNullOrEmpty(TextCodeCliente.Text)) return;
                Descto = Convert.ToDouble(TextDescto.Value);
                Retefte = Convert.ToDouble(TextRetefte.Value);
                Reteica = Convert.ToDouble(TextIca.Value);
                Mayorvlr = Convert.ToDouble(TextMayorVlr.Value);
                Menorvlr = Convert.ToDouble(TextMenorVlr.Value);
                Anticipo = Convert.ToDouble(TextAnticipo.Value);

                double.TryParse(dtCue.Compute("Sum(abono)", "").ToString(), out VlrAbonado);
                TotalAbono.Text = VlrAbonado.ToString();
                //VlrAbonado = Convert.ToDouble(TextVlrRecibido.Value);

                TotalRecaudo.Text = (VlrAbonado + Anticipo + Mayorvlr - Retefte - Reteica - Menorvlr - Descto).ToString("C");

            }
            catch (Exception W)
            {
                SiaWin.Func.SiaExeptionGobal(W);
                MessageBox.Show("sUMA DE ABONOS www:" + W);
            }
        }

        private void ActualizaTotal(object sender, RoutedEventArgs e)
        {
            sumaAbonos();
        }

        private void BtbCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BtbCancelar.Content.ToString() == "Cancelar")
                {
                    if (dtCue.Rows.Count > 0)
                    {
                        if (MessageBox.Show("Usted desea cancelar este documento..?", "Cancelar Recibo de Caja", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
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
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message);
            }
        }


        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                string valor = (CbMes.SelectedItem as ComboBoxItem).Content.ToString();

                if (valor == "Si")
                {
                    CmbVen.IsEnabled = true;
                    DataTable dt = dtVen.Select("").CopyToDataTable();
                    CmbVen.ItemsSource = dt.DefaultView;

                    TextRProv.IsEnabled = true;
                }
                else
                {
                    CmbVen.IsEnabled = true;
                    DataTable dt = dtVen.Select("cod_ven='A1' or cod_ven='A2'").CopyToDataTable();
                    CmbVen.ItemsSource = dt.DefaultView;

                    TextRProv.IsEnabled = false;
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("Error ComboBox_SelectionChanged:" + w);
            }
        }

        private void TextRProv_LostFocus(object sender, RoutedEventArgs e)
        {
            string texto = (sender as TextBox).Text;
            if (CmbVen.SelectedIndex < 0) return;
            if (validarReciboProvi(texto) == false) return;
            if (existenciaConbleReciboPrv(TextRProv.Text.Trim()) == true)
            {
                MessageBox.Show("el recibo:" + TextRProv.Text.Trim() + " ya ha sido generado en contabilidad");
                return;
            }
        }

        public bool validarReciboProvi(string texto)
        {
            bool bandera = true;

            if (CmbVen.SelectedValue.ToString().Trim() == "A1" || CmbVen.SelectedValue.ToString().Trim() == "A2")
            {
                if (string.IsNullOrEmpty(texto) || texto == "") bandera = true;
            }
            else
            {
                string valor = TextRProv.Text;
                string query = "select * from cotalon_rc where '" + valor + "' between desde and hasta";
                DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);

                if (dt.Rows.Count > 0)
                {
                    string VenTabla = dt.Rows[0]["cod_ven"].ToString().Trim().ToUpper();
                    string VenSele = CmbVen.SelectedValue.ToString().Trim().ToUpper();

                    if (VenTabla != VenSele)
                    {
                        MessageBox.Show("este recibo provisional le pertenece a otro vendedor:" + VenTabla);
                        TextRProv.Text = "";
                        bandera = false;
                    }
                }
                else
                {
                    MessageBox.Show("El recibo provisional no existe");
                    TextRProv.Text = "";
                    bandera = false;
                }
            }

            return bandera;
        }

        public bool existenciaConbleReciboPrv(string recibo)
        {
            if (string.IsNullOrEmpty(recibo) && (CmbVen.SelectedValue.ToString().Trim() == "A1" || CmbVen.SelectedValue.ToString().Trim() == "A2"))
                return false;

            bool bandera = false;
            string query = "select * from CoCab_doc where rc_prov='" + recibo + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
            if (dt.Rows.Count > 0) bandera = true;
            if (CmbVen.SelectedValue.ToString().Trim() == "A1" || CmbVen.SelectedValue.ToString().Trim() == "A2") bandera = false;
            return bandera;
        }
        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {                   
                string query = "select idreg,num_trn,cod_ven,InMae_mer.nom_mer,rc_prov,fec_trn from cocab_doc ";
                query += "left join InMae_mer on cocab_doc.cod_ven = inmae_mer.cod_mer ";
                query += "where cocab_doc.cod_trn='01B' and fec_trn between '" + TX_fecIni.Text+ "' and '"+TX_fecFin.Text+" 23:59:59' ";

                //MessageBox.Show(query);

                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);

                if (dt.Rows.Count>0)
                {
                    dataGridConsulta.ItemsSource = dt.DefaultView;
                    TotalReg.Text = dt.Rows.Count.ToString();
                }
                else
                {
                    MessageBox.Show("sin regisrtros:");
                    TotalReg.Text = "0";
                }
            
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            var option = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
            option.ExcelVersion = ExcelVersion.Excel2013;

            var excelEngine = dataGridConsulta.ExportToExcel(dataGridConsulta.View, option);
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

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridConsulta.SelectedIndex>=0)
                {
                    DataRowView row = (DataRowView)dataGridConsulta.SelectedItems[0];                
                    int idreg = Convert.ToInt32(row["idreg"]);
                    ImprimeRC(idreg);
                    //SiaWin.Func.ImprimeDocumentoContGenerico(idreg, SiaWin._BusinessId);
                }
                else
                {
                    MessageBox.Show("seleccione un documento");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al imprimir");
            }
        }

        private void ImprimeRC(int idregcab)
        {
            try
            {
                DataTable dtAud = new DataTable();
                dtAud = SiaWin.DB.SqlDT("select cod_trn,num_trn,fec_trn,cod_ven,inmae_mer.nom_mer from " + "co" + "cab_doc inner join inmae_mer on inmae_mer.cod_mer=cocab_doc.cod_ven where idreg=" + idregcab, "tmp", idemp);
                string _codtrn = "";
                string _numtrn = "";
                string _codven = "";
                string _nomven = "";
                if (dtAud.Rows.Count > 0)
                {
                    _codtrn = dtAud.Rows[0]["cod_trn"].ToString();
                    _numtrn = dtAud.Rows[0]["num_trn"].ToString();
                    _codven = dtAud.Rows[0]["cod_ven"].ToString();
                    _nomven = dtAud.Rows[0]["nom_mer"].ToString();
                }

                //MessageBox.Show("_codtrn :"+ _codtrn);
                //MessageBox.Show("_numtrn :" + _numtrn);

                if (_codtrn == "")
                {
                    MessageBox.Show("El documento no existe...", "ImprimeRC");
                    return;
                }
                // trae factuas canceladas

                //echo por don wilmer 
                //string sqltext = @"select string_agg(rtrim(doc_cruc),',') as facturas from cocab_doc inner join cocue_doc on cocue_doc.idregcab=cocab_doc.idreg where cocab_doc.cod_trn='" + _codtrn + "' and cocab_doc.num_trn='" + _numtrn + "' and  rtrim(doc_cruc)<>''";

                //echo por alejandro
                string sqltext = "select doc_cruc as facturas from CoCab_doc ";
                sqltext += " inner join cocue_doc on cocue_doc.idregcab = cocab_doc.idreg ";
                sqltext += " where cocab_doc.cod_trn = '" + _codtrn + "' and cocab_doc.num_trn = '" + _numtrn + "' and rtrim(doc_cruc)<> '' ";
                sqltext += " group by doc_cruc ";

                DataTable dtfacturas = SiaWin.DB.SqlDT(sqltext, "tmp", idemp);
                string _Facturas = "";
                if (dtfacturas.Rows.Count > 0)
                {
                    int com = 1;
                    foreach (System.Data.DataRow item in dtfacturas.Rows)
                    {
                        string coma = com == 1 ? "" : ",";
                        _Facturas += coma + item["facturas"].ToString().Trim() + "";
                        com++;
                    }
                }


                string sqltexttotal = @"select sum(iif(substring(cod_cta, 1, 2) = '11', deb_mov, 0)) as total from cocab_doc inner join cocue_doc on cocue_doc.idregcab=cocab_doc.idreg where cocab_doc.cod_trn='" + _codtrn + "' and cocab_doc.num_trn='" + _numtrn + "'";
                DataTable dtTotal = SiaWin.DB.SqlDT(sqltexttotal, "tmp", idemp);
                decimal totalfac = 0;
                if (dtTotal.Rows.Count > 0)
                {
                    totalfac = (decimal)dtTotal.Rows[0]["total"];
                }

                string enletras = SiaWin.Func.enletras(totalfac.ToString());  //valor en letra

                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(cod_empresa);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);

                ReportParameter paramcodtrn = new ReportParameter();
                paramcodtrn.Values.Add(_codtrn);
                paramcodtrn.Name = "codtrn";
                parameters.Add(paramcodtrn);
                ReportParameter paramnumtrn = new ReportParameter();
                paramnumtrn.Values.Add(_numtrn);
                paramnumtrn.Name = "numtrn";
                parameters.Add(paramnumtrn);

                ReportParameter paramFacturas = new ReportParameter();
                paramFacturas.Values.Add(_Facturas);
                paramFacturas.Name = "Facturas";
                parameters.Add(paramFacturas);

                ReportParameter paramValorLetras = new ReportParameter();
                paramValorLetras.Values.Add(enletras);
                paramValorLetras.Name = "ValorLetras";
                parameters.Add(paramValorLetras);


                string repnom = @"/Contabilidad/ReciboCredicontado";
                string TituloReport = "Recibo Credicontado -";
                SiaWin.Reportes(parameters, repnom, TituloReporte: TituloReport, Modal: true, idemp: idemp, ZoomPercent: 50);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }

        }







    }
}

