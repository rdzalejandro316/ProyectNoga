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

    //Sia.PublicarPnt(9621, "RecibosProvisionalesDescargados");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9621, "RecibosProvisionalesDescargados");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class RecibosProvisionalesDescargados : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int cosn = 0;
        public DataTable DTserver;
        public RecibosProvisionalesDescargados()
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
                this.Title = "Recibos Provicionales Descargados" + cod_empresa + "-" + nomempresa;


                DataTable dt = SiaWin.Func.SqlDT("select cod_mer as cod_ven,cod_mer+'-'+nom_mer as nom_ven from inmae_mer where estado=1  order by cod_mer", "inmae_mer", idemp);
                CmbVen.ItemsSource = dt.DefaultView;
                CmbVen.DisplayMemberPath = "nom_ven";
                CmbVen.SelectedValuePath = "cod_ven";
    
                Fec_ini.Text = DateTime.Now.AddMonths(-1).ToString();
                Fec_fin.Text = DateTime.Now.ToString("dd/MM/yyyy");


                DataTable dtPv = SiaWin.Func.SqlDT("select cod_pvt,cod_pvt+'-'+rtrim(nom_pvt) as nombre from Copventas where isPuntoVen='1'", "punto de venta", idemp);
                CBmPv.ItemsSource = dtPv.DefaultView;
                CBmPv.DisplayMemberPath = "nombre";
                CBmPv.SelectedValuePath = "cod_pvt";

                Fec_ini_pv.Text = DateTime.Now.AddMonths(-1).ToString();
                Fec_fin_pv.Text = DateTime.Now.ToString("dd/MM/yyyy");


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
                tabItemExt1.Header = "Consulta vendedor - " + cosn.ToString();

                string where = string.IsNullOrWhiteSpace(Tx_recibo.Text) ? " " : " and cabeza.rc_prov='" + Tx_recibo.Text.Trim() + "' ";

                parameters.Add(new ReportParameter("vend_pv", CmbVen.SelectedValue.ToString()));
                parameters.Add(new ReportParameter("FechaIni", Fec_ini.Text));
                parameters.Add(new ReportParameter("FechaFin", Fec_fin.Text));
                parameters.Add(new ReportParameter("where", where));
                parameters.Add(new ReportParameter("codEmpresa", cod_empresa));

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");

                string path = Incluir.IsChecked == true ? "/Contabilidad/RecibosDescargadosPendientes" : "/Contabilidad/RecibosDescargados";
                viewer.ServerReport.ReportPath = path;

                ///viewer.SetDisplayMode(DisplayMode.PrintLayout);
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
            this.Close();
        }

        private void Incluir_Checked(object sender, RoutedEventArgs e)
        {
            if (Incluir.IsChecked == true)
            {
                Tx_recibo.Text = "";
                Tx_recibo.IsEnabled = false;
            }
            else
            {
                Tx_recibo.Text = "";
                Tx_recibo.IsEnabled = true;
            }
        }

        private void BtnConsultarPv_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CBmPv.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione un punto de venta");
                    return;
                }

                cosn++;
                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Consulta Punto Venta " + cosn.ToString();

               // MessageBox.Show("ESTAMOS TRABAJO EN LA OPCION DE PUNTO DE VENTA PORFAVOR ESPERE Y NO LA USE");

                //string where = string.IsNullOrWhiteSpace(Tx_recibo.Text) ? " " : " and cabeza.rc_prov='" + Tx_recibo.Text.Trim() + "' ";

                parameters.Add(new ReportParameter("vend_pv", CBmPv.SelectedValue.ToString()));
                parameters.Add(new ReportParameter("FechaIni", Fec_ini_pv.Text));
                parameters.Add(new ReportParameter("FechaFin", Fec_fin_pv.Text));                
                parameters.Add(new ReportParameter("codEmpresa", cod_empresa));


                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");

                if (che_deta.IsChecked == true)
                    viewer.ServerReport.ReportPath = "/Contabilidad/RecibosCajaPuntoVenta";
                else
                    viewer.ServerReport.ReportPath = "/Contabilidad/RecibosCajaPuntoVentaDetallado";
                



                ///viewer.SetDisplayMode(DisplayMode.PrintLayout);
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
                MessageBox.Show("error al consultar contacte con el administrador:"+w);
            }
        }





    }
}
