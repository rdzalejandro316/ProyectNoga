using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
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

    //   Sia.PublicarPnt(9649,"PedirFondos");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9649,"PedirFondos");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class PedirFondos : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public PedirFondos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId;
            LoadConfig();
        }
        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);                 
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                string alias = foundRow["BusinessAlias"].ToString().Trim();

                this.Title = "Peticion de fondos " + cod_empresa + "-" + nomempresa;
                TxtValorUnitario.Culture = new CultureInfo("en-US");

                tx_title.Text = "SOLICITUD DE DINERO PUNTO DE VENTA - " + alias+" ("+cod_empresa+")";

                this.MaxHeight = 400;
                this.MinHeight = 400;
                this.MaxWidth = 500;
                this.MinWidth = 500;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                
                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar("copventas", "cod_pvt", "nom_pvt", "nom_pvt", "idrow", "Puntos de venta", cnEmp, true, "isPuntoVen=1", idEmp: idemp);
                xx.ShowInTaskbar = false;
                xx.Owner = Application.Current.MainWindow;
                xx.Height = 400;
                xx.Width = 300;
                xx.ShowDialog();
                idr = xx.IdRowReturn;
                code = xx.Codigo.Trim();
                nombre = xx.Nombre;
                xx = null;
                if (idr > 0)
                {
                    tx_codepv.Text = code.Trim();
                    tx_nompv.Text = nombre.Trim();
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
                e.Handled = true;
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al abrir keydown:" + w);
            }
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tx_codepv.Text))
                {
                    MessageBox.Show("seleccione un punto de venta", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(tx_descripcion.Text))
                {
                    MessageBox.Show("seleccione un punto de venta", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                //validar solicitudes diarias
                string validar = "select* from solicitudDineros where cod_pvt='"+tx_codepv.Text+"' and convert(varchar, fecha_solic,103) = '"+DateTime.Now.ToString("dd/MM/yyyy")+"'";                
                DataTable dt = SiaWin.Func.SqlDT(validar, "table", idemp);

                if (dt.Rows.Count>0)
                {
                    MessageBox.Show("el dia:"+ DateTime.Now.ToString("dd/MM/yyyy") +" ya se genero una solicitud de dinero por favor contacte con el adminstrador del sistema", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                string query = "insert into solicitudDineros (cod_pvt,concepto,valor,fecha_solic,usu_solicitud,estado_soli,estado) values ('" + tx_codepv.Text + "','" + tx_descripcion.Text + "'," + TxtValorUnitario.Value + ",GETDATE()," + SiaWin._UserId + ",'SOLICITUD DE APROBACION',0)";                

                if (MessageBox.Show("Usted desea generar una solicitud de dinero al punto:"+tx_codepv.Text, "Generar solicitud", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        MessageBox.Show("Solicitud enviada con exito", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, 5, -1, -9, "Genero una solicitud de dinero al punto:" + tx_codepv.Text + " de valor" + TxtValorUnitario.Value,"");
                        tx_codepv.Text = "";
                        tx_nompv.Text = "";
                        tx_descripcion.Text = "";
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar la solicitud:" + w);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }


    }
}
