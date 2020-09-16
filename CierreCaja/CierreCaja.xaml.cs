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

    //Sia.PublicarPnt(9619, "CierreCaja");
    //Sia.TabU(9619);

    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9619, "CierreCaja");  
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class CierreCaja : UserControl
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        dynamic tabitem;
        public DataTable DTserver;
        public CierreCaja(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.MultiTab = true;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            if (tabitem.idemp <= 0) idemp = SiaWin._BusinessId;

            tabitem.Title = "Cierre de caja";
            //tabitem.Logo(9, ".png");
            //idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "ciere de caja (" + aliasemp + ")";
                //this.Title = "Cierre de caja " + cod_empresa + "-" + nomempresa;

                Tx_fecierre.Text = DateTime.Now.ToString("dd/MM/yyyy");
                Tx_consg.Text = DateTime.Now.ToString("dd/MM/yyyy");

                DTserver = cargarDatosSerividor();

                loadDate();
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

        public void loadCons()
        {
            try
            {
                string query = "select fec_rc,fec_cons,con_cie from Co_confi";
                DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
                if (dt.Rows.Count > 0)
                {
                    Tx_consecutivo.Text = dt.Rows[0]["con_cie"].ToString();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar las fechas");
            }
        }

        public void loadDate()
        {
            try
            {
                string query = "select fec_rc,fec_cons,con_cie from Co_confi";
                DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
                if (dt.Rows.Count > 0)
                {
                    Tx_cierre_actual.Text = dt.Rows[0]["fec_rc"].ToString();
                    Tx_consi_actual.Text = dt.Rows[0]["fec_cons"].ToString();
                    Tx_consecutivo.Text = dt.Rows[0]["con_cie"].ToString();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar las fechas");
            }
        }

        public bool validRg()
        {
            string query = "select * from CoCie_caja where fecha='" + Tx_fecierre.Text + "'; ";
            DataTable dt = SiaWin.Func.SqlDT(query, "valida", idemp);
            return dt.Rows.Count > 0 ? true : false;
        }
        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //TabControl1
                TabControl2.Items.Clear();
                bool bandera = false;

                bool flag = false;
                if (validRg() == true && ComCiere.SelectedIndex == 1)
                {
                    MessageBox.Show("caja cerrada");
                    MessageBox.Show("la caja ya esta cerrada", "alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, 2, -1, -9, " la caja del "+ Tx_fecierre.Text + " ya esta cerrada de la empresa:"+idemp);
                    flag = true;
                }

                if (flag == false)
                {
                    if (ComCiere.SelectedIndex == 1)
                    {

                        DateTime fcon = Convert.ToDateTime(Tx_consi_actual.Text);

                        int dia = getday();
                        DateTime dt = Convert.ToDateTime(Tx_consi_actual.Text).AddDays(dia);
                        
                        if (MessageBox.Show("esta seguro de realizar el cierre de caja de la fecha:" + Tx_fecierre.Text, "Ejecutar Cierre", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {                        
                            string update = "update Co_confi set fec_rc='" + Tx_consg.Text + "',fec_cons='" + dt.ToString("dd/MM/yyyy") + "',con_cie=ISNULL(con_cie,0)+1;";
                            if (SiaWin.Func.SqlCRUD(update, idemp) == true)
                            {                         
                                int con = Convert.ToInt32(Tx_consecutivo.Text)+1;
                                string insert = "insert into CoCie_caja (fecha,ind_cie,consecutivo) values ('" + Tx_consi_actual.Text + "','1','" + con + "')";
                                if (SiaWin.Func.SqlCRUD(insert, idemp) == true)
                                {
                                    MessageBox.Show("se realizo el cierre de la fecha:" + Tx_fecierre.Text);
                                    //SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, 2, -1, -9, "se realizo el cierre de la fecha:" + Tx_fecierre.Text+" de la empresa:"+idemp);
                                    loadCons();
                                    bandera = true;
                                }
                            }
                        }
                    }
                }

                //return;
                //MessageBox.Show("A4");  
                reporte1(flag);
                reporte2(flag);

                foreach (CheckBox item in GridCheck.Children)
                {
                    if (item.IsChecked == true)
                    {
                        string name = item.Name;
                        if (name == "R1") reporte3(flag);
                        if (name == "R2") reporte4(flag);
                    }

                }

                tabItemExt2.IsSelected = true;

                if (bandera == true)
                {
                    loadDate();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }

        public string consecutivo()
        {
            string con = "";

            bool c = cajaCerrada(Tx_fecierre.Text);
            
            if (c == true)
            {
                string query = "select consecutivo from CoCie_caja where fecha='"+ Tx_fecierre.Text + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "consecutivos", idemp);
                if (dt.Rows.Count > 0)
                {
                    con = dt.Rows[0]["consecutivo"].ToString();
                    //MessageBox.Show("cogio consecutivo antiguo");
                }
            }
            else
            {
                con = Tx_consecutivo.Text;
            }
            return con;
        }


        public bool cajaCerrada(string fecha)
        {
            string query = "select consecutivo from CoCie_caja where fecha='" + fecha + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "consecutivos", idemp);
            return dt.Rows.Count > 0 ? true : false;
        }


        public void reporte1(bool flag)
        {
            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Consulta ";
                tabItemExt1.Name = "tab1";
                parameters.Add(new ReportParameter("fec_con", Tx_consg.Text));
                parameters.Add(new ReportParameter("fec_cierre", Tx_fecierre.Text));
                parameters.Add(new ReportParameter("consecutivo", consecutivo()));
                parameters.Add(new ReportParameter("codEmpresa", cod_empresa));

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");
                viewer.ServerReport.ReportPath = "/Contabilidad/CierreRecibosCaja";

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
                TabControl2.Items.Add(tabItemExt1);
                UpdateLayout();

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el reporte 1:" + w);
            }
        }

        public void reporte2(bool flag)
        {
            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Recaudo Domicilio";
                tabItemExt1.Name = "tab2";
                parameters.Add(new ReportParameter("fec_con", Tx_consg.Text));
                parameters.Add(new ReportParameter("fec_cierre", Tx_fecierre.Text));
                parameters.Add(new ReportParameter("consecutivo", consecutivo()));
                parameters.Add(new ReportParameter("codEmpresa", cod_empresa));

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");
                viewer.ServerReport.ReportPath = "/Contabilidad/CierreRecibosCajaDomicilio";

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
                TabControl2.Items.Add(tabItemExt1);
                UpdateLayout();

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el reporte 2:" + w);
            }
        }

        public void reporte3(bool flag)
        {
            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Consignacion";
                parameters.Add(new ReportParameter("fec_con", Tx_consg.Text));
                parameters.Add(new ReportParameter("fec_cierre", Tx_fecierre.Text));
                parameters.Add(new ReportParameter("consecutivo", consecutivo()));
                parameters.Add(new ReportParameter("codEmpresa", cod_empresa));

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");
                viewer.ServerReport.ReportPath = "/Contabilidad/CierreRecibosCajaConsignacion";

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
                TabControl2.Items.Add(tabItemExt1);
                UpdateLayout();

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el reporte 2:" + w);
            }
        }

        public void reporte4(bool flag)
        {
            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Cheques Postfechados";
                parameters.Add(new ReportParameter("fec_con", Tx_consg.Text));
                parameters.Add(new ReportParameter("fec_cierre", Tx_fecierre.Text));
                parameters.Add(new ReportParameter("consecutivo", consecutivo()));
                parameters.Add(new ReportParameter("codEmpresa", cod_empresa));

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.12:7333/ReportserverGS");
                viewer.ServerReport.ReportPath = "/Contabilidad/CierreRecibosCajaChePost";

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
                TabControl2.Items.Add(tabItemExt1);
                UpdateLayout();

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el reporte 2:" + w);
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {            
            if (SiaWin._UserId == 21)
            {                
                int dia = getday();
                DateTime dt = Convert.ToDateTime(Tx_consi_actual.Text).AddDays(dia);               
            }
            else
            {
                tabitem.Cerrar(0);
            }
        }

        public int getday()
        {
            int dia = 1;
            DateTime d = Convert.ToDateTime(Tx_consi_actual.Text);
            if (DayOfWeek.Saturday == d.DayOfWeek) dia++;

            string query = "select * from CoMae_fes where fecha='" + d.AddDays(dia) + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "festivos", idemp);
            if (dt.Rows.Count > 0) dia++;
            return dia;
        }





    }
}
