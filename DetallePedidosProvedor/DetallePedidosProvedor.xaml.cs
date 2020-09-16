using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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


    //Sia.PublicarPnt(9670,"DetallePedidosProvedor");
    //dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9670,"DetallePedidosProvedor");
    //WinDescto.ShowInTaskbar = false;
    //WinDescto.Owner = Application.Current.MainWindow;
    //WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //WinDescto.ShowDialog(); 

    public partial class DetallePedidosProvedor : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        int moduloid = 0;

        public string bodega = string.Empty;
        public string referencia = string.Empty;
        public string mesini = string.Empty;
        public string fec_con = string.Empty;
        public string backorder = string.Empty;
        public string empresa = string.Empty;
        public string fec_pedido = string.Empty;
        public DetallePedidosProvedor()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                string cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());
                Title = "Detalle: " + cod_empresa + "-" + nomempresa;


                Cod_Ref.Text = referencia;
                Cod_Bod.Text = bodega;
                TXT_mesini.Text = mesini;
                TXT_fec_con.Text = fec_con;
                TXT_backorder.Text = backorder;
                TXT_empresa.Text = empresa;
                TXT_fec_pedido.Text = fec_pedido;

                Name_Ref.Text = referencia;
                Name_Ref2.Text = referencia;

                cargarConsulta();
            }
            catch (Exception)
            {
                MessageBox.Show("error al cargar el Load");
            }
        }

        public void cargarConsulta()
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("GeneracionPedidosProvedoresDETALLE", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_bod", Cod_Bod.Text);
                cmd.Parameters.AddWithValue("@cod_ref", Cod_Ref.Text);
                cmd.Parameters.AddWithValue("@mesIni", TXT_mesini.Text);
                cmd.Parameters.AddWithValue("@fec_pedido", TXT_fec_pedido.Text);
                cmd.Parameters.AddWithValue("@fec_back", TXT_backorder.Text);
                cmd.Parameters.AddWithValue("@fec_fin", TXT_fec_con.Text);
                cmd.Parameters.AddWithValue("@cod_empresa", TXT_empresa.Text);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();


                dataGridPedido.ItemsSource = ds.Tables[0];
                Total.Text = ds.Tables[0].Rows.Count.ToString();

                dataGridbackorder.ItemsSource = ds.Tables[1];
                Total2.Text = ds.Tables[1].Rows.Count.ToString();

                dataGridCompra.ItemsSource = ds.Tables[2];
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar la consulata" + w);
            }
        }

        private void BTNdetalle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tag = (sender as Button).Tag.ToString().Trim();
                string cod_trn = "";
                switch (tag)
                {
                    case "1": cod_trn = "500"; break;
                    case "2": cod_trn = "001"; break;
                    case "3": cod_trn = "505"; break;
                }

                string query = "";
                if (tag == "1")
                {
                    DataRowView row = (DataRowView)dataGridbackorder.SelectedItems[0];
                    string numtrn = row["num_trn"].ToString().Trim();
                    query = "select * From incab_doc where num_trn='" + numtrn + "' and cod_trn='" + cod_trn + "' ";
                }

                if (tag == "2")
                {
                    DataRowView row = (DataRowView)dataGridCompra.SelectedItems[0];
                    string numtrn = row["num_trn"].ToString().Trim();
                    query = "select * From incab_doc where num_trn='" + numtrn + "' and cod_trn='" + cod_trn + "' ";
                }

                if (tag == "3")
                {
                    DataRowView row = (DataRowView)dataGridPedido.SelectedItems[0];
                    string numtrn = row["p_num_trn"].ToString().Trim();
                    query = "select * From incab_doc where num_trn='" + numtrn + "' and cod_trn='" + cod_trn + "' ";
                }


                DataTable dt = SiaWin.Func.SqlDT(query, "documento", idemp);
                if (dt.Rows.Count > 0)
                {
                    int idreg = Convert.ToInt32(dt.Rows[0]["idreg"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, moduloid, WinModal: true);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el documento:" + w);
            }
        }





    }
}
