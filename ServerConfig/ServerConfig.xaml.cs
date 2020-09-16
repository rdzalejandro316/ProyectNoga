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

    //Sia.PublicarPnt(9589,"ServerConfig");       
    //dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9589,"ServerConfig");
    //WinDescto.ShowInTaskbar = false;
    //WinDescto.Owner = Application.Current.MainWindow;
    //WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //WinDescto.ShowDialog(); 

    public partial class ServerConfig : Window
    {
        dynamic SiaWin;        
        int idemp = 0;
        string cnEmp = "";


        public ServerConfig()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;                        
            idemp = SiaWin._BusinessId;         
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();
                DataTable dtBod = SiaWin.Func.SqlDT("select UserServer,UserServerPassword from ReportServer;", "server", 0);
                if (dtBod.Rows.Count>0)
                {
                    Tx_usu.Text = dtBod.Rows[0]["UserServer"].ToString();
                    TX_pass.Text = dtBod.Rows[0]["UserServerPassword"].ToString();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:"+w);
            }           
        }

        private void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Tx_usu.Text) || string.IsNullOrWhiteSpace(TX_pass.Text))
                {
                    MessageBox.Show("llene todos los campos");
                    return;
                }

                string query = "update ReportServer set UserServer='"+ Tx_usu.Text + "',UserServerPassword='"+ TX_pass.Text + "' ";
                if (SiaWin.Func.SqlCRUD(query, 0) == true)
                {
                    MessageBox.Show("actualizacion exitosa");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error:"+w);
            }
        }




    }
}
