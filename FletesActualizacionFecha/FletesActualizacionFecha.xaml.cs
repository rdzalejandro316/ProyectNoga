using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
    //    Sia.PublicarPnt(9629,"FletesActualizacionFecha");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9629,"FletesActualizacionFecha");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();
    public partial class FletesActualizacionFecha : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public FletesActualizacionFecha()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            //MessageBox.Show("estamos en mantenimiento por favor espere");
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
                this.Title = "Actualizacion de fechas " + cod_empresa + "-" + nomempresa;

                Tx_fecini.Text = DateTime.Now.ToString();
                Tx_fecfin.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void Btnsearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "SELECT cab.idreg,cab.num_trn,convert(varchar,cab.fec_envi,103) as fec_envi ";
                query += "FROM InCab_doc cab ";
                query += "WHERE NOT EXISTS (SELECT NULL FROM indet_fle fletes  WHERE fletes.n_fra = cab.num_trn) ";
                query += "and  fec_trn between '"+Tx_fecini.Text+"' and '"+ Tx_fecfin.Text + " 23:59:59' ";
                query += "and cab.cod_trn='005' ";
                query += "order by cab.num_trn ";

                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);

                if (dt.Rows.Count>0)
                {
                    dataGridFlete.ItemsSource = dt.DefaultView;
                    Tx_Rows.Text = dt.Rows.Count.ToString();
                }
                else
                {
                    dataGridFlete.ItemsSource = null;
                    Tx_Rows.Text = "0";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en la busqueda:"+w);
            }
        }

        private void dataGridFlete_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {
                var reflector = this.dataGridFlete.View.GetPropertyAccessProvider();
                var rowData = dataGridFlete.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex);                
                if (DBNull.Value.Equals(reflector.GetValue(rowData, "fec_envi"))) return;

                string fecha = reflector.GetValue(rowData, "fec_envi").ToString();
                
                if (string.IsNullOrEmpty(fecha)) return;

                DateTime fs; string format = "ddMMyyyy";
                
                if (DateTime.TryParseExact(fecha, format , CultureInfo.InvariantCulture , DateTimeStyles.None, out fs) == false)
                {
                    MessageBox.Show("lo que introdujo en el campo 'fecha de entrega' no es una fecha por favor verifique el formato dela fecha es dd/mm/yyyy ","alert",MessageBoxButton.OK,MessageBoxImage.Stop);
                    reflector.SetValue(rowData, "fec_envi","");
                }
                else
                {
                    //MessageBox.Show(fs.ToString());
                    string query = "update incab_doc set fec_envi='"+ fs.ToString("dd/MM/yyyy") + "' where idreg='"+ reflector.GetValue(rowData, "idreg").ToString() +"' ";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == false)
                    {
                        MessageBox.Show("error al actualizar contacte con el administrador","alert",MessageBoxButton.OK,MessageBoxImage.Error);
                    }
                    else
                    {
                        DataRowView dr = (DataRowView)dataGridFlete.SelectedItems[0];
                        dr.BeginEdit();
                        dr["fec_envi"] = fs.ToString("dd/MM/yyyy"); 
                        dr.EndEdit();
                    }
                }
                
            }
            catch (Exception w)
            {
                MessageBox.Show("error al editar:"+w);
            }
        }



    }
}

