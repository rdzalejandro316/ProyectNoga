using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace AnalisisImportaciones
{
    public partial class Buscar : Window
    {
        dynamic SiaWin;
        public int idemp = 0;

        public DataRowView row;
        public bool selecciono = false;

        public bool PntImportacion = false;
        public string n_importacion = "";

        public Buscar(int idempresa)
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = idempresa;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SiaWin = System.Windows.Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId;

            if (PntImportacion == true)
            {
                GridImportaciones.Visibility = Visibility.Visible;
                GridDocImportacion.Visibility = Visibility.Hidden;
                try
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    CancellationToken token = source.Token;
                    dataGridSearch.IsEnabled = false;
                    sfBusyIndicator.IsBusy = true;

                    var slowTask = Task<DataTable>.Factory.StartNew(() => LoadData(source.Token), source.Token);
                    await slowTask;

                    if (((DataTable)slowTask.Result).Rows.Count > 0)
                    {
                        dataGridSearch.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                        Tx_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                    }

                    this.sfBusyIndicator.IsBusy = false;
                    dataGridSearch.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    this.sfBusyIndicator.IsBusy = false;
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                GridDocImportacion.Visibility = Visibility.Visible;
                GridImportaciones.Visibility = Visibility.Hidden;
                Tx_title.Text = "Documentos de la importacion " + n_importacion.Trim();

                try
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    CancellationToken token = source.Token;
                    dataGridSearchDoc.IsEnabled = false;
                    sfBusyIndicatorDoc.IsBusy = true;

                    string impo = n_importacion;

                    var slowTask = Task<DataTable>.Factory.StartNew(() => LoadDataDocum(impo, source.Token), source.Token);
                    await slowTask;

                    if (((DataTable)slowTask.Result).Rows.Count > 0)
                    {
                        dataGridSearchDoc.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                        Tx_totalDoc.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                    }

                    sfBusyIndicatorDoc.IsBusy = false;
                    dataGridSearchDoc.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    sfBusyIndicatorDoc.IsBusy = false;
                    MessageBox.Show(ex.Message);
                }
            }            
        }

        

        private DataTable LoadData(CancellationToken cancellationToken)
        {
            try
            {               
                //string query = "select cod_trn, n_imp, fec_trn From incab_doc ";
                string query = "select n_imp From incab_doc ";
                query += "where cod_trn = '980' ";
                query += "group by n_imp";
                //query += "order by fec_trn ";

                DataTable dt = SiaWin.Func.SqlDT(query, "Documentos", idemp);
                return dt;
            }
            catch (Exception e)
            {                
                this.sfBusyIndicator.IsBusy = false;             
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private DataTable LoadDataDocum(string importacion,CancellationToken cancellationToken)
        {
            try
            {
                string query = "select cod_trn,num_trn,tc,fec_trn,cod_prv from InCab_doc where n_imp = '" + importacion+"'";                
                DataTable dt = SiaWin.Func.SqlDT(query, "Documentos", idemp);
                return dt;
            }
            catch (Exception e)
            {
                sfBusyIndicatorDoc.IsBusy = false;
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void BtnSel_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridSearch.SelectedIndex>=0)
            {
                row = (DataRowView)dataGridSearch.SelectedItems[0];
                selecciono = true;
                this.Close();
            }
        }

        private void dataGrid_FilterChanged(object sender, GridFilterEventArgs e)
        {
            if ((sender as SfDataGrid).Name == "dataGridSearchDoc")            
                Tx_totalDoc.Text = (sender as SfDataGrid).View.Records.Count.ToString();            
            else            
                Tx_total.Text = (sender as SfDataGrid).View.Records.Count.ToString();            
        }

        private void BtnDoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridSearchDoc.SelectedItems[0];
                if (row == null) return;
                string num_trn = row["num_trn"].ToString();
                string cod_trn = row["cod_trn"].ToString();
                TraeDocumento(cod_trn, num_trn, "in", idemp);
                return;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el kardex:" + w);
            }
        }
        public void TraeDocumento(string cod_trn, string num_trn, string modulo, int idemp)
        {
            int _idregcab = 0;
            DataTable dtAud = new DataTable();
            dtAud = SiaWin.Func.SqlDT("select idreg,cod_trn,num_trn,fec_trn from " + modulo.Trim() + "cab_doc where cod_trn='" + cod_trn + "' and  num_trn='" + num_trn + "'", "tmp", idemp);
            if (dtAud.Rows.Count > 0)
            {                
                _idregcab = Convert.ToInt32(dtAud.Rows[0]["idreg"].ToString());
                SiaWin.TabTrn(0, idemp, true, _idregcab, 2, WinModal: true);
            }
        }





    }
}
