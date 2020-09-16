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

namespace Fletes
{    
    public partial class Consultar : Window
    {
        public bool flag = false;
        public string tipo = "";
        public string guia_doc = ""; 
        public Consultar()
        {
            InitializeComponent();
        }
        private void Cbx_envioClas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var tag = ((ComboBoxItem)(sender as ComboBox).SelectedItem).Tag.ToString();

                if (tag == "M")
                {
                    GridMercancia.Visibility = Visibility.Visible;
                    GridDocumentos.Visibility = Visibility.Hidden;
                }
                else
                {
                    GridMercancia.Visibility = Visibility.Hidden;
                    GridDocumentos.Visibility = Visibility.Visible;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cambiar:" + w);
            }
        }

        private void BtnConsutar_Click(object sender, RoutedEventArgs e)
        {
            if (Cbx_envioClas.SelectedIndex<0)
            {
                MessageBox.Show("seleccione un tipo de consulta");
            }
            else
            {
                tipo = ((ComboBoxItem)Cbx_envioClas.SelectedItem).Tag.ToString();
                string texto = tipo == "M" ? Tx_documento.Text : Tx_guiat.Text;
                if (string.IsNullOrEmpty(texto))
                {
                    MessageBox.Show(
                        tipo == "M" ? "ingrese el numero de documento":"ingrese el numero de guia",
                        "Alert",MessageBoxButton.OK,MessageBoxImage.Stop
                        );
                    return;
                }

                guia_doc = texto;
                flag = true;
                this.Close();
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }





    }
}
