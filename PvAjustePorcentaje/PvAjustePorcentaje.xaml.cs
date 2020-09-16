using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
    //Sia.PublicarPnt(9608,"PvAjustePorcentaje");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9608,"PvAjustePorcentaje");    
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog(); 

    public partial class PvAjustePorcentaje : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string codref = "";
        public string nomref = "";
        public double val_por_actu = 0;
        public double val_por_nuevo = 0;

        public double iva = 0;
        public double precioLista = 0;
        public double valreturn = 0;


        public bool flag = false;

        public PvAjustePorcentaje()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Ajuste de Porcentaje";
                TxtNombre.Text = nomref.Trim();
                TX_referencia.Text = codref;

                Tx_PorAnt.Culture = new CultureInfo("en-US");
                Tx_PorAnt.Value = val_por_actu;

                Tx_PorNuevo.Culture = new CultureInfo("en-US");
                Tx_PorNuevo.Value = val_por_actu;

                double porliena = loafPorLinea(codref);
                //Tx_PorNuevo.MaxValue = porliena == 0 ? val_por_actu : porliena;
                
                Tx_PorNuevo.MaxValue =  val_por_actu;

                Tx_PorNuevo.Focus();
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el load:" + w);
            }
        }


        public double loafPorLinea(string cod_ref)
        {
            string query = "SELECT InMae_tip.por_des as por_des from InMae_ref ";
            query += "inner join inmae_tip on InMae_ref.cod_tip = InMae_tip.cod_tip ";
            query += "where cod_ref = '4515in' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "porcentaje", 0);
            return dt.Rows.Count > 0 ? Convert.ToDouble(dt.Rows[0]["por_des"]) : 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void BTNterminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                flag = Tx_PorAnt.Value == Tx_PorNuevo.Value ? false : true;
                val_por_nuevo = Convert.ToDouble(Tx_PorNuevo.Value);
                validarPrecion(precioLista, val_por_nuevo);
                this.Close();
            }
            catch (Exception w)
            {
                MessageBox.Show("erro al ajustar el porcentaje:" + w);
            }
        }


        public void validarPrecion(double valor, double PorNevo)
        {
            try
            {
                double _desc = 1 - PorNevo / 100;
                double _valref = valor * _desc / (1 + (iva) / 100);
                valreturn = Math.Round(_valref, 0);
            }
            catch (Exception w)
            {
                MessageBox.Show("error :" + w);
            }
        }


        private void Btncancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }





    }
}
