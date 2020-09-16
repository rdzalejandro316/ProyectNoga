using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class DocumentosReportes : Window
    {
        //Sia.PublicarPnt(9461,"DocumentosReportes");
        dynamic SiaWin;
        public int idEmp = 0;
        string codemp = string.Empty;
        string nomemp = string.Empty;
        public int DocumentoIdCab = -1;
        public string ReportPath = string.Empty;
        public string ReportServerUrl = string.Empty;
        public string UserCredencial = string.Empty;
        public string PassCredencial = string.Empty;
        public string TituloReporte = string.Empty;
        public string UserDB = string.Empty;
        public string PassDB = string.Empty;
        public bool PrintOk = false;
        public string Tag1 = string.Empty;
        public string Tag2 = string.Empty;
        public string Tag3 = string.Empty;
        public string Tag4 = string.Empty;
        public string Tag5 = string.Empty;
        public string Tag6 = string.Empty;
        public string Tag7 = string.Empty;
        public string Tag8 = string.Empty;
        public string Tag9 = string.Empty;
        public string Tag10 = string.Empty;
        public string titlePie = string.Empty;
        public string usuario = string.Empty;        

        //configuracion impresora
        public string printName = string.Empty;
        public int Copias = 3;
        public bool DirecPrinter = false;
        public int ZoomPercent =0 ;
        public bool CreateFilePDF = false;
        public string NameFilePDF = "";

        List<ReportParameter> parameters = new List<ReportParameter>();
        //configuracion otros
        public bool ShowParameterPrompts =false;
        // conifguracion Parametros Parameter

        public DocumentosReportes()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;            
        }
        
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {                
                // carga codigo de empresa
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idEmp);

                nomemp = foundRow["BusinessName"].ToString().Trim();
                codemp = foundRow["BusinessCode"].ToString().Trim();

                this.Title = "Empesa:" + codemp + "-" + nomemp.Trim();

                // PERMITE PROGRAMAR BUTTON EXPORTAR 
                //var toolStrip = (ToolStrip)viewer.Controls.Find("toolStrip1", true).First();
                //((ToolStripDropDownButton)toolStrip.Items["export"]).ShowDropDownArrow = false;
                //((ToolStripDropDownButton)toolStrip.Items["export"]).DropDownOpening += (obj, arg) =>
                //{
                //    ((ToolStripDropDownButton)obj).DropDownItems.Clear();
                //};
                //((ToolStripDropDownButton)toolStrip.Items["export"]).Click += (obj, arg) =>
                //{
                //    var pdf = viewer.LocalReport.ListRenderingExtensions()
                //        .Where(x => x.Name == "PDF").First();

                //    viewer.ExportDialog(pdf);
                //};

                if (this.DocumentoIdCab <= 0)
                {
                    System.Windows.MessageBox.Show("No hay documento para imprimir");
                    this.Close();
                    return;
                }
                loaddocumento();

                if(CreateFilePDF && NameFilePDF.Trim()!="")
                {
                    GeneraPdf();
                    //AutoPrint();
                    this.Close();
                    return;


                }
                if (DirecPrinter == true)
                {
                    AutoPrint();
                    //RefreshReportTituloPie("wilmer r");
                    //RefreshReportTituloPie("wilmer b");

                    this.Close();
                    return;
                }

                this.UpdateLayout();
                viewer.Focus();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }

        }
        private void AutoPrint()
        {
            try
            {
                
                ReportDirect autoprintme = new ReportDirect(viewer.ServerReport);
                if (!string.IsNullOrEmpty(printName.Trim())) autoprintme.PrinterSettings.PrinterName = printName.Trim();
                PrinterSettings ps1 = new PrinterSettings();
                //            ps1.PrinterName = "HP DeskJet 5820 series";
                ps1.Copies = Convert.ToInt16(Copias);
                //ps1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Letter", 850, 1100);
                //ps1.DefaultPageSettings.Margins = new System.Drawing.Printing.Margins(3, 200, 3, 3);
                //autoprintme.PrinterSettings.PaperSizes = size;
                if (CreateFilePDF == true)
                {
                    //GeneraPdf();
                }
                else
                {
                    autoprintme.PrinterSettings = ps1;
                    autoprintme.Print();
                    autoprintme = null;
                    PrintOk = true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }
        }

        private void GeneraPdf() 
        {
            try
            {
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string filenameExtension;
                byte[] bytes = viewer.ServerReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                //string FilePath = Server.MapPath("/data/");
                string FilePath = DocumentoIdCab.ToString() + ".PDF";
                if(NameFilePDF!="")  FilePath = NameFilePDF.Trim()+".PDF";
                using (FileStream fs = new FileStream(FilePath, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("possible missing information :: " + ex);
            }
        }

        public void loaddocumento()
        {
            try
            {
                viewer.PrinterSettings.Copies = 3;
                viewer.Reset();
                //viewer.PaperHeight = 1056;
                //viewer.PaperWidth = 816;
                //ServerReport serverReport = viewer.ServerReport;
                //string xnameReporte = @"/Otros/FrmDocumentos/pvfacturapos";
                //        string xnameReporte=@"/Empresas/Lecollezioni/Cartera/coMaestraDeTerceros ";
                viewer.ServerReport.ReportPath = this.ReportPath;
                //System.Windows.Forms.MessageBox.Show(this.ReportPath);
                //viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/Reportservergs");
                //viewer.ServerReport.ReportServerUrl = new Uri("http://cop0pa5app01:80/ReportServer");
                

                DataTable dt = SiaWin.Func.SqlDT("select ServerIP,UserServer,UserServerPassword,UserSql,UserSqlPassword from ReportServer", "server", 0);
                string user ="";
                string pass = "";
                if (dt.Rows.Count>0)
                {
                    user = dt.Rows[0]["UserServer"].ToString();
                    pass = dt.Rows[0]["UserServerPassword"].ToString();
                }
                else
                {
                    System.Windows.MessageBox.Show("No existe servidor de reportes...","Siasoft");
                    return;
                }

                viewer.ServerReport.ReportServerUrl = new Uri(dt.Rows[0]["ServerIP"].ToString());

                //viewer.SetDisplayMode(DisplayMode.PrintLayout);
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                //rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(@"grupo\wilmer.barrios", "Q1w2e3r4*/*");               
                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(user, pass);

                List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();
                //List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramx = new ReportParameter();
                paramx.Name = "idregcab";
                paramx.Values.Add(DocumentoIdCab.ToString());
                parameters.Add(paramx);

                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(codemp);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);

                //tag1
                if (!string.IsNullOrEmpty(Tag1))
                {
                    //System.Windows.MessageBox.Show("tag1." + Tag1);
                    ReportParameter paramTag1 = new ReportParameter();
                    paramTag1.Values.Add(Tag1);
                    paramTag1.Name = "Tag1";
                    parameters.Add(paramTag1);
                }
                //tag2
                if (!string.IsNullOrEmpty(Tag2))
                {
                    ReportParameter paramTag2 = new ReportParameter();
                    paramTag2.Values.Add(Tag2);
                    paramTag2.Name = "Tag2";
                    parameters.Add(paramTag2);
                }
                //tag3
                if (!string.IsNullOrEmpty(Tag3))
                {
                    ReportParameter paramTag3 = new ReportParameter();
                    paramTag3.Values.Add(Tag3);
                    paramTag3.Name = "Tag3";
                    parameters.Add(paramTag3);
                }
                //tag4
                if (!string.IsNullOrEmpty(Tag4))
                {
                    ReportParameter paramTag4 = new ReportParameter();
                    paramTag4.Values.Add(Tag4);
                    paramTag4.Name = "Tag4";
                    parameters.Add(paramTag4);
                }
                //tag5
                if (!string.IsNullOrEmpty(Tag5))
                {
                    ReportParameter paramTag5 = new ReportParameter();
                    paramTag5.Values.Add(Tag5);
                    paramTag5.Name = "Tag5";
                    parameters.Add(paramTag5);
                }
                //tag6
                if (!string.IsNullOrEmpty(Tag6))
                {
                    ReportParameter paramTag6 = new ReportParameter();
                    paramTag6.Values.Add(Tag6);
                    paramTag6.Name = "Tag6";
                    parameters.Add(paramTag6);
                }
                //tag7
                if (!string.IsNullOrEmpty(Tag7))
                {
                    ReportParameter paramTag7 = new ReportParameter();
                    paramTag7.Values.Add(Tag7);
                    paramTag7.Name = "Tag7";
                    parameters.Add(paramTag7);
                }
                //tag8
                if (!string.IsNullOrEmpty(Tag8))
                {
                    ReportParameter paramTag8 = new ReportParameter();
                    paramTag8.Values.Add(Tag8);
                    paramTag8.Name = "Tag8";
                    parameters.Add(paramTag8);
                }
                //tag9
                if (!string.IsNullOrEmpty(Tag9))
                {
                    ReportParameter paramTag9 = new ReportParameter();
                    paramTag9.Values.Add(Tag9);
                    paramTag9.Name = "Tag9";
                    parameters.Add(paramTag9);
                }
                //tag10
                if (!string.IsNullOrEmpty(Tag10))
                {
                    ReportParameter paramTag10 = new ReportParameter();
                    paramTag10.Values.Add(Tag10);
                    paramTag10.Name = "Tag10";
                    parameters.Add(paramTag10);
                }

                //title pie
                if (!string.IsNullOrEmpty(titlePie))
                {
                    ReportParameter paramPie = new ReportParameter();
                    paramPie.Values.Add(titlePie);
                    paramPie.Name = "tituloPie";
                    parameters.Add(paramPie);
                }


                if (!string.IsNullOrEmpty(usuario))
                {
                    ReportParameter paramUser = new ReportParameter();
                    paramUser.Values.Add(usuario);
                    paramUser.Name = "usuario";
                    parameters.Add(paramUser);
                }
                
                //tag4
                //ReportParameter paramTag4 = new ReportParameter();
                //paramTag4.Values.Add("select cue.idreg,cue.cod_bod,nom_bod,ref.cod_ref,ref.cod_ant,ref.cod_tip,tip.nom_tip,ref.cod_prv,ref.nom_ref,cue.cantidad,cue.val_uni,subtotal,val_iva,cue.val_des,cue.por_des,cue.tot_tot,cue.cos_uni,cue.cos_tot from incue_doc as cue inner join incab_doc on incab_doc.idreg=cue.idregcab and incab_doc.idreg="+ DocumentoIdCab.ToString()+"  inner join inmae_ref as ref on ref.cod_ref=cue.cod_ref inner join inmae_tip as tip on tip.cod_tip=ref.cod_tip inner join inmae_bod as bod on bod.cod_bod=cue.cod_bod");
                //paramTag4.Name = "SqlCuerpo";
                //parameters.Add(paramTag4);
                //tag5
                //System.Text.StringBuilder _sqlcab = new System.Text.StringBuilder();
                //_sqlcab.Append("SELECT cab.fec_trn, cab.fec_ven, cab.cod_trn, cab.num_trn, cab.cod_ven, cab.ord_comp, mer.nom_mer, ter.nom_ter, ter.cod_ter, ter.ciudad, ter.dir, ter.tel1, cab.for_pag, cab.val_ret, cab.val_riva, cab.val_rica, cab.fa_cufe, suc.cod_suc, nom_suc, suc.dir as dir_suc, dir_corres, suc.tel as tel_suc, fax, suc.cod_ven as cod_ven_suc, cod_rut, suc.cod_ciu as cod_ciu_suc, suc.estado as estado_suc, suc.cod_zona as cod_zona_suc, suc.ciudad as ciudad_suc ");
                //_sqlcab.Append(" FROM InCab_doc AS cab left JOIN  InMae_mer AS mer ON mer.cod_mer = cab.cod_ven INNER JOIN InMae_trn AS trn ON trn.cod_trn = cab.cod_trn INNER JOIN Comae_ter AS ter ON ter.cod_ter = cab.cod_cli ");
                //_sqlcab.Append(" left join inmae_suc as suc on suc.cod_ter = cab.cod_cli");
                //_sqlcab.Append(" WHERE cab.idreg = "+DocumentoIdCab.ToString());
                //System.Windows.Clipboard.SetText(_sqlcab.ToString());

                //ReportParameter paramTag5 = new ReportParameter();
                //paramTag5.Values.Add(_sqlcab.ToString());
                //paramTag5.Name = "SqlCabeza";
                //parameters.Add(paramTag5);

                viewer.ServerReport.SetParameters(parameters);
                foreach (var dataSource in viewer.ServerReport.GetDataSources())
                {
                    DataSourceCredentials credn = new DataSourceCredentials();
                    credn.Name = dataSource.Name;
                    credn.UserId = dt.Rows[0]["UserSql"].ToString();
                    credn.Password = dt.Rows[0]["UserSqlPassword"].ToString();
                    //credn.UserId = "sa";
                    //credn.Password = "W654321*";
                    crdentials.Add(credn);
                }
                viewer.ServerReport.SetDataSourceCredentials(crdentials);
                //viewer.ServerReport..PaperHeight = 1056;
                //viewer.PaperWidth = 816;
                //viewer.Update();
                viewer.PrinterSettings.Copies = Convert.ToInt16(Copias);

                //viewer.ZoomPercent = 50;
                //if (ZoomPercent > 0)
                //{
                //  viewer.ZoomMode = ZoomMode.Percent;

                //viewer.ZoomPercent = ZoomPercent;
                //}
                //viewer.PrinterSettings.PrinterName = "HP DeskJet 5820 series";
                //            viewer.PrinterSettings.PrintRange = PrintRange..AllPages;
                viewer.ProcessingMode = ProcessingMode.Remote;
                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                //viewer.SetDisplayMode(DisplayMode.Normal);
                viewer.ZoomMode = ZoomMode.PageWidth;


                //viewer.PrinterSettings.Collate = false;
                //viewer.PrinterSettings.Copies = 3;
                
                if (CreateFilePDF==false) viewer.RefreshReport();
            }
            catch (Exception ex)
            {
               System.Windows.MessageBox.Show(ex.Message.ToString(), "DocumentosReportes-loaddocumento");
            }
        }
        public void RefreshReportTituloPie(string titulopie)
        {
            if (!string.IsNullOrEmpty(titulopie))
            {
                ReportParameter paramPie = new ReportParameter();
                paramPie.Values.Add(titulopie);
                paramPie.Name = "tituloPie";
                parameters.Add(paramPie);
                viewer.RefreshReport();
                if (DirecPrinter == true)
                {
                    AutoPrint();
                    this.Close();
                    return;
                }
            }
        }
        private void viewer_Print(object sender, ReportPrintEventArgs e)
        {

            PrintOk = true;
            viewer.Focus();
            //AuditoriaDoc(DocumentoIdCab, "Imprimio ", idEmp);
        }
        private void AuditoriaDoc(int iddoc, string evento, int idemp)
        {
            try
            {
                PrintOk = true;
                DataTable dtAud = new DataTable();
                dtAud = SiaWin.DB.SqlDT("select cod_trn,num_trn from incab_doc where idreg=" + iddoc, "tmp", idemp);
                if (dtAud.Rows.Count > 0)
                {
                    string __audCodTrn = dtAud.Rows[0]["cod_trn"].ToString();
                    string __audNumTrn = dtAud.Rows[0]["num_trn"].ToString();
                    string titulo = "Documento:";
                    if (__audCodTrn == "004") titulo = " Factura POS ";
                    if (__audCodTrn == "005") titulo = " Factura Credito ";
                    if (__audCodTrn == "007") titulo = " Nota Credito Anulacion ";
                    if (__audCodTrn == "008") titulo = " Nota Credito Devolucion Item ";
                    if (__audCodTrn == "011") titulo = " Cotizacion ";
                    if (__audCodTrn == "505") titulo = " Pedido ";

                    //string _BusinessName = SiaWIn.foundRow["BusinessName"].ToString().Trim();
                    SiaWin.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, 1, SiaWin._ModulesId, -1, 0, evento + " " + titulo + " " + __audCodTrn + "/" + __audNumTrn + " - Modulo:PV -Empresa:" + codemp, "");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }
        }
        private void winFormsHost_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key==System.Windows.Input.Key.Escape)
            {
                this.Close();
                e.Handled = true;
            }
            if(e.Key== System.Windows.Input.Key.F6)
            {
                AutoPrint();
                PrintOk = true;
                viewer.Focus();
            }
        }
    }
}
