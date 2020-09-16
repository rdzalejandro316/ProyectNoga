using NotificationProcess;
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

    //Sia.PublicarPnt(10712,"NotificationProcess");
    //dynamic w = ((Inicio)Application.Current.MainWindow).WindowExt(10712,"NotificationProcess");
    //w.ShowInTaskbar = false;
    //w.Owner = Application.Current.MainWindow;
    //w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //w.ShowDialog(); 

    public partial class NotificationProcess : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public NotificationProcess()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (idemp <= 0) idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Configuracion de Correos-Tareas";
                LoadProcess();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void LoadProcess()
        {
            try
            {
                DataTable dt = SiaWin.Func.SqlDT("select codeprocess,nameprocess from ProcessEmailNotification", "Clientes", 0);
                GridProcess.ItemsSource = dt.DefaultView;
                TxTotProcess.Text = dt.Rows.Count.ToString();

                GridProcessEmail.ItemsSource = null;
                TxTotProcessEmail.Text = "0";
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }

        private void GridProcess_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (GridProcess.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)GridProcess.SelectedItems[0];
                    string process = row["codeprocess"].ToString().Trim();
                    LoadUserProcess(process);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al seccionar");
            }
        }

        public void LoadUserProcess(string code_process)
        {
            try
            {
                string query = "select noti.codeprocess,noti.userid,usu.username,usu.useralias,usu.email,noti.stateprocess ";
                query += "from ProcessUserNotification as noti ";
                query += "inner join Seg_User usu on usu.userId = noti.UserId ";
                query += "where noti.CodeProcess='" + code_process + "' ";

                DataTable dt = SiaWin.Func.SqlDT(query, "Clientes", 0);
                if (dt.Rows.Count > 0)
                {
                    GridProcessEmail.ItemsSource = dt.DefaultView;
                    TxTotProcessEmail.Text = dt.Rows.Count.ToString();
                }
                else
                {
                    GridProcessEmail.ItemsSource = null;
                    TxTotProcessEmail.Text = "0";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }

        private void Btnadd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = (sender as Button).Name.ToString().Trim();

                switch (name)
                {
                    case "BtnAdd":
                        AddProcess w = new AddProcess();
                        w.ShowInTaskbar = false;
                        w.Owner = Application.Current.MainWindow;
                        w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        w.ShowDialog();
                        if (w.refresh) LoadProcess();
                        break;
                    case "BtnAdd2":
                        if (GridProcess.SelectedIndex >= 0)
                        {
                            DataRowView row = (DataRowView)GridProcess.SelectedItems[0];
                            string process = row["codeprocess"].ToString();
                            AddUserProcess v = new AddUserProcess();
                            v.TxProcess.Text = process;
                            v.ShowInTaskbar = false;
                            v.Owner = Application.Current.MainWindow;
                            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            v.ShowDialog();
                            if (v.refresh) LoadUserProcess(process);
                        }
                        break;
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al agregar:" + w);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = (sender as Button).Name.ToString().Trim();

                switch (name)
                {
                    case "BtnEdit":
                        if (GridProcess.SelectedIndex >= 0)
                        {
                            DataRowView row = (DataRowView)GridProcess.SelectedItems[0];

                            AddProcess w = new AddProcess();
                            w.isedit = true;
                            w.TxCode.Text = row["codeprocess"].ToString().Trim();
                            w.TxName.Text = row["nameprocess"].ToString().Trim();
                            w.ShowInTaskbar = false;
                            w.Owner = Application.Current.MainWindow;
                            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            w.ShowDialog();
                            if (w.refresh) LoadProcess();
                        }
                        else { MessageBox.Show("seleccione un proceso para editarlo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                        break;
                    case "BtnEdit2":
                        if (GridProcessEmail.SelectedIndex >= 0)
                        {
                            DataRowView row = (DataRowView)GridProcessEmail.SelectedItems[0];
                            string process = row["codeprocess"].ToString();
                            string usuario = row["userid"].ToString();
                            int estado = Convert.ToInt32(row["stateprocess"]);
                            AddUserProcess v = new AddUserProcess();
                            v.TxProcess.Text = process;
                            v.usu = usuario;
                            v.isedit = true;
                            v.CHestado.IsChecked = estado == 1 ? true : false;
                            v.ShowInTaskbar = false;
                            v.Owner = Application.Current.MainWindow;
                            v.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            v.ShowDialog();
                            if (v.refresh) LoadUserProcess(process);
                        }
                        else { MessageBox.Show("seleccione un usuario para editarlo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
                        break;

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al agregar:" + w);
            }
        }


        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string name = (sender as Button).Name.ToString().Trim();
                switch (name)
                {
                    case "BtnDelete":
                        if (GridProcess.SelectedIndex >= 0)
                        {
                            DataRowView row = (DataRowView)GridProcess.SelectedItems[0];
                            string code = row["codeprocess"].ToString().Trim();
                            if (MessageBox.Show("Usted desea eliminar el proceso " + code + " ", "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                if (SiaWin.Func.SqlCRUD("delete ProcessEmailNotification where codeProcess='" + code + "' ", 0) == true)
                                {
                                    MessageBox.Show("Eliminacion exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                                    LoadProcess();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("seleccione un proceso para eliminarlo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        break;
                    case "BtnDelete2":
                        if (GridProcessEmail.SelectedIndex >= 0)
                        {
                            DataRowView row = (DataRowView)GridProcessEmail.SelectedItems[0];
                            string code = row["codeprocess"].ToString().Trim();
                            string userid = row["userid"].ToString().Trim();
                            if (MessageBox.Show("Usted desea eliminar el usuario " + userid + " del procesos:" + code, "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                if (SiaWin.Func.SqlCRUD("delete ProcessUserNotification where codeProcess='" + code + "' and userid='" + userid + "' ", 0) == true)
                                {
                                    MessageBox.Show("Eliminacion exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                                    LoadUserProcess(code);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("seleccione un usuario para eliminarlo", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        break;
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar:" + w);
            }
        }




    }
}



