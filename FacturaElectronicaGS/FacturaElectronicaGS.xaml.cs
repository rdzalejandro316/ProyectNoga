﻿//using FacturaElectronicaGS.ServiceAdjuntos;
using FacturaElectronicaGS.ServiceEnvio;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Xml.Serialization;
using SrvEnvio = FacturaElectronicaGS.ServiceEnvio;
using SrvAjunto = FacturaElectronicaGS.ServiceAdjuntos;
using FacturaElectronicaGS.ServiceAdjuntos;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9618,"FacturaElectronicaGS");

    public partial class FacturaElectronicaGS : Window
    {
        BasicHttpBinding port;
        //= new BasicHttpBinding();
        //SrvAjunto.ServiceClient serviceClientAdjunto = new SrvAjunto.ServiceClient();
        SrvEnvio.ServiceClient serviceClienteEnvio;
        //      BasicHttpBinding port = new BasicHttpBinding();

        SrvAjunto.ServiceClient serviceClientAdjunto = new SrvAjunto.ServiceClient();


        //SrvEnvio.ServiceClient serviceClienteEnvio = new SrvEnvio.ServiceClient();
        //public string Tipo_Documento = "";

        dynamic SiaWin;
        public string tokenEmpresa = string.Empty;
        public string tokenAuthorizacion = string.Empty;
        public string Url = "";
        public int idrowcab = 0;
        public int cantidadAnexos_ = 1;
        public string NumRegCab = string.Empty;
        DataSet dsImprimir = new DataSet();
        DataSet dsAnulaFactura = new DataSet();
        public string NumDocElect = string.Empty;
        public string Codigo = string.Empty;
        public string Msg = string.Empty;
        public string FechaResp = string.Empty;
        public string Cufe = string.Empty;
        public int _ModuloId = 0;
        public int _EmpresaId = 0;
        public int _AccesoId = 0;

        public string Tipo_Documento = string.Empty;

        public string codpvt = string.Empty;
        public String cnEmp = string.Empty;
        public int idemp = 0;
        string cod_empresa = string.Empty;

        public FacturaElectronicaGS()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            //MessageBox.Show("iniclass");
            if (idemp <= 0) idemp = SiaWin._BusinessId;
            //idemp = SiaWin._BusinessId; 


            this.tbxFechaEmision.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            BtnEnviar.Focus();
            //this.tbxFechaEmision.Text = "2019-12-01 07:36:01";           

        }

        //#region Construcción del Objeto Factura
        private FacturaGeneral BuildFactura()
        {
            try
            {

                //armo el objeto factura
                FacturaGeneral facturaDemo = new FacturaGeneral
                {
                    cantidadDecimales = "2"
                };

                #region cliente
                Cliente cliente = new Cliente
                {
                    actividadEconomicaCIIU = "0010",

                    destinatario = new Destinatario[1]
                };
                Destinatario destinatario = new Destinatario
                {
                    canalDeEntrega = "0"
                };

                Destinatario destinatario1 = destinatario;

                string[] correoEntrega = new string[1];
                correoEntrega[0] = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim();
                //correoEntrega[1] = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim();
                destinatario1.email = correoEntrega;
                destinatario1.fechaProgramada = tbxFechaEmision.Text.Trim();

                destinatario1.nitProveedorReceptor = "1";
                destinatario1.telefono = dsImprimir.Tables[0].Rows[0]["tel1"].ToString().Trim();
                cliente.destinatario[0] = destinatario1;
                cliente.detallesTributarios = new Tributos[1];
                Tributos tributos1 = new Tributos
                {
                    codigoImpuesto = "01"
                };
                cliente.detallesTributarios[0] = tributos1;

                string codciu = dsImprimir.Tables[0].Rows[0]["cod_ciu"].ToString().Trim();
                string depart = "";
                if (codciu.Trim() != "" && codciu.Trim().Length > 3) depart = codciu.Substring(0, 2);
                //MessageBox.Show(codciu+"-"+depart);
                Direccion direccionFiscal = new Direccion
                {
                    ciudad = dsImprimir.Tables[0].Rows[0]["ciudad"].ToString().Trim(),
                    codigoDepartamento = depart,
                    departamento = dsImprimir.Tables[0].Rows[0]["nom_ciudane"].ToString().Trim(),
                    direccion = dsImprimir.Tables[0].Rows[0]["dir1"].ToString().Trim(),
                    lenguaje = "es",
                    municipio = codciu,
                    pais = "CO",
                    zonaPostal = ""
                };

                cliente.direccionFiscal = direccionFiscal;
                cliente.email = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim();
                //OrdenDeCompra ordenCompra = new OrdenDeCompra();
                //ordenCompra.numeroPedido= dsImprimir.Tables[0].Rows[0]["ord_copmpra"].ToString().Trim();
                //cliente.email = "wilmer1104@yahoo.com";

                InformacionLegal informacionLegal = new InformacionLegal
                {
                    codigoEstablecimiento = "00001",
                    nombreRegistroRUT = dsImprimir.Tables[0].Rows[0]["nom_ter"].ToString().Trim(),
                    numeroIdentificacion = dsImprimir.Tables[0].Rows[0]["cod_clisin"].ToString().Trim(),
                    numeroIdentificacionDV = dsImprimir.Tables[0].Rows[0]["dv"].ToString().Trim(),
                    tipoIdentificacion = dsImprimir.Tables[0].Rows[0]["tdoc"].ToString().Trim()
                };
                InformacionLegal informacionLegalCliente = informacionLegal;
                cliente.informacionLegalCliente = informacionLegalCliente;
                cliente.nombreRazonSocial = dsImprimir.Tables[0].Rows[0]["nom_ter"].ToString().Trim();
                cliente.notificar = "SI";

                cliente.numeroDocumento = dsImprimir.Tables[0].Rows[0]["cod_clisin"].ToString().Trim();
                cliente.numeroIdentificacionDV = dsImprimir.Tables[0].Rows[0]["dv"].ToString().Trim();
                cliente.responsabilidadesRut = new Obligaciones[1];

                string tip_prv = dsImprimir.Tables[0].Rows[0]["tip_prv"].ToString().Trim();
                string tdoc = dsImprimir.Tables[0].Rows[0]["tdoc"].ToString().Trim();                
                string rango = "";
                switch (tip_prv)
                {
                    case "0": rango = "0-48"; break;
                    case "1": rango = "0-49"; break;
                    case "2": rango = "0-13"; break;
                    default: rango = "0-48"; break;
                }
                Obligaciones obligaciones1 = new Obligaciones
                {
                    //iif(tip_prv=1,"O-48",iif(tip_prv=2,"O-49",iif(tip_prv=3,"O-13",""))) as tip_prv,;                    
                    obligaciones = rango,
                    regimen = tdoc == "13" ? "05" : "04"
                    //obligaciones = "O-14",
                    //regimen = "04"
                };
                cliente.responsabilidadesRut[0] = obligaciones1;

                cliente.tipoIdentificacion = dsImprimir.Tables[0].Rows[0]["tdoc"].ToString().Trim();
                cliente.tipoPersona = "1";

                facturaDemo.cliente = cliente;
                #endregion
                facturaDemo.consecutivoDocumento = dsImprimir.Tables[0].Rows[0]["numtrn"].ToString().Trim();

                #region detalleDeFactura
                int ItemsCue = dsImprimir.Tables[1].Rows.Count;
                facturaDemo.detalleDeFactura = new FacturaDetalle[ItemsCue];
                int item = 0;
                foreach (DataRow row in dsImprimir.Tables[1].Rows)
                {
                    FacturaDetalle producto1 = new FacturaDetalle
                    {
                        cantidadPorEmpaque = "1",
                        cantidadReal = "1.00",
                        cantidadRealUnidadMedida = "94",
                        cantidadUnidades = row["cantidad"].ToString().Trim(),
                        codigoProducto = row["cod_ref"].ToString().Trim(),
                        descripcion = row["nom_ref"].ToString().Trim(),
                        descripcionTecnica = row["nom_ref"].ToString().Trim(),
                        estandarCodigo = "999",
                        estandarCodigoProducto = row["cod_ref"].ToString().Trim(),
                        impuestosDetalles = new FacturaImpuestos[1],
                        cargosDescuentos = new CargosDescuentos[1]

                    };
                    FacturaImpuestos impuesto1 = new FacturaImpuestos
                    {
                        baseImponibleTOTALImp = Convert.ToDecimal(row["base"]).ToString(),
                        codigoTOTALImp = "01",
                        controlInterno = "",
                        porcentajeTOTALImp = Convert.ToDecimal(row["por_iva"]).ToString(),
                        unidadMedida = "94",
                        unidadMedidaTributo = "",
                        valorTOTALImp = Convert.ToDecimal(row["val_iva"]).ToString(),
                        valorTributoUnidad = ""
                    };
                    if (Convert.ToDecimal(row["val_des"]) > 0)
                    {
                        CargosDescuentos cargoDescto = new CargosDescuentos
                        {
                            codigo = "07",
                            monto = Convert.ToDecimal(row["val_des"]).ToString(),
                            montoBase = Convert.ToDecimal(row["val_uni"]).ToString(),
                            porcentaje = Convert.ToDecimal(row["por_des"]).ToString(),
                            indicador = "0",
                            secuencia = "1",
                            descripcion = "Descuento de temporada "
                        };
                        producto1.cargosDescuentos[0] = cargoDescto;
                    }
                    producto1.impuestosDetalles[0] = impuesto1;

                    producto1.impuestosTotales = new ImpuestosTotales[1];
                    ImpuestosTotales impuestoTOTAL1 = new ImpuestosTotales
                    {
                        codigoTOTALImp = "01",
                        montoTotal = Convert.ToDecimal(row["val_iva"]).ToString()
                    };
                    producto1.impuestosTotales[0] = impuestoTOTAL1;
                    producto1.marca = "HKA";
                    producto1.muestraGratis = "0";
                    producto1.precioTotal = Convert.ToDecimal(row["tot_tot"]).ToString();

                    producto1.precioTotalSinImpuestos = Convert.ToDecimal(row["base"]).ToString();
                    producto1.precioVentaUnitario = Convert.ToDecimal(row["val_uni"]).ToString();
                    producto1.secuencia = Convert.ToDecimal(row["secuencia"]).ToString();
                    producto1.unidadMedida = "94";
                    facturaDemo.detalleDeFactura[item] = producto1;
                    item++;
                }
                #endregion
                #region DocumentosReferenciados
                //               String Tipo_Documento = "";
                if (Tipo_Documento == "Nota Credito" || Tipo_Documento == "Nota Debito")
                {
                    facturaDemo.documentosReferenciados = new DocumentoReferenciado[2];

                    #region DiscrepansyResponse
                    DocumentoReferenciado DocumentoReferenciado1 = new DocumentoReferenciado
                    {
                        codigoEstatusDocumento = "2",
                        codigoInterno = "4",
                        cufeDocReferenciado = dsImprimir.Tables[6].Rows[0]["facufe"].ToString().Trim()
                    };

                    string[] descripcion = new string[1];
                    descripcion[0] = "Nota";
                    DocumentoReferenciado1.descripcion = descripcion;
                    DocumentoReferenciado1.numeroDocumento = dsImprimir.Tables[6].Rows[0]["numerfactu"].ToString().Trim();
                    #endregion
                    facturaDemo.documentosReferenciados[0] = DocumentoReferenciado1;

                    #region BillingReference
                    DocumentoReferenciado DocumentoReferenciado2 = new DocumentoReferenciado
                    {
                        codigoInterno = "5",
                        cufeDocReferenciado = dsImprimir.Tables[6].Rows[0]["facufe"].ToString().Trim(),
                        fecha = Convert.ToDateTime(dsImprimir.Tables[6].Rows[0]["fechafactu"].ToString().Trim()).ToString("yyyy-MM-dd"),
                        numeroDocumento = dsImprimir.Tables[6].Rows[0]["numerfactu"].ToString().Trim()
                    };
                    #endregion
                    facturaDemo.documentosReferenciados[1] = DocumentoReferenciado2;
                }
                #endregion
                #region impuestosGenerales

                facturaDemo.impuestosGenerales = new FacturaImpuestos[1];
                FacturaImpuestos impuestoGeneral1 = new FacturaImpuestos
                {
                    baseImponibleTOTALImp = dsImprimir.Tables[2].Rows[0]["base"].ToString().Trim(),
                    codigoTOTALImp = "01",
                    porcentajeTOTALImp = dsImprimir.Tables[2].Rows[0]["por_iva"].ToString().Trim(),
                    unidadMedida = "94",
                    valorTOTALImp = dsImprimir.Tables[2].Rows[0]["val_iva"].ToString().Trim()
                };

                if (Convert.ToDecimal(dsImprimir.Tables[2].Rows[0]["val_des"].ToString().Trim()) > 1)
                {
                    facturaDemo.cargosDescuentos = new CargosDescuentos[1];
                    CargosDescuentos cargoDescuentos = new CargosDescuentos
                    {
                        codigo = "07",

                        monto = Convert.ToDecimal(dsImprimir.Tables[2].Rows[0]["val_des"].ToString()).ToString(),
                        montoBase = Convert.ToDecimal(dsImprimir.Tables[2].Rows[0]["subtotal"].ToString().Trim()).ToString(),
                        //cargoDescuentos.porcentaje = Convert.ToDecimal(row["por_des"]).ToString();
                        indicador = "0",
                        secuencia = "1",
                        descripcion = "Descuento de temporada "
                    };

                    facturaDemo.cargosDescuentos[0] = cargoDescuentos;
                }

                facturaDemo.impuestosGenerales[0] = impuestoGeneral1;
                #endregion
                #region impuestosTotales
                facturaDemo.impuestosTotales = new ImpuestosTotales[1];
                ImpuestosTotales impuestoGeneralTOTAL1 = new ImpuestosTotales
                {
                    codigoTOTALImp = "01",
                    montoTotal = dsImprimir.Tables[2].Rows[0]["val_iva"].ToString().Trim()
                };
                facturaDemo.impuestosTotales[0] = impuestoGeneralTOTAL1;
                #endregion

                #region mediosDePago
                facturaDemo.mediosDePago = new MediosDePago[1];
                MediosDePago medioPago1 = new MediosDePago
                {
                    medioPago = "10",
                    metodoDePago = "2",
                    numeroDeReferencia = "01",
                    fechaDeVencimiento = Convert.ToDateTime(dsImprimir.Tables[0].Rows[0]["fec_ven"].ToString().Trim()).ToString("yyyy-MM-dd")
                };
                facturaDemo.mediosDePago[0] = medioPago1;
                #endregion
                facturaDemo.moneda = "COP";

                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "005")
                    facturaDemo.rangoNumeracion = dsImprimir.Tables[4].Rows[0]["rangonumeracion_"].ToString().Trim();
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "007")
                    facturaDemo.rangoNumeracion = dsImprimir.Tables[4].Rows[0]["rangonumeracionc_"].ToString().Trim();
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "008")
                    facturaDemo.rangoNumeracion = dsImprimir.Tables[4].Rows[0]["rangonumeracionc_"].ToString().Trim();

                facturaDemo.redondeoAplicado = "0.00";

                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "005")
                    facturaDemo.tipoDocumento = "01";
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "007")
                    facturaDemo.tipoDocumento = "91";
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "008")
                    facturaDemo.tipoDocumento = "91";

                int numitems = dsImprimir.Tables[1].Rows.Count;

                //facturaDemo.tipoOperacion = "05";
                facturaDemo.tipoOperacion = "10";
                facturaDemo.totalBaseImponible = dsImprimir.Tables[2].Rows[0]["base"].ToString().Trim();
                facturaDemo.totalBrutoConImpuesto = dsImprimir.Tables[2].Rows[0]["tot_tot"].ToString().Trim();
                facturaDemo.totalMonto = dsImprimir.Tables[2].Rows[0]["tot_tot"].ToString().Trim();
                facturaDemo.totalProductos = numitems.ToString();
                facturaDemo.totalSinImpuestos = dsImprimir.Tables[2].Rows[0]["base"].ToString().Trim();

                return facturaDemo;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "BuildFactrua1");
                MessageBox.Show(ex.StackTrace.ToString(), "BuildFactrua2");
                return null;

            }
        }
        //#endregion


        #region Enviar (Web Service SOAP Emisión)
        //        private void BtnEnviar_Click(object sender, EventArgs e)
        private void Enviando()
        {
            try
            {

                //MessageBox.Show("enviando");
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "005")
                    Tipo_Documento = "Factura";
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "007")
                    Tipo_Documento = "Nota Credito";
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "008")
                    Tipo_Documento = "Nota Credito";
                //if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "009")
                //  Tipo_Documento = "Nota Debito";

                FacturaGeneral factura = BuildFactura(); // Se invoca el metodo para construir el objeto factura

                if (factura == null)
                {
                    MessageBox.Show("Error en creacion de factura..", "BuildFactura");
                    return;
                }

                factura.fechaEmision = tbxFechaEmision.Text.Trim();
                //MessageBox.Show(Convert.ToDateTime(dsImprimir.Tables[0].Rows[0]["fec_ven"].ToString().Trim()).ToString("yyyy-MM-dd"));
                factura.fechaVencimiento = Convert.ToDateTime(dsImprimir.Tables[0].Rows[0]["fec_ven"].ToString().Trim()).ToString("yyyy-MM-dd");

                //               factura.fechaEmision = "2019-12-01 07:36:01";
                //           FacturaGeneral factura = BuildFactura(); // Se invoca el metodo para construir el objeto factura
                string ArchivoRequest = factura.consecutivoDocumento.Trim() + ".txt";
                StreamWriter MyFile = new StreamWriter(ArchivoRequest); //ruta y name del archivo request a almecenar
                XmlSerializer Serializer1 = new XmlSerializer(typeof(FacturaGeneral));
                Serializer1.Serialize(MyFile, factura); // Objeto serializado
                MyFile.Close();
                DocumentResponse docRespuesta; //objeto Response del metodo enviar
                rtxInformacion.Clear();
                this.Cursor = Cursors.Wait;
                rtxInformacion.Text = "Envio de Factura:" + Environment.NewLine;
                string cantidadAnexos = dsImprimir.Tables[4].Rows[0]["ad_junto"].ToString().Trim();
                cantidadAnexos = "1";
                string trnenviar = dsImprimir.Tables[0].Rows[0]["cod_trn"].ToString();
                if (trnenviar == "007" || trnenviar == "008") cantidadAnexos = "0";
                //                string cantidadAnexos = "0";

                if (cantidadAnexos == "0")
                {

                    //MessageBox.Show(tokenEmpresa);
                    //MessageBox.Show(tokenAuthorizacion);
                    if (MessageBox.Show("Confirmar envio ?", "Enviando documento", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        docRespuesta = serviceClienteEnvio.Enviar(tokenEmpresa, tokenAuthorizacion, factura, "0");
                        StringBuilder msgError = new StringBuilder();
                        if (docRespuesta.mensajesValidacion != null)
                        {
                            //MessageBox.Show(docRespuesta.mensajesValidacion.Count().ToString());
                            int nReturnMsg = docRespuesta.mensajesValidacion.Count();
                            for (int i = 0; i < nReturnMsg; i++)
                            {
                                //MessageBox.Show(i.ToString() + "-" + msgError.ToString());
                                msgError.Append(docRespuesta.mensajesValidacion[i].ToString() + Environment.NewLine);
                            }
                        }
                        if (docRespuesta.codigo == 114)  //documento emitdo previa mente
                        {
                            DocumentStatusResponse resp = serviceClienteEnvio.EstadoDocumento(tokenEmpresa, tokenAuthorizacion, factura.consecutivoDocumento.ToString());
                            if (resp.codigo == 200)
                            {
                                rtxInformacion.Text = "ReEnvio de Factura emitido previa mente:" + docRespuesta.codigo + Environment.NewLine;
                                ActualizaDocFacturaElectronicaRespuesta(resp);
                                rtxInformacion.Text += "Codigo: " + resp.codigo.ToString() + Environment.NewLine +
                               "Consecutivo Documento: " + resp.consecutivo + Environment.NewLine +
                               "Cufe: " + resp.cufe + Environment.NewLine +
                               "Mensaje: " + resp.mensaje + Environment.NewLine +
                               "Resultado: " + resp.resultado + Environment.NewLine + Environment.NewLine;

                                this.Cursor = Cursors.Arrow;
                                return;

                            }

                        }





                        //MessageBox.Show(docRespuesta.mensajesValidacion[0].ToString());
                        //MessageBox.Show(docRespuesta.mensajesValidacion[1].ToString());
                        //MessageBox.Show(docRespuesta.mensajesValidacion[0].ToString());
                        //MessageBox.Show(docRespuesta.mensajesValidacion[1].ToString());
                        //envio factura 

                        if (docRespuesta.codigo == 200 || docRespuesta.codigo == 201)
                        {
                            ActualizaDocFacturaElectronica(docRespuesta);
                            this.rtxInformacion.Text += "Codigo: " + docRespuesta.codigo.ToString() + Environment.NewLine +
                                 "Consecutivo Documento: " + docRespuesta.consecutivoDocumento + Environment.NewLine +
                                 "Cufe: " + docRespuesta.cufe + Environment.NewLine +
                                 "Mensaje: " + docRespuesta.mensaje + Environment.NewLine +
                                 "Resultado: " + docRespuesta.resultado + Environment.NewLine;
                            //this.Close();
                        }
                        else
                        {
                            //ActualizaDocFacturaElectronica(docRespuesta);
                            this.rtxInformacion.Text += "Codigo: " + docRespuesta.codigo.ToString() + Environment.NewLine +
                                "Mensaje: " + docRespuesta.mensaje + Environment.NewLine +
                                "Resultado: " + docRespuesta.resultado + Environment.NewLine +
                                "Errores  :" + msgError.ToString();
                        }
                    }
                    else
                    {
                        rtxInformacion.Text = "Proceso cancelado";
                    }
                }
                else
                {
                    factura.cliente.notificar = "SI";

                    docRespuesta = serviceClienteEnvio.Enviar(tokenEmpresa, tokenAuthorizacion, factura, "11");
                    //envio factura 
                    StringBuilder msgError = new StringBuilder();
                    if (docRespuesta.mensajesValidacion != null)
                    {
                        //MessageBox.Show(docRespuesta.mensajesValidacion.Count().ToString());
                        int nReturnMsg = docRespuesta.mensajesValidacion.Count();
                        for (int i = 0; i < nReturnMsg; i++)
                        {
                            //MessageBox.Show(i.ToString() + "-" + msgError.ToString());
                            msgError.Append(docRespuesta.mensajesValidacion[i].ToString() + Environment.NewLine);
                        }
                    }
                    if (docRespuesta.codigo == 114)  //documento emitdo previa mente
                    {
                        DocumentStatusResponse resp = serviceClienteEnvio.EstadoDocumento(tokenEmpresa, tokenAuthorizacion, factura.consecutivoDocumento.ToString());
                        if (resp.codigo == 200)
                        {
                            rtxInformacion.Text = "ReEnvio de Factura emitido previa mente:" + docRespuesta.codigo + Environment.NewLine;
                            ActualizaDocFacturaElectronicaRespuesta(resp);
                            rtxInformacion.Text += "Codigo: " + resp.codigo.ToString() + Environment.NewLine +
                           "Consecutivo Documento: " + resp.consecutivo + Environment.NewLine +
                           "Cufe: " + resp.cufe + Environment.NewLine +
                           "Mensaje: " + resp.mensaje + Environment.NewLine +
                           "Resultado: " + resp.resultado + Environment.NewLine + Environment.NewLine;

                            int resultado = EnviarArchivosAdjuntosRespuesta(cantidadAnexos_, resp);
                            if (resultado > 0)
                            {
                                rtxInformacion.Text += resultado.ToString() + "PROCESO EXITOSO: Archivos adjuntos procesados correctamente!!!";
                            }
                            else
                            {
                                rtxInformacion.Text += Environment.NewLine + "ERROR: procesando archivos adjuntos!!!";
                            }

                            this.Cursor = Cursors.Arrow;
                            return;

                        }

                    }
                    if (docRespuesta.codigo == 200 || docRespuesta.codigo == 201)
                    {
                        rtxInformacion.Text += "Codigo: " + docRespuesta.codigo.ToString() + Environment.NewLine +
                                               "Consecutivo Documento: " + docRespuesta.consecutivoDocumento + Environment.NewLine +
                                               "Cufe: " + docRespuesta.cufe + Environment.NewLine +
                                               "Mensaje: " + docRespuesta.mensaje + Environment.NewLine +
                                               "Resultado: " + docRespuesta.resultado + Environment.NewLine + Environment.NewLine;

                        rtxInformacion.Text += "--------------------------------------------------" + Environment.NewLine;
                        rtxInformacion.Text += "Envio de adjuntos:" + Environment.NewLine;
                        ActualizaDocFacturaElectronica(docRespuesta);
                        int resultado = EnviarArchivosAdjuntos(cantidadAnexos_, docRespuesta);
                        if (resultado > 0)
                        {
                            rtxInformacion.Text += resultado.ToString() + "PROCESO EXITOSO: Archivos adjuntos procesados correctamente!!!";
                        }
                        else
                        {
                            rtxInformacion.Text += Environment.NewLine + "ERROR: procesando archivos adjuntos!!!";
                        }
                    }
                    else
                    {
                        rtxInformacion.Text += docRespuesta.codigo.ToString() + Environment.NewLine + docRespuesta.mensaje + Environment.NewLine + docRespuesta.resultado;
                        //ActualizaDocFacturaElectronica(docRespuesta);
                        this.rtxInformacion.Text += "Codigo: " + docRespuesta.codigo.ToString() + Environment.NewLine +
                            "Mensaje: " + docRespuesta.mensaje + Environment.NewLine +
                            "Resultado: " + docRespuesta.resultado + Environment.NewLine +
                            "Errores  :" + msgError.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //+Environment.NewLine + ex.StackTrace
                this.Cursor = Cursors.Arrow;
            }
            this.Cursor = Cursors.Arrow;
        }
        #endregion

        #region CargarAdjuntos (Web Service SOAP Adjuntos)


        private int EnviarArchivosAdjuntos(int numeroDeArchivos, DocumentResponse docInfo)
        {

            if (numeroDeArchivos <= 0) return 0;
            SiaWin.Func.ImprimeLoteFacturas(dsImprimir.Tables[0], codpvt, codpvt, false, idemp, true);
            //SiaWin.Func.ImprimeFacturaCredito(idrowcab, codpvt,codpvt, false, totalFac, tituloPie, idEmp, false);
            string filepdf = docInfo.consecutivoDocumento.Trim() + ".PDF";
            int procesados = 0;
            for (int i = 0; i < numeroDeArchivos; i++)
            {
                // archivo a trasnmitir es el archivo pdf creado 
                FileInfo file = new FileInfo(filepdf);
                if (file.Exists)
                {
                    //MessageBox.Show("eXISTE");
                    BinaryReader bReader = new BinaryReader(file.OpenRead());
                    byte[] anexByte = bReader.ReadBytes((int)file.Length);
                    //anexB64 = Convert.ToBase64String(anexByte);
                    CargarAdjuntos uploadAttachment = new CargarAdjuntos
                    {
                        archivo = anexByte,
                        numeroDocumento = docInfo.consecutivoDocumento
                    };
                    string[] correoEntrega = new string[1];
                    correoEntrega[0] = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim(); ;
                    uploadAttachment.email = correoEntrega;
                    //MessageBox.Show(file.Name.Substring(0, file.Name.Length - 4).ToString());
                    uploadAttachment.nombre = file.Name.Substring(0, file.Name.Length - 4);

                    //uploadAttachment.formato = file.Extension.Substring(1);
                    uploadAttachment.formato = "pdf";
                    uploadAttachment.numeroDocumento = docInfo.consecutivoDocumento.Trim();
                    //MessageBox.Show(uploadAttachment.formato.ToString());
                    //string ArchivoRequest = docInfo.consecutivoDocumento.Trim() + "Adjunto.txt";
                    //StreamWriter MyFile = new StreamWriter(ArchivoRequest); //ruta y name del archivo request a almecenar
                    //XmlSerializer Serializer1 = new XmlSerializer(typeof(CargarAdjuntos));
                    //Serializer1.Serialize(MyFile, uploadAttachment); // Objeto serializado
                    //MyFile.Close();


                    uploadAttachment.tipo = "2";
                    uploadAttachment.enviar = "1";
                    //                    if (i + 1 == 1)
                    //                  {
                    //                    uploadAttachment.enviar = "1";
                    //              }
                    //            else
                    //          {
                    //            uploadAttachment.enviar = "0";
                    //      }
                    //    uploadAttachment.enviar = "1";


                    StringBuilder msgError = new StringBuilder();
                    UploadAttachmentResponse fileRespuesta = serviceClientAdjunto.CargarAdjuntos(tokenEmpresa, tokenAuthorizacion, uploadAttachment);
                    if (fileRespuesta.mensajesValidacion != null)
                    {
                        //MessageBox.Show(docRespuesta.mensajesValidacion.Count().ToString());
                        int nReturnMsg = fileRespuesta.mensajesValidacion.Count();
                        for (int ii = 0; ii < nReturnMsg; ii++)
                        {
                            //MessageBox.Show(i.ToString() + "-" + msgError.ToString());
                            msgError.Append(fileRespuesta.mensajesValidacion[ii].ToString() + Environment.NewLine);
                        }
                    }

                    if (fileRespuesta.codigo == 200 || fileRespuesta.codigo == 201)
                    {
                        rtxInformacion.Text += "Archivo: " + file.Name + " procesado correctamente" + Environment.NewLine + msgError.ToString(); ;
                        procesados++;
                    }
                    else
                    {

                        rtxInformacion.Text += "Archivo: " + file.Name + " - no fue transmitido" + Environment.NewLine + fileRespuesta.mensaje.ToString();
                    }
                }
                else
                {
                    rtxInformacion.Text += Environment.NewLine + "ERROR: procesando archivos adjuntos!!!";
                    //no debería entrar a este ciclo
                }

            }
            return procesados;
        }
        private int EnviarArchivosAdjuntosRespuesta(int numeroDeArchivos, DocumentStatusResponse docInfo)
        {

            if (numeroDeArchivos <= 0) return 0;
            SiaWin.Func.ImprimeLoteFacturas(dsImprimir.Tables[0], codpvt, codpvt, false, idemp, true);
            //SiaWin.Func.ImprimeFacturaCredito(idrowcab, codpvt,codpvt, false, totalFac, tituloPie, idEmp, false);
            string filepdf = docInfo.consecutivo.Trim() + ".PDF";
            int procesados = 0;
            for (int i = 0; i < numeroDeArchivos; i++)
            {
                // archivo a trasnmitir es el archivo pdf creado 
                FileInfo file = new FileInfo(filepdf);
                if (file.Exists)
                {
                    //MessageBox.Show("eXISTE");
                    BinaryReader bReader = new BinaryReader(file.OpenRead());
                    byte[] anexByte = bReader.ReadBytes((int)file.Length);
                    //anexB64 = Convert.ToBase64String(anexByte);
                    CargarAdjuntos uploadAttachment = new CargarAdjuntos
                    {
                        archivo = anexByte,
                        numeroDocumento = docInfo.consecutivo
                    };
                    string[] correoEntrega = new string[1];
                    correoEntrega[0] = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim(); ;
                    uploadAttachment.email = correoEntrega;
                    //MessageBox.Show(file.Name.Substring(0, file.Name.Length - 4).ToString());
                    uploadAttachment.nombre = file.Name.Substring(0, file.Name.Length - 4);

                    //uploadAttachment.formato = file.Extension.Substring(1);
                    uploadAttachment.formato = "pdf";
                    uploadAttachment.numeroDocumento = docInfo.consecutivo.Trim();
                    //MessageBox.Show(uploadAttachment.formato.ToString());
                    //string ArchivoRequest = docInfo.consecutivo.Trim() + "Adjunto.txt";
                    //StreamWriter MyFile = new StreamWriter(ArchivoRequest); //ruta y name del archivo request a almecenar
                    //XmlSerializer Serializer1 = new XmlSerializer(typeof(CargarAdjuntos));
                    //Serializer1.Serialize(MyFile, uploadAttachment); // Objeto serializado
                    //MyFile.Close();


                    uploadAttachment.tipo = "2";
                    uploadAttachment.enviar = "1";
                    //                    if (i + 1 == 1)
                    //                  {
                    //                    uploadAttachment.enviar = "1";
                    //              }
                    //            else
                    //          {
                    //            uploadAttachment.enviar = "0";
                    //      }
                    //    uploadAttachment.enviar = "1";


                    StringBuilder msgError = new StringBuilder();
                    UploadAttachmentResponse fileRespuesta = serviceClientAdjunto.CargarAdjuntos(tokenEmpresa, tokenAuthorizacion, uploadAttachment);
                    if (fileRespuesta.mensajesValidacion != null)
                    {
                        //MessageBox.Show(docRespuesta.mensajesValidacion.Count().ToString());
                        int nReturnMsg = fileRespuesta.mensajesValidacion.Count();
                        for (int ii = 0; ii < nReturnMsg; ii++)
                        {
                            //MessageBox.Show(i.ToString() + "-" + msgError.ToString());
                            msgError.Append(fileRespuesta.mensajesValidacion[ii].ToString() + Environment.NewLine);
                        }
                    }

                    if (fileRespuesta.codigo == 200 || fileRespuesta.codigo == 201)
                    {
                        rtxInformacion.Text += "Archivo: " + file.Name + " procesado correctamente" + Environment.NewLine + msgError.ToString(); ;
                        procesados++;
                    }
                    else
                    {

                        rtxInformacion.Text += "Archivo: " + file.Name + " - no fue transmitido" + Environment.NewLine + fileRespuesta.mensaje.ToString();
                    }
                }
                else
                {
                    rtxInformacion.Text += Environment.NewLine + "ERROR: procesando archivos adjuntos!!!";
                    //no debería entrar a este ciclo
                }

            }
            return procesados;
        }

        #endregion 


        public bool LoadData(int idregdoc, string codpvta, string cn)
        {
            try
            {
                // retorna tablas 0 = cabeza factura y datos del cliente
                // 1 = cuerpo de factura y tarifas de iva
                // 2 = totales de factura factura y tarifas de iva
                // 3 = formas de pago
                // 4 = informacion del punto de venta
                // 5 = informacion config

                SqlConnection con = new SqlConnection(cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                //DataSet dsImprimir = new DataSet();
                //PvFacturaElectronicaAnulacion
                cmd = new SqlCommand("_EmpPvFacturaElectronica", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@NumRegCab", idrowcab);//if you have parameters.
                cmd.Parameters.AddWithValue("@CodPvt", codpvt);//if you have parameters.
                cmd.Parameters.AddWithValue("@codemp", cod_empresa);//if you have parameters.

                da = new SqlDataAdapter(cmd);
                dsImprimir.Clear();
                da.Fill(dsImprimir);
                tokenEmpresa = dsImprimir.Tables[5].Rows[0]["stockenemp_"].ToString().Trim();
                tokenAuthorizacion = dsImprimir.Tables[5].Rows[0]["stockenpas_"].ToString().Trim();


                if (string.IsNullOrEmpty(tokenEmpresa))
                {
                    System.Windows.MessageBox.Show("Token de empresa null o vacio");
                    return false;
                }
                if (string.IsNullOrEmpty(tokenAuthorizacion))
                {
                    System.Windows.MessageBox.Show("Token autorizacion  de empresa null o vacio");
                    return false;
                }

                int nItems = dsImprimir.Tables[0].Rows.Count;
                if (nItems <= 0)
                {
                    System.Windows.MessageBox.Show("No hay registro en cabeza de documento..");
                    return false;
                }
                nItems = dsImprimir.Tables[1].Rows.Count;
                if (nItems <= 0)
                {
                    System.Windows.MessageBox.Show("No hay registro en cuerpo de documento..");
                    return false;
                }
                if ((string)dsImprimir.Tables[0].Rows[0]["cod_trn"] == "005x")
                {
                    nItems = dsImprimir.Tables[3].Rows.Count;
                    if (nItems <= 0)
                    {
                        System.Windows.MessageBox.Show("No hay registro en formas de pago en documento..");
                        return false;
                    }
                }
                nItems = dsImprimir.Tables[4].Rows.Count;
                if (nItems <= 0)
                {
                    System.Windows.MessageBox.Show("No hay registro informacion punto de venta...");
                    return false;
                }
                if (nItems <= 0)
                {
                    System.Windows.MessageBox.Show("No hay registro informacion Config...");
                    return false;
                }
                this.tbxnit.Text = dsImprimir.Tables[0].Rows[0]["cod_clisin"].ToString().Trim();
                this.tbxnombre.Text = dsImprimir.Tables[0].Rows[0]["nom_ter"].ToString().Trim();
                this.tbxEmail.Text = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim().ToUpper(); ;
                this.tbxFechaEmision.Text = Convert.ToDateTime(dsImprimir.Tables[0].Rows[0]["fec_trn"].ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");
                this.txtNumFactura.Text = dsImprimir.Tables[0].Rows[0]["num_trn"].ToString().Trim();

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "LoadData");
            }
            return false;

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
                this.Title = "Facturacion electronica SiaApp" + cod_empresa + "-" + nomempresa;

                string query = "select * from incab_doc where idreg='" + idrowcab + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "transacciones", idemp);
                if (dt.Rows.Count > 0)
                {
                    string cod_trn = dt.Rows[0]["ano_doc"].ToString();
                    string num_anu = dt.Rows[0]["num_anu"].ToString();
                    if (cod_trn == "005")
                    {
                        int año = Convert.ToInt32(dt.Rows[0]["ano_doc"]);
                        if (año <= 2019)
                        {
                            BtnEnviar.Visibility = Visibility.Collapsed;
                            BtnImprimir.Content = "IMPRIMIR FACTURA ANTIGUA";
                            BtnImprimir.Width = 200;
                        }
                    }
                    else
                    {
                        string query_nota = "select * from incab_doc where num_trn='" + num_anu + "' and cod_trn='005';";
                        DataTable dt_nota = SiaWin.Func.SqlDT(query_nota, "transacciones", idemp);
                        if (dt_nota.Rows.Count > 0)
                        {
                            int año = Convert.ToInt32(dt_nota.Rows[0]["ano_doc"]);
                            if (año <= 2019)
                            {
                                BtnEnviar.Visibility = Visibility.Collapsed;
                                BtnImprimir.Content = "IMPRIMIR NC FACTURA ANTIGUA";
                                BtnImprimir.Width = 200;
                            }
                        }
                    }
                }


                //MessageBox.Show(SiaWin._cn);
                //LoadData(idrowcab,codpvt, SiaWin._cn);
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }
        private void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                port = null;
                serviceClienteEnvio = null;
                port = new BasicHttpBinding();
                //SrvAjunto.ServiceClient serviceClientAdjunto = new SrvAjunto.ServiceClient();
                serviceClienteEnvio = new SrvEnvio.ServiceClient();

                //FacturaGeneral factura = BuildFactura(); // Se invoca el metodo para construir el objeto factura
                //if(factura==null)
                //{
                //  MessageBox.Show("error en BuildFactura, retorno null");
                //}
                if (Validacion())
                    Enviando();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                // MessageBox.Show(ex.StackTrace.ToString());

            }
        }
        private bool Validacion()
        {
            try
            {

                ///validar datos del cliente

                ///
                ///validar valores de factura
                ///

                ///// fddfsjsdfjsdsdjjssd
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);

            }
            return false;

        }

        public void ActualizaDocFacturaElectronica(DocumentResponse resp)
        {
            string numdocele = resp.consecutivoDocumento;
            string cufe = resp.cufe.Trim();
            string fecharesp = resp.fechaRespuesta.ToString();
            string msg = resp.mensaje;
            string code = resp.codigo.ToString();
            DateTime dtime = DateTime.Now;

            if (!string.IsNullOrEmpty(fecharesp))
            {
                dtime = Convert.ToDateTime(fecharesp);
            }
            /// envia a base de datos en cabeza de documento
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                connection.Open();
                StringBuilder errorMessages = new StringBuilder();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = connection.BeginTransaction("Transaction");
                command.Connection = connection;
                command.Transaction = transaction;
                try
                {

                    string sqlcab = string.Empty;
                    if (!string.IsNullOrEmpty(fecharesp))
                    {
                        sqlcab = @"update incab_doc set fa_docelect='" + numdocele.Trim() + "',fa_cufe='" + cufe + "',fa_msg='" + msg + "',fa_fecharesp='" + dtime.ToString() + "',fa_codigo='" + code + "' where idreg=" + idrowcab.ToString();
                    }
                    else
                    {
                        sqlcab = @"update incab_doc set fa_docelect='" + numdocele.Trim() + "',fa_cufe ='" + cufe + "',fa_msg='" + msg + "',fa_codigo='" + code + "' where idreg=" + idrowcab.ToString();
                    }
                    command.CommandText = sqlcab;
                    command.ExecuteScalar();
                    transaction.Commit();
                    this.Cufe = cufe;
                    this.Codigo = code;

                    connection.Close();

                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());

                }
                catch (Exception ex)
                {
                    errorMessages.Append("c Error:#" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                }

            }
        }
        public void ActualizaDocFacturaElectronicaRespuesta(DocumentStatusResponse resp)
        {
            string numdocele = resp.consecutivo;
            string cufe = resp.cufe.Trim();
            string fecharesp = resp.fechaDocumento.ToString();
            string msg = resp.mensaje;
            string code = resp.codigo.ToString();
            DateTime dtime = DateTime.Now;

            if (!string.IsNullOrEmpty(fecharesp))
            {
                dtime = Convert.ToDateTime(fecharesp);
            }
            /// envia a base de datos en cabeza de documento
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                connection.Open();
                StringBuilder errorMessages = new StringBuilder();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = connection.BeginTransaction("Transaction");
                command.Connection = connection;
                command.Transaction = transaction;
                try
                {

                    string sqlcab = string.Empty;
                    if (!string.IsNullOrEmpty(fecharesp))
                    {
                        sqlcab = @"update incab_doc set fa_docelect='" + numdocele.Trim() + "',fa_cufe='" + cufe + "',fa_msg='" + msg + "',fa_fecharesp='" + dtime.ToString() + "',fa_codigo='" + code + "' where idreg=" + idrowcab.ToString();
                    }
                    else
                    {
                        sqlcab = @"update incab_doc set fa_docelect='" + numdocele.Trim() + "',fa_cufe ='" + cufe + "',fa_msg='" + msg + "',fa_codigo='" + code + "' where idreg=" + idrowcab.ToString();
                    }
                    command.CommandText = sqlcab;
                    command.ExecuteScalar();
                    transaction.Commit();
                    this.Cufe = cufe;
                    this.Codigo = code;

                    connection.Close();

                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());

                }
                catch (Exception ex)
                {
                    errorMessages.Append("c Error:#" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                }

            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            LoadConfig();
            //MessageBox.Show(SiaWin._cn);
            if (!LoadData(idrowcab, codpvt, SiaWin._cn))
            {
                MessageBox.Show("Error al cargar los datos del documento....");
                this.Close();
                return;
            }

        }
        private Boolean ValidaEmail(String email)
        {
            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, expresion))
            {
                if (Regex.Replace(email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            Cufe = "A";
            Codigo = "200";
            this.Close();
        }







    }
}
