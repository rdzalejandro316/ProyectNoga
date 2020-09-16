using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;


namespace SiasoftAppExt
{
 
    //Sia.PublicarPnt(9480,"ImporatDocXML");    
    //dynamic ww = ((Inicio)Application.Current.MainWindowf).WindowExt(9480, "ImporatDocXML");
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog(); 



    public partial class ImporatDocXML : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string empNit = "";
        string cod_empresa = "";

        public Boolean minibandera = false;

        public int errores = 0;

        private ObservableCollection<Productos> _productos;
        public ObservableCollection<Productos> Produ
        {
            get { return _productos; }
            set { _productos = value; }
        }
        public object iva = new object();
        public string ruta;
        public string second;
        public System.Data.DataTable TablaXML = new DataTable();
        public Boolean bandera = false;
        public DateTime fechaPeri;
        public DateTime FechaDocumento;
        public DateTime FechaXML;
        public List<object> lalo = new List<object>();

        List<object> totaList = new List<object>();
        List<object> produList = new List<object>();
        List<object> legaList = new List<object>();
        List<object> subList = new List<object>();
        List<object> uniqueList = new List<object>();
        XNamespace fe = "http://www.dian.gov.co/contratos/facturaelectronica/v1";
        XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";


        int data_erroneos = 0;

        public string bodega = "";

        public ImporatDocXML()
        {
            InitializeComponent();

            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            //LoadConfig();
            TablaXML.Columns.Add("Cod_ref");
            TablaXML.Columns.Add("Descripcion");
            TablaXML.Columns.Add("Cantidad");
            TablaXML.Columns.Add("Valor_unitario");
            TablaXML.Columns.Add("Valor_Iva");
            TablaXML.Columns.Add("Valor_SubTotal");
            TablaXML.Columns.Add("Valor_TotalIva");
            TablaXML.Columns.Add("Valor_Total");
            TablaXML.Columns.Add("Cod_tiva");
            TablaXML.Columns.Add("doc_cruc");
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                //idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                empNit = foundRow["BusinessNit"].ToString().Trim(); ;
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Importar XML - Empresa:" + cod_empresa + "-" + nomempresa + "-Nit:" + empNit;
                CargarBodegas(cod_empresa);
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }
        public void CargarBodegas(string cod_emp)
        {
            DataTable dtBod = SiaWin.Func.SqlDT("select cod_bod,cod_bod+'-'+nom_bod as nom_bod from inmae_bod where cod_emp='" + cod_emp + "';", "Bodegas", idemp);
            comboBoxBodegas.ItemsSource = dtBod.DefaultView;
        }


        private void Llenar()
        {
            TablaXML.Clear();
            //subList.Clear();
            data_erroneos = 0;
            if (_productos != null) _productos.Clear();
            //_productos.Clear();
            DataProducto.ItemsSource = null;
            int cont = 0, con1 = 0, con2 = 0, con3 = 0, con4 = 0, con9 = 0, conFina = 0;
            #region Modificacion lectura CDATA
            XDocument message = XDocument.Load(ruta);
            XCData cdata = message.DescendantNodes().OfType<XCData>().Where(m => m.Parent.Name == cbc + "Description").ToList()[0];
            string cDataContent = cdata.Value;
            XmlDocument xDoc = new XmlDocument();
            cDataContent = message.Root.Descendants(cbc + "Description").First().Value;
            xDoc.LoadXml(cDataContent);
            string eme = xDoc.ToString();

            XElement xelement = XElement.Load(new XmlNodeReader(xDoc));
            #endregion

            var unique = from el in xelement.Elements(cac + "InvoiceLine") select el;
            var sub = from el in xelement.Elements(cac + "LegalMonetaryTotal").Elements(cbc + "LineExtensionAmount") select el;
            var codigo = from el in xelement.Elements(cac + "InvoiceLine").Elements(cac + "Item").Elements(cac + "StandardItemIdentification").Elements(cbc + "ID") select el;
            var cantidad = from el in xelement.Elements(cac + "InvoiceLine").Elements(cbc + "InvoicedQuantity") select el;
            var description = from el in xelement.Elements(cac + "InvoiceLine").Elements(cac + "Item").Elements(cbc + "Description") select el;
            var valUnit = from el in xelement.Elements(cac + "InvoiceLine").Elements(cac + "Price").Elements(cbc + "PriceAmount") select el;
            var IVA = from el in xelement.Elements(cac + "TaxTotal").Elements(cac + "TaxSubtotal").Elements(cac + "TaxCategory").Elements(cbc + "Percent") select el;
            var TotIVA = from el in xelement.Elements(cac + "TaxTotal").Elements(cbc + "TaxAmount") select el;
            var valTot = from el in xelement.Elements(cac + "InvoiceLine").Elements(cbc + "LineExtensionAmount") select el;
            var totalPago = xelement.Descendants(cbc + "PayableAmount");

            var numFac = xelement.Descendants(cbc + "ID").FirstOrDefault();
            var fechFac = xelement.Descendants(cbc + "IssueDate").FirstOrDefault();


            foreach (var el in unique)
            {
                cont += 1;
            }
            object[] sharpArray = new object[cont];
            object[] codigoArray = new object[cont];
            object[] cantidadArrray = new object[cont];
            object[] descripcionArray = new object[cont];
            object[] valunitArray = new object[cont];
            object[] totArray = new object[cont];
            FacTXT.Text = numFac.Value;

            //DateTime dateValue = Convert.ToDateTime(fechFac.Value);
            //FechaXML = dateValue;
            TX_FecXML.Text = fechFac.Value;

            _productos = new ObservableCollection<Productos>();
            foreach (var item in codigo)
            {
                codigoArray[con1] = item.Value;
                //lalo.Add(textos[1]);
                //sharpArray[con1] = textos[0];
                con1 += 1;
            }
            foreach (var item in cantidad)
            {
                string def = QuitarZero(item.Value);
                cantidadArrray[con2] = def;
                con2 += 1;
            }
            foreach (var item in description)
            {
                descripcionArray[con3] = item.Value;
                con3 += 1;
            }
            foreach (var item in valUnit)
            {
                string def = QuitarZero(item.Value);
                valunitArray[con4] = def;
                con4 += 1;
            }
            foreach (var item in IVA)
            {
                string def = QuitarZero(item.Value);
                iva = def;
            }
            foreach (var item in valTot)
            {
                string def = QuitarZero(item.Value);
                totArray[con9] = def;
                con9 += 1;
            }
            int tutIVA = 0, tutal;
            
            foreach (var item in unique)
            {
                tutIVA = ((Convert.ToInt32(totArray[conFina]) * Convert.ToInt32(iva)) / 100);
                tutal = (tutIVA + Convert.ToInt32(totArray[conFina]));
                //TablaXML.Rows.Add(codigoArray[conFina], cantidadArrray[conFina], descripcionArray[conFina], valunitArray[conFina], "IVA", iva, "Descuento", totArray[conFina]);

                //MessageBox.Show("ref:"+);
                string refe = codigoArray[conFina].ToString().Trim();
                //MessageBox.Show("ref:"+refe);                

                //if (string.IsNullOrEmpty(referencia.Item1)) data_erroneos++;


                _productos.Add(
                    new Productos(
                        Convert.ToString(codigoArray[conFina]),
                        Convert.ToString(descripcionArray[conFina]),
                        Convert.ToString(cantidadArrray[conFina]),
                        Convert.ToString(valunitArray[conFina]),
                        Convert.ToString(iva),
                        Convert.ToString(totArray[conFina]),
                        Convert.ToString(tutIVA),
                        Convert.ToString(tutal),
                        "C"
                        )
                    );

                conFina += 1;//En el xml no se encuentran datos del descuento por producto, solo el total de el descuento al final de la factura en pdf                
            }
            foreach (var item in sub)
            {
                subList.Add(item.Value);
            }
            foreach (var item in TotIVA)
            {
                subList.Add(item.Value);
            }
            foreach (var item in totalPago)
            {
                TxtTotal.Text = (item.Value);
            }


            DataProducto.ItemsSource = null;

            DataProducto.ItemsSource = Produ;

            TotalReg.Text = Convert.ToString(DataProducto.View.Records.Count());

            //            TotalFall.Text = data_erroneos.ToString();

            string sTotal = Convert.ToString(subList[0]);
            double STotal = Convert.ToDouble(sTotal);
            Sotal.Text = (sTotal);
            TIVA.Text = Convert.ToString(subList[1]);
            txtIva.Text = "Iva " + iva + "% : "; ;

        }

        public bool ReturnRefe(string code)
        {
            string query = "select * from inmae_ref where cod_ref = '" + code + "' AND estado=1;";
            System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
            string refe = dt.Rows.Count > 0 ? dt.Rows[0]["cod_ref"].ToString() : "nada";
            return dt.Rows.Count > 0 ? true : false;
        }



        public string QuitarZero(string zero)
        {
            string[] esplit = zero.Split('.');
            //MessageBox.Show(esplit[0]);
            return esplit[0];
        }

        private void LeerXML()
        {
            try
            {


                if (ruta != null)
                {

                    #region Modificacion lectura CDATA
                    XDocument message = XDocument.Load(ruta);
                    XCData cdata = message.DescendantNodes().OfType<XCData>().Where(m => m.Parent.Name == cbc + "Description").ToList()[0];
                    string cDataContent = cdata.Value;
                    XmlDocument xDoc = new XmlDocument();
                    cDataContent = message.Root.Descendants(cbc + "Description").First().Value;
                    xDoc.LoadXml(cDataContent);
                    string eme = xDoc.ToString();

                    XElement xelement = XElement.Load(new XmlNodeReader(xDoc));
                    #endregion

                    var proID = xelement.Elements(cac + "AccountingSupplierParty").Elements(cac + "Party").Elements(cac + "PartyName").Elements(cbc + "Name");
                    var proNIT = xelement.Elements(cac + "AccountingSupplierParty").Elements(cac + "Party").Elements().Elements(cbc + "CompanyID");
                    var proDir = xelement.Elements(cac + "AccountingSupplierParty").Elements(cac + "Party").Elements().Elements().Elements().Elements(cbc + "Line");
                    var cliID = xelement.Elements(cac + "AccountingCustomerParty").Elements(cac + "Party").Elements().Elements(cbc + "Name");
                    var cliNIT = xelement.Elements(cac + "AccountingCustomerParty").Elements(cac + "Party").Elements().Elements(cbc + "CompanyID");
                    var cliDir = xelement.Elements(cac + "AccountingCustomerParty").Elements(cac + "Party").Elements().Elements().Elements().Elements(cbc + "Line");
                    var city = xelement.Descendants(cbc + "CityName");
                    var dire = xelement.Descendants(cbc + "Line");
                    //var tele = xelement.Descendants(cbc + "Telephone");
                    var unique = from el in xelement.Elements(cac + "InvoiceLine") select el;
                    var priNod = from el in xelement.Elements(cac + "InvoiceLine").Elements() select el;
                    var producto = from el in xelement.Elements(cac + "InvoiceLine").Elements().Elements() select el;
                    var total = from el in xelement.Elements(cac + "InvoiceLine").Elements().Elements().Elements() select el;


                    //XCData xcdata = (XCData)xelement.FirstNode;
                    //MessageBox.Show(xcdata.ToString());

                    //MessageBox.Show("cdata=\n"+cDataContent);

                    List<object> idList = new List<object>();
                    List<object> cityList = new List<object>();

                    foreach (var item in proID)//Nombre proveedor
                    {
                        NombreTXT.Text = item.Value;
                    }
                    foreach (var item in proNIT)
                    {
                        NITTXT.Text = item.Value;
                    }

                    foreach (var item in proDir)
                    {
                        DirTXT.Text = item.Value;
                    }

                    foreach (var item in cliID)
                    {
                        NombreTXT2.Text = item.Value;
                    }

                    foreach (var item in cliNIT)
                    {
                        //MessageBox.Show("NIT ="+item.Value);
                        NITTXT2.Text = item.Value;
                    }
                    foreach (var item in cliDir)
                    {
                        DirTXT2.Text = item.Value;
                    }
                    foreach (XElement el in producto)
                    {
                        produList.Add(el.Value);
                    }
                    foreach (var item in total)
                    {
                        totaList.Add(item.Value);
                    }
                    foreach (var item in city)
                    {
                        cityList.Add(item.Value);
                    }
                    //if (NITTXT2.Text.ToString().Trim() != empNit.Trim())
                    //{
                    //    MessageBox.Show("El nit de la factura no corresponde a la empresa actual... Proceso detenido");
                    //    return;
                    //}
                    Llenar();
                    int erroneosFucn = RecordItemsIvalidos();
                    TotalFall.Text = erroneosFucn.ToString();
                    errores = erroneosFucn;
                }
                else
                {
                    //App.Current.MainWindow.Close();
                    MessageBox.Show("no pudo leer");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("no se pudo leer bien:" + w);
            }
        }

        private void BuscarArchivo()
        {
            try
            {

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
                {
                    DefaultExt = ".xml",
                    Filter = "XML Files (*.xml)|*.xml"
                };
                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {
                    string filename = dlg.FileName;
                    ruta = filename;
                }
                LeerXML();
            }
            catch (Exception w)
            {
                MessageBox.Show("error:" + w);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BuscarArchivo();
        }

        public Boolean validarNitNumeroFact()
        {
            string cadena = "select num_trn,cod_prv,doc_ref from InCab_doc where cod_prv='" + NITTXT.Text + "' and doc_ref='" + FacTXT.Text + "' ";
            DataTable tabla = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);


            if (tabla.Rows.Count > 0)
            {
                MessageBox.Show("se encontro el numero de la factura en el documento:" + tabla.Rows[0]["num_trn"].ToString());
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean validarFecha()
        {
            string fecMesDoc = FechaDocumento.ToString("MM").Trim();
            string fecAnoDoc = FechaDocumento.ToString("yyyy").Trim();

            string FecMesxml = FechaXML.ToString("MM").Trim();
            string FecAnoxml = FechaXML.ToString("yyyy").Trim();

            if (fecMesDoc == FecMesxml && fecAnoDoc == FecAnoxml)
            {
                return false;
            }
            else
            {
                MessageBox.Show("la fecha del Documento es diferente a la fecha del XML fecha_documento:" + FechaDocumento.ToString("dd/MM/yyyy").Trim() + " fecha_xml:" + FechaXML.ToString("dd/MM/yyyy").Trim() + " debe de pertenecer al mismo periodo");
                return true;
            }
        }

        private void BTNvalidar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (DataProducto.ItemsSource == null)
                {
                    MessageBox.Show("no hay nada para importar");
                    return;
                }

                if (comboBoxBodegas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione la bodega");
                    return;
                }

                foreach (var data in _productos)
                {
                    System.Data.DataRow row = TablaXML.NewRow();
                    row["cod_ref"] = data.Referencia;
                    row["Descripcion"] = data.Nombre;
                    row["Cantidad"] = data.Cantidad;
                    row["Valor_unitario"] = data.ValUnit;
                    row["Valor_Iva"] = data.IVA;
                    row["Valor_SubTotal"] = data.SubTotal;
                    row["Valor_TotalIva"] = data.ValIVA;
                    row["Valor_Total"] = data.Total;
                    row["Cod_tiva"] = data.Cod_tiva;
                    row["doc_cruc"] = CrucTXT.Text;
                    TablaXML.Rows.Add(row);
                }


                #region Validacion
                if (errores > 0)
                {
                    MessageBox.Show("Verifique que no existan errores en la grilla");
                    return;
                }

                //SiaWin.Browse(TablaXML);
                bandera = true;
                bodega = comboBoxBodegas.SelectedValue.ToString();
                this.Close();
                #endregion
            }
            catch (Exception w)
            {
                MessageBox.Show("error al validar:" + w);
            }

        }


        public int RecordItemsIvalidos()
        {
            int total_err = 0;
            foreach (var item in _productos)
            {
                if (ReturnRefe(item.Referencia) == false) total_err++;
            }
            return total_err;
        }

        public Boolean BuscarRef(string referencia)
        {
            string cadena = "select * from inmae_ref where cod_ref='" + referencia + "' and estado=1 ";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                //BTNvalidar.IsEnabled = false;
                return false;
            }
        }

        public Boolean Buscar(string referencia)
        {
            string cadena = "select * from inmae_ref where cod_ref='" + referencia + "' AND ESTADO=1 ";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show("encontrado-----:" + referencia);
                return true;
            }
            else
            {
                MessageBox.Show("no esta---:" + referencia);
                return false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            TX_FecDoc.Text = fechaPeri.ToString();
            FechaDocumento = fechaPeri;
        }


        private void DataProducto_CurrentCellValueChanged(object sender, CurrentCellValueChangedEventArgs e)
        {

            //MessageBox.Show("sii");

        }





        private void TextBox1_KeyPress(object sender, KeyEventArgs e)
        {
            //textBox2.AppendText($"KeyUp code: {e.KeyCode}, value: {e.KeyValue}, modifiers: {e.Modifiers}" + "\r\n");
        }


        private void DataProducto_CurrentCellValidating(object sender, CurrentCellValidatingEventArgs e)
        {

            try
            {
                //if ((Keyboard.IsKeyDown(Key.F8)) || (Keyboard.IsKeyDown(Key.F6)))
                //{
                //    int idr = 0; string code = ""; string nom = "";
                //    dynamic winb = SiaWin.WindowBuscar("inmae_ref", "cod_ref", "nom_ref", "cod_ref", "idrow", "Maestra dereferencia", SiaWin.Func.DatosEmp(idemp), false, " estado=1", idEmp: idemp);
                //    winb.ShowInTaskbar = false;
                //    winb.Owner = Application.Current.MainWindow;
                //    winb.Height = 300;
                //    winb.Width = 400;
                //    winb.ShowDialog();
                //    idr = winb.IdRowReturn;
                //    code = winb.Codigo;
                //    nom = winb.Nombre;
                //    winb = null;


                //    //Productos productos = new Productos(Convert.ToString(code), Convert.ToString(nom), "", "", "", "");

                //    //_productos.Add(new Productos(Convert.ToString(code), Convert.ToString(nom),"","","",""));
                //    //var uiElement = e.OriginalSource as UIElement;
                //    //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));


                //    //Productos row = (DataRowView)DataProducto.SelectedItems[0];
                //    //row["cod_ref"] = code;

                //    //Productos pr = new Productos(code, code, code, code, code, code);
                //    //pr.Referencia = "ejmeplo";
                //    //DataProducto.UpdateDataRow(e.RowColumnIndex.RowIndex);

                //    //if (idr > 0)
                //    //{
                //    //    TB_CodigoZonaSuc.Text = code;
                //    //    TB_ZonaSuc.Text = nom;
                //    //    var uiElement = e.OriginalSource as UIElement;
                //    //    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                //    //}
                //}
                //else
                //{
                //    MessageBox.Show("no llego");
                //}
            }
            catch (Exception w)
            {
                //MessageBox.Show("error!!:" + w);
            }
        }

        //public Boolean GrillaRed(ObservableCollection<Productos> p)
        //{
        //    Boolean bandera = true;
        //    foreach (var item in p)
        //    {
        //        if (!string.IsNullOrEmpty(item.Error)) bandera = false;
        //    }
        //    return bandera;
        //}

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            bandera = false;
            this.Close();
        }

        private void DataProducto_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //if (string.IsNullOrEmpty(_productos[DataProducto.SelectedIndex].Referencia))
                //{

                //DataProducto.View.Refresh();
                if (e.Key == Key.F8 || e.Key == Key.Enter || e.Key == Key.Delete || e.Key == Key.Back)
                {
                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar("inmae_ref", "cod_ref", "nom_ref", "cod_ref", "idrow", "Maestra de referencia", SiaWin.Func.DatosEmp(idemp), false, " estado=1", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = System.Windows.Application.Current.MainWindow;
                    winb.Height = 300;
                    winb.Width = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;

                    if (!string.IsNullOrEmpty(code))
                    {
                        _productos[DataProducto.SelectedIndex].Referencia = code.Trim();
                        _productos[DataProducto.SelectedIndex].Nombre = nom.Trim();
                        DataProducto.View.Refresh();
                        errores--;
                        TotalFall.Text = errores.ToString();


                    }


                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error!!:" + w);
            }
        }

        private void DataProducto_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            try
            {

                var reflector = this.DataProducto.View.GetPropertyAccessProvider();
                var rowData = DataProducto.GetRecordAtRowIndex(DataProducto.SelectedIndex + 1);
                string refer = reflector.GetValue(rowData, "Referencia").ToString();
                var flag = ReferenciaSearch(refer);
                if (flag.Item1)
                {
                    string nom_ref = flag.Item2.Rows[0]["nom_ref"].ToString().Trim();
                    string cod_tiva = flag.Item2.Rows[0]["cod_tiva"].ToString().Trim();

                    string error = reflector.GetValue(rowData, "Error").ToString();
                    if (!string.IsNullOrEmpty(error))
                    {
                        errores--;
                        TotalFall.Text = errores.ToString();
                    }

                    reflector.SetValue(rowData, "Nombre", nom_ref);
                    reflector.SetValue(rowData, "Cod_tiva", cod_tiva);
                    reflector.SetValue(rowData, "Error", "");
                }
                else
                {
                    reflector.SetValue(rowData, "Error", "se mantiene el error");
                }


                DataProducto.UpdateDataRow(e.RowColumnIndex.RowIndex);

                DataProducto.UpdateLayout();
                //DataProducto.Columns["Error"].AllowEditing = false;
                DataProducto.Columns["Cod_tiva"].AllowEditing = false;
                DataProducto.Columns["Nombre"].AllowEditing = false;

            }
            catch (Exception w)
            {
                MessageBox.Show("error al editar:" + w);
            }
        }

        public Tuple<bool, DataTable> ReferenciaSearch(string referencia)
        {
            string cadena = "select * from inmae_ref where cod_ref='" + referencia + "' and estado=1 ";
            DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
            return new Tuple<bool, DataTable>(dt.Rows.Count > 0 ? true : false, dt);
        }

        private void NITTXT2_LostFocus(object sender, RoutedEventArgs e)
        {
            DataTable dt = SiaWin.Func.SqlDT("select * from comae_ter where cod_ter='" + (sender as TextBox).Text + "' ", "Clientes", idemp);
            if (dt.Rows.Count > 0)
            {
                NombreTXT2.Text = dt.Rows[0]["nom_ter"].ToString().Trim();
            }
            else
            {
                NombreTXT2.Text = "";
                MessageBox.Show("no se encontro el codigo:" + (sender as TextBox).Text);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;

                var excelEngine = DataProducto.ExportToExcel(DataProducto.View, options);
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

                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archivo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
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

    }

    public class Productos : IDataErrorInfo
    {

        public Boolean existencia;
        private string referencia;

        public string Referencia
        {
            get { return referencia; }
            set { referencia = value; }
        }
        private string nombre;

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }
        private string cantidad;

        public string Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }
        private string valUnit;

        public string ValUnit
        {
            get { return valUnit; }
            set { valUnit = value; }
        }
        private string iva;

        public string IVA
        {
            get { return iva; }
            set { iva = value; }
        }
        private string subTotal;

        public string SubTotal
        {
            get { return subTotal; }
            set { subTotal = value; }
        }
        private string valIVA;

        public string ValIVA
        {
            get { return valIVA; }
            set { valIVA = value; }
        }

        private string cod_tiva;

        public string Cod_tiva
        {
            get { return cod_tiva; }
            set { cod_tiva = value; }
        }

        private string total;
        public string Total
        {
            get { return total; }
            set { total = value; }
        }
        private string codRef;

        public string codigo(string code)
        {
            CodReferencia = code;
            return null;
        }
        [Display(AutoGenerateField = false)]
        public string CodReferencia
        {
            get { return codRef; }
            set { codRef = value; }
        }

        [Display(AutoGenerateField = false)]
        public string Error { get; set; }


        public string this[string columnName]
        {
            get
            {
                if (!columnName.Equals("Referencia"))
                    return string.Empty;


                ImporatDocXML principal = new ImporatDocXML();
                principal.minibandera = principal.BuscarRef(Referencia);
                existencia = principal.minibandera;

                if (principal.minibandera == false)
                {
                    Error = "La referencia no existe: " + this.Referencia;
                    return "La referencia no existe: " + this.Referencia;
                }

                return string.Empty;
            }
        }

        public Productos(string refe, string nom, string cant, string unit, string Iva, string subto, string tIVA, string tot, string tiva)
        {
            Referencia = refe;
            Nombre = nom;
            Cantidad = cant;
            ValUnit = unit;
            IVA = Iva;
            SubTotal = subto;
            ValIVA = tIVA;
            Total = tot;
            Cod_tiva = tiva;
        }

    }


}


