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

namespace AnalisisImportaciones
{
    
    public partial class PntGuardar : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public bool guardar = false;
        public Tuple<string,string,string> val_ret;

        public PntGuardar(int idempresa)
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = idempresa;   
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
                this.Title = "Guardar " + cod_empresa + "-" + nomempresa;
                Tx_fecha.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadConfig();

                //string query = "select cod_bod,nom_bod from InMae_bod where cod_emp='" + cod_empresa + "'";
                string query = "select cod_bod,cod_bod+'-'+nom_bod as nom_bod from InMae_bod";
                DataTable dtBod = SiaWin.Func.SqlDT(query, "bodegas", idemp);
                comboBoxBodegas.ItemsSource = dtBod.DefaultView;
                    
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar:"+w);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Tx_document.Text))
                {
                    MessageBox.Show("llene el campo de documento traslado");
                    return;
                }

                if (string.IsNullOrEmpty(Tx_fecha.Text))
                {
                    MessageBox.Show("llene el campo de fecha");
                    return;
                }

                if (comboBoxBodegas.SelectedIndex<0)
                {
                    MessageBox.Show("seleccione una bodega");
                    return;
                }
            
                val_ret = new Tuple<string, string, string>(Tx_document.Text, Tx_fecha.Text, comboBoxBodegas.SelectedValue.ToString());
                guardar = true;
                this.Close();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar PNtguardar:"+w);
            }
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            guardar = false;
            this.Close();
        }


    }
}
