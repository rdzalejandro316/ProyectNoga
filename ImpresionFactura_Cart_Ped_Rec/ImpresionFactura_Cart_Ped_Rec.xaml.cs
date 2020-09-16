using ImpresionFactura_Cart_Ped_Rec;
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
using System.Windows.Controls.Primitives;
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
    //Sia.PublicarPnt(9586,"ImpresionFactura_Cart_Ped_Rec");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9586,"ImpresionFactura_Cart_Ped_Rec");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class ImpresionFactura_Cart_Ped_Rec : Window
    {
        dynamic SiaWin;
        public int idemp = 0;        
        public string cnEmp = "";
        string cod_empresa = "";


        public string tercero = "";
        public string id_pedido = "";

        public string cod_pvt = "";


        DataTable DTserver;
        public ImpresionFactura_Cart_Ped_Rec()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            DTserver = cargarDatosSerividor();        
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();
                cartera();
                Recibos();
                if (!string.IsNullOrEmpty(id_pedido)) Pedido();                
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el load:" + w);
            }
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
                this.Title = "Informacion General - " + cod_empresa + "-" + nomempresa;                
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void cartera()
        {
            try
            {

                string user = ""; string pass = ""; string ip = "";
                

                ip = DTserver.Rows[0]["ServerIP"].ToString();
                user = DTserver.Rows[0]["UserServer"].ToString();
                pass = DTserver.Rows[0]["UserServerPassword"].ToString();

                

                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Reporte Cartera";                

                string path = "/PuntoDeVenta/Cartera_punto_pv";
                parameters.Add(new ReportParameter("Ter", tercero));
                parameters.Add(new ReportParameter("Cta", "13050505,280505"));
                parameters.Add(new ReportParameter("TipoApli", "-1"));
                parameters.Add(new ReportParameter("Resumen", "1"));
                parameters.Add(new ReportParameter("Fecha", DateTime.Now.ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("TrnCo", ""));
                parameters.Add(new ReportParameter("NumCo", ""));
                parameters.Add(new ReportParameter("Cco", ""));
                parameters.Add(new ReportParameter("Ven", ""));
                parameters.Add(new ReportParameter("codemp", cod_empresa));
                

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri(ip);
                viewer.ServerReport.ReportPath = path;                

                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                viewer.ProcessingMode = ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(user, pass);
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
                viewer.ZoomMode = ZoomMode.PageWidth;


                winFormsHost.Child = viewer;
                tabItemExt1.Content = winFormsHost;
                TabControlPricipal.Items.Add(tabItemExt1);
                UpdateLayout();

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el rerporte:" + w);
            }
        }

        public void Recibos()
        {
            try
            {

                string user = ""; string pass = ""; string ip = "";


                ip = DTserver.Rows[0]["ServerIP"].ToString();
                user = DTserver.Rows[0]["UserServer"].ToString();
                pass = DTserver.Rows[0]["UserServerPassword"].ToString();


                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Reporte Recibos";

                string path = "/PuntoDeVenta/RecibosProvisionales_punto_pv";
                parameters.Add(new ReportParameter("cod_ter", tercero));
                parameters.Add(new ReportParameter("fecha_ini", DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("fecha_fin", DateTime.Now.ToString("dd/MM/yyyy")));
                parameters.Add(new ReportParameter("codemp", cod_empresa));
                

                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri(ip);
                viewer.ServerReport.ReportPath = path;                

                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                viewer.ProcessingMode = ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(user, pass);
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
                viewer.ZoomMode = ZoomMode.PageWidth;

                winFormsHost.Child = viewer;
                tabItemExt1.Content = winFormsHost;
                TabControlPricipal.Items.Add(tabItemExt1);
                UpdateLayout();

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el rerporte:" + w);
            }
        }


        public void Pedido()
        {
            try
            {

                string user = ""; string pass = ""; string ip = "";


                ip = DTserver.Rows[0]["ServerIP"].ToString();
                user = DTserver.Rows[0]["UserServer"].ToString();
                pass = DTserver.Rows[0]["UserServerPassword"].ToString();


                List<ReportParameter> parameters = new List<ReportParameter>();
                TabItemExt tabItemExt1 = new TabItemExt();
                tabItemExt1.Header = "Reporte Pedido";

                string path = "/Otros/FrmDocumentos/PvPedidoCotiza";

                System.Text.StringBuilder _sqlcue = new System.Text.StringBuilder();
                _sqlcue.Append("select cue.idreg,cue.cod_bod,nom_bod,ref.cod_ref,ref.cod_ant,ref.cod_tip,tip.nom_tip,ref.cod_prv,ref.nom_ref,cue.cantidad,cue.val_uni,subtotal,val_iva,");
                _sqlcue.Append("cue.val_des,cue.por_des,cue.tot_tot,cue.cos_uni,cue.cos_tot,cue.val_riva,cue.val_ret,cue.val_ica from incue_doc as cue inner join incab_doc on incab_doc.idreg=cue.idregcab and incab_doc.idreg=" + id_pedido);
                _sqlcue.Append("inner join inmae_ref as ref on ref.cod_ref=cue.cod_ref inner join inmae_tip as tip on tip.cod_tip=ref.cod_tip inner join inmae_bod as bod on bod.cod_bod=cue.cod_bod order by cod_prv ");
                DataTable dtcue = SiaWin.Func.SqlDT(_sqlcue.ToString(), "cuerpo", idemp);
                //SiaWin.Browse(dtcue);                            
                decimal suma = dtcue.Compute("Sum(tot_tot)", "") == DBNull.Value ? 0 : Convert.ToDecimal(dtcue.Compute("Sum(tot_tot)", ""));
                decimal totalFac = dtcue.Rows.Count > 0 ? suma : 0;

                System.Text.StringBuilder _sqlcab = new System.Text.StringBuilder();
                _sqlcab.Append(" SELECT trn.nom_trn, cab.fec_trn, cab.fec_ven, cab.cod_trn, cab.num_trn, cab.cod_ven, cab.ord_comp, mer.nom_mer, ter.nom_ter, ter.cod_ter, ter.ciudad, ter.dir, ter.tel1, cab.for_pag, cab.val_ret, cab.val_riva, cab.val_rica, cab.fa_cufe, suc.cod_suc, nom_suc, suc.dir as dir_suc, dir_corres, suc.tel as tel_suc, fax, suc.cod_ven as cod_ven_suc, cod_rut, suc.cod_ciu as cod_ciu_suc, suc.estado as estado_suc, suc.cod_zona as cod_zona_suc,isnull(muni.nom_muni,'') as ciudad_suc ");
                _sqlcab.Append(" FROM InCab_doc AS cab left JOIN  InMae_mer AS mer ON mer.cod_mer = cab.cod_ven INNER JOIN InMae_trn AS trn ON trn.cod_trn = cab.cod_trn INNER JOIN Comae_ter AS ter ON ter.cod_ter = cab.cod_cli ");
                _sqlcab.Append(" left join inmae_suc as suc on suc.cod_ter = cab.cod_cli");
                _sqlcab.Append("  left join MmMae_muni as muni on muni.cod_depa=suc.cod_ciu ");
                _sqlcab.Append(" WHERE cab.idreg = " + id_pedido);

                parameters.Add(new ReportParameter("idregcab", id_pedido));
                parameters.Add(new ReportParameter("codemp", cod_empresa));
                parameters.Add(new ReportParameter("Tag1", _sqlcab.ToString()));
                parameters.Add(new ReportParameter("Tag2", _sqlcue.ToString()));
                parameters.Add(new ReportParameter("Tag3", "select * from inmae_bod where cod_bod='" + cod_pvt + "'"));
                parameters.Add(new ReportParameter("Tag4", "select * from copventas where cod_pvt='" + cod_pvt + "'"));
                parameters.Add(new ReportParameter("Tag5", SiaWin.Func.enletras(totalFac.ToString())));
                parameters.Add(new ReportParameter("usuario", SiaWin._UserAlias));
                parameters.Add(new ReportParameter("tituloPie", "ORGINAL              R"));                


                WindowsFormsHost winFormsHost = new WindowsFormsHost();
                ReportViewer viewer = new ReportViewer();
                viewer.ServerReport.ReportServerUrl = new Uri(ip);
                viewer.ServerReport.ReportPath = path;                

                viewer.SetDisplayMode(DisplayMode.PrintLayout);
                viewer.ProcessingMode = ProcessingMode.Remote;
                ReportServerCredentials rsCredentials = viewer.ServerReport.ReportServerCredentials;
                rsCredentials.NetworkCredentials = new System.Net.NetworkCredential(user, pass);
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
                viewer.ZoomMode = ZoomMode.PageWidth;

                winFormsHost.Child = viewer;
                tabItemExt1.Content = winFormsHost;
                TabControlPricipal.Items.Add(tabItemExt1);
                UpdateLayout();

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el rerporte:" + w);
            }
        }

        public DataTable cargarDatosSerividor()
        {
            DataTable dt = SiaWin.Func.SqlDT("select ServerIP, UserServer, UserServerPassword, UserSql, UserSqlPassword from ReportServer", "Empresas", 0);
            return dt;
        }









    }
}
