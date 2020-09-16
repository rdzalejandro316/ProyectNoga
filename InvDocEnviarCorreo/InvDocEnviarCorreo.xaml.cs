using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9583,"InvDocEnviarCorreo");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9583,"InvDocEnviarCorreo");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //ww.ShowDialog();

    public partial class InvDocEnviarCorreo : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public int idreg = 0;

        DataTable Dtdocumento;

        public InvDocEnviarCorreo()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Envio Documento" + cod_empresa + "-" + nomempresa;


                string select = "select InCab_doc.cod_cli,InCab_doc.cod_prv,InCab_doc.num_trn,InCab_doc.fec_trn, ";
                select += "InCue_doc.cod_ref,InCue_doc.cantidad,InCue_doc.cos_uni,InCue_doc.cos_tot ";
                select += "from InCab_doc  ";
                select += "inner join InCue_doc on InCab_doc.idreg = InCue_doc.idregcab ";
                select += "where InCab_doc.idreg='"+ idreg + "' ";


                Dtdocumento = SiaWin.Func.SqlDT(select, "tabla", idemp);

                if (Dtdocumento.Rows.Count>0)
                {
                    TxDocum.Text = Dtdocumento.Rows[0]["num_trn"].ToString().Trim();
                }

            }
            catch (Exception w) { MessageBox.Show("error al enviar documento:" + w); }
        }


        public void enviar()
        {
            try
            {
                MailMessage mail = new MailMessage();

                var tag = ((ComboBoxItem)cob_smpt.SelectedItem).Tag.ToString();
                string serv = "smtp."+ tag + ".com";

                SmtpClient SmtpServer = new SmtpClient(serv);

                mail.From = new MailAddress(tx_coore.Text);
                mail.To.Add(Tx_des.Text);
                mail.Subject = Tx_Asu.Text;

                mail.IsBodyHtml = true;
                string htmlBody;

                htmlBody = "<!DOCTYPE html>" +
                "<html>" +
                "<head>" +
                "<title>Documentos</title>" +                
                "<style type='text/css'>" +
                    "html, body, div, span, applet, object, iframe,h1, h2, h3, h4, h5, h6, p, blockquote, pre,a, abbr, acronym, address, big, cite, code,del, dfn, em, img, ins, kbd, q, s, samp,small, strike, strong, sub, sup, tt, var,b, u, i, center,dl, dt, dd, ol, ul, li,fieldset, form, label, legend,table, caption, tbody, tfoot, thead, tr, th, td,article, aside, canvas, details, embed,figure, figcaption, footer, header, hgroup,menu, nav, output, ruby, section, summary,time, mark, audio, video {margin: 0;padding: 0;border: 0;font-size: 100%;vertical-align: baseline;}" +
                    "article, aside, details, figcaption, figure,footer, header, hgroup, menu, nav, section {display: block;}" +
                    "body {line-height: 1;}" +
                    "ol, ul {list-style: none;}" +
                    "blockquote, q {    quotes: none;}" +
                    "blockquote:before, blockquote:after,q:before, q:after {content: '';content: none;}" +
                    "*{font - family: 'Roboto', sans - serif}" +
                    ".carta{width: 400px;height: 400px;    margin-left: 10px;margin-top: 10px;}" +
                    ".card {box-shadow: 0 4px 8px 0 rgba(0, 0, 0, 0.2);padding: 10px;text-align: center;background-color: #f1f1f1;}" +
                    ".title {font-size: 20px;text-align: center} " +
                    "hr{display: block;  margin-top: 0.5em;margin-bottom: 0.5em;margin-left: auto;margin-right: auto; border-style: inset; border-width: 1px;}" +
                    ".text_cab{margin-top: 10px;text-align: left}" +
                    ".text_cab_ti{font-weight: bold;}" +
                    "#customers {border-collapse: collapse;width: 100%;}" +
                    "#customers td, #customers th {border-bottom: 1px solid #ddd;  padding: 8px;}" +
                    "#customers tr:nth-child(even){background-color: #f2f2f2;}" +
                    "#customers tr:hover {background-color: #ddd;}" +
                    "#customers th {padding-top: 12px;  padding-bottom: 12px;  text-align: center;  background-color: #4CAF50;  color: white;}" +
                "</style>" +
                "</head>" +
                "<body>" +
                "<div class='carta'>" +
                    "<div class='card'>" +
                        "<h3 class='title'>DOCUMENTO</h3>" +
                        "<hr>" +
                        "<div class='cabeza'>" +
                            "<p class='text_cab'>" +
                                "<span class='text_cab_ti'>Documento:</span>" +
                                "<span>" + Dtdocumento.Rows[0]["num_trn"].ToString().Trim() + "</span>" +
                            "</p>" +
                            "<p class='text_cab'>" +
                                "<span class='text_cab_ti'>Fecha:</span>" +
                                "<span>" + Dtdocumento.Rows[0]["fec_trn"].ToString().Trim() + "</span>" +
                            "</p>" +
                            "<p class='text_cab'>" +
                                "<span class='text_cab_ti'>Cliente/Provedor:</span>" +
                                "<span>" + Dtdocumento.Rows[0]["cod_prv"].ToString().Trim() + "</span>" +
                            "</p>" +
                        "</div>" +
                        "<hr>" +
                        "<div class='cuerpo'>" +
                            "<table id='customers'>" +
                                "<tr>" +
                                    "<th>Referencia</th>" +
                                    "<th>Cantidad</th>" +
                                    "<th>Costo</th>" +
                                "</tr>";
                                foreach (DataRow dr in Dtdocumento.Rows)
                                {
                                    htmlBody += "<tr>";
                                    htmlBody += "<td>"+dr["cod_ref"].ToString().Trim()+"</td>";
                                    htmlBody += "<td>" + dr["cantidad"].ToString().Trim()+ "</td>";
                                    htmlBody += "<td>" + dr["cos_tot"].ToString().Trim() + "</td>";
                                    htmlBody += "</tr>";
                                }

                htmlBody += "</table>";
                htmlBody += "</div>";
                htmlBody += "</div>";
                htmlBody += "</body>";
                htmlBody += "</html>";

                mail.Body = htmlBody;

                AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);

                //LinkedResource pic1 = new LinkedResource(PathImg, MediaTypeNames.Image.Jpeg);
                //pic1.ContentId = "Pic1";
                //avHtml.LinkedResources.Add(pic1);

                //MessageBox.Show(htmlBody);

                mail.AlternateViews.Add(avHtml);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(tx_coore.Text, tx_pass.Password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                MessageBox.Show("mail Send");


            }
            catch (Exception w) { MessageBox.Show("no se envio el correo:" + w); }
        }

        private void BtnClick_Click(object sender, RoutedEventArgs e)
        {
            

            if (string.IsNullOrWhiteSpace(tx_coore.Text))
            {
                MessageBox.Show("el correo del remitente tiene que esta lleno");
                return;
            }
            if (string.IsNullOrWhiteSpace(tx_pass.Password))
            {
                MessageBox.Show("la contraseña esta vacia");
                return;
            }
            if (string.IsNullOrWhiteSpace(Tx_des.Text))
            {
                MessageBox.Show("el correo de destino esta vacio");
                return;
            }
            if (cob_smpt.SelectedIndex<0)
            {
                MessageBox.Show("seleccione el tipo de servidor");
                return;
            }



            if (Dtdocumento.Rows.Count > 0)
            {
                enviar();
            }
            else { MessageBox.Show("no se envio documento por que esta vacio"); }

        }





    }
}
