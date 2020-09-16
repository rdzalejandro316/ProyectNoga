using NotasEmpleados;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9681,"NotasEmpleados");    
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9681, "NotasEmpleados");
    //ww.ShowInTaskbar=false;    
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.cod_empleado = "1033796537";    
    //ww.ShowDialog(); 

    public partial class NotasEmpleados : Window
    {

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public string cod_empleado = "";

        public NotasEmpleados()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;

            this.MaxWidth = 500;
            this.MinWidth = 500;
        }



        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();

                if (string.IsNullOrEmpty(cod_empleado))
                {
                    Win.IsEnabled = false;
                    Txt_ocu.Visibility = Visibility.Visible;
                    return;
                }

                TX_empleado.Text = cod_empleado;
                getList(cod_empleado.ToString());
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar");
            }
        }



        public void getList(string empleado)
        {
            try
            {
                string select = "select ROW_NUMBER() OVER(ORDER BY idrow ASC) AS id,rtrim(cod_ter) as cod_ter,rtrim(fecha) as fecha,rtrim(title) as title,rtrim(nota) as nota,idrow from CoMae_terNota where cod_ter='" + empleado + "' ";
                DataTable tabla = SiaWin.Func.SqlDT(select, "Clientes", idemp);
                list.ItemsSource = tabla.Rows.Count > 0 ? tabla.DefaultView : null;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la consulta:" + w);
            }
        }




        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NotaAdd ww = new NotaAdd();
                ww.ShowInTaskbar = false;
                ww.Owner = Application.Current.MainWindow;
                ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ww.empleado = TX_empleado.Text.ToString();
                ww.ShowDialog();

                if (ww.actualizo == true) getList(TX_empleado.Text.ToString());
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir la pantalla de adicion:" + w, "alerta");
            }

        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool flag = false;
                string query = "";
                for (int i = 0; i < list.Items.Count; i++)
                {
                    ContentPresenter c = (ContentPresenter)list.ItemContainerGenerator.ContainerFromItem(list.Items[i]);
                    ToggleButton tb = c.ContentTemplate.FindName("btnYourButtonName", c) as ToggleButton;
                    { }

                    if (tb.IsChecked.Value)
                    {
                        query += "delete CoMae_terNota where idrow='" + tb.Tag + "';";
                        flag = true;
                    }
                }
                if (flag == true)
                {
                    if (MessageBox.Show("Usted desea eliminar las notas seleccionadas?", "Eliminar Notas", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                        {
                            MessageBox.Show("Eliminacion exitosa", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            getList(TX_empleado.Text.ToString());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("seleccione las notas que desea eliminar", "Opcion", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL ELIMINAR");
            }
        }










    }
}
