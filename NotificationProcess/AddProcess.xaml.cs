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
    public partial class AddProcess : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public bool isedit = false;
        public bool refresh = false;

        public AddProcess()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isedit)
                {
                    TxTitle.Text = "EDITAR";
                    TxCode.IsEnabled = false;
                }
                else
                {
                    TxTitle.Text = "GUARDAR";
                    TxCode.IsEnabled = true;
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

                if (string.IsNullOrWhiteSpace(TxCode.Text))
                {
                    MessageBox.Show("debe de ingresar el codigo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(TxName.Text))
                {
                    MessageBox.Show("debe de ingresar el codigo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                #endregion

                if (isedit)
                {
                    if (SiaWin.Func.SqlCRUD("update ProcessEmailNotification set NameProcess='" + TxName.Text + "' where codeProcess='" + TxCode.Text + "' ", 0) == true)
                    {
                        MessageBox.Show("actulizacion exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                        refresh = true;
                        this.Close();
                    }
                }
                else
                {

                    DataTable dt = SiaWin.Func.SqlDT("select * from ProcessEmailNotification where codeProcess='" + TxCode.Text + "'", "Clientes", 0);
                    if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show("el codigo " + TxCode.Text + " ya existe ingrese otro", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }

                    string query = "insert into ProcessEmailNotification (codeProcess,NameProcess) values ('" + TxCode.Text + "','" + TxName.Text + "')";
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
