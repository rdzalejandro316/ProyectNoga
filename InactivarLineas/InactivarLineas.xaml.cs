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

//    Sia.PublicarPnt(9701,"InactivarLineas");
//    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9701,"InactivarLineas");
//    ww.ShowInTaskbar = false;
//    ww.Owner = Application.Current.MainWindow;
//    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
//    ww.ShowDialog();

namespace SiasoftAppExt
{

    public partial class InactivarLineas : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public InactivarLineas()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            cargarLinea();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
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
                this.Title = "Inactivar Linea - " + cod_empresa + "-" + nomempresa;

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el load" + w);
            }
        }

        public void cargarLinea()
        {
            DataTable dt = SiaWin.Func.SqlDT("select cod_tip,rtrim(cod_tip)+'-'+rtrim(nom_tip) as nom_tip from inmae_tip order by cod_tip", "table", idemp);
            CB_linea.ItemsSource = dt.DefaultView;
            CB_linea.DisplayMemberPath = "nom_tip";
            CB_linea.SelectedValuePath = "cod_tip";
        }

        private void BtnInactivar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_linea.SelectedIndex >= 0)
                {
                    string tit = CB_estado.SelectedIndex == 0 ? "InActivo" : "Activo";

                    if (MessageBox.Show("Usted desea "+ (CB_estado.SelectedIndex == 0 ? "Inactivar" : "Activar" )+" las referencias que tengan la linea "+CB_linea.SelectedValue.ToString(), "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {                        
                        int estado = CB_estado.SelectedIndex == 0 ? 0 : 1;

                        string update = "update inmae_ref set estado='" + estado + "' where cod_tip='" + CB_linea.SelectedValue.ToString() + "' ";

                        if (SiaWin.Func.SqlCRUD(update, idemp) == true)
                        {
                            MessageBox.Show("se inactivaron las referencias exitosamente");
                            SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, 2, -1, -9, tit + " LA LINEA :" + CB_linea.SelectedValue + "", "");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("seleccione una linea", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al incativar linea:" + w);
            }
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CB_linea.SelectedIndex >= 0)
                {
                    DataTable dt = SiaWin.Func.SqlDT("select estado,* from inmae_ref where cod_tip='" + CB_linea.SelectedValue.ToString() + "' ", "table", idemp);
                    if (dt.Rows.Count > 0)
                    {
                        SiaWin.Browse(dt);
                    }
                }
                else
                {
                    MessageBox.Show("seleccione una linea", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al ver las referencias de la linea:" + w);
            }
        }


    }
}
