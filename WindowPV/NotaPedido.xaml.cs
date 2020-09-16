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
using System.Windows.Shapes;

namespace WindowPV
{
    public partial class NotaPedido : Window
    {
        public string pedido = "";
        public int idemp = 0;
        dynamic SiaWin;
        public string nota = "";
        public bool flag = false;

        public NotaPedido()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tx_Pedido.Text = pedido.Trim();
            NotaPed.Text = nota;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update incab_doc set observ='" + NotaPed.Text + "' where num_trn='" + pedido + "' and cod_trn='505'";
                
                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    MessageBox.Show("se guardo la nota al pedido " + pedido + " exitosamente", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                    flag = true;
                }
                else
                {
                    MessageBox.Show("errro al guardar", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



    }
}
