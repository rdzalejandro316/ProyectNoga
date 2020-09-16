using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

    //Sia.PublicarPnt(9479, "DescuentoPorLinea");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9479, "DescuentoPorLinea");    
    //ww.ShowInTaskbar=false;
    //ww.codigo_tercero="860054978";
    //ww.nombre_tercero="ADISPETROL S.A.";
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();

    public partial class DescuentoPorLinea : Window
    {
        public string codigo_tercero = "";
        public string nombre_tercero = "";

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public DescuentoPorLinea()
        {
            InitializeComponent();
            //if (idemp>0) idemp = idempresa;                        
            pantalla();
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                //if (idemp <= 0) idemp = SiaWin._BusinessId;
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                //idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Descuento por Linea - Empresa:" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void pantalla()
        {
            this.MinHeight = 400;
            this.MaxHeight = 400;
            this.MinWidth = 550;
            this.MaxWidth = 550;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            TX_name.Text = nombre_tercero;
            TX_codTer.Text = codigo_tercero;
            LoadConfig();
            CargarGrid(codigo_tercero);
        }

        private void TX_NameLin_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string tag = ((TextBox)sender).Tag.ToString();

                if (tag == "InMae_tip")
                {

                    string cmptabla = tag; string cmpcodigo = "cod_tip"; string cmpnombre = "nom_tip"; string cmporden = "cod_tip"; string cmpidrow = "cod_tip"; string cmptitulo = "Maestra de Lineas"; string cmpconexion = cnEmp; Boolean mostrartodo = true; string cmpwhere = "";
                    int idr = 0; string code = ""; string nom = "";

                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, SiaWin.Func.DatosEmp(idemp), mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;

                    if (idr > 0)
                    {
                        TX_CodeLin.Text = code;
                        TX_NameLin.Text = nom.Trim();

                        if (DetectarLinea(code))
                        {
                            MessageBox.Show("El tercero ya cuenta con un descuento en esa Linea");
                            limpiar();
                        }

                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;

                }
                if (e.Key == Key.Enter)
                {
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public void CargarGrid(string tercero)
        {
            try
            {
                string cadena = "select descuento.cod_ter,descuento.cod_tip,linea.nom_tip,descuento.por_des,descuento.des_mos,descuento.des_ppag from InTer_tip as descuento ";
                cadena = cadena + "inner join InMae_tip as linea on descuento.Cod_tip = linea.cod_tip ";
                cadena = cadena + "where descuento.Cod_ter='" + tercero + "' ";

                DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
                DataGridDesLine.ItemsSource = dt.DefaultView;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar descuentos:" + w);
            }
        }

        public void desbloquearFormulario(Boolean bloquear)
        {
            FormAdd.IsEnabled = bloquear;
        }

        public Boolean DetectarLinea(string linea)
        {

            Boolean bandera = false;
            try
            {
                string cadena = "select * from InTer_tip where Cod_ter='" + TX_codTer.Text + "' and Cod_tip='" + linea + "' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);

                if (dt.Rows.Count > 0)
                {
                    bandera = true;
                }

                return bandera;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar linea");
                return false;
            }
        }

        public void limpiar()
        {
            TX_NameLin.Text = "";
            TX_CodeLin.Text = "";
            TX_PorDesc.Value = 0.00;
            TX_Mos.Value = 0.00;
            TX_PPag.Value = 0.00;
        }

        private void BTNagregar_Click(object sender, RoutedEventArgs e)
        {
            desbloquearFormulario(true);
        }

        public Boolean CamposLlenos()
        {
            Boolean bandera = false;

            if (TX_NameLin.Text == "" || string.IsNullOrEmpty(TX_NameLin.Text))
            {
                bandera = true;
            }
            return bandera;
        }

        private void BTNguardar_Click(object sender, RoutedEventArgs e)
        {
            if (CamposLlenos() == true)
            {
                MessageBox.Show("LLene todos los campos para poder guardar");
            }
            else
            {
                guardar();
            }
        }


        public void guardar() {
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = "insert into InTer_tip (Cod_ter,Cod_tip,Por_des,des_mos,des_ppag,fecha_aded) values (@Cod_ter,@Cod_tip,@Por_des,@des_mos,@des_ppag,@fecha_aded)";
                        cmd.Parameters.AddWithValue("@Cod_ter", TX_codTer.Text);
                        cmd.Parameters.AddWithValue("@Cod_tip", TX_CodeLin.Text);
                        cmd.Parameters.AddWithValue("@Por_des", TX_PorDesc.Value);
                        cmd.Parameters.AddWithValue("@des_mos", TX_Mos.Value);
                        cmd.Parameters.AddWithValue("@des_ppag", TX_PPag.Value);
                        cmd.Parameters.AddWithValue("@fecha_aded", DateTime.Now.ToString("dd/MM/yyyy H:mm"));
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Se agrego el descuento a la linea " + TX_NameLin.Text.Trim() + " exitosamenta");

                        string msg = "se inserto el cliente(InTer_tip):" + TX_codTer.Text;
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, msg, "");
                        CargarGrid(TX_codTer.Text);
                        limpiar();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error Interno Al guardar", MessageBoxButton.OK, MessageBoxImage.Stop);

                    }
                }
            }
        }

        private void BTNcancelar_Click(object sender, RoutedEventArgs e)
        {
            limpiar();
            desbloquearFormulario(false);
        }

        private void DataGridDesLine_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)DataGridDesLine.SelectedItems[0];
                string linea = row["Cod_tip"].ToString();
                string tercero = row["Cod_ter"].ToString();
                decimal Por_des = Convert.ToDecimal(row["Por_des"]);
                decimal des_mos = Convert.ToDecimal(row["des_mos"]);
                decimal des_ppag = Convert.ToDecimal(row["des_ppag"]);

                string query = "update InTer_tip set Por_des=" + Por_des.ToString("F", CultureInfo.InvariantCulture) + ",des_mos=" + des_mos.ToString("F", CultureInfo.InvariantCulture) + ",des_ppag=" + des_ppag.ToString("F", CultureInfo.InvariantCulture) + " where Cod_ter='" + tercero.Trim() + "' and Cod_tip='" + linea + "' ";
                //SqlCRUD(query, 1);

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    MessageBox.Show("Actualizacion exitosa");
                    string msg = "Actualizacion inter_tip:" + tercero;

                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, msg, "");
                }
                SiaWin.Func.SqlCRUD(query, idemp);
                //CargarGrid(TX_codTer.Text);
                //MessageBox.Show("linea:"+ linea);
                //MessageBox.Show("tercero:" + tercero);
                //MessageBox.Show("Por_des:" + Por_des);


            }
            catch (Exception w)
            {
                MessageBox.Show("error al editar" + w);
            }
        }

        public Boolean SqlCRUD(string _query,int IdBuss) {
            try
            {

                //string cn = null;
                //if (IdBuss <= 0) cn = ConfiguracionApp();
                //if (IdBuss > 0) cn = DatosEmp(IdBuss);
                //if (string.IsNullOrEmpty(cn)) return null;


                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(cnEmp);
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = _query;
                cmd.Connection = conn;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch (Exception w)
            {
                MessageBox.Show("error en el global code:"+w);
                return false;
            }
        }

        

    }
}
