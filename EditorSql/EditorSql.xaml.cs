using Syncfusion.Windows.Edit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


//Sia.PublicarPnt(9630,"EditorSql");
//dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9630, "EditorSql");
//WinDescto.ShowInTaskbar = false;
//WinDescto.Owner = Application.Current.MainWindow;
//WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
//WinDescto.ShowDialog();


namespace SiasoftAppExt
{


    public class CustomIntelliSenseItem : IIntellisenseItem
    {

        public ImageSource Icon
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public IEnumerable<IIntellisenseItem> NestedItems
        {
            get;
            set;
        }

    }

    public partial class EditorSql : Window
    {

        dynamic SiaWin;
        int idemp = 0;
        string cnEmp = "";

        public EditorSql()
        {
            try
            {
                InitializeComponent();
                SiaWin = Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;
                LoadConfig();
                llenar();
            }
            catch (Exception w)
            {
                MessageBox.Show("1:" + w);
            }
        }

        private void LoadConfig()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                //Boolean a = Convert.ToBoolean(foundRow["BusinessId"]);
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "aquiio");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = EditControl1.Text;
                DataTable dt = SqlDT(query, "temporal", idemp);
                Grid.Visibility = Visibility.Visible;

                Grid.ItemsSource = dt.DefaultView;
                TOTAL.Text = dt.Rows.Count.ToString();
            }
            catch (Exception w)
            {
                MessageBox.Show("2:" + w);
            }



        }

        //public void llenar()
        //{
        //    try
        //    {
        //        ObservableCollection<CustomIntelliSenseItem> tablas = new ObservableCollection<CustomIntelliSenseItem>();
        //        DataTable DtTablas = SiaWin.Func.SqlDT("SELECT TABLE_NAME as tablas FROM  INFORMATION_SCHEMA.TABLES order by TABLE_NAME ", "Tablas", idemp);

        //        foreach (DataRow row in DtTablas.Rows)
        //        {
        //            ObservableCollection<CustomIntelliSenseItem> columnas = new ObservableCollection<CustomIntelliSenseItem>();
        //            string query = "select column_name as columnas from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" + row["tablas"].ToString() + "' order by column_name ";
        //            DataTable DtColumnas = SiaWin.Func.SqlDT(query, "Columnas", idemp);
        //            foreach (DataRow rowColum in DtColumnas.Rows)
        //            {
        //                columnas.Add(new CustomIntelliSenseItem() { Text = rowColum["columnas"].ToString() });
        //            }
        //            tablas.Add(new CustomIntelliSenseItem() { Text = row["tablas"].ToString(), NestedItems = columnas });
        //        }

        //        EditControl1.IntellisenseCustomItemsSource = tablas;
        //    }
        //    catch (Exception w)
        //    {

        //        MessageBox.Show("3:" + w);
        //    }

        //}

        public void llenar()
        {
            try
            {
                ObservableCollection<CustomIntelliSenseItem> tablas = new ObservableCollection<CustomIntelliSenseItem>();
                DataTable DtTablas = SiaWin.Func.SqlDT("SELECT TABLE_NAME as tablas FROM  INFORMATION_SCHEMA.TABLES order by TABLE_NAME ", "Tablas", idemp);

                foreach (DataRow row in DtTablas.Rows)
                {
                    ObservableCollection<CustomIntelliSenseItem> columnas = new ObservableCollection<CustomIntelliSenseItem>();
                    string query = "select column_name as columnas from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" + row["tablas"].ToString() + "' order by column_name ";
                    DataTable DtColumnas = SiaWin.Func.SqlDT(query, "Columnas", idemp);
                    foreach (DataRow rowColum in DtColumnas.Rows)
                    {
                        columnas.Add(new CustomIntelliSenseItem() { Text = rowColum["columnas"].ToString() });
                    }
                    tablas.Add(new CustomIntelliSenseItem() { Text = row["tablas"].ToString(), NestedItems = columnas });
                }

                EditControl1.IntellisenseCustomItemsSource = tablas;
            }
            catch (Exception w)
            {

                MessageBox.Show("3:" + w);
            }

        }

        public Boolean SqlCRUD(string _query, int IdBuss)
        {
            try
            {

                string cn = null;
                if (IdBuss <= 0) cn = SiaWin.Func.ConfiguracionApp();
                if (IdBuss > 0) cn = SiaWin.Func.DatosEmp(IdBuss);

                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(cn);
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = _query;
                cmd.Connection = conn;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el global code funcion SqlCRUD:" + w);
                return false;
            }
        }

        public DataTable SqlDT(string _sql, string nomtabla, int IdBuss)
        {
            _sql = SiaWin.Func.RutaTablaBase(_sql);
            DataTable dt = new DataTable(nomtabla);
            DataRow row;
            try
            {
                using (SqlConnection sqlCon = new System.Data.SqlClient.SqlConnection(SiaWin.Func.DatosEmp(IdBuss)))
                {
                    using (SqlDataAdapter SqlDa = new SqlDataAdapter(_sql, sqlCon))
                    {
                        SqlDa.Fill(dt);
                    }
                }
            }
            catch (SqlException SQLex)
            {
                dt = new DataTable("Error");
                dt.Columns.Add("Error", typeof(String));
                row = dt.NewRow();
                row["Error"] = "Error:" + SQLex.Message;
                dt.Rows.Add(row);
                MessageBox.Show(SQLex.Message);
                dt = null;

            }
            catch (Exception ex)
            {
                dt = new DataTable("Error");
                dt.Columns.Add("Error", typeof(String));
                row = dt.NewRow();
                row["Error"] = "Error:" + ex.Message;
                dt.Rows.Add(row);
                dt = null;
            }
            return dt;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;
                EditControl1.IsEnabled = false;

                var slowTask = Task<ObservableCollection<CustomIntelliSenseItem>>.Factory.StartNew(() => SlowDude(source.Token), source.Token);
                await slowTask;

                if (((ObservableCollection<CustomIntelliSenseItem>)slowTask.Result).Count > 0)
                {
                    EditControl1.IntellisenseCustomItemsSource = ((ObservableCollection<CustomIntelliSenseItem>)slowTask.Result);
                }
                EditControl1.IsEnabled = true;
                this.sfBusyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Errror en el load" + ex);

            }
        }

        private ObservableCollection<CustomIntelliSenseItem> SlowDude(CancellationToken cancellationToken)
        {
            try
            {
                ObservableCollection<CustomIntelliSenseItem> jj = LoadData(cancellationToken);
                return jj;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }

        private ObservableCollection<CustomIntelliSenseItem> LoadData(CancellationToken cancellationToken)
        {
            try
            {
                ObservableCollection<CustomIntelliSenseItem> tablas = new ObservableCollection<CustomIntelliSenseItem>();
                DataTable DtTablas = SiaWin.Func.SqlDT("SELECT TABLE_NAME as tablas FROM  INFORMATION_SCHEMA.TABLES order by TABLE_NAME ", "Tablas", idemp);

                foreach (DataRow row in DtTablas.Rows)
                {
                    string query = "select column_name as columnas from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='" + row["tablas"].ToString() + "' order by column_name ";
                    DataTable DtColumnas = SiaWin.Func.SqlDT(query, "Columnas", idemp);
                    ObservableCollection<CustomIntelliSenseItem> columnas = new ObservableCollection<CustomIntelliSenseItem>();
                    foreach (DataRow rowColum in DtColumnas.Rows)
                    {
                        columnas.Add(new CustomIntelliSenseItem() { Text = rowColum["columnas"].ToString() });
                    }
                    tablas.Add(new CustomIntelliSenseItem() { Text = row["tablas"].ToString(), NestedItems = columnas });
                }
                return tablas;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }







    }
}
