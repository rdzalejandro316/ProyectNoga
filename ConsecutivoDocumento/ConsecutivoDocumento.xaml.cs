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
    //Sia.PublicarPnt(9625, "ConsecutivoDocumento");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9625, "ConsecutivoDocumento");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class ConsecutivoDocumento : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        //inventario es 2
        int modulo = 2;
        public ConsecutivoDocumento()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Consecutivo de Documentos" + cod_empresa + "-" + nomempresa;

                DataTable dt = SiaWin.Func.SqlDT("select cod_trn,nom_trn,ind_con,num_act,lon_num,ind_modi,inicial  from " + getTable(modulo) + " order by cod_trn", "transacciones", idemp);
                dataGridDoc.ItemsSource = dt.DefaultView;

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        public string getTable(int modulo)
        {
            string table = "";
            switch (modulo)
            {
                case 1: table = "comae_trn"; break;
                case 2: table = "inmae_trn"; break;
                case 3: table = "nomina"; break;
                case 7: table = "niif"; break;
                case 8: table = "activos_fijos"; break;
            }
            return table;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnExample_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dataGridDoc_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridDoc.SelectedItems[0];
                Cb_consec.SelectedIndex = Convert.ToInt32(row["ind_con"]);
                Tx_consecutivo.Text = row["num_act"].ToString();
                Cb_long.SelectedIndex = Convert.ToInt32(row["lon_num"]);
                Tx_ini.Text = row["inicial"].ToString();
                Cb_mod.SelectedIndex = Convert.ToInt32(row["Ind_modi"]);

            }
            catch (Exception w)
            {
                MessageBox.Show("error _"+w);
            }
        }



    }
}


