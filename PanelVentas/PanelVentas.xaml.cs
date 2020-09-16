using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

    //Sia.PublicarPnt(9633, "PanelVentas");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9633, "PanelVentas");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();

    public partial class PanelVentas : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        DataTable dt = new DataTable();
        public PanelVentas()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                //if (idemp <= 0) idemp = SiaWin._BusinessId;
                idemp = SiaWin._BusinessId;

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Panel" + cod_empresa + "-" + nomempresa;                

                dt = SiaWin.Func.SqlDT("select rtrim(InCab_doc.num_trn) as num_trn,COUNT(InCue_doc.cantidad) as cnt from InCab_doc inner join InCue_doc on InCue_doc.idregcab =  InCab_doc.idreg where fec_trn>='08/01/2020 16:00:00' and InCab_doc.cod_trn='005' group by InCab_doc.num_trn", "bod", idemp);

                ChartCircle.ItemsSource = dt;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Chart1.SuspendSeriesNotification();

                //Chart1.ResumeSeriesNotification();

                MethodInfo info = ChartCircle.GetType().GetMethod("UpdateArea",
                                BindingFlags.NonPublic | BindingFlags.Instance,
                                null,
                                new Type[] { typeof(bool) },
                                null);

                info?.Invoke(ChartCircle, new object[] { true });


                dt.Rows.Add("FCFT90709", 5);

            }
            catch (Exception w)
            {
                MessageBox.Show("eero :"+w);
            }
        }


    }
}
