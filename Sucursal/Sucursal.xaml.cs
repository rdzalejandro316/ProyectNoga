using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using DataRow = System.Data.DataRow;

namespace SiasoftAppExt
{

    //Sia.PublicarPnt(9469, "Sucursal");  
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9469, "Sucursal");    
    //ww.ShowInTaskbar=false;
    //ww.codigo_tercero="19267771";
    //ww.nombre_tercero="alejandro";
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();
    public partial class Sucursal : Window
    {

        public string codigo_tercero = "";
        public string nombre_tercero = "";
        public Boolean bandera = false;
        public bool ind_suc = false;

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int moduloid = 0;
        

       Boolean banderaSelection = true;

        public Sucursal()
        {
            InitializeComponent();
            
            //idemp = SiaWin._BusinessId; ;
            //LoadConfig();
            controlesBlock(1);
            pantalla();
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                //idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());

                this.Title = "Sucursal - Empresa:" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TXT_name.Text = nombre_tercero;
            TXT_code.Text = codigo_tercero;
            LoadConfig();
            //string dir = "AV TRONCAL DE OCCIDENTE 18 76 MZ ALT 2 PAR INDUSTRIAL SANTO DOMINGO                                                                                                                                     ";
            //string temporal = dir.Remove(12);
            //MessageBox.Show("dir:"+ dir.Remove(12));


            CargarSuc(codigo_tercero);
            banderaSelection = true;
        }
        public void pantalla()
        {
            this.MinHeight = 400;
            this.MaxHeight = 400;
            this.MinWidth = 550;
            this.MaxWidth = 550;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                string tag = ((TextBox)sender).Tag.ToString();

                if (tag == "inmae_zona")
                {
                    string cmptabla = tag; string cmpcodigo = "cod_zona"; string cmpnombre = "nom_zona"; string cmporden = "cod_zona"; string cmpidrow = "idrow"; string cmptitulo = "Maestra de Zonas"; string cmpconexion = cnEmp; Boolean mostrartodo = true; string cmpwhere = "";
                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, SiaWin.Func.DatosEmp(idemp), mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 400;
                    winb.Width = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (!string.IsNullOrEmpty(code))
                    {
                        TB_CodigoZonaSuc.Text = code;
                        TB_ZonaSuc.Text = nom;                        
                    }                    
                }

                if (tag == "comae_ciu")
                {
                    string cmptabla = tag; string cmpcodigo = "cod_ciu"; string cmpnombre = "nom_ciu"; string cmporden = "cod_ciu"; string cmpidrow = "cod_ciu"; string cmptitulo = "Maestra de Ciudad"; string cmpconexion = cnEmp; Boolean mostrartodo = false; string cmpwhere = "";
                    int idr = 0; string code = ""; string nom = "";

                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, SiaWin.Func.DatosEmp(idemp), mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 400;
                    winb.Width = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;

                    if (idr > 0)
                    {
                        TB_CiuSuc.Text = nom;
                        TB_CodCiuSuc.Text = code;
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

       

        public void CargarSuc(string tercero)
        {
            try
            {
                DataTable dt_ter = SiaWin.Func.SqlDT("select * from comae_ter where cod_ter='" + tercero + "'", "tercero", idemp);
                if (dt_ter.Rows.Count > 0)
                {
                    //int ind_suc = dt_ter.Rows[0]["ind_suc"] == DBNull.Value ? 0 : Convert.ToInt32(dt_ter.Rows[0]["ind_suc"]);
                    if (ind_suc == false)
                    {
                        dataGridSuc.ItemsSource = null;
                        GridPrin.IsEnabled = false;
                    }
                    else
                    {

                        string cadena = "select InMae_suc.cod_ter,InMae_suc.cod_suc,InMae_suc.nom_suc,InMae_suc.dir,InMae_suc.dir_corres,InMae_suc.tel, InMae_suc.cod_ciu,Comae_ciu.nom_ciu,InMae_suc.estado,InMae_suc.cod_zona,InMae_suc.fecha_aded,InMae_zona.Nom_zona,inmae_suc.ciudad from InMae_suc ";
                        cadena = cadena + "Left join InMae_zona on InMae_suc.cod_zona=InMae_zona.cod_zona ";
                        cadena = cadena + "Left join Comae_ciu on InMae_suc.cod_ciu=Comae_ciu.cod_ciu ";
                        cadena = cadena + "where cod_ter='" + tercero + "' ";

                        DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);
                        banderaSelection = false;
                        dataGridSuc.ItemsSource = dt.DefaultView;
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar sucursales::" + w);
            }

        }

        public void controlesBlock(int tipoBlock)
        {
            if (tipoBlock == 1)
            {
                TB_codSuc.IsEnabled = false;
                TB_NomSuc.IsEnabled = false;
                TB_DirSuc.IsEnabled = false;
                TB_DirCorSuc.IsEnabled = false;
                TB_TelSuc.IsEnabled = false;
                TB_CiuSuc.IsEnabled = false;
                TB_EstSuc.IsEnabled = false;
                TB_ZonaSuc.IsEnabled = false;
                BTnGuardar.IsEnabled = false;
                //Style 
                TB_codSuc.Style = (Style)FindResource("boxBlock");
                TB_NomSuc.Style = (Style)FindResource("boxBlock");
                TB_DirSuc.Style = (Style)FindResource("boxBlock");
                TB_DirCorSuc.Style = (Style)FindResource("boxBlock");
                TB_TelSuc.Style = (Style)FindResource("boxBlock");
                TB_CiuSuc.Style = (Style)FindResource("boxBlock");
                TB_ZonaSuc.Style = (Style)FindResource("boxBlock");
            }
            if (tipoBlock == 2)
            {
                TB_codSuc.IsEnabled = true;
                TB_NomSuc.IsEnabled = true;
                TB_DirSuc.IsEnabled = true;
                TB_DirCorSuc.IsEnabled = true;
                TB_TelSuc.IsEnabled = true;
                TB_CiuSuc.IsEnabled = true;
                TB_EstSuc.IsEnabled = true;
                TB_ZonaSuc.IsEnabled = true;
                BTnGuardar.IsEnabled = true;
                //Style 
                TB_codSuc.Style = (Style)FindResource("boxValues");
                TB_NomSuc.Style = (Style)FindResource("boxValues");
                TB_DirSuc.Style = (Style)FindResource("boxValues");
                TB_DirCorSuc.Style = (Style)FindResource("boxValues");
                TB_TelSuc.Style = (Style)FindResource("boxValues");
                TB_CiuSuc.Style = (Style)FindResource("boxValues");
                TB_ZonaSuc.Style = (Style)FindResource("boxValues");

            }
            if (tipoBlock == 3)
            {

                TB_codSuc.IsEnabled = false;
                TB_NomSuc.IsEnabled = true;
                TB_DirSuc.IsEnabled = true;
                TB_DirCorSuc.IsEnabled = true;
                TB_TelSuc.IsEnabled = true;
                TB_CiuSuc.IsEnabled = true;
                TB_EstSuc.IsEnabled = true;
                TB_ZonaSuc.IsEnabled = true;
                BTnGuardar.IsEnabled = true;
                //Style 
                TB_codSuc.Style = (Style)FindResource("boxValues");
                TB_NomSuc.Style = (Style)FindResource("boxValues");
                TB_DirSuc.Style = (Style)FindResource("boxValues");
                TB_DirCorSuc.Style = (Style)FindResource("boxValues");
                TB_TelSuc.Style = (Style)FindResource("boxValues");
                TB_CiuSuc.Style = (Style)FindResource("boxValues");
                TB_ZonaSuc.Style = (Style)FindResource("boxValues");
            }
        }

        private void BTNuevo_Click(object sender, RoutedEventArgs e)
        {
            controlesBlock(2);
            limpiar();
        }

        private void TB_codSuc_LostFocus(object sender, RoutedEventArgs e)
        {
            actualizaCampoSuc(TB_codSuc.Text);

        }

        public void actualizaCampoSuc(string sucursal)
        {
            try
            {

                string cadena = "select InMae_suc.cod_ter,InMae_suc.cod_suc,InMae_suc.nom_suc,InMae_suc.dir,InMae_suc.dir_corres,InMae_suc.tel, InMae_suc.cod_ciu,Comae_ciu.nom_ciu,InMae_suc.estado,InMae_suc.cod_zona,InMae_suc.fecha_aded,InMae_zona.Nom_zona,inmae_suc.ciudad from InMae_suc ";
                cadena = cadena + "Left join InMae_zona on InMae_suc.cod_zona=InMae_zona.cod_zona ";
                cadena = cadena + "Left join Comae_ciu on InMae_suc.cod_ciu=Comae_ciu.cod_ciu ";
                cadena = cadena + "where cod_ter='" + TXT_code.Text + "' and cod_suc='" + sucursal + "' ";

                DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);

                TB_codSuc.Text = dt.Rows[0]["cod_suc"].ToString().Trim();
                TB_NomSuc.Text = dt.Rows[0]["nom_suc"].ToString().Trim();
                TB_DirSuc.Text = dt.Rows[0]["dir"].ToString().Trim();
                TB_DirCorSuc.Text = dt.Rows[0]["dir_corres"].ToString().Trim();
                TB_TelSuc.Text = dt.Rows[0]["tel"].ToString().Trim();
                TB_CiuSuc.Text = dt.Rows[0]["ciudad"].ToString().Trim();
                TB_CodCiuSuc.Text = dt.Rows[0]["cod_ciu"].ToString().Trim();

                TB_EstSuc.SelectedIndex = Convert.ToInt32(dt.Rows[0]["estado"]);
                TB_ZonaSuc.Text = dt.Rows[0]["Nom_zona"].ToString().Trim();

                bandera = true;
            }
            catch (Exception)
            {
                bandera = false;
            }
        }

        private void BTnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BTnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (bandera == true)
            {
                if (validarCampos() == false)
                {
                    MessageBox.Show("debe de llenar todos los campos para poder actualizar");
                    return;
                }
                actualizar();
            }
            else
            {
                if (validarCampos() == false)
                {
                    MessageBox.Show("debe de llenar todos los campos para poder guardar");
                    return;
                }
                guardar();
            }

        }

        public Boolean validarCampos()
        {
            Boolean variable = true;
            if (String.IsNullOrEmpty(TB_codSuc.Text) || String.IsNullOrEmpty(TB_NomSuc.Text) || String.IsNullOrEmpty(TB_DirSuc.Text) || String.IsNullOrEmpty(TB_DirCorSuc.Text) || String.IsNullOrEmpty(TB_TelSuc.Text) || String.IsNullOrEmpty(TB_CiuSuc.Text) || String.IsNullOrEmpty(TB_ZonaSuc.Text))
            {
                variable = false;
            }
            return variable;
        }

        public void actualizar()
        {
            try
            {
                var TagEstado = ((ComboBoxItem)TB_EstSuc.SelectedItem).Tag.ToString();
                string cadena = "update InMae_suc set nom_suc = '" + TB_NomSuc.Text.Trim() + "', dir='" + TB_DirSuc.Text.Trim() + "', dir_corres = '" + TB_DirCorSuc.Text.Trim() + "', tel = '" + TB_TelSuc.Text.Trim() + "', ciudad = '" + TB_CiuSuc.Text.Trim() + "', cod_ciu='"+ TB_CodCiuSuc.Text+ "',estado = '" + TagEstado.Trim() + "', cod_zona = '" + TB_CodigoZonaSuc.Text.Trim() + "' where cod_ter = '" + TXT_code.Text.Trim() + "' and cod_suc = '" + TB_codSuc.Text.Trim() + "' ";
                if (SiaWin.Func.SqlCRUD(cadena, idemp) == true)
                {
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, "actualizo la sucursal:" + TB_codSuc.Text.Trim() + " del cliente : " + TXT_code.Text.Trim() + "", "");
                   MessageBox.Show("actualizacion exitosa");

                    CargarSuc(TXT_code.Text);
                    controlesBlock(1);
                    limpiar();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error actualizar SS:" + w);
            }
        }


        public void guardar()
        {
            try
            {
                var TagEstado = ((ComboBoxItem)TB_EstSuc.SelectedItem).Tag.ToString();

                string cadena = "insert into InMae_suc (cod_ter,cod_suc,nom_suc,dir,dir_corres,tel,ciudad,cod_ciu,estado,cod_zona,fecha_aded) values ('" + TXT_code.Text.Trim() + "','" + TB_codSuc.Text.Trim() + "','" + TB_NomSuc.Text.Trim() + "','" + TB_DirSuc.Text.Trim() + "','" + TB_DirCorSuc.Text.Trim() + "','" + TB_TelSuc.Text.Trim() + "','" + TB_CiuSuc.Text.Trim() + "','"+ TB_CodCiuSuc.Text + "','" + TagEstado.Trim() + "','" + TB_CodigoZonaSuc.Text.Trim() + "','" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "' )";
                if (SiaWin.Func.SqlCRUD(cadena, idemp) == true)
                {
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, "insertol la sucursal:" + TB_codSuc.Text.Trim() + " del cliente : " + TXT_code.Text.Trim() + "", "");
                    MessageBox.Show("insercion exitosa");
                    CargarSuc(TXT_code.Text);
                    limpiar();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error guardar:" + w);
            }
        }

        private void BTNeliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridSuc.SelectedItems[0];
                string codigo_sucur = row["cod_suc"].ToString();

                if (RegistroCabeza(codigo_sucur) == true)
                {
                    string cadena = "delete InMae_suc where cod_ter='" + TXT_code.Text + "' and cod_suc='" + codigo_sucur + "' ";

                    if (SiaWin.Func.SqlCRUD(cadena,idemp) == true)
                    {
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, "elimino la sucursal:" + codigo_sucur + " del cliente : "+ TXT_code.Text + "", "");
                        MessageBox.Show("eliminacion exitosa");
                        CargarSuc(TXT_code.Text);
                        limpiar();
                    }                    
                }
                else
                {
                    MessageBox.Show("no se puede eliminar la sucursal por que tiene registros");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("Seleccione una sucursal:" + w);
            }
        }

        public Boolean RegistroCabeza(string cod_suc)
        {
            try
            {
                string cadena = "select cod_cli,suc_cli from InCab_doc where cod_cli='" + TXT_code.Text + "' and suc_cli='" + cod_suc + "'  ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);

                if (dt.Rows.Count == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("error al consultar la cabeza");
                return false;
            }
        }

        public void limpiar()
        {
            TB_codSuc.Text = "";
            TB_NomSuc.Text = "";
            TB_DirSuc.Text = "";
            TB_DirCorSuc.Text = "";
            TB_TelSuc.Text = "";
            TB_CiuSuc.Text = "";
            //TB_EstSuc.Text = "";
            TB_ZonaSuc.Text = "";
        }

        private void BtnActulizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridSuc.SelectedItems[0];

                string codigo_sucur = row["cod_suc"].ToString();

                TB_codSuc.Text = row["cod_suc"].ToString().Trim();
                TB_NomSuc.Text = row["nom_suc"].ToString().Trim();
                TB_DirSuc.Text = row["dir"].ToString().Trim();
                TB_DirCorSuc.Text = row["dir_corres"].ToString().Trim();
                TB_TelSuc.Text = row["tel"].ToString().Trim();
                TB_CiuSuc.Text = row["ciudad"].ToString().Trim();
                TB_EstSuc.SelectedIndex = Convert.ToInt32(row["estado"]);
                TB_CodigoZonaSuc.Text = row["cod_zona"].ToString().Trim();
                TB_ZonaSuc.Text = row["Nom_zona"].ToString().Trim();

                bandera = true;
                controlesBlock(3);

            }
            catch (Exception)
            {
                MessageBox.Show("selecciona la sucursal");
            }

        }

        private void dataGridSuc_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (banderaSelection == true)
                {
                    controlesBlock(1);
                    DataRowView row = (DataRowView)dataGridSuc.SelectedItems[0];
                    string codigo_sucur = row["cod_suc"].ToString();
                    TB_codSuc.Text = row["cod_suc"].ToString().Trim();
                    TB_NomSuc.Text = row["nom_suc"].ToString().Trim();
                    TB_DirSuc.Text = row["dir"].ToString().Trim();
                    TB_DirCorSuc.Text = row["dir_corres"].ToString().Trim();
                    TB_TelSuc.Text = row["tel"].ToString().Trim();
                    TB_CiuSuc.Text = row["ciudad"].ToString().Trim();

                    TB_EstSuc.SelectedIndex = Convert.ToInt32(row["estado"]);
                    TB_CodigoZonaSuc.Text = row["cod_zona"].ToString().Trim();
                    TB_ZonaSuc.Text = row["Nom_zona"].ToString().Trim();
                }
                banderaSelection = true;
            }
            catch (Exception w)
            {
                MessageBox.Show("errro en la selecion:::" + w);
            }
        }






    }

}
