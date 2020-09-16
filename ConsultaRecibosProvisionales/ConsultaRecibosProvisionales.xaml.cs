using Microsoft.Reporting.WinForms;
using Syncfusion.Windows.Tools.Controls;
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
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9609, "ConsultaRecibosProvisionales");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9609, "ConsultaRecibosProvisionales");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();

    public partial class ConsultaRecibosProvisionales : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int cosn = 0;
        public DataTable DTserver;
        public ConsultaRecibosProvisionales()
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
                this.Title = "Consulta Recibos Provicionales " + cod_empresa + "-" + nomempresa;


                DataTable dt = SiaWin.Func.SqlDT("select cod_mer as cod_ven,cod_mer+'-'+nom_mer as nom_ven from inmae_mer where estado=1  order by cod_mer", "inmae_mer", idemp);
                CmbVen.ItemsSource = dt.DefaultView;
                CmbVen.DisplayMemberPath = "nom_ven";
                CmbVen.SelectedValuePath = "cod_ven";

                Fec_ini.Text = DateTime.Now.AddMonths(-1).ToString();
                Fec_fin.Text = DateTime.Now.ToString("dd/MM/yyyy");

                DTserver = cargarDatosSerividor();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public DataTable cargarDatosSerividor()
        {
            DataTable dt = SiaWin.Func.SqlDT("select ServerIP, UserServer, UserServerPassword, UserSql, UserSqlPassword from ReportServer", "Empresas", 0);
            return dt;
        }


        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CmbVen.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione un vendedor");
                    return;
                }


                cosn++;
                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Consulta " + cosn.ToString();

                string ter = string.IsNullOrWhiteSpace(Tx_Tercer.Text) ? "" : "and ter.cod_ter = '" + Tx_Tercer.Text.Trim() + "'";

                parameters.Add(new ReportParameter("cod_ven", CmbVen.SelectedValue.ToString()));
                parameters.Add(new ReportParameter("fecha_ini", Fec_ini.Text));
                parameters.Add(new ReportParameter("fecha_fin", Fec_fin.Text));
                parameters.Add(new ReportParameter("cod_ter", ter));
                parameters.Add(new ReportParameter("codEmpresa", cod_empresa));

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");
                //viewer.ServerReport.ReportPath = "/Contabilidad/ConsultaRecibosProvisionales";
                viewer.ServerReport.ReportPath = "/Contabilidad/ConsultaRecibosProvisionalespruebas";
                


                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                viewer.ProcessingMode = ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(DTserver.Rows[0]["UserServer"].ToString(), DTserver.Rows[0]["UserServerPassword"].ToString());
                List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();

                foreach (var dataSource in viewer.ServerReport.GetDataSources())
                {
                    DataSourceCredentials credn = new DataSourceCredentials();
                    credn.Name = dataSource.Name;
                    System.Windows.MessageBox.Show(dataSource.Name);
                    credn.UserId = DTserver.Rows[0]["UserSql"].ToString();
                    credn.Password = DTserver.Rows[0]["UserSqlPassword"].ToString();
                    crdentials.Add(credn);
                }

                viewer.ServerReport.SetDataSourceCredentials(crdentials);
                viewer.ServerReport.SetParameters(parameters);
                viewer.RefreshReport();


                winFormsHost.Child = viewer;
                tabItemExt1.Content = winFormsHost;
                TabControl1.Items.Add(tabItemExt1);
                UpdateLayout();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {



                this.Close();


            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }


        private void Tx_Tercer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8 || e.Key == Key.Enter)
                {
                    string cmptabla = "comae_ter"; string cmpcodigo = "cod_ter"; string cmpnombre = "nom_ter"; string cmporden = "idrow"; string cmpidrow = "idrow"; string cmptitulo = "Maestra de tercero"; bool mostrartodo = false; string cmpwhere = "";
                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Width = 500;
                    winb.Height = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (!string.IsNullOrEmpty(code))
                    {
                        Tx_Tercer.Text = code.Trim();
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;
                    if (e.Key == Key.Enter)
                    {
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:" + w);
            }
        }

        private void Tx_Tercer_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace((sender as TextBox).Text)) return;

                string selec = "select * from comae_ter where cod_ter='" + (sender as TextBox).Text.Trim() + "' ";
                DataTable dt = SiaWin.Func.SqlDT(selec, "inmae_mer", idemp);
                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("el tercero ingresado no existe");
                    (sender as TextBox).Text = "";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al verificar");
            }
        }

        private void BtnConsultarSecondFormart_Click(object sender, RoutedEventArgs e)
        {
            if (CmbVen.SelectedIndex < 0)
            {
                MessageBox.Show("seleccione un vendedor");
                return;
            }

            cosn++;
            List<ReportParameter> parameters = new List<ReportParameter>();
            TabItemExt tabItemExt1 = new TabItemExt();
            tabItemExt1.Header = "Consulta " + cosn.ToString();

            string ter = string.IsNullOrWhiteSpace(Tx_Tercer.Text) ? "" : "and ter.cod_ter = '" + Tx_Tercer.Text.Trim() + "'";

            parameters.Add(new ReportParameter("cod_ven", CmbVen.SelectedValue.ToString()));
            parameters.Add(new ReportParameter("fecha_ini", Fec_ini.Text));
            parameters.Add(new ReportParameter("fecha_fin", Fec_fin.Text));
            parameters.Add(new ReportParameter("cod_ter", ter));
            parameters.Add(new ReportParameter("codEmpresa", cod_empresa));

            WindowsFormsHost winFormsHost = new WindowsFormsHost();
            ReportViewer viewer = new ReportViewer();
            viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");
            viewer.ServerReport.ReportPath = "/Contabilidad/ConsultaRecibosProvisionalespruebas";
            //viewer.ServerReport.ReportPath = "/Contabilidad/ConsultaRecibosProvisionalesArreglo";



            viewer.SetDisplayMode(DisplayMode.PrintLayout);
            viewer.ProcessingMode = ProcessingMode.Remote;
            ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
            rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(DTserver.Rows[0]["UserServer"].ToString(), DTserver.Rows[0]["UserServerPassword"].ToString());
            List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();

            foreach (var dataSource in viewer.ServerReport.GetDataSources())
            {
                DataSourceCredentials credn = new DataSourceCredentials();
                credn.Name = dataSource.Name;
                System.Windows.MessageBox.Show(dataSource.Name);
                credn.UserId = DTserver.Rows[0]["UserSql"].ToString();
                credn.Password = DTserver.Rows[0]["UserSqlPassword"].ToString();
                crdentials.Add(credn);
            }

            viewer.ServerReport.SetDataSourceCredentials(crdentials);
            viewer.ServerReport.SetParameters(parameters);
            viewer.RefreshReport();


            winFormsHost.Child = viewer;
            tabItemExt1.Content = winFormsHost;
            TabControl1.Items.Add(tabItemExt1);
            UpdateLayout();

        }










    }
}
