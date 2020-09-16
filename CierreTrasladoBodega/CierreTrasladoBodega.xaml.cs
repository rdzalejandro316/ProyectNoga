using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9472, "CierreTrasladoBodega");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9472, "CierreTrasladoBodega");
    //ww.cod_transaccion="015";    
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();
    public partial class CierreTrasladoBodega : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string cod_transaccion = "";
        string cod_tras_cambio = "";

        public Boolean bandera = false;

        public string bodega = "";

        public CierreTrasladoBodega()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId;

            if (dataGridCabeza.SelectedIndex >= 0)
            {
                dataGridCabeza.Focus();
                dataGridCabeza.MoveCurrentCell(new RowColumnIndex(1, 1), false);
                dataGridCabeza.ScrollInView(new RowColumnIndex(1, 1));
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
            GridCabeza();
            codigo_cam(cod_transaccion);
            TrnText.Text = "Transaccion: " + cod_tras_cambio;
        }

        public void codigo_cam(string codigo)
        {
            if (codigo == "015")
            {
                cod_tras_cambio = "001";
                col_cod_prv.IsHidden = false;
                col_nom_ter.IsHidden = false;
                col_Provedor.IsHidden = false;
            }
            if (codigo == "016")
            {
                cod_tras_cambio = "051";
                col_cod_prv.IsHidden = true;
                col_nom_ter.IsHidden = true;
                col_Provedor.IsHidden = true;
            }
            if (codigo == "018")
            {
                cod_tras_cambio = "003";
            }
        }

        private void LoadConfig()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Cierre de Traslado - Empresa:" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void GridCabeza()
        {
            try
            {
                string where = string.IsNullOrEmpty(bodega) ? " " : " and cuerpo.cod_bod='" + bodega + "'  ";

                string cadena = "select cabeza.idreg,cabeza.cod_trn,cabeza.num_trn,convert(datetime, cabeza.fec_trn, 103) as fec_trn,cabeza.cod_prv,tercero.nom_ter,cuerpo.cod_bod,cabeza.bod_tra  ";
                cadena = cadena + "from InCab_doc as cabeza  ";
                cadena = cadena + "left join comae_ter as tercero on cabeza.cod_prv = tercero.cod_ter ";
                cadena = cadena + "inner join InCue_doc as cuerpo on cabeza.idreg = cuerpo.idregcab ";
                cadena = cadena + "where cabeza.cod_trn='" + cod_transaccion + "'  " + where + "  ";
                cadena = cadena + "group by cabeza.idreg,cabeza.cod_trn,cabeza.num_trn,cabeza.fec_trn,cabeza.cod_prv,tercero.nom_ter,cuerpo.cod_bod,cabeza.bod_tra ";


                DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);

                if (dt.Rows.Count > 0)
                {
                    bandera = true;
                    dataGridCabeza.ItemsSource = dt.DefaultView;
                    TxRegi.Text = dt.Rows.Count.ToString();
                    //dataGridCabeza.Focus();
                    //dataGridCabeza.MoveCurrentCell(new RowColumnIndex(1, 1), false);
                    //dataGridCabeza.ScrollInView(new RowColumnIndex(1, 1));
                }
                else
                {
                    TxRegi.Text = "sin registros";
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al cargar el documento" + w);
            }
        }



        private void dataGridCabeza_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridCabeza.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                    string idreg = row["idreg"].ToString();
                    UltimoDoc.Text = row["num_trn"].ToString();

                    string cadena = "select cuerpo.cod_ref,inmae_ref.cod_ant,inmae_ref.nom_ref,cabeza.cod_prv,cuerpo.cantidad from InCue_doc as cuerpo ";
                    cadena = cadena + "inner join InCab_doc as cabeza on cuerpo.idregcab = cabeza.idreg ";
                    cadena = cadena + "inner join inmae_ref on cuerpo.cod_ref = inmae_ref.cod_ref ";
                    cadena = cadena + "where idregcab='" + idreg + "' ";

                    DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
                    dataGridCuerpo.ItemsSource = dt.DefaultView;

                    double TotCant = 0;
                    if (dt.Rows.Count > 0)
                    {
                        TotCant = Convert.ToDouble(dt.Compute("Sum(cantidad )", "").ToString());
                    }

                    TotalCantidades.Text = TotCant.ToString();
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error en el change:" + w);
            }

        }

        private void BTnsalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BTnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridCabeza.SelectedIndex >= 0)
                {
                    if (MessageBox.Show("Desea realizar el cambio?", "Siasoft", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {

                        DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                        string idregCab = row["idreg"].ToString();
                        string cod_trn_ant = row["cod_trn"].ToString();
                        string isPvBog = row["bod_tra"].ToString();

                        string cadena = "update InCab_doc set cod_trn='" + cod_tras_cambio + "' where idreg='" + idregCab + "'; ";
                        cadena = cadena + "update InCue_doc set cod_trn='" + cod_tras_cambio + "' where idregcab='" + idregCab + "'; ";


                        if (SiaWin.Func.SqlCRUD(cadena, idemp) == true)
                        {

                            SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, "CAMBIO DEL DOCUMENTO:" + idregCab + "  (COD_TRN ANTIGUO:" + cod_trn_ant + " - COD_TRN NUEVO:" + cod_tras_cambio + ")", "");

                            MessageBox.Show("Cambio de Tipo de Transaccion Exitoso");

                            if (cod_transaccion == "015")
                            {
                                GenerarDocumentoContabe(idregCab.Trim(), "001");
                                GenerarDocumentoNIIF(idregCab.Trim());
                            }
                            if (cod_transaccion == "018")
                            {
                                GenerarDocumentoContabe(idregCab.Trim(), "003");
                                //GenerarDocumentoNIIF(idregCab.Trim());
                            }

                            //dataGridCuerpo.SelectedItems.Clear();
                            dataGridCuerpo.ItemsSource = null;
                            bandera = false;
                            GridCabeza();
                        }
                    }
                }
                else
                {

                    MessageBox.Show("Seleccione un documento");
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("selecione un documento:" + w);
            }
        }

        private void BTNConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string cabeza = "select cabeza.idreg,cabeza.cod_trn,num_trn,cabeza.fec_trn,cabeza.cod_prv,provedor.nit_prv,provedor.nom_prv ";
                cabeza = cabeza + "from InCab_doc as cabeza  ";
                cabeza = cabeza + "left join InMae_prv as provedor on cabeza.cod_prv = provedor.cod_prv ";
                cabeza = cabeza + "where cabeza.cod_trn='" + cod_tras_cambio + "' and num_trn='" + TX_Documento.Text.Trim() + "'; ";
                DataTable DTCabeza = SiaWin.Func.SqlDT(cabeza, "Clientes", idemp);
                dataGridCabezaConsulta.ItemsSource = DTCabeza.DefaultView;

                string cuerpo = "select cuerpo.cod_ref,referencia.cod_ant,referencia.nom_ref,cabeza.cod_prv,cuerpo.cantidad from InCue_doc as cuerpo ";
                cuerpo = cuerpo + "inner join InCab_doc as cabeza on cuerpo.idregcab = cabeza.idreg ";
                cuerpo = cuerpo + "inner join inmae_ref as referencia on cuerpo.cod_ref = referencia.cod_ref ";
                cuerpo = cuerpo + "where cuerpo.cod_trn='" + cod_tras_cambio + "' and cuerpo.num_trn='" + TX_Documento.Text.Trim() + "';  ";
                DataTable DTCuerpo = SiaWin.Func.SqlDT(cuerpo, "Clientes", idemp);
                dataGridCuerpoConsulta.ItemsSource = DTCuerpo.DefaultView;


            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("Error en la consulta del documeto");
            }
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                string idTab = ((TextBox)sender).Tag.ToString();
                if (idTab.Length > 0)
                {
                    string tag = ((TextBox)sender).Tag.ToString();
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = false; string cmpwhere = "";
                    if (string.IsNullOrEmpty(tag)) return;

                    if (tag == "incab_doc")
                    {
                        cmptabla = tag; cmpcodigo = "idreg"; cmpnombre = "num_trn"; cmporden = "idreg"; cmpidrow = "idreg"; cmptitulo = "Documentos"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                    }
                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (idr > 0)
                    {
                        if (tag == "incab_doc")
                        {
                            TX_Documento.Text = nom;
                        }
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message.ToString());
            }

        }

        public void GenerarDocumentoContabe(string idreg, string trn)
        {
            try
            {

                //documento
                string num_anu = "";

                string numero_trn = "";
                string TranCode = trn == "001" ? "18" : "16";
                //tercero 
                string fec_fac = "";
                string terceroInterempresa = "";
                string centrocosto = "";
                //retenciones
                bool re_iva = false;
                bool re_ica = false;
                bool re_ret = true;
                bool maneja_iva = true;

                //cuentas y porcentajes
                //ica
                string cta_ica = "";
                decimal por_cta_ica = 0;
                decimal base_cta_ica = 0;
                //retefuente
                string cta_ret = "";
                decimal por_cta_ret = 0;
                decimal base_cta_ret = 0;
                //reteiva
                string cta_ret_iva = "";
                decimal por_cta_ret_iva = 0;
                decimal base_cta_ret_iva = 0;

                //cxc
                string cta_cxc = "";

                //valores debito y credito
                decimal debito = 0;
                decimal credito = 0;

                //MessageBox.Show("1#");
                #region validacion del tercero si se le hace retenciones             
                string queryTercero = "select InCab_doc.fec_trn,InCab_doc.num_trn,ISNULL(InCab_doc.cod_prv,'') as cod_ter,isnull(InMae_mer.cod_cco,'') as cod_cco,InCab_doc.bod_tra,InCab_doc.num_anu, ";
                queryTercero += "isnull(CoMae_ter.ind_ret,0) as ind_ret,isnull(CoMae_ter.ret_iva,0) as ret_iva,isnull(CoMae_ter.ret_ica,0) as ret_ica,isnull(CoMae_ter.ind_iva,1) as ind_iva ";
                queryTercero += "from incab_doc ";
                queryTercero += "inner join InCue_doc on incab_doc.idreg = InCue_doc.idregcab ";
                queryTercero += "inner join InMae_bod on InCue_doc.cod_bod = InMae_bod.cod_bod ";
                queryTercero += "left join Comae_ter on InMae_bod.cod_ter = Comae_ter.cod_ter ";
                queryTercero += "left join InMae_mer on Comae_ter.cod_ven = InMae_mer.cod_mer ";
                queryTercero += "where incab_doc.idreg='" + idreg + "' ";
                queryTercero += "group by InCab_doc.fec_trn,InCab_doc.num_trn,InCab_doc.cod_prv,InMae_mer.cod_cco,InCab_doc.bod_tra,InCab_doc.num_anu,CoMae_ter.ind_ret,CoMae_ter.ret_iva,CoMae_ter.ret_ica,CoMae_ter.ind_iva ";
                //MessageBox.Show(queryTercero);

                DataTable dtvalidacion = SiaWin.Func.SqlDT(queryTercero, "conceptos", idemp);


                //SiaWin.Browse(dtvalidacion);
                if (dtvalidacion.Rows.Count > 0)
                {
                    num_anu = dtvalidacion.Rows[0]["num_anu"].ToString().Trim();
                    terceroInterempresa = dtvalidacion.Rows[0]["cod_ter"].ToString().Trim();
                    numero_trn = dtvalidacion.Rows[0]["num_trn"].ToString().Trim();
                    centrocosto = dtvalidacion.Rows[0]["cod_cco"].ToString().Trim();
                    fec_fac = dtvalidacion.Rows[0]["fec_trn"].ToString().Trim();
                    string bodegaTras = dtvalidacion.Rows[0]["bod_tra"].ToString().Trim();
                }

                //005,010,011,001
                //punto de venta de bogota
                ///bool isPVBogt = bodegaTras == "001" || bodegaTras == "005" || bodegaTras == "010" || bodegaTras == "011" ? true : false;



                //retenciones
                //decimal val_manejaiva = Convert.ToDecimal(dtvalidacion.Rows[0]["ind_iva"]);
                decimal val_manejaiva = 1;
                decimal val_retefuente = 2;

                //Convert.ToDecimal(dtvalidacion.Rows[0]["ind_ret"]);                
                //decimal val_reteica = Convert.ToDecimal(dtvalidacion.Rows[0]["ret_ica"]);

                //decimal val_reteica = isPVBogt == true ? 2 :0 ;

                //decimal val_reteiva = Convert.ToDecimal(dtvalidacion.Rows[0]["ret_iva"]);
                decimal val_reteiva = 0;
                #endregion

                //MessageBox.Show("2#");
                #region traer cuentas con sus porcentajes

                string queryCuentas = "select ";
                queryCuentas += "Cta_ret,cta_ret.por_cta as por_cta_ret,cta_ret.vlr_min as vlr_min_ret, ";
                queryCuentas += "Cta_ica,cta_ica.por_cta as por_cta_ica,cta_ica.vlr_min as vlr_min_ica, ";
                queryCuentas += "Cta_iva,cta_iva.por_cta as por_cta_iva,cta_iva.vlr_min as vlr_min_iva, ";
                queryCuentas += "Cta_cxc ";
                queryCuentas += "from InMae_con ";
                queryCuentas += "inner join Comae_cta as cta_ret on InMae_con.Cta_ret = cta_ret.cod_cta ";
                queryCuentas += "inner join Comae_cta as cta_ica on InMae_con.Cta_ica = cta_ica.cod_cta ";
                queryCuentas += "inner join Comae_cta as cta_iva on InMae_con.Cta_iva = cta_iva.cod_cta ";
                queryCuentas += "inner join Comae_cta as cta_cxc on InMae_con.Cta_cxc = cta_cxc.cod_cta  ";
                queryCuentas += "where Cod_con='050' ";

                DataTable dt = SiaWin.Func.SqlDT(queryCuentas, "conceptos", idemp);

                cta_ret = dt.Rows[0]["Cta_ret"].ToString().Trim();
                por_cta_ret = Convert.ToDecimal(dt.Rows[0]["por_cta_ret"]);
                base_cta_ret = Convert.ToDecimal(dt.Rows[0]["vlr_min_ret"]);

                cta_ica = dt.Rows[0]["Cta_ica"].ToString();
                por_cta_ica = Convert.ToDecimal(dt.Rows[0]["por_cta_ica"]);
                base_cta_ica = Convert.ToDecimal(dt.Rows[0]["vlr_min_ica"]);

                cta_ret_iva = dt.Rows[0]["Cta_iva"].ToString();
                por_cta_ret_iva = Convert.ToDecimal(dt.Rows[0]["por_cta_iva"]);
                base_cta_ret_iva = Convert.ToDecimal(dt.Rows[0]["vlr_min_iva"]);

                cta_cxc = dt.Rows[0]["Cta_cxc"].ToString();
                #endregion

                //MessageBox.Show("3#");
                #region traer compra y formar los valore para agregar a la contabilidad

                DataTable dtCompra = SiaWin.Func.SqlDT("select * from incue_doc where idregcab='" + idreg + "' and cod_trn='" + trn + "'", "Compra", idemp);
                decimal __valica = 0, __valret = 0, __cos_tot = 0;
                foreach (DataRow item in dtCompra.Rows)
                {

                    decimal _cos_tot = Convert.ToDecimal(item["cos_tot"]);

                    decimal porica = Convert.ToDecimal(item["por_ica"]);
                    decimal _valica = (_cos_tot * porica) / 100;

                    decimal _valret = (_cos_tot * por_cta_ret) / 100;

                    //ica
                    __valica += _valica;
                    //retefuente
                    __valret += _valret;
                    //total
                    __cos_tot += _cos_tot;
                }


                //iva agrupacion de valores y sus cuentas
                string ivaAgrupa = "select sum(val_iva) as val_iva_total,InCue_doc.por_iva,InCue_doc.cod_tiva,InMae_tiva.cod_ctac ";
                ivaAgrupa += "from InCue_doc ";
                ivaAgrupa += "left join InMae_tiva ON InCue_doc.cod_tiva = InMae_tiva.cod_tiva ";
                ivaAgrupa += "where idregcab='" + idreg + "' and cod_trn='" + trn + "' ";
                ivaAgrupa += "group by InCue_doc.por_iva,InCue_doc.cod_tiva,InMae_tiva.cod_ctac ";
                DataTable dtIva = SiaWin.Func.SqlDT(ivaAgrupa, "IvaAgrupado", idemp);

                string cuentaIvaArmable = "";

                string cuentaReteIvaArmable = "";
                decimal totalReteIva = 0;

                decimal _val_ret_iva = 0;

                foreach (DataRow item in dtIva.Rows)
                {
                    decimal ivaVal = Convert.ToDecimal(item["val_iva_total"]);

                    if (ivaVal != 0)
                    {
                        string debito_credito = TranCode == "18" ? "deb_mov" : "cre_mov";
                        cuentaIvaArmable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov," + debito_credito + ") values (@NewTrn_001,'" + TranCode + "','" + numero_trn + "','" + item["cod_ctac"] + "','" + terceroInterempresa + "','','COMPRA MERCANCIA IVA-" + numero_trn + "'," + __cos_tot.ToString("F", CultureInfo.InvariantCulture) + "," + item["val_iva_total"] + "); ";
                        totalReteIva += ivaVal;
                        //debito += cos_totalLinea;
                    }
                }


                //totalReteIva = Math.Round(totalReteIva);
                //if (totalReteIva > 0)
                //{
                //_val_ret_iva += (totalReteIva * por_cta_ret_iva) / 100;
                //_val_ret_iva = Math.Round(_val_ret_iva);

                //string debito_credito = TranCode == "18" ? "cre_mov" : "deb_mov";
                //cuentaReteIvaArmable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov," + debito_credito + ") values (@NewTrn_001,'" + TranCode + "','" + numero_trn + "','" + cta_ret_iva + "','" + terceroInterempresa + "','','COMPRA MERCANCIA RETEIVA-" + numero_trn + "'," + __cos_tot.ToString("F", CultureInfo.InvariantCulture) + "," + _val_ret_iva.ToString("F", CultureInfo.InvariantCulture) + "); ";
                //                }



                string subtLinea = "select inmae_tip.cta_inv,sum(cos_tot) as cos_tot from incue_doc  ";
                subtLinea += "inner join inmae_ref on InCue_doc.cod_ref = inmae_ref.cod_ref ";
                subtLinea += "inner join inmae_tip on inmae_ref.cod_tip = inmae_tip.cod_tip ";
                subtLinea += "inner join Comae_cta on inmae_tip.cta_inv = Comae_cta.cod_cta ";
                subtLinea += "where idregcab='" + idreg + "' ";
                subtLinea += "group by inmae_tip.cta_inv ";
                //subtLinea += "group by inmae_ref.cod_tip,inmae_tip.cta_inv ";


                DataTable dtSubLinea = SiaWin.Func.SqlDT(subtLinea, "LineaTot", idemp);

                //SiaWin.Browse(dtSubLinea);

                string cuentainv = "";
                foreach (DataRow item in dtSubLinea.Rows)
                {
                    decimal cos_totalLinea = Convert.ToDecimal(item["cos_tot"]);

                    string debito_credito = TranCode == "18" ? "deb_mov" : "cre_mov";
                    cuentainv += @"insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov," + debito_credito + ") values (@NewTrn_001,'" + TranCode + "','" + numero_trn + "','" + item["cta_inv"] + "','" + terceroInterempresa + "','" + centrocosto + "','COMPRA MERCANCIA INV-" + numero_trn + "'," + cos_totalLinea.ToString("F", CultureInfo.InvariantCulture) + "); ";
                    debito += cos_totalLinea;
                }

                #endregion

                //MessageBox.Show("4#");
                #region validacion de retenciones

                __valret = Math.Round(__valret);
                __valica = Math.Round(__valica);

                //MANEJA IVA
                //maneja_iva = val_manejaiva == 1 ? true : false;
                maneja_iva = true;
                //retefuente
                if (val_retefuente == 0) re_ret = false;
                //if (val_retefuente == 1) re_ret = __cos_tot >= base_cta_ret ? true : false;
                //reteica
                if (__valica > 0) re_ica = true;
                //if (val_reteica == 1) re_ica = __cos_tot >= base_cta_ica ? true : false;
                //reteiva
                //if (val_reteiva == 0) re_iva = false;
                //if (val_reteiva == 1) re_iva = __cos_tot >= base_cta_ret_iva ? true : false;

                #endregion

                //MessageBox.Show("5#");
                #region sumar debitos y creditos                

                //debito
                //la suma del ingreso se realiza arriba por agrupacion de la linea
                //suma maneja iva
                debito += maneja_iva == true && totalReteIva > 0 ? totalReteIva : 0;

                //credito 
                //suma  de retefuente
                credito += re_ret == true ? __valret : 0;
                //suma  de reteica
                credito += re_ica == true ? __valica : 0;
                //suma  de rete iva
                credito += re_iva == true ? _val_ret_iva : 0;

                decimal totalcxp = debito - credito;
                #endregion

                //MessageBox.Show("6#");
                #region generar el documento contable
                using (SqlConnection connection = new SqlConnection(cnEmp))
                {

                    connection.Open();
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;

                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;


                    string sqlConsecutivo = @"declare @fecdoc as datetime;
                        set @fecdoc = getdate();declare @ini as char(4);DECLARE @NewTrn_001 INT;";

                    string sqlcab001co = sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,dia_plaz,fec_ven,factura) values ('" + TranCode + "','" + numero_trn + "','" + fec_fac + "',90,dateadd(DAY,90,@fecdoc),'" + numero_trn + "');SELECT @NewTrn_001 = SCOPE_IDENTITY();";

                    //ingreso
                    string sqlcue001co = cuentainv;
                    //iva 
                    if (maneja_iva == true)
                        sqlcue001co += cuentaIvaArmable;
                    //retefte
                    if (re_ret == true)
                    {
                        string debito_credito = TranCode == "18" ? "cre_mov" : "deb_mov";
                        sqlcue001co += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov," + debito_credito + ") values (@NewTrn_001,'" + TranCode + "','" + numero_trn + "','" + cta_ret + "','" + terceroInterempresa + "','','COMPRA MERCANCIA RETEFUENTE-" + numero_trn + "'," + __cos_tot.ToString("F", CultureInfo.InvariantCulture) + "," + __valret.ToString("F", CultureInfo.InvariantCulture) + "); ";
                    }

                    //reteica
                    if (re_ica == true)
                    {
                        string debito_credito = TranCode == "18" ? "cre_mov" : "deb_mov";
                        sqlcue001co += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov," + debito_credito + ") values (@NewTrn_001,'" + TranCode + "','" + numero_trn + "','" + cta_ica + "','" + terceroInterempresa + "','','COMPRA MERCANCIA RETEICA-" + numero_trn + "'," + __cos_tot.ToString("F", CultureInfo.InvariantCulture) + "," + __valica.ToString("F", CultureInfo.InvariantCulture) + "); ";
                    }

                    //rete_iva
                    if (re_iva == true)
                        sqlcue001co += cuentaReteIvaArmable;

                    //cxp  
                    string debito_creditoCxP = TranCode == "18" ? "cre_mov" : "deb_mov";
                    sqlcue001co += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,doc_ref,doc_cruc,bas_mov," + debito_creditoCxP + ") values (@NewTrn_001,'" + TranCode + "','" + numero_trn + "','" + cta_cxc + "','" + terceroInterempresa + "','','COMPRA MERCANCIA CXP-" + numero_trn + "','" + num_anu + "','" + num_anu + "'," + __cos_tot.ToString("F", CultureInfo.InvariantCulture) + "," + totalcxp.ToString("F", CultureInfo.InvariantCulture) + "); ";


                    command.CommandText = sqlcab001co + sqlcue001co + @"select CAST(@NewTrn_001 AS int);";
                    //MessageBox.Show(command.CommandText.ToString());
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    MessageBox.Show("documento contable generado");
                    //idreg = Convert.ToInt32(r.ToString());
                }
                #endregion


            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("Error al generar el documento contable:" + w);
            }

        }

        public void GenerarDocumentoNIIF(string idreg)
        {
            try
            {
                bool bandera = false;
                //tercero
                string cod_ter = "";
                string centrocosto = "";
                //documeto
                string num_trn = "";
                //cuentas
                //string cta_inv = "";
                string cta_cxp_niif = "";


                //MessageBox.Show("#1");
                #region fijar cuentas 
                string select = "select cxp_niif from InMae_con where Cod_con = '050'";
                DataTable dt = SiaWin.Func.SqlDT(select, "conceptos", idemp);
                //cta_inv = dt.Rows[0]["Cta_inv"].ToString().Trim();
                cta_cxp_niif = dt.Rows[0]["cxp_niif"].ToString().Trim();
                #endregion

                //MessageBox.Show("#2");
                #region obtener tercero
                string queryTercero = "select InCab_doc.num_trn,InCue_doc.cod_bod,isnull(InMae_bod.cod_ter,'') as cod_ter,ISNULL(InMae_mer.cod_cco,'') as cod_cco  ";
                queryTercero += "from incab_doc ";
                queryTercero += "inner join InCue_doc on incab_doc.idreg = InCue_doc.idregcab ";
                queryTercero += "inner join InMae_bod on InCue_doc.cod_bod = InMae_bod.cod_bod ";
                queryTercero += "left join Comae_ter on InMae_bod.cod_ter = Comae_ter.cod_ter ";
                queryTercero += "left join InMae_mer on Comae_ter.cod_ven = InMae_mer.cod_mer ";
                queryTercero += "where incab_doc.idreg='" + idreg + "' ";
                queryTercero += "group by InCab_doc.num_trn,InCue_doc.cod_bod,InMae_bod.cod_ter,InMae_mer.cod_cco ";

                DataTable dtvalidacion = SiaWin.Func.SqlDT(queryTercero, "conceptos", idemp);
                if (dtvalidacion.Rows.Count > 0)
                {
                    cod_ter = dtvalidacion.Rows[0]["cod_ter"].ToString().Trim();
                    num_trn = dtvalidacion.Rows[0]["num_trn"].ToString().Trim();
                    centrocosto = dtvalidacion.Rows[0]["cod_cco"].ToString().Trim();
                }
                #endregion

                #region obtener valores con el porcentaje para actualizar los valores niif de la compra

                string queryvalores = "select InCue_doc.idregcab,InCue_doc.cod_ref,cantidad,cos_tot,InDto_Inte.dto_pprv  ";
                queryvalores += "from InCue_doc ";
                queryvalores += "inner join InMae_ref on InCue_doc.cod_ref = InMae_ref.cod_ref ";
                queryvalores += "inner join InDto_Inte on InMae_ref.cod_tip = InDto_Inte.cod_tip ";
                queryvalores += "where InCue_doc.idregcab='" + idreg + "' ";
                DataTable dtCosto = SiaWin.Func.SqlDT(queryvalores, "Costos", idemp);
                //SiaWin.Browse(dtCosto);
                #endregion

                //MessageBox.Show("#3");
                #region armar valores

                string selectProducto = "select Sum(InCue_doc.cos_tot) as cos_tot from incue_doc where idregcab='" + idreg + "' ";
                DataTable dtCompra = SiaWin.Func.SqlDT(selectProducto, "Compra", idemp);
                decimal __cos_tot = Convert.ToDecimal(dtCompra.Rows[0]["cos_tot"]);

                string subtLinea = "select InMae_ref.cod_tip,sum(cos_tot) as cos_tot,InMae_tip.cta_inv,InDto_Inte.dto_pprv  ";
                subtLinea += "from InCue_doc   ";
                subtLinea += "inner join InMae_ref on InCue_doc.cod_ref = InMae_ref.cod_ref ";
                subtLinea += "inner join InMae_tip on InMae_ref.cod_tip = InMae_tip.cod_tip ";
                subtLinea += "inner join Comae_cta on InMae_tip.cta_ing = Comae_cta.cod_cta ";
                subtLinea += "left join InDto_Inte on InMae_ref.cod_tip = InDto_Inte.cod_tip ";
                subtLinea += "where idregcab='" + idreg + "' ";
                subtLinea += "group by InMae_ref.cod_tip,InMae_tip.cta_inv,InDto_Inte.dto_pprv ";
                DataTable dtSubLinea = SiaWin.Func.SqlDT(subtLinea, "LineaGrupo", idemp);
                //SiaWin.Browse(dtSubLinea);

                string cuentainv = "";

                decimal __cost_niif = 0;

                foreach (DataRow item in dtSubLinea.Rows)
                {
                    decimal cos_totalLinea = Convert.ToDecimal(item["cos_tot"]);
                    decimal porcentaje = Convert.ToDecimal(item["dto_pprv"]);
                    decimal cost_niif = cos_totalLinea * (1 - porcentaje / 100);

                    //__cost_niif += cost_niif;
                    decimal Costodiferncia = cos_totalLinea == cost_niif ? cos_totalLinea : cos_totalLinea - cost_niif;
                    __cost_niif += Costodiferncia;

                    cuentainv += @"insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov,cre_mov) values (@NewTrn_001,'18N','" + num_trn + "','" + item["cta_inv"] + "','" + cod_ter + "','" + centrocosto + "','COMPRA MERCANCIA-" + num_trn + "'," + __cos_tot.ToString("F", CultureInfo.InvariantCulture) + "," + Costodiferncia.ToString("F", CultureInfo.InvariantCulture) + "); ";
                }

                #endregion

                //MessageBox.Show("#4");
                #region generar documento niif
                using (SqlConnection connection = new SqlConnection(cnEmp))
                {

                    connection.Open();
                    StringBuilder errorMessages = new StringBuilder();
                    SqlCommand command = connection.CreateCommand();
                    SqlTransaction transaction;

                    transaction = connection.BeginTransaction("Transaction");
                    command.Connection = connection;
                    command.Transaction = transaction;


                    string sqlConsecutivo = @"declare @fecdoc as datetime;
                        set @fecdoc = getdate();declare @ini as char(4);DECLARE @NewTrn_001 INT;";

                    string sqlcab001co_niif = sqlConsecutivo + @" INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,dia_plaz,fec_ven,factura) values ('18N','" + num_trn + "',@fecdoc,90,dateadd(DAY,90,@fecdoc),'" + num_trn + "');SELECT @NewTrn_001 = SCOPE_IDENTITY();";


                    string sqlcue001co_niif = cuentainv;
                    //contra
                    sqlcue001co_niif += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov,deb_mov) values (@NewTrn_001,'18N','" + num_trn + "','" + cta_cxp_niif + "','" + cod_ter + "','','COMPRA MERCANCIA NIIF-" + num_trn + "'," + __cos_tot.ToString("F", CultureInfo.InvariantCulture) + "," + __cost_niif.ToString("F", CultureInfo.InvariantCulture) + "); ";

                    command.CommandText = sqlcab001co_niif + sqlcue001co_niif + @"select CAST(@NewTrn_001 AS int);";
                    //MessageBox.Show(command.CommandText.ToString());
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    MessageBox.Show("documento NIIF generado");
                    bandera = true;
                }
                #endregion

                #region actualizar costo niif en la compra

                string query = "";
                foreach (DataRow item in dtCosto.Rows)
                {
                    decimal cos_tot = Convert.ToDecimal(item["cos_tot"]);

                    decimal porcentaje = Convert.ToDecimal(item["dto_pprv"]);
                    decimal cost_niif = cos_tot * (1 - porcentaje / 100);

                    decimal cost_niif_uni = cost_niif / Convert.ToDecimal(item["cantidad"]);
                    //diferencia.ToString("F", CultureInfo.InvariantCulture)
                    query += "update InCue_doc set cos_unin=" + Math.Round(cost_niif_uni) + ",cos_totn=" + Math.Round(cost_niif) + "   where idregcab='" + idreg + "' and cod_ref='" + item["cod_ref"].ToString().Trim() + "';";
                }

                if (bandera == true)
                {
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        MessageBox.Show("se actualizo los campos de costos niif del cuerpo de la compra");
                    }
                };
                #endregion

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("Error al generar el documento niif:" + w);
            }
        }

        private void DataGridCabeza_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F8)
            {
                BTnUpdate.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }




    }
}
