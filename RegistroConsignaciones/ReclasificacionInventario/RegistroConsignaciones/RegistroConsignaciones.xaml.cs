using System;
using System.Collections.Generic;
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
    //Sia.PublicarPnt(9610,"RegistroConsignaciones");    
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9610,"RegistroConsignaciones");
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog(); 
    public partial class RegistroConsignaciones : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string empNit = "";
        string cod_empresa = "";
        public RegistroConsignaciones()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
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
                this.Title = "Registro de consignacion:" + cod_empresa + "-" + nomempresa;
                TxtValorUnitario.Culture = new CultureInfo("en-US");
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        private void TxCtaBanc_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar("comae_cta", "cod_cta", "nom_cta", "cod_cta", "idrow", "Maestra dereferencia", SiaWin.Func.DatosEmp(idemp), true, " cod_cta between '1110' and '1120' and tip_cta='A' ", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Height = 300;
                winb.Width = 400;
                winb.ShowDialog();            
                code = winb.Codigo;
                nom = winb.Nombre;
                winb = null;

                if (!string.IsNullOrWhiteSpace(code))
                {
                    TxCtaBanc.Text = code;
                    TxCtaNameBanc.Text = nom;
                }
            


            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar:"+w);
            }
        }

        private void BtnProcesar_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
