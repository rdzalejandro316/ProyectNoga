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
using System.Windows.Shapes;

namespace AnalisisImportaciones
{
    
    public partial class Imprimir : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        public string cod_empresa = "";
        public string doc_impo = "";
        public decimal fac_impo = 0;

        public DataTable DTserver;
        public Imprimir(int idEmpresa)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = idEmpresa;            
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                //idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Impresion:" + cod_empresa + "-" + nomempresa;

                DTserver = cargarDatosSerividor();
            }
            catch (Exception e)
            {
                MessageBox.Show("aqui-" + e.Message);
            }
        }



        public DataTable cargarDatosSerividor()
        {
            DataTable dt = SiaWin.Func.SqlDT("select ServerIP, UserServer, UserServerPassword, UserSql, UserSqlPassword from ReportServer", "Empresas", 0);
            return dt;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Tx_Impor.Text = doc_impo;
            Tx_facImpo.Text = fac_impo.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TabControl2.Items.Clear();

                bool flag = false;
                foreach (CheckBox item in GridCheck.Children)
                {
                    if (item.IsChecked == true) flag = true;                    
                }

                if (flag == true)
                {
                    foreach (CheckBox item in GridCheck.Children)
                    {
                        if (item.IsChecked == true)
                        {
                            reporte(item.Tag.ToString());
                        }                        
                    }

                    tabItemExt2.IsSelected = true;
                }
                else
                {
                    MessageBox.Show("seleccione un reporte","alert", MessageBoxButton.OK,MessageBoxImage.Stop);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al imprimir:"+w);
            }
        }

        public void reporte(string tag)
        {
            try
            {                
                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();                

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");


                if (tag=="1")
                {
                    tabItemExt1.Header = "Contable";
                    viewer.ServerReport.ReportPath = "/Importacion/ContabilidadImportacion";                    
                    parameters.Add(new ReportParameter("codEmpresa", cod_empresa));
                    parameters.Add(new ReportParameter("num_import", Tx_Impor.Text));
                    parameters.Add(new ReportParameter("fac_impo", Tx_facImpo.Text));
                }
                if (tag == "2")
                {
                    tabItemExt1.Header = "Importacion";
                    viewer.ServerReport.ReportPath = "/Importacion/DocumentoImportacion";
                    parameters.Add(new ReportParameter("codEmpresa", cod_empresa));
                    parameters.Add(new ReportParameter("num_import", Tx_Impor.Text));
                    parameters.Add(new ReportParameter("fac_impo", Tx_facImpo.Text));
                }
                if (tag == "3")
                {
                    tabItemExt1.Header = "Lista Precios";
                    viewer.ServerReport.ReportPath = "/Importacion/ListPreciosImportacion";
                    parameters.Add(new ReportParameter("codEmpresa", cod_empresa));
                    parameters.Add(new ReportParameter("num_import", Tx_Impor.Text));
                    parameters.Add(new ReportParameter("fac_impo", Tx_facImpo.Text));
                }                



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
                    MessageBox.Show(DTserver.Rows[0]["UserSql"].ToString());
                    MessageBox.Show(DTserver.Rows[0]["UserSqlPassword"].ToString());
                    crdentials.Add(credn);
                }

                
                viewer.ServerReport.SetDataSourceCredentials(crdentials);
                viewer.ServerReport.SetParameters(parameters);
                viewer.RefreshReport();
                winFormsHost.Child = viewer;
                tabItemExt1.Content = winFormsHost;
                TabControl2.Items.Add(tabItemExt1);
                UpdateLayout();

            }
            catch (Exception w)
            {
                MessageBox.Show("errro en el reporte"+w);
            }
        }






    }
}
