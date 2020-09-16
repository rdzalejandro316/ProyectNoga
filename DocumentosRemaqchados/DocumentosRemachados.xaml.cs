using Syncfusion.UI.Xaml.Grid;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9550,"DocumentosRemachados");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9550, "DocumentosRemachados");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class DocumentosRemachados : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        int moduloid = 0;
        string cnEmp = "";
        string cod_empresa = "";

        DataTable dt_doc = new DataTable();

        public DocumentosRemachados()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Ingreso de Ordenes de Remachados " + cod_empresa + "-" + nomempresa;

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());

                Fec_ini.Text = DateTime.Now.ToString();
                Fec_fin.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string select = "select idrow,FEC_TRN as fec_trn,NUM_TRN as num_trn,COD_REF as cod_ref,BOD_DOC as bod_doc from InOrd_Pro where FEC_TRN between '" + Fec_ini.Text + "' and '" + Fec_fin.Text + " 23:59:59' ";
                dt_doc.Clear();
                dt_doc = SiaWin.Func.SqlDT(select, "temporal", idemp);

                if (dt_doc.Rows.Count > 0)
                {
                    GridConfig.ItemsSource = dt_doc.DefaultView;
                    Tx_total.Text = dt_doc.Rows.Count.ToString();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en la consulta");
            }
        }

        private void GridConfig_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F8)
            {
                GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;

                if (Colum.MappingName == "bod_doc")
                {
                    int idr = 0; string code = ""; string nombre = "";
                    dynamic xx = SiaWin.WindowBuscar("InMae_bod", "cod_bod", "nom_bod", "cod_bod", "idrow", "Maestra de Bodegas", cnEmp, true, "", idEmp: idemp);
                    xx.ShowInTaskbar = false;
                    xx.Owner = Application.Current.MainWindow;
                    xx.Height = 500;
                    xx.ShowDialog();
                    idr = xx.IdRowReturn;
                    code = xx.Codigo;
                    nombre = xx.Nombre;
                    if (idr > 0)
                    {
                        System.Data.DataRow dr = dt_doc.Rows[GridConfig.SelectedIndex];
                        dr.BeginEdit();
                        dr["bod_doc"] = code;
                        dr.EndEdit();
                    }
                }
            }
        }

        private void GridConfig_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {


            }
            catch (Exception w)
            {
                MessageBox.Show("errro al editar:" + w);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dt_doc.Rows.Count > 0)
                {
                    bool ban = true;
                    foreach (System.Data.DataRow row in dt_doc.Rows)
                    {
                        string bodega = row["bod_doc"].ToString();
                        string id = row["idrow"].ToString();                        

                        if (!string.IsNullOrEmpty(bodega))
                        {
                            if (SiaWin.Func.SqlCRUD("update InOrd_Pro set bod_doc='" + bodega + "' where idrow='" + id + "';", idemp) == false)
                            {
                                ban = false;
                            }
                            else
                            {
                                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, "CAMBIO DE BODEGA A LA ORDEN DE REMACHADO", "");
                            }                            
                        }
                    }

                    if (ban) {

                        MessageBox.Show("actualizacion exitosa");
                    }

                    dt_doc.Clear();
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("errro al editar:" + w);
            }
        }






    }
}
