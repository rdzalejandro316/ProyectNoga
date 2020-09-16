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
using Microsoft.Reporting.WinForms;

namespace AnalisisDeCuentasPorPagar
{
    
    public partial class ReporteCxP : Window
    {
        dynamic SiaWin;
        public bool PrintOk = false;
        public DataTable DTserver;

        public ReporteCxP(List<ReportParameter> parameters, string reporteNombre)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;

            DTserver = cargarDatosSerividor();
            loaddocumento(parameters, reporteNombre);
        }

        public DataTable cargarDatosSerividor()
        {
            DataTable dt = SiaWin.Func.SqlDT("select ServerIP, UserServer, UserServerPassword, UserSql, UserSqlPassword from ReportServer", "Empresas", 0);
            return dt;
        }

        public int ZoomPercent { get; private set; } = 50;
        public void loaddocumento(List<ReportParameter> parameter, string reporteNombre)
        {
            try
            {
                viewer.Reset();
                string xnameReporte = reporteNombre;
                viewer.ServerReport.ReportPath = xnameReporte;
                viewer.ServerReport.ReportServerUrl = new Uri(DTserver.Rows[0]["ServerIP"].ToString().Trim());
                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                viewer.ProcessingMode = ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(DTserver.Rows[0]["UserServer"].ToString(), DTserver.Rows[0]["UserServerPassword"].ToString());
                List<DataSourceCredentials> crdentials = new List<DataSourceCredentials>();
                //List<ReportParameter> parameters = new List<ReportParameter>();
                viewer.ServerReport.SetParameters(parameter);
                foreach (var dataSource in viewer.ServerReport.GetDataSources())
                {
                    DataSourceCredentials credn = new DataSourceCredentials();
                    credn.Name = dataSource.Name;                   
                    credn.UserId = DTserver.Rows[0]["UserSql"].ToString();
                    credn.Password = DTserver.Rows[0]["UserSqlPassword"].ToString();
                    crdentials.Add(credn);
                }

                viewer.ServerReport.SetDataSourceCredentials(crdentials);                
                if (ZoomPercent > 0)
                {
                    viewer.ZoomMode = ZoomMode.Percent;
                    viewer.ZoomPercent = ZoomPercent;
                }                
                viewer.PrinterSettings.Collate = false;
                viewer.RefreshReport();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString(), "DocumentosReportes-loaddocumento");
            }
        }
        private void winFormsHost_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
                e.Handled = true;
            }
            if (e.Key == System.Windows.Input.Key.F6)
            {
                //AutoPrint();
                PrintOk = true;
                viewer.Focus();
            }
        }
        private void viewer_Print(object sender, ReportPrintEventArgs e)
        {

            PrintOk = true;
            viewer.Focus();
            //AuditoriaDoc(DocumentoIdCab, "Imprimio ", idEmp);
        }
    }
}
