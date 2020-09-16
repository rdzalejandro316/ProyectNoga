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

namespace RecibosDeCaja
{
    public partial class ViewDocuments : Window
    {
        public dynamic SiaWin;
        public int idemp = 0;
        public string BusinessCode;
        public DataTable dt;
        public DataTable DTserver;
        public ViewDocuments()
        {
            InitializeComponent();
        }

        public DataTable cargarDatosSerividor()
        {
            DataTable dt = SiaWin.Func.SqlDT("select ServerIP, UserServer, UserServerPassword, UserSql, UserSqlPassword from ReportServer", "Empresas", 0);
            return dt;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DTserver = cargarDatosSerividor();

                foreach (DataRow dr in dt.Rows)
                {
                    int idreg = Convert.ToInt32(dr["idreg"]);
                    string num_trn = dr["num_trn"].ToString();
                    string cod_trn  = dr["cod_trn"].ToString();
                    string cod_ven = dr["cod_ven"].ToString();
                    string nom_ven = dr["nom_ven"].ToString();

                    if (idreg > 0) ImprimeRC(idreg,cod_trn,num_trn,cod_ven,nom_ven);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:"+w);
            }
        }


        private void ImprimeRC(int idregcab,string cod_trn,string num_trn, string cod_ven, string nom_ven)
        {
            try
            {                
                string _codtrn = cod_trn;
                string _numtrn = num_trn;
                string _codven = cod_ven;
                string _nomven = nom_ven;                
              
                if (_codtrn == "")
                {
                    MessageBox.Show("El documento no existe...", "ImprimeRC");
                    return;
                }
                             
                string sqltext = "select doc_cruc as facturas from CoCab_doc ";
                sqltext += " inner join cocue_doc on cocue_doc.idregcab = cocab_doc.idreg ";
                sqltext += " where cocab_doc.cod_trn = '" + _codtrn + "' and cocab_doc.num_trn = '" + _numtrn + "' and rtrim(doc_cruc)<> '' ";
                sqltext += " group by doc_cruc ";

                DataTable dtfacturas = SiaWin.DB.SqlDT(sqltext, "tmp", idemp);
                string _Facturas = "";
                if (dtfacturas.Rows.Count > 0)
                {
                    int com = 1;
                    foreach (System.Data.DataRow item in dtfacturas.Rows)
                    {
                        string coma = com == 1 ? "" : ",";
                        _Facturas += coma + item["facturas"].ToString().Trim() + "";
                        com++;
                    }
                }


                string sqltexttotal = @"select sum(iif(substring(cod_cta, 1, 2) = '11', deb_mov, 0)) as total from cocab_doc inner join cocue_doc on cocue_doc.idregcab=cocab_doc.idreg where cocab_doc.cod_trn='" + _codtrn + "' and cocab_doc.num_trn='" + _numtrn + "'";
                DataTable dtTotal = SiaWin.DB.SqlDT(sqltexttotal, "tmp", idemp);
                decimal totalfac = 0;
                if (dtTotal.Rows.Count > 0)
                {
                    totalfac = (decimal)dtTotal.Rows[0]["total"];
                }

                #region parametros                
                string enletras = SiaWin.Func.enletras(totalfac.ToString());  //valor en letra

                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(BusinessCode);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);

                ReportParameter paramcodtrn = new ReportParameter();
                paramcodtrn.Values.Add(_codtrn);
                paramcodtrn.Name = "codtrn";
                parameters.Add(paramcodtrn);
                ReportParameter paramnumtrn = new ReportParameter();
                paramnumtrn.Values.Add(_numtrn);
                paramnumtrn.Name = "numtrn";
                parameters.Add(paramnumtrn);

                ReportParameter paramFacturas = new ReportParameter();
                paramFacturas.Values.Add(_Facturas);
                paramFacturas.Name = "Facturas";
                parameters.Add(paramFacturas);

                ReportParameter paramValorLetras = new ReportParameter();
                paramValorLetras.Values.Add(enletras);
                paramValorLetras.Name = "ValorLetras";
                parameters.Add(paramValorLetras);

                #endregion

                string repnom = @"/Contabilidad/ReciboDeCajaOficial";                

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");
                viewer.ServerReport.ReportPath = repnom;
                viewer.ShowParameterPrompts = false;                

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

                
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "DOC:"+ _numtrn;

                viewer.ServerReport.SetDataSourceCredentials(crdentials);
                viewer.ServerReport.SetParameters(parameters);
                viewer.RefreshReport();
                winFormsHost.Child = viewer;
                tabItemExt1.Content = winFormsHost;
                TabControl1.Items.Add(tabItemExt1);
                UpdateLayout();

                //SiaWin.Reportes(parameters, repnom, TituloReporte: TituloReport, Modal: true, idemp: idemp, ZoomPercent: 50);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }

        }


    


    }
}
