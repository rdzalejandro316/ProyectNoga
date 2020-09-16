using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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

    //Sia.PublicarPnt(9644,"GenerarSalidaCompra");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9644,"GenerarSalidaCompra");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class GenerarSalidaCompra : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        int moduloid = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public GenerarSalidaCompra()
        {
            InitializeComponent();            
            SiaWin = System.Windows.Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessIcon"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());

                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Generacion de salida por compra - "+cod_empresa + nomempresa;
                Tx_fecha.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Tx__LostFocus(object sender, RoutedEventArgs e)
        {
            string num_trn = (sender as TextBox).Text.Trim();
            if (string.IsNullOrEmpty(num_trn)) return;

            DataTable dtCab = SiaWin.Func.SqlDT("select * From incab_doc where num_trn='"+ num_trn + "' and cod_trn='001' ", "temporal", idemp);
            if (dtCab.Rows.Count > 0)
            {
                Tx_compra.Text = dtCab.Rows[0]["num_trn"].ToString().Trim();                
            }
            else
            {
                MessageBox.Show("la compra ingresada no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                Tx_compra.Text = "";   
            }
        }
        
        private void Tx__PreviewKeyDown(object sender, KeyEventArgs e)
        {            
            if (e.Key == Key.F8 || e.Key == Key.Enter)
            {
                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar("incab_doc", "cod_trn", "num_trn", "cod_trn", "idreg", "Documentos", cnEmp, false, "cod_trn='001' ", idEmp: idemp);
                xx.ShowInTaskbar = false;
                xx.Owner = Application.Current.MainWindow;
                xx.Height = 500;
                xx.ShowDialog();
                idr = xx.IdRowReturn;
                code = xx.Codigo;
                nombre = xx.Nombre;

                Tx_compra.Text = idr > 0 ? nombre : "";
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        public bool validar(string num_trn)
        {
            bool flag = false;
            DataTable dtCab = SiaWin.Func.SqlDT("select * From incab_doc where num_trn='" + num_trn + "' and cod_trn='140';", "temporal", idemp);
            if (dtCab.Rows.Count > 0) flag = true;
            return flag;
        }

        private async void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Tx_compra.Text))
                {
                    MessageBox.Show("el campo de compra debe de estar lleno", "alert", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (validar(Tx_compra.Text))
                {
                    MessageBox.Show("la compra ya tiene una salida generada", "alert", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                
                string doc = Tx_compra.Text;
                string fecha = Tx_fecha.Text;
                string emp = cod_empresa;                

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadScript(doc, fecha, emp,source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    MessageBox.Show("se genero la salida la compra", "alert", MessageBoxButton.OK, MessageBoxImage.Information);
                    DataTable dt = ((DataSet)slowTask.Result).Tables[0];                    
                    Tx_document.Text = dt.Rows[0]["num_trn"].ToString().Trim();
                    Tx_document.Tag = dt.Rows[0]["idreg"].ToString().Trim();
                    BtnDoc.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("no se genero el procesos de salida de compra","alert",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al generar el proceso");
            }
        }

        private DataSet LoadScript(string documento, string fecha,string empresa, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpGenerarDocSalida", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@num_trn", documento);
                cmd.Parameters.AddWithValue("@_fecha", fecha);
                cmd.Parameters.AddWithValue("@codemp", empresa);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show("error#" + e.Message);
                return null;
            }
        }

        private void BtnDoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int idreg = Convert.ToInt32(Tx_document.Tag);
                if (idreg > 0)
                    SiaWin.TabTrn(0, idemp, true, idreg, moduloid, WinModal: true);

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el documento:"+w);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }
    }
}
