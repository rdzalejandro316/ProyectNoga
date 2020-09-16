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
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9596,"SqlDependency");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9596,"SqlDependency");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();    

    public class customer
    {
        public string cod_bod { get; set; }
        public string nom_bod { get; set; }
    }
    public partial class SqlDependency : Window
    {
        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public SqlDependency()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
        }
        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                //cnEmp = foundRow["BusinessCn"].ToString().Trim();
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
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
                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;
                LoadConfig();


                var conne = cnEmp;
                using (var tabledepen = new SqlTableDependency<customer>(conne, "customer"))
                {
                    tabledepen.OnChanged += changed;
                    tabledepen.Start();
                    MessageBox.Show("inicio");
                    tabledepen.Stop();                    
                }



                

            }
            catch (Exception w)
            {
                MessageBox.Show("erro a:" + w);
            }

        }


        public void changed(object sender, RecordChangedEventArgs<customer> e)
        {
            MessageBox.Show("bb");
            if (e.ChangeType != ChangeType.None)
            {
                MessageBox.Show("aa");
                var chan = e.Entity;
                Tx_text.Text += e.ChangeType;
                Tx_text.Text += chan.cod_bod;
            }
        }







    }
}
