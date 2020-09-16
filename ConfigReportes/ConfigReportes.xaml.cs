using System;
using System.Collections.Generic;
using System.Data;
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
    //Sia.PublicarPnt(9588,"ConfigReportes");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9588,"ConfigReportes");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class ConfigReportes : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public ConfigReportes()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
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
                this.Title = "Configuracion de reportes " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();
                DataTable dt = SiaWin.Func.SqlDT("select * from ReportServer", "server", 0);
                if (dt.Rows.Count > 0)
                {
                    TX_ipserver.Text = dt.Rows[0]["ServerIP"].ToString();
                    TX_user.Text = dt.Rows[0]["UserServer"].ToString();
                    TX_password.Text = dt.Rows[0]["UserServerPassword"].ToString();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el load" + w);
            }
        }

        private void BtnClick_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TX_ipserver.Text) || string.IsNullOrEmpty(TX_user.Text) || string.IsNullOrEmpty(TX_password.Text))
                {
                    MessageBox.Show("los campos deben de estar llenos");
                    return;
                }

                string cadena = "update ReportServer set ServerIP='"+ TX_ipserver.Text+ "',UserServer='"+ TX_user.Text+ "',UserServerPassword='"+ TX_password.Text+ "' where idrow='1'";

                if (SiaWin.Func.SqlCRUD(cadena, 0) == true)
                {
                    MessageBox.Show("actualizacion exitosa");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error en modificar" + w);
            }
        }




    }
}
