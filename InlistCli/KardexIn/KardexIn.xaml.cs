using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
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

    /// Sia.PublicarPnt(9668,"KardexIn");
    /// Sia.TabU(9668);
    public partial class KardexIn : UserControl
    {

        dynamic SiaWin;
        dynamic tabitem;
        int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string sqlerror = "";
        string nitEmp = "";
        public KardexIn(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            idemp = SiaWin._BusinessId;
            CargarEmpresas();
            LoadConfig();
        }

        public void CargarEmpresas()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select businessid, businesscode, businessname, Businessalias from business where (select Seg_AccProjectBusiness.Access from Seg_AccProjectBusiness where GroupId = " + SiaWin._UserGroup.ToString() + "  and ProjectId = " + SiaWin._ProyectId.ToString() + " and Access = 1 and Business.BusinessId = Seg_AccProjectBusiness.BusinessId)= 1");
            DataTable empresas = SiaWin.Func.SqlDT(sb.ToString(), "Empresas", 0);
            comboBoxEmpresas.ItemsSource = empresas.DefaultView;
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                nitEmp = foundRow["BusinessNit"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Kardex Inv";
                Fec.Value = DateTime.Now.ToShortDateString();
                TabControl1.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        private async void BtnEjecutar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Fec.Value.ToString()))
                {
                    MessageBox.Show("llene los campos de las fecha", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                if (comboBoxEmpresas.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione una empresa", "filtro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                GridCosteo.ClearFilters();
                GridCosteo.ItemsSource = null;                
                BtnEjecutar.IsEnabled = false;

                DateTime fec = Convert.ToDateTime(Fec.Value.ToString());
                int fecha = fec.Year;
                DateTime per = Convert.ToDateTime(Periodo.Value);
                int periodo = per.Month;
                sqlerror = "";               
                string codemp = comboBoxEmpresas.SelectedValue.ToString();                
                
                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(fecha, periodo, codemp, source.Token), source.Token);
                await slowTask;
                BtnEjecutar.IsEnabled = true;                
                

                if (((DataSet)slowTask.Result) == null)
                {
                    BtnEjecutar.IsEnabled = true;
                    tabitem.Progreso(false);
                    this.sfBusyIndicator.IsBusy = false;
                    GridConfiguracion.IsEnabled = true;
                    if (sqlerror == "") MessageBox.Show("Error al cargar datos ó Periodo sin información:" + sqlerror);
                    if (sqlerror != "") MessageBox.Show(sqlerror);
                    return;
                }

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    GridCosteo.ItemsSource = ((DataSet)slowTask.Result).Tables[0];                    
                }
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
            }
            catch (SqlException ex)
            {                
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                //this.Opacity = 1;
                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                MessageBox.Show(ex.Message);
            }
        }

        private DataSet LoadData(int fecha, int periodo, string empresas, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpInKardex", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ano", fecha);
                cmd.Parameters.AddWithValue("@Per", periodo);                
                cmd.Parameters.AddWithValue("@codemp", empresas);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (SqlException ex)
            {
                sqlerror = ex.Message;
                return null;
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {




        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
