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
    //Sia.PublicarPnt(9622, "AnulacioRecibosProvi");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9622, "AnulacioRecibosProvi");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class AnulacioRecibosProvi : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int cosn = 0;

        public AnulacioRecibosProvi()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
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
                this.Title = "Anulacion de Recibos Provicionales " + cod_empresa + "-" + nomempresa;

                DataTable dt = SiaWin.Func.SqlDT("select cod_mer as cod_ven,cod_mer+'-'+nom_mer as nom_ven from inmae_mer where estado=1  order by cod_mer", "inmae_mer", idemp);
                CmbVen.ItemsSource = dt.DefaultView;
                CmbVen.DisplayMemberPath = "nom_ven";
                CmbVen.SelectedValuePath = "cod_ven";
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void BtnAnular_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region validacion de campos

                if (CmbVen.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione un vendedor");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Tx_recibo.Text))
                {
                    MessageBox.Show("llene el campo de recibo provisional");
                    return;
                }
                #endregion

                #region validacion de existencia

                string query = "SELECT * from co_rprovanu where cod_ven='" + CmbVen.SelectedValue.ToString().Trim() + "' and rc_prov='" + Tx_recibo.Text.Trim() + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "existencia", idemp);
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show("el recibo ingresado ya se encuentra en la lista de anulados", "Alert", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                string querycon = "select * From CoCab_doc where cod_trn='01' and cod_ven='" + CmbVen.SelectedValue.ToString().Trim() + "' and rc_prov='" + Tx_recibo.Text.Trim() + "' ";
                DataTable dtcon = SiaWin.Func.SqlDT(querycon, "contabilidad", idemp);
                if (dtcon.Rows.Count > 0)
                {
                    MessageBox.Show("el recibo ingresado ya se encuentra registrado en contabilidad", "Alert", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
                #endregion

                #region otro
                
                string valor = Tx_recibo.Text;
                string vali = "select * from cotalon_rc where '" + valor + "' between desde and hasta";
                DataTable dt_valida = SiaWin.Func.SqlDT(vali, "table", idemp);                

                if (dt_valida.Rows.Count > 0)
                {
                    
                    string VenTabla = dt_valida.Rows[0]["cod_ven"].ToString().Trim().ToLower();
                    string VenSele = CmbVen.SelectedValue.ToString().Trim().ToLower();                    
                    if (VenTabla != VenSele)
                    {
                        MessageBox.Show("este recibo provisional le pertenece a otro vendedor:" + VenTabla);
                        return;
                    }
                    else
                    {
                        InserVal();
                    }
                }
                else
                {
                    MessageBox.Show("El recibo provisional no existe");
                    return;
                }
                #endregion


            }
            catch (Exception w)
            {
                MessageBox.Show("errro al anular:" + w);
            }
        }


        public void InserVal()
        {
            try
            {
                string query = "insert into co_rprovanu (cod_ven,rc_prov) values ('" + CmbVen.SelectedValue.ToString().Trim() + "','" + Tx_recibo.Text.Trim() + "');";
                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    MessageBox.Show("recibo:" + Tx_recibo.Text.Trim() + " anulado exitosamente");
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, 1, -1, -9, "anUlo el recibo provisional:" + Tx_recibo.Text, "");
                    CmbVen.SelectedIndex = -1;
                    Tx_recibo.Text = "";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al insertar:" + w);
            }
        }


        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }




    }
}
