using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using System.IO;
using System.Data.SqlClient;
using System.Xml.Serialization;
using SiasoftAppExt;
using System.Windows;
using TrnFacturaElectronica.ServiceReference1;
using TrnFacturaElectronica.ServiceReference2;

namespace SiasoftAppExt
{
    public partial class TrnFacturaElectronica1 : Window
    {

        TrnFacturaElectronica1.ServiceReference1.ServiceClient servicioemisor;

        public TrnFacturaElectronica1()
        {
            InitializeComponent();
            
            serviceClient = new TrnFacturaElectronica1.ServiceEmision.ServiceClient();
            serviceArchivos = new ServiceAdjuntos.ServiceClient();
        }
        #endregion

        #region Construcción del Objeto Factura
        private FacturaGeneral BuildFactura()
        {
            //armo el objeto factura
            FacturaGeneral facturaSiasoft = new FacturaGeneral();

            #region anticipos
            facturaSiasoft.anticipos = null;
            #endregion

            #region autorizado
            facturaSiasoft.autorizado = null;
            #endregion

            facturaSiasoft.cantidadDecimales = "2";

            #region cargosDescuentos
            facturaSiasoft.cargosDescuentos = null;
            #endregion

            #region cliente
            Cliente cliente = new Cliente();

            cliente.actividadEconomicaCIIU = "0010";
            cliente.apellido = null;

            cliente.destinatario = new Destinatario[1];
            Destinatario destinatario1 = new Destinatario();
            destinatario1.canalDeEntrega = "0";
            string[] correoEntrega = new string[1];
            correoEntrega[0] = "wilmer1104@yahoo.com";
            destinatario1.email = correoEntrega;
            destinatario1.fechaProgramada = "2019-06-01 00:00:00";
            destinatario1.mensajePersonalizado = null;
            destinatario1.nitProveedorReceptor = "1";
            destinatario1.telefono = "123456789";
            cliente.destinatario[0] = destinatario1;

            cliente.detallesTributarios = new Tributos[1];
            Tributos tributos1 = new Tributos();
            tributos1.codigoImpuesto = "01";
            tributos1.extras = null;
            cliente.detallesTributarios[0] = tributos1;
            //cliente.detallesTributarios = null;

            Direccion direccionFiscal = new Direccion();
            direccionFiscal.aCuidadoDe = null;
            direccionFiscal.aLaAtencionDe = null;
            direccionFiscal.bloque = null;
            direccionFiscal.buzon = null;
            direccionFiscal.calle = null;
            direccionFiscal.calleAdicional = null;
            direccionFiscal.ciudad = "MANIZALES";
            direccionFiscal.codigoDepartamento = "11";
            direccionFiscal.correccionHusoHorario = null;
            direccionFiscal.departamento = "Bogotá";
            direccionFiscal.departamentoOrg = null;
            direccionFiscal.direccion = "Direccion";
            direccionFiscal.distrito = null;
            direccionFiscal.habitacion = null;
            direccionFiscal.lenguaje = "es";
            direccionFiscal.localizacion = null;
            direccionFiscal.municipio = "11001";
            direccionFiscal.nombreEdificio = null;
            direccionFiscal.numeroEdificio = null;
            direccionFiscal.numeroParcela = null;
            direccionFiscal.pais = "CO";
            direccionFiscal.piso = null;
            direccionFiscal.region = null;
            direccionFiscal.subDivision = null;
            direccionFiscal.ubicacion = null;
            direccionFiscal.zonaPostal = "110211";
            cliente.direccionFiscal = direccionFiscal;
            //cliente.direccionFiscal = null;

            cliente.email = "wilmer1104@yahoo.com";
            cliente.extras = null;

            InformacionLegal informacionLegalCliente = new InformacionLegal();
            informacionLegalCliente.codigoEstablecimiento = "00001";
            informacionLegalCliente.nombreRegistroRUT = "PRUEBA SIASOFT .NET";
            informacionLegalCliente.numeroIdentificacion = "901041710";
            informacionLegalCliente.numeroIdentificacionDV = "5";
            informacionLegalCliente.numeroMatriculaMercantil = null;
            informacionLegalCliente.prefijoFacturacion = null;
            informacionLegalCliente.tipoIdentificacion = "31";
            cliente.informacionLegalCliente = informacionLegalCliente;

            cliente.nombreComercial = null;
            cliente.nombreContacto = null;
            cliente.nombreRazonSocial = "SIASOFT SAS";
            cliente.nota = null;
            cliente.notificar = "SI";
            cliente.numeroDocumento = "901041710";
            cliente.numeroIdentificacionDV = "5";

            cliente.responsabilidadesRut = new Obligaciones[1];
            Obligaciones obligaciones1 = new Obligaciones();
            obligaciones1.obligaciones = "O-13";
            obligaciones1.regimen = "04";
            obligaciones1.extras = null;
            cliente.responsabilidadesRut[0] = obligaciones1;

            cliente.segundoNombre = null;
            cliente.telefax = null;
            cliente.telefono = null;
            cliente.tipoIdentificacion = "31";
            cliente.tipoPersona = "1";

            facturaSiasoft.cliente = cliente;
            #endregion 

            #region condicionPago
            facturaSiasoft.condicionPago = null;
            #endregion

            facturaSiasoft.consecutivoDocumento = this.tbxConsecutivo.Text.Trim();

            #region detalleDeFactura

            facturaSiasoft.detalleDeFactura = new FacturaDetalle[1];
            FacturaDetalle producto1 = new FacturaDetalle();
            producto1.cantidadPorEmpaque = "1";
            producto1.cantidadReal = "1.00";
            producto1.cantidadRealUnidadMedida = "KGM";
            producto1.cantidadUnidades = "1.00";
            producto1.cargosDescuentos = null;
            producto1.codigoFabricante = null;
            producto1.codigoIdentificadorPais = null;
            producto1.codigoProducto = "P000001";
            producto1.codigoTipoPrecio = null;
            producto1.descripcion = "Impresora HKA80";
            producto1.descripcionTecnica = "Impresora térmica de punto de venta, ideal para puntos de venta con alto rendimiento";
            producto1.documentosReferenciados = null;
            producto1.estandarCodigo = "999";
            producto1.estandarCodigoID = null;
            producto1.estandarCodigoIdentificador = null;
            producto1.estandarCodigoNombre = null;
            producto1.estandarCodigoProducto = "HKA80";
            producto1.estandarOrganizacion = null;
            producto1.estandarSubCodigoProducto = null;

            producto1.impuestosDetalles = new FacturaImpuestos[1];
            FacturaImpuestos impuesto1 = new FacturaImpuestos();
            impuesto1.baseImponibleTOTALImp = "1003.00";
            impuesto1.codigoTOTALImp = "01";
            impuesto1.controlInterno = "";
            impuesto1.porcentajeTOTALImp = "19.00";
            impuesto1.unidadMedida = "";
            impuesto1.unidadMedidaTributo = "";
            impuesto1.valorTOTALImp = "190.57";
            impuesto1.valorTributoUnidad = "";
            producto1.impuestosDetalles[0] = impuesto1;

            producto1.impuestosTotales = new ImpuestosTotales[1];
            ImpuestosTotales impuestoTOTAL1 = new ImpuestosTotales();
            impuestoTOTAL1.codigoTOTALImp = "01";
            impuestoTOTAL1.montoTotal = "190.57";
            producto1.impuestosTotales[0] = impuestoTOTAL1;

            producto1.informacionAdicional = null;
            producto1.mandatorioNumeroIdentificacion = null;
            producto1.mandatorioNumeroIdentificacionDV = null;
            producto1.mandatorioTipoIdentificacion = null;
            producto1.marca = "HKA";
            producto1.modelo = null;
            producto1.muestraGratis = "0";
            producto1.nombreFabricante = null;
            producto1.nota = null;
            producto1.precioReferencia = null;
            producto1.precioTotal = "90.00";
            producto1.precioTotalSinImpuestos = "1003.00";
            producto1.precioVentaUnitario = "1003.00";
            producto1.secuencia = "1";
            producto1.seriales = null;
            producto1.subCodigoFabricante = null;
            producto1.subCodigoProducto = null;
            producto1.tipoAIU = null;
            producto1.unidadMedida = "KGM";
            facturaSiasoft.detalleDeFactura[0] = producto1;
            #endregion

            #region documentosReferenciados
            facturaSiasoft.documentosReferenciados = null;
            #endregion

            #region entregaMercancia
            facturaSiasoft.entregaMercancia = null;
            #endregion

            #region extras
            facturaSiasoft.extras = null;
            #endregion

            facturaSiasoft.fechaEmision = DateTime.Now.ToString("yyyy-MM-dd 00:00:00"); ;
            facturaSiasoft.fechaFinPeriodoFacturacion = null;
            facturaSiasoft.fechaInicioPeriodoFacturacion = null;
            facturaSiasoft.fechaPagoImpuestos = null;
            facturaSiasoft.fechaVencimiento = null;

            #region impuestosGenerales
            facturaSiasoft.impuestosGenerales = new FacturaImpuestos[1];
            FacturaImpuestos impuestoGeneral1 = new FacturaImpuestos();
            impuestoGeneral1.baseImponibleTOTALImp = "1003.00";
            impuestoGeneral1.codigoTOTALImp = "01";
            impuestoGeneral1.controlInterno = null;
            impuestoGeneral1.porcentajeTOTALImp = "19.00";
            impuestoGeneral1.unidadMedida = "WSD";
            impuestoGeneral1.unidadMedidaTributo = null;
            impuestoGeneral1.valorTOTALImp = "190.57";
            impuestoGeneral1.valorTributoUnidad = null;
            facturaSiasoft.impuestosGenerales[0] = impuestoGeneral1;
            #endregion

            #region impuestosTotales
            facturaSiasoft.impuestosTotales = new ImpuestosTotales[1];
            ImpuestosTotales impuestoGeneralTOTAL1 = new ImpuestosTotales();
            impuestoGeneralTOTAL1.codigoTOTALImp = "01";
            impuestoGeneralTOTAL1.montoTotal = "190.57";
            facturaSiasoft.impuestosTotales[0] = impuestoGeneralTOTAL1;
            #endregion

            #region informacionAdicional
            facturaSiasoft.informacionAdicional = null;
            #endregion

            #region mediosDePago
            facturaSiasoft.mediosDePago = new MediosDePago[1];
            MediosDePago medioPago1 = new MediosDePago();
            medioPago1.codigoBanco = null;
            medioPago1.codigoCanalPago = null;
            medioPago1.codigoReferencia = null;
            medioPago1.extras = null;
            medioPago1.fechaDeVencimiento = null;
            medioPago1.medioPago = "10";
            medioPago1.metodoDePago = "1";
            medioPago1.nombreBanco = null;
            medioPago1.numeroDeReferencia = "01";
            medioPago1.numeroDias = null;
            medioPago1.numeroTransferencia = null;
            facturaSiasoft.mediosDePago[0] = medioPago1;
            #endregion

            facturaSiasoft.moneda = "COP";

            #region ordenDeCompra
            facturaSiasoft.ordenDeCompra = null;
            #endregion

            facturaSiasoft.propina = null;
// consecutivo
            facturaSiasoft.rangoNumeracion = "SIAS50";  
            facturaSiasoft.redondeoAplicado = "0.00";

            #region tasaDeCambio
            facturaSiasoft.tasaDeCambio = null;
            #endregion

            #region tasaDeCambioAlternativa
            facturaSiasoft.tasaDeCambioAlternativa = null;
            #endregion

            #region terminosEntrega
            facturaSiasoft.terminosEntrega = null;
            #endregion

            facturaSiasoft.tipoDocumento = "01";
            facturaSiasoft.tipoOperacion = "05";
            facturaSiasoft.totalAnticipos = null;
            facturaSiasoft.totalBaseImponible = "1003.00";
            facturaSiasoft.totalBrutoConImpuesto = "1193.57";
            facturaSiasoft.totalCargosAplicados = null;
            facturaSiasoft.totalDescuentos = null;
            facturaSiasoft.totalMonto = "1193.57";
            facturaSiasoft.totalProductos = "1";
            facturaSiasoft.totalSinImpuestos = "1003.00";

            return facturaSiasoft;
        }
        #endregion

        #region Enviar (Web Service SOAP Emisión)
        private void btnEnviar_Click(object sender, EventArgs e)
        {
            FacturaGeneral factura = BuildFactura(); // Se invoca el metodo para construir el objeto factura

            StreamWriter MyFile = new StreamWriter(@"Request_factura.txt"); //ruta y name del archivo request a almecenar
            XmlSerializer Serializer1 = new XmlSerializer(typeof(FacturaGeneral));
            Serializer1.Serialize(MyFile, factura); // Objeto serializado
            MyFile.Close();

            DocumentResponse docRespuesta; //objeto Response del metodo enviar
 //           rtxInformacion.Clear();
 //           this.Cursor = Cursors.WaitCursor;
            rtxInformacion.Text = "Envio de Factura:" + Environment.NewLine;
            int cantidadAnexos = 0;

            if (cantidadAnexos < 1)
            {
                DialogResult dRes = System.Windows.Forms.MessageBox.Show("No hay anexos para enviar como adjuntos, ¿Desea continuar?", "No se encontraron archivos anexos", MessageBoxButtons.YesNo);
                if (dRes == System.Windows.Forms.DialogResult.Yes)
                {
                    docRespuesta = serviceClient.Enviar(tbxTokenEmpresa.Text.Trim(), tbxTokenPassword.Text.Trim(), factura, "0");
                    //envio factura 

                    if (docRespuesta.codigo == 200)
                    {
                        rtxInformacion.Text += "Codigo: " + docRespuesta.codigo.ToString() + Environment.NewLine +
                                               "Consecutivo Documento: " + docRespuesta.consecutivoDocumento + Environment.NewLine +
                                               "Cufe: " + docRespuesta.cufe + Environment.NewLine +
                                               "Mensaje: " + docRespuesta.mensaje + Environment.NewLine +
                                               "Resultado: " + docRespuesta.resultado;
                    }
                    else
                    {
                        rtxInformacion.Text += "Codigo: " + docRespuesta.codigo.ToString() + Environment.NewLine +
                                               "Mensaje: " + docRespuesta.mensaje + Environment.NewLine +
                                               "Resultado: " + docRespuesta.resultado;
                    }
                }
                else
                {
                    rtxInformacion.Text = "Proceso cancelado";
                }
            }
            else
            {
                docRespuesta = serviceClient.Enviar(tbxTokenEmpresa.Text.Trim(), tbxTokenPassword.Text.Trim(), factura, "1");
                //envio factura 
                if (docRespuesta.codigo == 200)
                {
                    rtxInformacion.Text += "Codigo: " + docRespuesta.codigo.ToString() + Environment.NewLine +
                                           "Consecutivo Documento: " + docRespuesta.consecutivoDocumento + Environment.NewLine +
                                           "Cufe: " + docRespuesta.cufe + Environment.NewLine +
                                           "Mensaje: " + docRespuesta.mensaje + Environment.NewLine +
                                           "Resultado: " + docRespuesta.resultado + Environment.NewLine + Environment.NewLine;

                    rtxInformacion.Text += "--------------------------------------------------" + Environment.NewLine;
                    rtxInformacion.Text += "Envio de adjuntos:" + Environment.NewLine;
                    int resultado = ServiceAdjuntos(cantidadAnexos, docRespuesta);
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
                }
            }
//            this.Cursor = Cursors.Default;
        }
        #endregion

        #region EstadoDocumento (Web Service SOAP Emisión)
        private void btnEstadoDocumento_Click_1(object sender, EventArgs e)
        {
 //           rtxInformacion.Clear();
            DocumentStatusResponse resp = serviceClient.EstadoDocumento(tbxTokenEmpresa.Text.Trim(), tbxTokenPassword.Text.Trim(), tbxEstadoDocumento.Text.Trim());
            System.Windows.Forms.MessageBox.Show(resp.codigo + Environment.NewLine + resp.estatusDocumento + Environment.NewLine + resp.mensaje, "Estado de Documento");
        }
        #endregion
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
                    connection.Close();
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    System.Windows.MessageBox.Show(errorMessages.ToString());

                }
                catch (Exception ex)
                {
                    errorMessages.Append("c Error:#" + ex.Message.ToString());
                    transaction.Rollback();
                    System.Windows.MessageBox.Show(errorMessages.ToString());
                }

            }
        }

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

                SqlConnection con = new SqlConnection(cnEmp);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                //DataSet dsImprimir = new DataSet();
                //PvFacturaElectronicaAnulacion
                cmd = new SqlCommand("PvFacturaElectronica", con)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@NumRegCab", idrowcab);//if you have parameters.
                cmd.Parameters.AddWithValue("@CodPvt", codpvt);//if you have parameters.
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
                nItems = dsImprimir.Tables[3].Rows.Count;
                if (nItems <= 0)
                {
                    System.Windows.MessageBox.Show("No hay registro en formas de pago en documento..");
                    return false;
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
                tbxnit.Text = dsImprimir.Tables[0].Rows[0]["cod_cli"].ToString().Trim();
                tbxnombre.Text = dsImprimir.Tables[0].Rows[0]["nom_ter"].ToString().Trim();
                tbxEmail.Text = dsImprimir.Tables[0].Rows[0]["email"].ToString().Trim().ToUpper(); ;
                tbxFechaEmision.Text = dsImprimir.Tables[0].Rows[0]["fec_trn"].ToString().Trim();
                //               txtNumFactura.Text = dsImprimir.Tables[0].Rows[0]["num_trn"].ToString().Trim();

                return true;
            }
            catch (Exception ex)
            {
                 System.Windows.MessageBox.Show(ex.Message, "LoadData");
            }
            return false;

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!LoadData(idrowcab, codpvt, cnEmp))
            {
                System.Windows.MessageBox.Show("Error al cargar los datos del documento....");
                this.Close();
                return;
            }

        }
}

