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

namespace AceptacioSolicitud
{
    
    public partial class Nota : Window
    {
        public bool flag = false;
        public string descripcion = "";
        public Nota()
        {
            InitializeComponent();
        }

        private void BtnGenerarNota_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tx_descripcion.Text))
            {
                flag = false;
                this.Close();
            }
            else
            {
                flag = true;
                descripcion = tx_descripcion.Text;
                this.Close();
            }
        }





    }
}
