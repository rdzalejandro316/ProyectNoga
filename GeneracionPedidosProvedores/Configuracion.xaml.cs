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

namespace GeneracionPedidosProvedores
{    
    public partial class Configuracion : Window
    {
        public bool flag = false;
        dynamic SiaWin;
        public DataTable dt;

        public Configuracion()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SiaWin = Application.Current.MainWindow;
            string query = "select col_peso,col_peso_width,col_total,col_total_width,col_ped_pen,col_ped_pen_width,";
            query += "col_saldoinv,col_saldoinv_width,col_bod900,col_bod900_width,col_promedio,col_promedio_width,";
            query += "col_backorder,col_backorder_width,col_alcance,col_alcance_width,col_sugerido,col_sugerido_width,fuente ";
            query += "from configPntPedidosProv  where UserId='200'";            

            dt = SiaWin.Func.SqlDT(query, "config", 0);
            if (dt.Rows.Count>0)
            {
                bool peso_v = Convert.ToBoolean(dt.Rows[0]["col_peso"]);
                width_peso.Text = dt.Rows[0]["col_peso_width"].ToString();
                Che_peso.IsChecked = peso_v;

                bool tot_v = Convert.ToBoolean(dt.Rows[0]["col_total"]);
                width_tot.Text = dt.Rows[0]["col_total_width"].ToString();
                Che_tot.IsChecked = tot_v;

                bool pedpen_v = Convert.ToBoolean(dt.Rows[0]["col_ped_pen"]);
                width_pedpen.Text = dt.Rows[0]["col_ped_pen_width"].ToString();
                Che_pedpen.IsChecked = pedpen_v;

                Che_salInv.IsChecked = Convert.ToBoolean(dt.Rows[0]["col_saldoinv"]);
                width_salInv.Text = dt.Rows[0]["col_saldoinv_width"].ToString();
                
                Che_Bod900.IsChecked = Convert.ToBoolean(dt.Rows[0]["col_bod900"]);
                width_Bod900.Text = dt.Rows[0]["col_bod900_width"].ToString();

                Che_Prom.IsChecked = Convert.ToBoolean(dt.Rows[0]["col_promedio"]);
                width_Prom.Text = dt.Rows[0]["col_promedio_width"].ToString();

                Che_Back.IsChecked = Convert.ToBoolean(dt.Rows[0]["col_backorder"]);
                width_Back.Text = dt.Rows[0]["col_backorder_width"].ToString();

                Che_Alcan.IsChecked = Convert.ToBoolean(dt.Rows[0]["col_alcance"]);
                width_alcn.Text = dt.Rows[0]["col_alcance_width"].ToString();

                Che_Sugerido.IsChecked = Convert.ToBoolean(dt.Rows[0]["col_sugerido"]);
                width_sugerido.Text = dt.Rows[0]["col_sugerido_width"].ToString();

                width_fuente.Text = dt.Rows[0]["fuente"].ToString();
            }
            
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab)
            {
                e.Handled = false;
            }
            else
            {
                MessageBox.Show("este campo solo admite valores numericos");
                e.Handled = true;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "update configPntPedidosProv set	col_peso="+ Convert.ToInt32(Che_peso.IsChecked) +",col_peso_width="+ width_peso.Text + ",";
                query += "col_total="+ Convert.ToInt32(Che_tot.IsChecked)+ ",col_total_width="+ width_tot.Text+ ",col_ped_pen="+ Convert.ToInt32(Che_pedpen.IsChecked)+ ",col_ped_pen_width="+ width_pedpen.Text+ ",col_saldoinv="+ Convert.ToInt32(Che_salInv.IsChecked)+ ",";
                query += "col_saldoinv_width="+ width_salInv.Text+ " ,col_bod900="+ Convert.ToInt32(Che_Bod900.IsChecked)+ " ,col_bod900_width="+ width_Bod900.Text + " ,col_promedio="+ Convert.ToInt32(Che_Prom.IsChecked)+ " ,col_promedio_width="+ width_Prom.Text+ " ,col_backorder="+ Convert.ToInt32(Che_Back.IsChecked)+ " , ";
                query += "col_backorder_width="+ width_Back.Text+ ",col_alcance="+ Convert.ToInt32(Che_Alcan.IsChecked)+ " ,col_alcance_width="+ width_alcn.Text+ " ,col_sugerido="+ Convert.ToInt32(Che_Sugerido.IsChecked)+ ",fuente="+ width_fuente.Text+ ",  ";
                query += "col_sugerido_width="+width_sugerido.Text+ " where UserId='200' ";
                
                if (SiaWin.Func.SqlCRUD(query, 0) == true)
                {
                    MessageBox.Show("actualizacion de configuracion de pantalla exitosa");
                    flag = true;
                    this.Close();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("erroral guardar:"+w);
            }
        }

        




    }
}
