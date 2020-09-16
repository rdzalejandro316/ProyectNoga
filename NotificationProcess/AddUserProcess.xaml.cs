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

namespace NotificationProcess
{
    public partial class AddUserProcess : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public bool isedit = false;
        public bool refresh = false;
        public string usu = "";

        public AddUserProcess()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = SiaWin.Func.SqlDT("select userid,rtrim(userid)+'-'+rtrim(username) as username From Seg_User", "Clientes", 0);
                CBuser.ItemsSource = dt.DefaultView;

                if (isedit)
                {
                    TxTitle.Text = "EDITAR";
                    CBuser.SelectedValue = usu;
                    CBuser.IsEnabled = false;
                }
                else
                {
                    TxTitle.Text = "GUARDAR";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones

                if (CBuser.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione un usuario", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                #endregion

                if (isedit)
                {

                    int estado = CHestado.IsChecked == true ? 1 : 0;
                    if (SiaWin.Func.SqlCRUD("update ProcessUserNotification set stateprocess='" + estado + "' where userid='" + CBuser.SelectedValue.ToString() + "' and  codeprocess='" + TxProcess.Text + "' ", 0) == true)
                    {
                        MessageBox.Show("actulizacion exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        refresh = true;
                        this.Close();
                    }
                }
                else
                {

                    DataTable dt = SiaWin.Func.SqlDT("select * from ProcessUserNotification where userid='" + CBuser.SelectedValue.ToString() + "' and  codeprocess='" + TxProcess.Text + "' ", "Clientes", 0);
                    if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("el usuario " + CBuser.SelectedValue.ToString() + " ya existe en el proceso:" + TxProcess.Text, "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    int estado = CHestado.IsChecked == true ? 1 : 0;
                    string query = "insert into ProcessUserNotification (userid,codeprocess,stateprocess) values ('" + CBuser.SelectedValue.ToString() + "','" + TxProcess.Text + "'," + estado + ")";
                    if (SiaWin.Func.SqlCRUD(query, 0) == true)
                    {
                        MessageBox.Show("insercion exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        refresh = true;
                        this.Close();
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
