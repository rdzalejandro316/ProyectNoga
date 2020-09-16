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
using System.Windows.Shapes;

namespace PvTrasladosBodega
{

    public partial class DocCruce : Window
    {
        public DataTable dt;
        public int idemp;
        dynamic SiaWin;

        public DocCruce()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                dataGrid.ItemsSource = dt.DefaultView;
                Tx_Total.Text = dt.Rows.Count.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }

        private void BtnViewDoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedIndex < 0) return;
                DataRowView row = (DataRowView)dataGrid.SelectedItems[0];                
                string num_trn = row["doc_cruc"].ToString().Trim();
                DataTable dt_ped = SiaWin.Func.SqlDT("select * From incab_doc where cod_trn='505' and num_trn='"+num_trn+"' ", "pedido", idemp);
                if (dt_ped.Rows.Count>0)
                {
                    int idreg = Convert.ToInt32(dt_ped.Rows[0]["idreg"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, 2, WinModal: true);
                }
                else
                {
                    MessageBox.Show("no contiene documento cruce");
                }
                
            }
            catch (Exception w)
            {
                MessageBox.Show("errro al abrir el documento:" + w);
            }
        }


    }
}
