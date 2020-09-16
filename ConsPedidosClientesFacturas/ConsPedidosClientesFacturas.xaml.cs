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
    //Sia.PublicarPnt(9632, "ConsPedidosClientesFacturas");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9632, "ConsPedidosClientesFacturas");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class ConsPedidosClientesFacturas : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int cosn = 0;
        public DataTable DTserver;
        public ConsPedidosClientesFacturas()
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
                this.Title = "Pedidos de Clientes con facturas" + cod_empresa + "-" + nomempresa;

                Fec_ini.Text = DateTime.Now.AddMonths(-1).ToString();
                Fec_fin.Text = DateTime.Now.ToString("dd/MM/yyyy");

                DataTable dt = SiaWin.Func.SqlDT("select cod_bod,RTRIM(nom_bod)+'-'+RTRIM(cod_bod) as nom_bod from inmae_bod where cod_emp='" + cod_empresa + "'; ", "inmae_mer", idemp);
                CmbBod.ItemsSource = dt.DefaultView;
                CmbBod.DisplayMemberPath = "nom_bod";
                CmbBod.SelectedValuePath = "cod_bod";

             
                Fec_ini_det.Text = DateTime.Now.AddMonths(-1).ToString();
                Fec_fin_det.Text = DateTime.Now.ToString("dd/MM/yyyy");

                CmbBodDet.ItemsSource = dt.DefaultView;
                CmbBodDet.DisplayMemberPath = "nom_bod";
                CmbBodDet.SelectedValuePath = "cod_bod";


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

                if (CmbBod.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una bodega");
                    return;
                }

                cosn++;
                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Consulta General - " + cosn.ToString();                

                parameters.Add(new ReportParameter("bod", CmbBod.SelectedValue.ToString()));
                parameters.Add(new ReportParameter("fec_ini", Fec_ini.Text));
                parameters.Add(new ReportParameter("fec_fin", Fec_fin.Text));                
                parameters.Add(new ReportParameter("codEmpresa", cod_empresa));

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");
                
                viewer.ServerReport.ReportPath = "/Inventarios/Pedidos/PedidosClientesFacturasGeneral";
                
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

        private void BtnConsultarDetallada_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (CmbBodDet.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una bodega");
                    return;
                }

                cosn++;
                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Consulta Detallada - " + cosn.ToString();

                parameters.Add(new ReportParameter("bod", CmbBodDet.SelectedValue.ToString()));
                parameters.Add(new ReportParameter("fec_ini", Fec_ini_det.Text));
                parameters.Add(new ReportParameter("fec_fin", Fec_fin_det.Text));
                parameters.Add(new ReportParameter("codEmpresa", cod_empresa));

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");

                viewer.ServerReport.ReportPath = "/Inventarios/Pedidos/PedidosClientesFacturasDetallada";

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


    }
}
