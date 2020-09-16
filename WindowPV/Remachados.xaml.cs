using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowPV
{

    public partial class Remachados : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public DataTable dt_rem = new DataTable();
        public string tercero = "";
        public string num_ord = "";

        public Remachados(int idEmpresa)
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = idEmpresa;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Remachados " + cod_empresa + "-" + nomempresa;
                Tx_search.Focus();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            //int idr = 0; string code = ""; string nombre = "";
            //dynamic xx = SiaWin.WindowBuscar("InOrd_Pro", "num_trn", "cod_cli", "num_trn", "idrow", "Ordenes", cnEmp, false, "", idEmp: idemp);
            //xx.ShowInTaskbar = false;
            //xx.Owner = Application.Current.MainWindow;
            //xx.Width = 400;
            //xx.Height = 300;
            //xx.ShowDialog();
            //idr = xx.IdRowReturn;
            //code = xx.Codigo;
            //nombre = xx.Nombre;
            //if (idr > 0)
            //{
            //    Tx_search.Text = code;
            //    buscar(code);
            //}
            try
            {
                Buscar ventana = new Buscar();
                ventana.ShowInTaskbar = false;
                ventana.Owner = Application.Current.MainWindow;
                ventana.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                ventana.ShowDialog();


                if (ventana.flag == true)
                {
                    string code = ventana.num_trnBusc;
                    Tx_search.Text = code;
                    buscar(code);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar orden:" + w);
            }
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(Tx_search.Text) || dt_rem.Rows.Count == 0)
                {
                    MessageBox.Show("Llene la orden que desea facturar");
                    return;
                }

                tercero = dt_rem.Rows[0]["cod_cli"].ToString();
                num_ord = Tx_search.Text.Trim();

                this.Close();

            }
            catch (Exception w)
            {
                MessageBox.Show("error al traer al documentor");
            }
        }


        public Tuple<string, string> retursMultil(string cli, string ord)
        {
            var tuple = new Tuple<string, string>(cli, ord);
            return tuple;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string ord = (sender as TextBox).Text.ToString().Trim();
            if (string.IsNullOrEmpty(ord)) return;
            buscar(ord);
        }


        public void buscar(string num_ord)
        {
            dt_rem.Clear();
            dt_rem = SiaWin.Func.SqlDT("select idrow,NUM_TRN as num_trn,COD_REF as cod_ref,COD_CLI as cod_cli,FEC_TRN as fec_trn,CANTIDAD as cantidad from InOrd_Pro where num_trn='" + num_ord + "'", "temporal", idemp);
            if (dt_rem.Rows.Count > 0)
            {
                GridConfig.ItemsSource = dt_rem.DefaultView;
                Tx_total.Text = dt_rem.Rows.Count.ToString(); ;
            }
            else
            {
                MessageBox.Show("no existe ese numero de orden");
                Tx_search.Text = "";
                Tx_total.Text = "0";
            }

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
                BtnGenerar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }

        private void Tx_search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.F8)
            {
                BtnBuscar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }









    }
}

