using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowPV
{

    public partial class Buscar : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public bool flag = false;
        public string num_trnBusc = "";
        public Buscar()
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
                this.Title = "Buscar " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();
                SiaWin = System.Windows.Application.Current.MainWindow;
                idemp = SiaWin._BusinessId;

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                dataGridCabeza.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                dataGridCabeza.ItemsSource = null;

                var slowTask = Task<DataTable>.Factory.StartNew(() => Load(source.Token), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    dataGridCabeza.ItemsSource = ((DataTable)slowTask.Result).DefaultView;                    
                }
                dataGridCabeza.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;
                dataGridCabeza.SelectedIndex = 0;
                dataGridCabeza.Focus();
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el load:" + w);
            }
        }
        

        public DataTable Load(CancellationToken cancellationToken)
        {
            DataTable dt = new DataTable();
            

            try
            {
                string cadena = "select InOrd_Pro.FEC_TRN as fec_trn,InOrd_Pro.COD_CLI as cod_cli,Comae_ter.nom_ter as nom_ter,NUM_TRN as num_trn,InOrd_Pro.num_doc from InOrd_Pro ";
                cadena += "inner join comae_ter on comae_ter.cod_ter = InOrd_Pro.cod_cli ";
                cadena += "group by InOrd_Pro.FEC_TRN,InOrd_Pro.COD_CLI,Comae_ter.nom_ter,InOrd_Pro.NUM_TRN,InOrd_Pro.num_doc order by FEC_TRN desc ";

                DataTable dtOrd = SiaWin.Func.SqlDT(cadena, "ordenes", idemp);
                if (dtOrd.Rows.Count > 0) dt = dtOrd;
            }
            catch (Exception w)
            {
                MessageBox.Show("erro en la consulta" + w);
            }
            return dt;
        }




        private void BtnSelecionar_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                if (dataGridCabeza.SelectedIndex>=0)
                {
                    DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                    string num_doc = row["num_doc"].ToString().Trim();
                    string num_trn = row["num_trn"].ToString().Trim();

                    if (string.IsNullOrEmpty(num_doc))
                    {
                        flag = true;
                        num_trnBusc = num_trn;
                        this.Close();
                    }
                    else
                    {
                        flag = false;
                        MessageBox.Show("la orden "+ num_trn + " ya se ha facturado");
                    }
                }
                else
                {
                    MessageBox.Show("seleccione una orden de remachado");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al seleccionar:"+w);
            }
        }

        private void dataGridCabeza_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                BtnSelecionar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }




    }
}
