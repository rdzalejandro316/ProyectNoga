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

namespace WindowPV
{
    
    public partial class DetalleCompra : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";        

        public string idreg = "";
        public string num_trn = "";

        public DetalleCompra()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            
        }

        private void LoadConfig()
        {
            try
            {
                idemp = SiaWin._BusinessId;
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Pedidos y Cotizaciones - Empresa:" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("aqui-" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            LoadConfig();

            Documento.Text = num_trn;
            cabeza(idreg);
            cuerpo(idreg);
        }

        public void cabeza(string idreg){
            try
            {
                string cabeza = "select cabeza.fec_trn,tercero.nom_ter,vendedor.nom_mer,cabeza.des_mov from InCab_doc as cabeza ";
                cabeza = cabeza + "inner join Comae_ter as tercero on cabeza.cod_cli = tercero.cod_ter ";
                cabeza = cabeza + "left join InMae_mer as vendedor on cabeza.cod_ven = vendedor.cod_mer ";
                cabeza = cabeza + "where idreg='"+idreg+"' ";
                DataTable DTcompra = SiaWin.Func.SqlDT(cabeza, "Compra", idemp);

                if (DTcompra.Rows.Count > 0)
                {
                    TX_fecTrn.Text = DTcompra.Rows[0]["fec_trn"].ToString().Trim();
                    TX_cod_cli.Text = DTcompra.Rows[0]["nom_ter"].ToString().Trim();
                    TX_vend.Text = DTcompra.Rows[0]["nom_mer"].ToString().Trim();
                    TextBx_obse.Text = DTcompra.Rows[0]["des_mov"].ToString().Trim();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar la cabeza"+w);
            }
        }

        public void cuerpo(string idregcab)
        {
            try
            {
                string cuerpo = "select cuerpo.cod_ref,referencia.nom_ref,cuerpo.cantidad,cuerpo.val_uni,cuerpo.subtotal,cuerpo.por_des,cuerpo.val_iva,cuerpo.tot_tot from InCue_doc as cuerpo ";
                cuerpo = cuerpo + "inner join InMae_ref as referencia on cuerpo.cod_ref = referencia.cod_ref ";
                cuerpo = cuerpo + "where idregcab='"+idregcab+"' ";                
                DataTable DTCuerpo = SiaWin.Func.SqlDT(cuerpo, "CompraCuerpo", idemp);
                dataGridCuerpo.ItemsSource = DTCuerpo.DefaultView;
                Total.Text = DTCuerpo.Rows.Count.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar el cuerpo"+w);
            }
        }



    }
}
