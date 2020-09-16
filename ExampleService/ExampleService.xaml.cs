using System;
using System.Collections.Generic;
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
//using ExampleService.ServiceAdjuntos;
using ExampleService.ServicesService;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9585, "ExampleService");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9585, "ExampleService");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class ExampleService : Window
    {
        dynamic SiaWin;        
        int idemp = 0;
        string cnEmp = "";

        //ServicesService.ServiceClient serviceClient;
        //ServiceAdjuntos.ServiceClient serviceArchivos;

        
        public ExampleService()
        {
            try
            {
                InitializeComponent();
                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;
                //LoadConfig();
                ServiceClient ss = new ServiceClient();
            }
            catch (Exception w)
            {
                MessageBox.Show("error :"+w);
            }            
            
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }




    }
}
