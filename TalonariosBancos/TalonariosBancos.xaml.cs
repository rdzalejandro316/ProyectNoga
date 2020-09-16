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
    //Sia.PublicarPnt(9547,"TalonariosBancos");
    //dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9547,"TalonariosBancos");
    //WinDescto.cod_ven = "AFR";
    //WinDescto.ShowInTaskbar = false;
    //WinDescto.Owner = Application.Current.MainWindow;
    //WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //WinDescto.ShowDialog(); 


    public partial class TalonariosBancos : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string cod_ven = "";
        public string nom_ven = "";

        public TalonariosBancos()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId; 
            //idemp = idempresa;            
            controls(true);
            BtnGrabar.Focus();
        }


        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                //idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Talonarios " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();

            DataTable dt = SiaWin.Func.SqlDT("select * from inmae_mer where cod_mer='" + cod_ven.Trim() + "'", "Clientes", idemp);
            Vendedor.Text = dt.Rows.Count > 0 ? dt.Rows[0]["nom_mer"].ToString().Trim() : "NO EXISTE";
            Vendedor.Tag = dt.Rows.Count > 0 ? dt.Rows[0]["cod_mer"].ToString().Trim() : "0";

            if (dt.Rows.Count > 0) loadTalonarios(cod_ven.Trim());
        }

        public void loadTalonarios(string ven)
        {
            DataTable dt_tal = SiaWin.Func.SqlDT("select RTRIM(desde) as desde,rtrim(hasta) as hasta,estado,idrow from cotalon_rc where cod_ven='" + ven + "'", "Talonarios", idemp);
            DataGridTal.ItemsSource = dt_tal.DefaultView;
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Usted desea eliminar el talonario registrado?", "Eliminar Talonario", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DataRowView row = (DataRowView)DataGridTal.SelectedItems[0];
                    string desde = row["desde"].ToString().Trim();
                    string hasta = row["hasta"].ToString().Trim();
                    string estado = row["estado"].ToString().Trim();
                    int id = Convert.ToInt32(row["idrow"]);

                    string sqlQuery = "delete cotalon_rc where idrow='"+id+"';";

                    if (SiaWin.Func.SqlCRUD(sqlQuery, idemp) == true)
                    {
                        loadTalonarios(Vendedor.Tag.ToString());
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar:" + w);
            }
        }

        public bool valRecCon(string desde, string hasta)
        {


            return true;
        }

        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {

            if (BtnGrabar.Content.ToString().Trim() == "Nuevo")
            {
                controls(false);
                return;
            }



            if (string.IsNullOrEmpty(Tx_desde.Text))
            {
                MessageBox.Show("Ingrese el campo desde");
                return;
            }
            if (string.IsNullOrEmpty(Tx_hasta.Text))
            {
                MessageBox.Show("Ingrese el campo hasta");
                return;
            }

            string query = "insert into cotalon_rc (cod_ven,desde,hasta,estado) values ('" + Vendedor.Tag.ToString().ToUpper() + "','" + Tx_desde.Text.ToUpper() + "','" + Tx_hasta.Text.ToUpper() + "'," + Convert.ToInt32(Tx_estado.IsChecked) + ")";

            if (SiaWin.Func.SqlCRUD(query, idemp) == true)
            {
                MessageBox.Show("insercion exitosa");
                loadTalonarios(Vendedor.Tag.ToString());
                controls(true);
            }

        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            if (BtnSalir.Content.ToString() == "Cancelar") { controls(true); return; }
            if (BtnSalir.Content.ToString() == "Salir") { this.Close(); return; }            
        }

        public void controls(bool block)
        {
            if (block == true)
            {
                Tx_desde.IsEnabled = false;
                Tx_hasta.IsEnabled = false;
                Tx_estado.IsEnabled = false;
                Tx_estado.IsChecked = false;
                Tx_desde.Text = "";
                Tx_hasta.Text = "";
                BtnGrabar.Content = "Nuevo";
                BtnSalir.Content = "Salir";
                
            }
            if (block == false)
            {
                Tx_desde.Focus();
                Tx_desde.IsEnabled = true;
                Tx_hasta.IsEnabled = true;
                Tx_estado.IsEnabled = true;
                Tx_desde.Text = "";
                Tx_hasta.Text = "";
                BtnGrabar.Content = "Guardar";
                BtnSalir.Content = "Cancelar";
            }
        }

        private void DataGridTal_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {

                
                DataRowView row = (DataRowView)DataGridTal.SelectedItems[0];
                string desde = Convert.ToString(row["desde"]);
                string hasta = Convert.ToString(row["hasta"]);
                int estado = Convert.ToInt32(row["estado"]);
                int id = Convert.ToInt32(row["idrow"]);

                string query = "update cotalon_rc set desde='"+desde+ "',hasta='" + hasta + "',estado='"+estado+ "' where idrow ='"+id+"' ";

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {                   
                    //loadTalonarios(Vendedor.Tag.ToString());                    
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al editar:"+w);
            }

        }



    }
}
