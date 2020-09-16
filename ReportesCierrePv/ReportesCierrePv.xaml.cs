using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Microsoft.Reporting.WinForms;
using Reportes;
using ReportesCierrePv;
using Syncfusion.Windows.Tools.Controls;

namespace SiasoftAppExt
{
    public partial class ReportesCierrePv : Window
    {
        //Sia.PublicarPnt(9516,"ReportesCierrePv");


        //alejandro prueba
        //Sia.PublicarPnt(9561,"ReportesCierrePv");
        //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9561,"ReportesCierrePv");        
        //ww.ShowInTaskbar=false;
        //ww.Owner = Application.Current.MainWindow;
        //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
        //ww.ShowDialog();   

        dynamic SiaWin;
        //public string fechaCorte = DateTime.Now.ToShortDateString();
        public string fechaCorte = "";

        public int ReporteId = 0;
        public int DocumentoIdCab = -1;
        public int idEmp = 0;
        public int _totalentregado = 0;
        public int x_efec = 0;
        public int x_ck = 0;
        public int x_domic = 0;
        public int x_tarj = 0;
        public int x_cred = 0;
        public int x_otro = 0;
        public int x_total1 = 0;
        public int x_total2 = 0;
        public bool PrintOk = false;
        public bool condatos = true;
        private string cnemp = string.Empty;
        private string codemp = string.Empty;
        private string nomemp = string.Empty;
        private string nomemp1 = string.Empty;
        private string diremp = string.Empty;
        private string nitemp = string.Empty;
        private string x_resuciu = string.Empty;
        public string codBod = string.Empty;
        public string serialpc = string.Empty;
        public string codpvta = string.Empty;
        public string ReportPath = string.Empty;
        public string ReportServerUrl = string.Empty;
        public string UserCredencial = string.Empty;
        public string PassCredencial = string.Empty;
        public string TituloReporte = string.Empty;
        public string UserDB = string.Empty;
        public string PassDB = string.Empty;
        public string ti_tulo = string.Empty;


        DataTable Pventas = new DataTable();
        DataTable dt = new DataTable();

        DataTable DtServer = new DataTable();

        bool expand = true;

        //configuracion impresora
        public string printName = string.Empty;
        public int Copias = 1;
        public bool DirecPrinter = false;
        public int ZoomPercent = 30;
        List<ReportParameter> parameters = new List<ReportParameter>();


        public ReportesCierrePv()
        {

            SiaWin = System.Windows.Application.Current.MainWindow;
            InitializeComponent();
            this.MinWidth = 1000;
            this.MinHeight = 500;

            fechaCorte = tbxFechaEmision1.Text;
        }

        private void tb_GotFocus(object sender, RoutedEventArgs e)
        {

            TextBox tb = sender as TextBox;
            if (tb != null)
            {
                tb.SelectAll(); //select all text in TextBox
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SiaWin = System.Windows.Application.Current.MainWindow;
            idEmp = SiaWin._BusinessId;
            codpvta = SiaWin._UserTag;
            Pventas.Clear();
            Pventas = SiaWin.DB.SqlDT("select top 1 * from copventas  where  cod_pvt in (select * from string_split('" + codpvta + "',','))", "pventas", idEmp);
            codpvta = Pventas.Rows[0]["cod_pvt"].ToString();
            codBod = Pventas.Rows[0]["cod_bod"].ToString();
            serialpc = Pventas.Rows[0]["pc_serial"].ToString();

            DtServer = SiaWin.Func.SqlDT("select * from ReportServer", "server", 0);


            if (idEmp <= 0)
            {
                System.Windows.MessageBox.Show("Id Empresa:" + idEmp.ToString() + " no existe");
                this.IsEnabled = false;
                return;
            }

            if (string.IsNullOrEmpty(codpvta) || codpvta == "")
            {
                System.Windows.MessageBox.Show("Punto de Venta:" + codpvta + " no existe");
                this.IsEnabled = false;
                return;
            }

            if (string.IsNullOrEmpty(codBod) || codBod == "")
            {
                System.Windows.MessageBox.Show("Bodega" + codBod + " no existe");
                this.IsEnabled = false;
                return;
            }
            // carga codigo de empresa

            //            DataRow foundRow = SiaWin.Empresas.Rows.Find(idEmp);
            DataRow foundRow = SiaWin.Empresas.Rows.Find(idEmp);
            nomemp = foundRow["BusinessName"].ToString().Trim();
            codemp = foundRow["BusinessCode"].ToString().Trim();
            diremp = foundRow["BusinessAddress"].ToString().Trim();
            nitemp = foundRow["BusinessNit"].ToString().Trim();
            cnemp = foundRow["Businesscn"].ToString().Trim();
            this.tbxFechaEmision1.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.tbxnomepresa.Text = "Empresa: " + nomemp.Trim() + " / PV: " + codpvta.Trim() + " / Bod: " + codBod;
            this.Title = "Cierrte :" + nitemp + "-" + nomemp;

        }


        private void AutoPrint()
        {
            //ReportDirect autoprintme = new ReportDirect(viewer.ServerReport);
            //if (!string.IsNullOrEmpty(printName.Trim())) autoprintme.PrinterSettings.PrinterName = printName.Trim();
            //PrinterSettings ps1 = new PrinterSettings();            
            //ps1.Copies = Convert.ToInt16(Copias);            
            //autoprintme.PrinterSettings = ps1;
            //autoprintme.Print();
            //PrintOk = true;
        }

        public void loaddocumento(int reporteId, string name)
        {
            try
            {
                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();

                viewer.ServerReport.ReportPath = this.ReportPath;
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");

                //if (reporteId == 1) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/cierrepv1";
                //if (reporteId == 2) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/cierrepv_1"
                //if (reporteId == 8) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/cierrepv2";
                //if (reporteId == 4) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/cierrepv4";
                //if (reporteId == 5) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/cierrepv3";
                //if (reporteId == 6) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/cierrepv5";


                //ventas
                if (reporteId == 1) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/GS_Cierre_Ventas";

                //Compras
                if (reporteId == 2) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/GS_Cierre_Compras";

                //notas credito contado
                if (reporteId == 3) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/GS_Cierre_notas_cre_cont";

                //ventas con tarjeta
                if (reporteId == 4) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/GS_Cierre_ventas_tarjeta";

                //Reca credicon
                if (reporteId == 5) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/GS_Cierre_reibos_credicontado";

                //Reca credicon pendiente
                if (reporteId == 6) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/GS_Cierre_reibos_cred_pendie";

                //Recibos de caja
                if (reporteId == 7) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/GS_Cierre_reibos_caja";

                //Entr/salidas traslados
                if (reporteId == 8) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/GS_Cierre_traslados";


                //interempresa
                if (reporteId == 9) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/GS_Cierre_traslados_interempresa";

                //Documentos Diarios
                if (reporteId == 10) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePVentas/GS_Cierre_Documentos_diarios";

                //Ventas Resumidas                
                if (reporteId == 11) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePventas/GS_CIERRE021";

                //fact domicilio pendtes           
                if (reporteId == 12) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePventas/CuentasPorPagarDetalladas";

                //fact domicilio pendtes           
                if (reporteId == 13) viewer.ServerReport.ReportPath = @"/Otros/FrmCierrePventas/GS_Efectivo";




                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                viewer.ProcessingMode = ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;

                string usuario = DtServer.Rows[0]["UserServer"].ToString().Trim();
                string pass = DtServer.Rows[0]["UserServerPassword"].ToString().Trim();

                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(usuario, pass);
                List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();
                parameters.Clear();
                //ReportParam(reporteId);

                if (reporteId == 10 || reporteId == 11 || reporteId == 12)
                {
                    if (reporteId == 10 || reporteId == 11)
                    {
                        parameters.Add(new ReportParameter("bodega", codBod));
                        parameters.Add(new ReportParameter("fecha", fechaCorte));
                        parameters.Add(new ReportParameter("codemp", codemp));
                    }

                    
                    if (reporteId == 12)
                    {
                        parameters.Add(new ReportParameter("codemp", codemp));
                        parameters.Add(new ReportParameter("Fecha", fechaCorte));
                        parameters.Add(new ReportParameter("Cta", "11050506"));
                        parameters.Add(new ReportParameter("Ter", ""));
                        parameters.Add(new ReportParameter("TrnCo", ""));
                        parameters.Add(new ReportParameter("NumCo", ""));
                        parameters.Add(new ReportParameter("Cco", ""));
                        string ven = "A1";
                        if (codBod == "008") ven = "A2";
                        parameters.Add(new ReportParameter("Ven",ven));
                        parameters.Add(new ReportParameter("Resumen", "1"));
                        parameters.Add(new ReportParameter("TipoApli", "1"));
                        parameters.Add(new ReportParameter("ExcluirInterEmpresa", "1"));
                        //parameters.Add(new ReportParameter("Altura", "0"));

                    }
                }
                else
                {
                    parameters.Add(new ReportParameter("bodega", codBod));
                    parameters.Add(new ReportParameter("fecha", fechaCorte));
                    parameters.Add(new ReportParameter("tipo", reporteId.ToString()));
                    parameters.Add(new ReportParameter("codemp", codemp));
                    parameters.Add(new ReportParameter("title", ret_title(reporteId)));
                }




                foreach (var dataSource in viewer.ServerReport.GetDataSources())
                {

                    DataSourceCredentials credn = new DataSourceCredentials();
                    credn.Name = dataSource.Name;
                    System.Windows.MessageBox.Show(dataSource.Name);
                    credn.UserId = "wilmer.barrios@siasoftsas.com";
                    credn.Password = "Camilo654321*";
                    crdentials.Add(credn);
                }

                viewer.ServerReport.SetDataSourceCredentials(crdentials);

                viewer.PrinterSettings.Copies = Convert.ToInt16(Copias);
                viewer.ZoomPercent = 30;
                if (ZoomPercent > 0)
                {
                    viewer.ZoomMode = ZoomMode.Percent;
                    viewer.ZoomPercent = 30;
                }
                //viewer.ShowParameterPrompts = false;

                //System.Drawing.Printing.PageSettings ps = new System.Drawing.Printing.PageSettings();
                //ps.Landscape = true;
                //ps.PaperSize = new System.Drawing.Printing.PaperSize("CARTA", 827, 1170);
                //ps.PaperSize.RawKind = (int)System.Drawing.Printing.PaperKind.Letter;
                //ps.Margins.Top = 5;
                //ps.Margins.Bottom = 5;
                //ps.Margins.Left = 5;
                //ps.Margins.Right = 5;
                //viewer.SetPageSettings(ps);
                viewer.ZoomPercent = 30;
                viewer.PrinterSettings.Collate = false;
                viewer.LocalReport.DataSources.Clear();
                viewer.ServerReport.SetParameters(parameters);
                viewer.RefreshReport();

                winFormsHost.Child = viewer;
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = name;
                tabItemExt1.Content = winFormsHost;
                TabControlPricipal.Items.Add(tabItemExt1);
                UpdateLayout();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("error en loaddocumento:" + ex);
            }
        }


        string ret_title(int id)
        {
            string title = "";

            switch (id)
            {
                case 1: title = nomemp + "\n" + "Ventas Contado"; break;
                case 2: title = nomemp + "\n" + "Ventas Credito"; break;
                case 3: title = nomemp + "\n" + "Reimpresiones"; break;
                case 4: title = nomemp + "\n" + "Ventas con Tarjeta"; break;
                case 5: title = nomemp + "\n" + "Recaudo credicontado"; break;
                case 6: title = nomemp + "\n" + "Recaudo credicontado pendiente"; break;
                case 7: title = nomemp + "\n" + "Recibos de caja"; break;
                case 8: title = nomemp + "\n" + "Traslados de bodega"; break;
                case 9: title = nomemp + "\n" + "Traslados Interempresa"; break;
                case 10: title = nomemp + "\n" + "Docuemntos Diarios"; break;
                case 11: title = nomemp + "\n" + "Compras"; break;
                case 13: title = nomemp + "\n" + "Efectivos"; break;
            }
            return title;
        }


        private void viewer_Print(object sender, ReportPrintEventArgs e)
        {
            //PrintOk = true;
            //viewer.Focus();
            //AuditoriaDoc(DocumentoIdCab, "Imprimio ", idEmp);
        }


        #region codigo de alejandro
        private void ReportParam(int reporteId)
        {
            ReportParameter _repoparam = new ReportParameter();
            try
            {
                //contado
                if (reporteId == 1)  //cierrepv  
                {
                    string fpag_contado = "99";
                    string fpag_dif_conta = "30";

                    ti_tulo = "NIT:" + nitemp + "\n" + "COMPROBANTE DE INFORME DIARIO" + nomemp + "\n" + "SERVIDOR         SERIE :MXQ3470GLG" + "\n" + "Estas facturas fueron dIgitadas en los PC de seriales: " + serialpc + "\n" + "ubicados en el establecimiento comercial en " + diremp + "\n" + "FECHA DE CIERRE :" + fechaCorte.ToString();

                    DataTable consultando = new DataTable();
                    string sql = "select iif(sum(cocue_doc.deb_mov-cocue_doc.cre_mov) is null,0,sum(cocue_doc.deb_mov - cocue_doc.cre_mov)) as xefec from cocue_doc where (cocue_doc.cod_trn = '04 ' or cocue_doc.cod_trn = '08 ') and cocue_doc.cod_cta = '11050501' and (cocue_doc.num_chq = '' or cocue_doc.cod_banc = '99' or cocue_doc.cod_banc = '') and cocue_doc.num_trn in (select incab_doc.num_trn from incab_doc,incue_doc where incue_doc.idregcab = incab_doc.idreg and (incue_doc.cod_BOD = '" + this.codBod + "' or incab_doc.bod_tra = '" + this.codBod + "') and (incab_doc.cod_trn BETWEEN '004' and '009')  AND incab_doc.for_pag='" + fpag_contado + "' and convert(date, incab_doc.fec_TRN, 103) = '" + this.fechaCorte + "')";

                    consultando = SiaWin.DB.SqlDT(sql, "temp", idEmp);

                    if (consultando.Rows.Count > 0)
                    {
                        foreach (DataRow dr in consultando.Rows) // search whole table
                        {
                            x_efec = Convert.ToInt32(dr["xefec"]);
                        }
                    }
                    else
                    {
                        x_efec = 0;
                    }


                    sql = " select iif(sum(cocue_doc.deb_mov-cocue_doc.cre_mov) is null,0,sum(cocue_doc.deb_mov-cocue_doc.cre_mov)) as xck from cocue_doc where (cocue_doc.cod_trn = '04 ' or cocue_doc.cod_trn = '08 ') and cocue_doc.cod_cta = '11050501' and (cocue_doc.num_chq != ''  or cocue_doc.cod_banc != '') and cocue_doc.num_trn in (select incab_doc.num_trn from incab_doc,incue_doc where incue_doc.idregcab = incab_doc.idreg and (incue_doc.cod_BOD = '" + this.codBod + "' or incab_doc.bod_tra = '" + this.codBod + "') and (incab_doc.cod_trn BETWEEN '004' and '009')  AND incab_doc.for_pag='" + fpag_contado + "' and convert(date, incab_doc.fec_TRN, 103) = '" + this.fechaCorte + "') ";

                    consultando = SiaWin.DB.SqlDT(sql, "temp", idEmp);
                    if (consultando.Rows.Count > 0)
                    {
                        foreach (DataRow dr in consultando.Rows)  // search whole table
                        {
                            x_ck = Convert.ToInt32(dr["xck"]);
                        }
                    }
                    else
                    {
                        x_ck = 0;
                    }
                    sql = " select iif(sum(cocue_doc.deb_mov-cocue_doc.cre_mov) is null,0,sum(cocue_doc.deb_mov-cocue_doc.cre_mov)) as xdomic  from cocue_doc where (cocue_doc.cod_trn = '04 ' or cocue_doc.cod_trn = '08 ') and cocue_doc.cod_cta = '11050506' and  cocue_doc.num_trn in (select incab_doc.num_trn from incab_doc,incue_doc where incue_doc.idregcab = incab_doc.idreg and (incue_doc.cod_BOD = '" + this.codBod + "' or incab_doc.bod_tra = '" + this.codBod + "') and (incab_doc.cod_trn BETWEEN '004' and '009')  AND incab_doc.for_pag='01' and convert(date, incab_doc.fec_TRN, 103) = '" + this.fechaCorte + "') ";
                    consultando = SiaWin.DB.SqlDT(sql, "temp", idEmp);
                    if (consultando.Rows.Count > 0)
                    {
                        foreach (DataRow dr in consultando.Rows)  // search whole table
                        {
                            x_domic = Convert.ToInt32(dr["xdomic"]);
                        }
                    }
                    else
                    {
                        x_domic = 0;
                    }
                    sql = " select iif(sum(cocue_doc.deb_mov-cocue_doc.cre_mov) is null,0,sum(cocue_doc.deb_mov-cocue_doc.cre_mov)) as xtarj   from cocue_doc where  (cocue_doc.cod_trn = '04 ' or cocue_doc.cod_trn = '08 ') and ( cod_cta='11100501' or substring(cod_cta,1,4)='1355'  or cod_cta='23657530') and cocue_doc.num_trn in (select incab_doc.num_trn from incab_doc,incue_doc where incue_doc.idregcab = incab_doc.idreg and (incue_doc.cod_BOD = '" + this.codBod + "' or incab_doc.bod_tra = '" + this.codBod + "') and (incab_doc.cod_trn BETWEEN '004' and '009')  AND incab_doc.for_pag='" + fpag_contado + "' and convert(date, incab_doc.fec_TRN, 103) = '" + this.fechaCorte + "')";
                    consultando = SiaWin.DB.SqlDT(sql, "temp", idEmp);
                    if (consultando.Rows.Count > 0)
                    {
                        foreach (DataRow dr in consultando.Rows)  // search whole table
                        {
                            x_tarj = Convert.ToInt32(dr["xtarj"]);
                        }
                    }
                    else
                    {
                        x_tarj = 0;
                    }

                    x_cred = 0;

                    sql = "select sum(round(incue.SUBTOTAL +incue.VAL_IVA - round(incue.VAL_RET, 0) - incue.VAL_ICA - incue.VAL_RIVA, 0) *iif(incue.cod_trn between '007' and  '008', 1, -1)) as total1 from incab_doc as incab inner join incue_doc as incue on incue.idregcab = incab.idreg inner join inmae_ref on inmae_ref.cod_ref = incue.cod_ref inner join comae_ter on comae_ter.cod_ter = incab.cod_cli where (incue.cod_bod = '" + this.codBod + "' or incab.bod_tra = '" + this.codBod + "') and(incab.cod_trn BETWEEN '007' and '008')  AND incab.for_pag='" + fpag_dif_conta + "' and (convert(date, incab.fec_trn, 103) = '" + this.fechaCorte + "')";
                    consultando = SiaWin.DB.SqlDT(sql, "temp", idEmp);
                    if (consultando.Rows.Count > 0)
                    {
                        foreach (DataRow dr in consultando.Rows)  // search whole table
                        {
                            if (string.IsNullOrEmpty(dr["total1"].ToString()))
                            {
                                x_total1 = 0;
                            }
                            else
                            {
                                x_total1 = Convert.ToInt32(dr["total1"]);
                            }
                        }
                    }
                    else
                    {
                        x_total1 = 0;
                    }

                    sql = "select sum(round(incue.SUBTOTAL +incue.VAL_IVA - round(incue.VAL_RET, 0) - incue.VAL_ICA - incue.VAL_RIVA, 0) *iif(incue.cod_trn between '004' and  '005', 1, -1)) as total2 from incab_doc as incab inner join incue_doc as incue on incue.idregcab = incab.idreg inner join inmae_ref on inmae_ref.cod_ref = incue.cod_ref inner join comae_ter on comae_ter.cod_ter = incab.cod_cli where (incue.cod_bod = '" + this.codBod + "' or incab.bod_tra = '" + this.codBod + "') and(incab.cod_trn BETWEEN '004' and '005')  AND incab.for_pag='" + fpag_contado + "' and (convert(date, incab.fec_trn, 103) = '" + this.fechaCorte + "')";

                    consultando = SiaWin.DB.SqlDT(sql, "temp", idEmp);
                    if (consultando.Rows.Count > 0)
                    {
                        foreach (DataRow dr in consultando.Rows)  // search whole table
                        {
                            if (string.IsNullOrEmpty(dr["total2"].ToString()))
                            {
                                x_total2 = 0;
                            }
                            else
                            {
                                x_total2 = Convert.ToInt32(dr["total2"]);
                            }
                        }
                    }
                    else
                    {
                        x_total2 = 0;
                    }

                    sql = " select count(incab_doc.cod_trn) as cuanto_ from incab_doc,incue_doc where incue_doc.idregcab=incab_doc.idreg and  (incue_doc.cod_BOD = '" + this.codBod + "' or incab_doc.bod_tra = '" + this.codBod + "') and  convert(date, incab_doc.fec_trn, 103) = '" + this.fechaCorte + "'  and incab_doc.for_pag='" + fpag_contado + "'  and (incab_doc.cod_trn = '004' or incab_doc.cod_trn = '005')";

                    consultando = SiaWin.DB.SqlDT(sql, "temp", idEmp);
                    if (consultando.Rows.Count > 0)
                    {
                        foreach (DataRow dr in consultando.Rows)  // search whole table
                        {
                            x_otro = Convert.ToInt32(dr["cuanto_"]);
                        }
                    }
                    else
                    {
                        x_otro = 0;
                    }

                    nomemp1 = x_otro.ToString() + "!" + nomemp;

                    sql = " select count(incab_doc.cod_trn) as cuanto_ from incab_doc,incue_doc where incue_doc.idregcab=incab_doc.idreg and   (incue_doc.cod_BOD = '" + codBod + "' or incab_doc.bod_tra = '" + codBod + "')" + " and  convert(date, incab_doc.fec_trn, 103) = '" + fechaCorte + "'  and incab_doc.for_pag='" + fpag_dif_conta + "' and (incab_doc.cod_trn = '007' or incab_doc.cod_trn = '008')";
                    consultando = SiaWin.DB.SqlDT(sql, "temp", idEmp);
                    if (consultando.Rows.Count > 0)
                    {
                        foreach (DataRow dr in consultando.Rows)  // search whole table
                        {
                            x_otro = Convert.ToInt32(dr["cuanto_"]);
                        }
                    }
                    else
                    {
                        x_otro = 0;
                    }

                    sql = "select incab.for_pag,comae_ter.cod_ciu as ciudad,comae_ciu.nom_ciu,DENSE_RANK() OVER(ORDER BY incab.for_pag, comae_ter.cod_ciu, comae_ciu.nom_ciu) As total,sum(round(incue.SUBTOTAL + incue.VAL_IVA - round(incue.VAL_RET, 0) - incue.VAL_ICA - incue.VAL_RIVA, 0) * iif(incue.cod_trn between '004' and  '005', 1, -1)) as totalx from incab_doc as incab inner join incue_doc as incue on incue.idregcab = incab.idreg inner join comae_ter on comae_ter.cod_ter = incab.cod_cli inner join comae_ciu on comae_ter.cod_ciu = comae_ciu.cod_ciu where (incue.cod_bod = '" + this.codBod + "' or incab.bod_tra = '" + this.codBod + "') and(incab.cod_trn BETWEEN '004' and '009')  and incab.for_pag='" + fpag_contado + "' and (convert(date, incab.fec_trn, 103) = '" + this.fechaCorte + "') group by incab.for_pag,comae_ter.cod_ciu,comae_ciu.nom_ciu  ";
                    consultando = SiaWin.DB.SqlDT(sql, "temp", idEmp);
                    if (consultando.Rows.Count > 0)
                    {
                        foreach (DataRow dr in consultando.Rows)  // search whole table
                        {
                            x_resuciu = dr["ciudad"].ToString() + " " + dr["nom_ciu"].ToString() + dr["totalx"].ToString() + "\n";
                        }
                    }
                    else
                    {
                        x_resuciu = "";
                    }

                    nomemp1 = nomemp1 + "?" + x_otro.ToString();


                    string Pasando = x_total1.ToString() + "/" + x_total2.ToString();
                    parameters.Clear();



                    this.parameters.Add(new ReportParameter("Tag1", this.ReportCierre(1)));
                    this.parameters.Add(new ReportParameter("Tag2", this.ReportCierre(2)));
                    this.parameters.Add(new ReportParameter("Pasando", Pasando));
                    this.parameters.Add(new ReportParameter("nomemp1", this.nomemp1));
                    this.parameters.Add(new ReportParameter("ti_tulo", this.ti_tulo.ToString()));
                    this.parameters.Add(new ReportParameter("x_resuciu", this.x_resuciu.ToString()));
                    this.parameters.Add(new ReportParameter("Pasando_", Pasando));
                    this.parameters.Add(new ReportParameter("nomemp2", this.nomemp1));
                    this.parameters.Add(new ReportParameter("nomemp2_", this.nomemp1));
                }

                //credito no se a probado revisar
                if (reporteId == 2)
                {
                    parameters.Clear();
                    parameters.Add(new ReportParameter("Tag1", ReportCierre(3)));
                    parameters.Add(new ReportParameter("ti_tulo", ti_tulo.ToString()));
                }

                //Notas Credito Cont
                if (reporteId == 3)
                {
                    parameters.Clear();
                    parameters.Add(new ReportParameter("Tag1", ReportCierre(4)));
                    parameters.Add(new ReportParameter("ti_tulo", ti_tulo.ToString()));
                }

                //ventas con tarjeta
                if (reporteId == 4)
                {
                    ti_tulo = nomemp + "\n" + "Ventas con Tarjeta";
                    parameters.Clear();
                    //parameters.Add(new ReportParameter("Tag1", ReportCierre(5)));
                    parameters.Add(new ReportParameter("titulo", ti_tulo.ToString()));
                }

                //Reca credicon
                if (reporteId == 5)
                {
                    ti_tulo = nomemp + "\n" + "Recaudo credicontado";
                    parameters.Clear();
                    parameters.Add(new ReportParameter("Tag1", ReportCierre(6)));
                    parameters.Add(new ReportParameter("titulo", ti_tulo.ToString()));
                }

                //Recibos de caja
                if (reporteId == 7)
                {
                    ti_tulo = nomemp + "\n" + "Recibos de caja";
                    parameters.Clear();
                    parameters.Add(new ReportParameter("Tag1", ReportCierre(8)));
                    parameters.Add(new ReportParameter("titulo", ti_tulo.ToString()));
                }

                //traslado de bodega
                if (reporteId == 8)
                {
                    String value = fechaCorte.ToString();
                    String substring = value.Substring(0, 10);

                    ti_tulo = nomemp + "\n" + "Traslados de bodega";

                    parameters.Clear();
                    parameters.Add(new ReportParameter("Tag1", ReportCierre(9)));
                    parameters.Add(new ReportParameter("titulo", ti_tulo.ToString()));
                }


                //traslado automatico 
                if (reporteId == 9)
                {
                    ti_tulo = nomemp + "\n" + "Traslados automaticos";

                    parameters.Clear();
                    parameters.Add(new ReportParameter("Tag1", ReportCierre(10)));
                    parameters.Add(new ReportParameter("titulo", ti_tulo.ToString()));
                }



                //if (reporteId == 10)    
                //{
                //    String value = fechaCorte.ToString();
                //    String substring = value.Substring(0, 10);

                //    ti_tulo = nomemp + "\n" + "SALIDAS Y ENTRADAS AUTOMATICAS :" + substring;
                //    parameters.Clear();
                //    parameters.Add(new ReportParameter("Tag1", ReportCierre(41)));
                //    parameters.Add(new ReportParameter("ti_tulo", ti_tulo.ToString()));
                //}

                if (reporteId == 10)  //cierrepv  
                {
                    String value = fechaCorte.ToString();
                    String substring = value.Substring(0, 10);

                    ti_tulo = nomemp + "\n" + "FACTURAS CLIENTES-PROVEEDORES Y RECIBOS PROVISIONALES :";
                    parameters.Clear();
                    parameters.Add(new ReportParameter("Tag1", ReportCierre(11)));
                    parameters.Add(new ReportParameter("ti_tulo", ti_tulo.ToString()));
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("error en la configuaracion de parametros:" + ex.Message.ToString());
            }
        }
        #endregion
        private void Limpiartablas_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(cnemp))
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
                    string sqlcab = @"delete from pvrcprovi delete from pvfrasprv delete from pvfrasclien"; ;
                    command.CommandText = sqlcab;
                    command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    System.Windows.MessageBox.Show("Documentos diarios borrados", "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
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
                    errorMessages.Append("c Error1:#" + ex.Message.ToString());
                    transaction.Rollback();
                    System.Windows.MessageBox.Show(errorMessages.ToString());
                }

            }

        }

        private void docvarios_Click(object sender, RoutedEventArgs e)
        {
            Botrcprovi botrcprovi = new Botrcprovi();
            botrcprovi.ShowInTaskbar = false;
            botrcprovi.Owner = System.Windows.Application.Current.MainWindow;
            botrcprovi.ShowDialog();
        }

        private void radiogeneral_click(object sender, RoutedEventArgs e)
        {
            foreach (System.Windows.Controls.CheckBox check in ListCheck.Children)
                check.IsChecked = (sender as System.Windows.Controls.CheckBox).IsChecked == true ? true : false;
        }


        private void Btngeneral_Click(object sender, RoutedEventArgs e)
        {


            this.fechaCorte = this.tbxFechaEmision1.Text;

            verificacion();

            //if (condatos == false)
            //{
            //    System.Windows.MessageBox.Show("NO tengo movimientos para esta Fecha!!!!");
            //    return;
            //}

            TabControlPricipal.Items.Clear();



            bool flag = false;
            foreach (System.Windows.Controls.CheckBox check in ListCheck.Children)
            {
                if (check.IsChecked == true) flag = true;



                if (check.IsChecked == true)
                {
                    if (check.Name == "sexto_")
                    {
                        DataTable consultando2 = new DataTable();
                        string sql2 = "select top 1 'RECIBOS PROVISIONALES' AS dato_ from pvrcprovi union select 'FACTURAS CLIENTES' AS dato_ from pvfrasclien  union select 'FACTURAS PROVEEDORES' AS dato_ from pvfrasprv order by dato_";
                        consultando2 = SiaWin.DB.SqlDT(sql2, "temp", idEmp);
                        if (consultando2.Rows.Count > 0)
                            this.hagoreporte(Convert.ToInt32(check.Tag), check.Content.ToString().Trim());
                        else
                            System.Windows.MessageBox.Show("NO ha capturado informacion de Recibos");
                    }
                    else
                    {
                        int idrep = Convert.ToInt32(check.Tag);
                        this.hagoreporte(idrep, check.Content.ToString().Trim());
                    }
                }
            }


            if (flag == false) System.Windows.MessageBox.Show("debe de seleccionar algun check para generar el reporte", "Alerta", MessageBoxButton.OK, MessageBoxImage.Stop);


            #region codigo de luis

            //if (primer.IsChecked == false)
            //{
            //    primer.IsChecked = true;
            //    this.ReporteId = 1;
            //    this.hagoreporte(this.ReporteId);

            //}
            //else
            //{
            //    if (segund.IsChecked == false)
            //    {
            //        segund.IsChecked = true;
            //        ReporteId = 11;
            //        hagoreporte(this.ReporteId);
            //    }
            //    else
            //    {
            //        if (tercer.IsChecked == false)
            //        {
            //            tercer.IsChecked = true;
            //            ReporteId = 2;
            //            hagoreporte(this.ReporteId);
            //        }
            //        else
            //        {
            //            if (quinto.IsChecked == false)
            //            {
            //                this.quinto.IsChecked = true;
            //                this.ReporteId = 4;
            //                this.hagoreporte(this.ReporteId);
            //            }
            //            else
            //            {
            //                if (cuarto.IsChecked == false)
            //                {
            //                    this.cuarto.IsChecked = true;
            //                    this.ReporteId = 3;
            //                    this.hagoreporte(this.ReporteId);
            //                }
            //                else
            //                {
            //                    if (sexto_.IsChecked == false)
            //                    {
            //                        DataTable consultando2 = new DataTable();
            //                        string sql2 = "select top 1 'RECIBOS PROVISIONALES' AS dato_ from pvrcprovi union select 'FACTURAS CLIENTES' AS dato_ from pvfrasclien  union select 'FACTURAS PROVEEDORES' AS dato_ from pvfrasprv order by dato_";
            //                        consultando2 = SiaWin.DB.SqlDT(sql2, "temp", idEmp);
            //                        if (consultando2.Rows.Count > 0)
            //                        {
            //                            this.sexto_.IsChecked = true;
            //                            this.ReporteId = 5;
            //                            this.hagoreporte(this.ReporteId);
            //                        }
            //                        else
            //                        {
            //                            System.Windows.MessageBox.Show("NO ha capturado informacion de Recibos");
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            #endregion
        }

        private void verificacion()
        {
            DataTable consultando2 = new DataTable();
            string sql2 = "select TOP 1 incue_doc.num_trn from incue_doc inner join incab_doc on incue_doc.idregcab=incab_doc.idreg where (incue_doc.cod_bod = '" + this.codBod + "' or incab_doc.bod_tra = '" + this.codBod + "') and (incab_doc.cod_trn BETWEEN '004' and '009') and (convert(date, incab_doc.fec_trn, 103) = '" + this.fechaCorte + "' ) ";
            consultando2 = SiaWin.DB.SqlDT(sql2, "temp", idEmp);
            if (consultando2.Rows.Count > 0)
                condatos = true;
            else
                condatos = false;
        }

        public void hagoreporte(int reporteId, string name)
        {

            fechaCorte = this.tbxFechaEmision1.Text;
            //this.viewer.Clear();
            //this.loaddocumento(this.ReporteId);
            this.loaddocumento(reporteId, name);

        }

        private void frasclien_Click(object sender, RoutedEventArgs e)
        {
            Frasclien ventana = new Frasclien();
            ventana.ShowInTaskbar = false;
            ventana.Owner = System.Windows.Application.Current.MainWindow;
            ventana.ShowDialog();

        }


        private void frasprv_Click(object sender, RoutedEventArgs e)
        {
            Frasprv ventana = new Frasprv();
            ventana.ShowInTaskbar = false;
            ventana.Owner = System.Windows.Application.Current.MainWindow;
            ventana.ShowDialog();
        }


        #region codigo de luis
        //private string ReportCierre(int reporteId)
        //{
        //    try
        //    {
        //        StringBuilder stringBuilder = new StringBuilder();
        //        if (reporteId == 11)
        //        {
        //            stringBuilder.Append("select incab.for_pag,incue.cod_trn,incue.num_trn,DENSE_RANK() OVER(ORDER BY incab.for_pag,incab.cod_trn, incab.num_trn) As num_col, iif(inmae_ref.cod_tip <> '000', incue.subtotal, 0)*iif(incue.cod_trn between '004' and '005', 1, -1) as gravada,iif(inmae_ref.cod_tip <> '000', SUBTOTAL, 0) * iif(incue.cod_trn between '004' and '005', 1, -1) as exenta,iif(incue.cod_tiva = 'A', '16', iif(incue.cod_tiva = 'C', '19', '')) as tar_iva,incue.VAL_IVA* iif(incue.cod_trn between '004' and  '005',1,-1) as val_iva,incue.VAL_RET* iif(incue.cod_trn between '004' and  '005',1,-1) as val_ret,incue.VAL_ICA* iif(incue.cod_trn between '004' and  '005',1,-1) as val_ica,incue.VAL_RIVA* iif(incue.cod_trn between '004' and  '005',1,-1) as val_riva,round(incue.SUBTOTAL + incue.VAL_IVA - round(incue.VAL_RET, 0) - incue.VAL_ICA - incue.VAL_RIVA, 0) * iif(incue.cod_trn between '004' and  '005', 1, -1) as total,incue.cantidad* iif(incue.cod_trn between '004' and  '005',1,-1) as cantidad, iif(incab.for_pag='30',round(incue.SUBTOTAL + incue.VAL_IVA - round(incue.VAL_RET, 0) - incue.VAL_ICA - incue.VAL_RIVA, 0) * iif(incue.cod_trn between '004' and  '005', 1, -1),0) as valcredito,incue.val_uni,inmae_ref.cod_tip,comae_ter.cod_ciu as ciudad  ");
        //            stringBuilder.Append("from incab_doc as incab inner join incue_doc as incue on incue.idregcab = incab.idreg inner join inmae_ref on inmae_ref.cod_ref = incue.cod_ref inner join comae_ter on comae_ter.cod_ter = incab.cod_cli ");
        //            stringBuilder.Append("where (incue.cod_bod = '" + this.codBod + "' or incab.bod_tra = '" + this.codBod + "') and(incab.cod_trn BETWEEN '004' and '009') AND incab.for_pag='01' ");
        //            stringBuilder.Append("and (convert(date, incab.fec_trn, 103) = '" + this.fechaCorte + "' )");
        //            stringBuilder.Append("order by incab.for_pag,incab.cod_trn,incab.num_trn ");
        //        }
        //        if (reporteId == 12)
        //        {
        //            stringBuilder.Append("declare @temporal  table(xtefec numeric(12,2),xtck numeric(12,2),xtdomic numeric(12,2),xttarj numeric(12,2),xtcred numeric(12,2),ciudad1 Text) ");
        //            stringBuilder.Append(" insert into @temporal (xtefec,xtck,xtdomic,xttarj,xtcred,ciudad1) values(" + (object)this.x_efec + "," + (object)this.x_ck + "," + (object)this.x_domic + "," + (object)this.x_tarj + "," + (object)this.x_cred + ",'') ");
        //            stringBuilder.Append("select * from @temporal");
        //        }
        //        if (reporteId == 111)
        //        {
        //            stringBuilder.Append("select incab.for_pag,incue.cod_trn,incue.num_trn,DENSE_RANK() OVER(ORDER BY incab.for_pag,incab.cod_trn, incab.num_trn) As num_col, iif(inmae_ref.cod_tip <> '000', incue.subtotal, 0)*iif(incue.cod_trn between '004' and '005', 1, -1) as gravada,iif(inmae_ref.cod_tip <> '000', SUBTOTAL, 0) * iif(incue.cod_trn between '004' and '005', 1, -1) as exenta,iif(incue.cod_tiva = 'A', '16', iif(incue.cod_tiva = 'C', '19', '')) as tar_iva,incue.VAL_IVA* iif(incue.cod_trn between '004' and  '005',1,-1) as val_iva,incue.VAL_RET* iif(incue.cod_trn between '004' and  '005',1,-1) as val_ret,incue.VAL_ICA* iif(incue.cod_trn between '004' and  '005',1,-1) as val_ica,incue.VAL_RIVA* iif(incue.cod_trn between '004' and  '005',1,-1) as val_riva,round(incue.SUBTOTAL + incue.VAL_IVA - round(incue.VAL_RET, 0) - incue.VAL_ICA - incue.VAL_RIVA, 0) * iif(incue.cod_trn between '004' and  '005', 1, -1) as total,incue.cantidad* iif(incue.cod_trn between '004' and  '005',1,-1) as cantidad, iif(incab.for_pag='30',round(incue.SUBTOTAL + incue.VAL_IVA - round(incue.VAL_RET, 0) - incue.VAL_ICA - incue.VAL_RIVA, 0) * iif(incue.cod_trn between '004' and  '005', 1, -1),0) as valcredito,incue.val_uni,inmae_ref.cod_tip,comae_ter.cod_ciu as ciudad  ");
        //            stringBuilder.Append("from incab_doc as incab inner join incue_doc as incue on incue.idregcab = incab.idreg inner join inmae_ref on inmae_ref.cod_ref = incue.cod_ref inner join comae_ter on comae_ter.cod_ter = incab.cod_cli ");
        //            stringBuilder.Append("where (incue.cod_bod = '" + this.codBod + "' or incab.bod_tra = '" + this.codBod + "') and(incab.cod_trn BETWEEN '004' and '009') AND incab.for_pag='30' ");
        //            stringBuilder.Append("and (convert(date, incab.fec_trn, 103) = '" + this.fechaCorte + "' )");
        //            stringBuilder.Append("order by incab.for_pag,incab.cod_trn,incab.num_trn ");
        //        }
        //        if (reporteId == 112)
        //        {
        //            stringBuilder.Append("declare @temporal  table(xtefec numeric(12,2),xtck numeric(12,2),xtdomic numeric(12,2),xttarj numeric(12,2),xtcred numeric(12,2),ciudad1 Text) ");
        //            stringBuilder.Append(" insert into @temporal (xtefec,xtck,xtdomic,xttarj,xtcred,ciudad1) values(" + (object)this.x_efec + "," + (object)this.x_ck + "," + (object)this.x_domic + "," + (object)this.x_tarj + "," + (object)this.x_cred + ",'') ");
        //            stringBuilder.Append("select * from @temporal");
        //        }
        //        if (reporteId == 21)
        //        {
        //            stringBuilder.Append("select incab_doc.num_trn,incab_doc.doc_ref,iif(incue_doc.cod_ref = '', inmae_ref.cod_ant, incue_doc.cod_ref) as codigo,incab_doc.cod_trn,incue_doc.cantidad,incue_doc.cod_ref,incab_doc.bod_tra,incab_doc.cod_pro,incab_doc.can_proc, ");
        //            stringBuilder.Append(" IIF((incue_doc.cod_trn between '051' and '057') or incue_doc.cod_trn = '148','   <===  ','   ===>  ')+inmae_bod.nom_bod as dato_ ");
        //            stringBuilder.Append(" from incab_doc, incue_doc, inmae_ref, inmae_bod ");
        //            stringBuilder.Append(" where incab_doc.ANO_DOC + incab_doc.PER_DOC + incab_doc.cod_trn + incab_doc.num_trn = incue_doc.ANO_DOC + incue_doc.PER_DOC + incue_doc.cod_trn + incue_doc.num_trn ");
        //            stringBuilder.Append(" and incue_doc.cod_ref = inmae_ref.cod_ref and incab_doc.bod_tra = inmae_bod.cod_bod and ");
        //            stringBuilder.Append(" ((incab_doc.cod_trn between '051' and '058') or(incab_doc.cod_trn between '141' and '148') or ");
        //            stringBuilder.Append(" (incab_doc.cod_trn between '801' and '802')) and incue_doc.cod_bod = '" + codBod + "' and ");
        //            stringBuilder.Append(" iif((incab_doc.cod_trn between '051' and '058'),convert(date, incab_doc.fecha_aded, 103),convert(date, incab_doc.fec_trn, 103))= '" + fechaCorte + "' ");
        //            stringBuilder.Append(" order by incab_doc.cod_trn,incab_doc.num_trn");
        //        }
        //        if (reporteId == 31)
        //        {
        //            stringBuilder.Append(" select iif((incue_doc.cod_trn between '004' and '005'),incue_doc.cod_trn,'007') as cod_trn,incab_doc.num_trn,incab_doc.FOR_PAG,sum(incue_doc.VAL_RET) * iif(incue_doc.cod_trn between '004' and '005', 1, -1) as val_ret,");
        //            stringBuilder.Append(" sum(incue_doc.VAL_ICA) * iif(incue_doc.cod_trn between '004' and '005', 1, -1) as val_ica,sum(incue_doc.VAL_RIVA) * iif(incue_doc.cod_trn between '004' and '005', 1, -1) as val_riva,sum(incue_doc.cantidad) * iif(incue_doc.cod_trn between '004' and '005', 1, -1) as cantidad,incab_doc.cod_CLI,sum(incue_doc.SUBTOTAL) * iif(incue_doc.cod_trn between '004' and '005', 1, -1) as subtotal,");
        //            stringBuilder.Append(" sum(incue_doc.VAL_IVA) * iif(incue_doc.cod_trn between '004' and '005', 1, -1) as val_iva,inmae_fpag.nom_pag");
        //            stringBuilder.Append(" from incab_doc inner  join incue_doc on incue_doc.idregcab = incab_doc.idreg");
        //            stringBuilder.Append(" inner join indet_fpag on incab_doc.idreg = indet_fpag.idregcab");
        //            stringBuilder.Append(" inner join inmae_fpag on indet_fpag.cod_pag = inmae_fpag.cod_pag  where(incue_doc.cod_BOD = '" + codBod + "' or incab_doc.bod_tra = '" + codBod + "') and (incab_doc.cod_trn between '004' and '009')  and indet_fpag.cod_pag='05 ' ");
        //            stringBuilder.Append(" and convert(date, incab_doc.fec_trn, 103)= '" + fechaCorte + "' and (indet_fpag.cod_pag = '05 ') group by incab_doc.FOR_PAG,incue_doc.cod_trn,incab_doc.num_trn,incab_doc.cod_CLI,inmae_fpag.nom_pag ");
        //        }
        //        if (reporteId == 41)
        //        {
        //            stringBuilder.Append(" select incab_doc.cod_trn,incab_doc.num_trn,incue_doc.cantidad,inmae_tip.nom_tip as linea,inmae_ref.cod_ant as codigo,inmae_ref.cod_prv as codprv,incue_doc.cos_uni,incue_doc.cos_tot,incab_doc.bod_tra,incue_doc.cod_bod,val_iva as iva,subtotal,val_uni as val_ref ");
        //            stringBuilder.Append(" from inmae_tip, incab_doc ");
        //            stringBuilder.Append(" inner join incue_doc on incab_doc.cod_trn + incab_doc.num_trn = incue_doc.cod_trn + incue_doc.num_trn ");
        //            stringBuilder.Append(" inner join inmae_ref on incue_doc.cod_ref = inmae_ref.cod_Ref ");
        //            stringBuilder.Append(" where incue_doc.cod_ref = inmae_ref.cod_ref and inmae_ref.cod_tip = inmae_tip.cod_tip and convert(date, incab_doc.fec_trn, 103)= '" + this.fechaCorte + "' and incab_doc.des_mov = 'Trasl-Bodega Automatico' and(cod_bod = '" + this.codBod + "' or bod_tra = '" + this.codBod + "') ");
        //            stringBuilder.Append(" order by incab_doc.cod_trn,incab_doc.num_trn");
        //        }
        //        if (reporteId == 51)
        //        {
        //            stringBuilder.Append(" select nrc, frc, cl, valor,'RECIBOS PROVISIONALES' AS dato_ from pvrcprovi ");
        //            stringBuilder.Append(" union ");
        //            stringBuilder.Append(" select nfr,ffr,cl,valor,'FACTURAS CLIENTES' AS dato_ from pvfrasclien ");
        //            stringBuilder.Append(" union ");
        //            stringBuilder.Append(" select nfr,fprv,prv,valor,'FACTURAS PROVEEDORES' AS dato_ from pvfrasprv ");
        //            stringBuilder.Append("  order by dato_");
        //        }
        //        return stringBuilder.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        int num = (int)System.Windows.MessageBox.Show(ex.Message.ToString());
        //        return string.Empty;
        //    }
        //}
        #endregion


        #region codigo de alejandro
        private string ReportCierre(int reporteId)
        {
            try
            {

                StringBuilder stringBuilder = new StringBuilder();
                //contado
                if (reporteId == 1)
                {
                    stringBuilder.Append("select incab.for_pag,incue.cod_trn,incue.num_trn,DENSE_RANK() OVER(ORDER BY incab.for_pag,incab.cod_trn, incab.num_trn) As num_col, iif(inmae_ref.cod_tip <> '000', incue.subtotal, 0)*iif(incue.cod_trn between '004' and '005', 1, -1) as gravada,iif(inmae_ref.cod_tip <> '000', SUBTOTAL, 0) * iif(incue.cod_trn between '004' and '005', 1, -1) as exenta,iif(incue.cod_tiva = 'A', '16', iif(incue.cod_tiva = 'C', '19', '')) as tar_iva,incue.VAL_IVA* iif(incue.cod_trn between '004' and  '005',1,-1) as val_iva,incue.VAL_RET* iif(incue.cod_trn between '004' and  '005',1,-1) as val_ret,incue.VAL_ICA* iif(incue.cod_trn between '004' and  '005',1,-1) as val_ica,incue.VAL_RIVA* iif(incue.cod_trn between '004' and  '005',1,-1) as val_riva,round(incue.SUBTOTAL + incue.VAL_IVA - round(incue.VAL_RET, 0) - incue.VAL_ICA - incue.VAL_RIVA, 0) * iif(incue.cod_trn between '004' and  '005', 1, -1) as total,incue.cantidad* iif(incue.cod_trn between '004' and  '005',1,-1) as cantidad, iif(incab.for_pag='30',round(incue.SUBTOTAL + incue.VAL_IVA - round(incue.VAL_RET, 0) - incue.VAL_ICA - incue.VAL_RIVA, 0) * iif(incue.cod_trn between '004' and  '005', 1, -1),0) as valcredito,incue.val_uni,inmae_ref.cod_tip,comae_ter.cod_ciu as ciudad  ");
                    stringBuilder.Append("from incab_doc as incab inner join incue_doc as incue on incue.idregcab = incab.idreg inner join inmae_ref on inmae_ref.cod_ref = incue.cod_ref inner join comae_ter on comae_ter.cod_ter = incab.cod_cli ");
                    stringBuilder.Append("where (incue.cod_bod = '" + this.codBod + "' or incab.bod_tra = '" + this.codBod + "') and(incab.cod_trn BETWEEN '004' and '009') AND incab.for_pag='99' ");
                    stringBuilder.Append("and (convert(date, incab.fec_trn, 103) = '" + this.fechaCorte + "' )");
                    stringBuilder.Append("order by incab.for_pag,incab.cod_trn,incab.num_trn ");
                }
                //contado
                if (reporteId == 2)
                {
                    stringBuilder.Append("declare @temporal  table(xtefec numeric(12,2),xtck numeric(12,2),xtdomic numeric(12,2),xttarj numeric(12,2),xtcred numeric(12,2),ciudad1 Text) ");
                    stringBuilder.Append(" insert into @temporal (xtefec,xtck,xtdomic,xttarj,xtcred,ciudad1) values(" + (object)this.x_efec + "," + (object)this.x_ck + "," + (object)this.x_domic + "," + (object)this.x_tarj + "," + (object)this.x_cred + ",'') ");
                    stringBuilder.Append("select * from @temporal");
                }
                //credito
                if (reporteId == 3) { }

                //notas credito contado revisar por q es con auditoria de reimpresion
                if (reporteId == 4)
                {
                    stringBuilder.Append("select * from imae_bod");

                    string query = stringBuilder.ToString();
                    DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idEmp);
                    SiaWin.Browse(dt);
                }

                //ventas con tarjeta
                if (reporteId == 5)
                {
                    stringBuilder.Append("select indet_fpag.cod_pag,InMae_fpag.nom_pag,indet_fpag.num_trn,indet_fpag.cod_trn,InDet_fpag.vlr_pagado ");
                    stringBuilder.Append("from indet_fpag  ");
                    stringBuilder.Append("inner join InMae_fpag on indet_fpag .cod_pag = InMae_fpag.cod_pag ");
                    stringBuilder.Append("inner join InCab_doc on indet_fpag.idregcab = InCab_doc.idreg ");
                    stringBuilder.Append("where convert(date, indet_fpag.fecha_aded, 103) = '" + fechaCorte + "' ");
                    stringBuilder.Append("and InDet_fpag.cod_pag in('06','07','08') and InCab_doc.bod_tra = '" + codBod + "' ");
                    stringBuilder.Append("order by indet_fpag.cod_pag");

                    //string query = stringBuilder.ToString();
                    //DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idEmp);
                    //SiaWin.Browse(dt);
                }

                //Reca credicon
                if (reporteId == 6)
                {
                    stringBuilder.Append("select CoCab_doc.num_trn,Cocue_doc.deb_mov,Cocue_doc.cod_cta,Cocue_doc.cod_ter,Comae_ter.nom_ter, ");
                    stringBuilder.Append("Cocue_doc.cod_pag,Comae_ban.nom_ban ");
                    stringBuilder.Append("from Cocue_doc ");
                    stringBuilder.Append("inner join CoCab_doc on CoCab_doc.idreg = Cocue_doc.idregcab ");
                    stringBuilder.Append("inner join Comae_ban on Comae_ban.cod_ban = Cocue_doc.cod_pag ");
                    stringBuilder.Append("inner join Comae_ter on Cocue_doc.cod_ter = Comae_ter.cod_ter ");
                    stringBuilder.Append("where  ");
                    stringBuilder.Append("convert(date, CoCab_doc.fec_trn, 103) = '" + fechaCorte + "' ");
                    stringBuilder.Append("and CoCab_doc.cod_trn='01B' and CoCab_doc.pun_ven = '" + codBod + "' ");
                    stringBuilder.Append("order by CoCab_doc.num_trn");

                    //string query = stringBuilder.ToString();
                    //DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idEmp);
                    //SiaWin.Browse(dt);
                }

                //Reca credicon pendiente --- pendiente por hacer
                if (reporteId == 7)
                {

                }

                //recibos de caja
                if (reporteId == 8)
                {
                    stringBuilder.Append("select CoCab_doc.num_trn,Cocue_doc.deb_mov,Cocue_doc.cod_cta,Cocue_doc.cod_ter,Comae_ter.nom_ter, ");
                    stringBuilder.Append("Cocue_doc.cod_pag,Comae_ban.nom_ban ");
                    stringBuilder.Append("from Cocue_doc ");
                    stringBuilder.Append("inner join CoCab_doc on CoCab_doc.idreg = Cocue_doc.idregcab ");
                    stringBuilder.Append("inner join Comae_ban on Comae_ban.cod_ban = Cocue_doc.cod_pag ");
                    stringBuilder.Append("inner join Comae_ter on Cocue_doc.cod_ter = Comae_ter.cod_ter ");
                    stringBuilder.Append("where  ");
                    stringBuilder.Append("convert(date, CoCab_doc.fec_trn, 103) = '" + fechaCorte + "' ");
                    stringBuilder.Append("and CoCab_doc.cod_trn='01' and CoCab_doc.pun_ven = '" + codBod + "' ");
                    stringBuilder.Append("order by CoCab_doc.num_trn");

                    //string query = stringBuilder.ToString();
                    //DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idEmp);
                    //SiaWin.Browse(dt);
                }


                //Entr/salidas traslados
                if (reporteId == 9)
                {
                    stringBuilder.Append("select InCab_doc.num_trn,InCab_doc.cod_trn,InMae_ref.nom_ref,InCue_doc.cantidad, ");
                    stringBuilder.Append("IIF((incue_doc.cod_trn between '051' and '057'),'   <===  ','   ===>  ')+inmae_bod.nom_bod as dato_ ");
                    stringBuilder.Append("from InCab_doc ");
                    stringBuilder.Append("inner join InCue_doc on InCue_doc.idregcab = InCab_doc.idreg ");
                    stringBuilder.Append("inner join InMae_ref on InCue_doc.cod_ref = InMae_ref.cod_ref ");
                    stringBuilder.Append("inner join InMae_bod on InCab_doc.bod_tra = InMae_bod.cod_bod ");
                    stringBuilder.Append("where fec_trn>='" + fechaCorte + "' and incue_doc.cod_bod = '" + codBod + "'  and ");
                    stringBuilder.Append("((incab_doc.cod_trn between '051' and '058') or (incab_doc.cod_trn between '141' and '148') ) ");
                    stringBuilder.Append("order by cod_trn ");

                    //string query = stringBuilder.ToString();
                    //DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idEmp);
                    //SiaWin.Browse(dt);
                }


                //inter empresa
                if (reporteId == 10)
                {
                    stringBuilder.Append("select incab_doc.cod_trn,incab_doc.num_trn,incab_doc.cod_prv,Comae_ter.nom_ter, ");
                    stringBuilder.Append("InCue_doc.cod_ref,InMae_ref.nom_ref,InCue_doc.cantidad,InCue_doc.cos_uni,InCue_doc.cos_tot ");
                    stringBuilder.Append("from incab_doc ");
                    stringBuilder.Append("inner join incue_doc on incab_doc.idreg = incue_doc.idregcab ");
                    stringBuilder.Append("inner join inmae_bod on incab_doc.bod_tra = inmae_bod.cod_bod and incab_doc.cod_prv = inmae_bod.cod_ter ");
                    stringBuilder.Append("inner join Comae_ter on InCab_doc.cod_prv = comae_ter.cod_ter ");
                    stringBuilder.Append("inner join InMae_ref on InCue_doc.cod_ref = InMae_ref.cod_ref ");
                    stringBuilder.Append("where incab_doc.cod_trn='001' and incab_doc.fec_trn>='" + fechaCorte + "' and InCue_doc.cod_bod='" + codBod + "' ");
                    stringBuilder.Append("order by cod_prv");

                    //string query = stringBuilder.ToString();
                    //DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idEmp);
                    //SiaWin.Browse(dt);
                }


                if (reporteId == 11)
                {
                    stringBuilder.Append(" select nrc, frc, cl, valor,'RECIBOS PROVISIONALES' AS dato_ from pvrcprovi ");
                    stringBuilder.Append(" union ");
                    stringBuilder.Append(" select nfr,ffr,cl,valor,'FACTURAS CLIENTES' AS dato_ from pvfrasclien ");
                    stringBuilder.Append(" union ");
                    stringBuilder.Append(" select nfr,fprv,prv,valor,'FACTURAS PROVEEDORES' AS dato_ from pvfrasprv ");
                    stringBuilder.Append(" order by dato_ ");
                }


                return stringBuilder.ToString();
            }
            catch (Exception ex)
            {
                int num = (int)System.Windows.MessageBox.Show(ex.Message.ToString());
                return string.Empty;
            }
        }
        #endregion



        private void BtnClickExpand(object sender, RoutedEventArgs e)
        {

            if (expand == true)
            {
                PanelSelc.Visibility = Visibility.Collapsed;

                Thickness marginMenu = PanelReport.Margin;
                marginMenu.Top = -120;
                PanelReport.Margin = marginMenu;

                expand = false;
            }
            else
            {
                PanelSelc.Visibility = Visibility.Visible;

                Thickness marginMenu = PanelReport.Margin;
                marginMenu.Top = 0;
                PanelReport.Margin = marginMenu;

                expand = true;
            }


        }

    }
}
