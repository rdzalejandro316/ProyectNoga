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

namespace ConFacturasConRecibosOficiales
{

    public partial class Fpago : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public string factura = "";
        public Fpago()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
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
                this.Title = "Forma de Pago" + cod_empresa + "-" + nomempresa;                
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Tx_facutura.Text = factura;
                LoadConfig();
              
                string query = "select cod_pag, nom_ban, Cocue_doc.deb_mov,convert(varchar,Cocue_doc.fec_con,103) as fec_con,convert(varchar,Cocue_doc.fec_venc,103) as fec_venc from Cocue_doc ";
                query += "inner join comae_ban on Cocue_doc.cod_pag = comae_ban.cod_ban ";
                query += "where num_trn = '"+ Tx_facutura.Text + "' ";

                DataTable dt = SiaWin.Func.SqlDT(query, "fpag", idemp);
                if (dt.Rows.Count>0)
                {
                    dataGridCxCD.ItemsSource = dt.DefaultView;
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:"+w);
            }
        }




    }
}
