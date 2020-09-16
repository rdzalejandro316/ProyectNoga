using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9548,"Remachados");
    //dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9548,"Remachados");    
    //WinDescto.ShowInTaskbar = false;
    //WinDescto.Owner = Application.Current.MainWindow;
    //WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //WinDescto.ShowDialog(); 

    public partial class Remachados : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        int moduloid = 0;
        string cnEmp = "";
        string cod_empresa = "";

        DataTable dt_orden = new DataTable();

        public string pv = "";


        public Remachados()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            loadCombo();
            loadOrden();
            controls(1);
            BtnSave.Focus();


        }

        public void loadOrden()
        {
            dt_orden.Columns.Add("cod_ref");
            dt_orden.Columns.Add("can_rec");
            dt_orden.Columns.Add("cantidad", typeof(double));
            dt_orden.Columns.Add("cant_nr", typeof(double));
            dt_orden.Columns.Add("obs");
            dt_orden.Columns.Add("r_h", typeof(double));
            dt_orden.Columns.Add("r_r", typeof(double));
            dt_orden.Columns.Add("r_t", typeof(double));
            dt_orden.Columns.Add("r_o", typeof(double));
            dt_orden.Rows.Add("", "", 1, 0, "", 0, 0, 0, 0);
            GridConfig.ItemsSource = dt_orden.DefaultView;
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessIcon"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());


                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Ingreso de Ordenes de Remachados " + cod_empresa + "-" + nomempresa;
                loadImage(idLogo);

                GridConfig.SelectionController = new GridSelectionControllerExt(GridConfig); // enter avance a la siguiente columna


                Fec_Act.Text = DateTime.Now.ToString();
                Fec_ent.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void loadImage(int id)
        {
            try
            {
                string select = "select * from Images where ImageId='" + id + "'";
                DataTable dt = SiaWin.Func.SqlDT(select, "Imagen", 0);
                if (dt.Rows.Count > 0)
                {
                    byte[] blob = (byte[])dt.Rows[0]["Image"];
                    MemoryStream stream = new MemoryStream();
                    stream.Write(blob, 0, blob.Length);
                    stream.Position = 0;
                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                    bi.BeginInit();
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                    bi.EndInit();
                    this.Icon = bi;
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error en el loadImage:" + w);
            }
        }



        public class GridSelectionControllerExt : GridSelectionController
        {
            private SfDataGrid grid;
            public GridSelectionControllerExt(SfDataGrid datagrid) : base(datagrid)
            {
                grid = datagrid;
            }
            protected override void ProcessKeyDown(KeyEventArgs args)
            {
                try
                {
                    var currentKey = args.Key;
                    var arguments = new KeyEventArgs(args.KeyboardDevice, args.InputSource, args.Timestamp, Key.Tab)
                    {
                        RoutedEvent = args.RoutedEvent
                    };
                    if (currentKey == Key.Enter)
                    {
                        if (grid.IsReadOnly == false && grid.CurrentColumn is GridTextColumn) { }
                        base.ProcessKeyDown(arguments);
                        args.Handled = arguments.Handled;
                        return;
                    }

                    if (currentKey == Key.Up)
                    {
                        if (grid.View.IsAddingNew == true && grid.View.IsCurrentBeforeFirst == true)
                        {
                            grid.View.CancelEdit();
                            grid.View.CancelNew();
                        }
                        grid.UpdateLayout();
                    }


                    base.ProcessKeyDown(args);
                }
                catch (Exception w)
                {
                    MessageBox.Show("errro:::" + w);
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (BtnSave.Content.ToString().Trim() == "Nuevo") return;
            if (e.Key == Key.F5 && BtnSave.Content.ToString().Trim() == "Guardar")
            {
                if (e.Key == System.Windows.Input.Key.F5)
                {
                    BtnSave.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    return;
                }
            }

        }

        public void loadCombo()
        {
            try
            {
                DataTable dtRema = SiaWin.Func.SqlDT("select rtrim(cod_rema) as cod_rema,rtrim(nom_rema) as nom_rema from InRemacha", "temporal", idemp);
                comboBoxSoltador.ItemsSource = dtRema.DefaultView;
                comboBoxLimpiador.ItemsSource = dtRema.DefaultView;
                comboBoxPintor.ItemsSource = dtRema.DefaultView;
                comboBoxRemac.ItemsSource = dtRema.DefaultView;

                DataTable dtTran = SiaWin.Func.SqlDT("select rtrim(cod_tran) as cod_tran,rtrim(nom_tran) as nom_tran from InTrans_Rem", "trans", idemp);
                comboBoxEntr.ItemsSource = dtTran.DefaultView;
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al cargar datos");
            }
        }


        public void controls(int val)
        {
            if (val == 1)
            {
                dt_orden.Clear();
                BtnSave.Content = "Nuevo";
                BtnExit.Content = "Salir";
                Fec_Act.IsEnabled = false;
                Fec_Act.Text = "";
                Tx_clie.IsEnabled = false;
                Tx_clie.Text = "";
                Tx_nume.IsEnabled = false;
                Tx_nume.Text = "";
                Fec_ent.IsEnabled = false;
                Fec_ent.Text = "";

                Tx_User.Text = "---";

                comboBoxSoltador.IsEnabled = false;
                comboBoxSoltador.SelectedIndex = -1;

                comboBoxLimpiador.IsEnabled = false;
                comboBoxLimpiador.SelectedIndex = -1;

                comboBoxPintor.IsEnabled = false;
                comboBoxPintor.SelectedIndex = -1;

                comboBoxRemac.IsEnabled = false;
                comboBoxRemac.SelectedIndex = -1;

                comboBoxEntr.IsEnabled = false;
                comboBoxEntr.SelectedIndex = -1;
            }
            if (val == 2)
            {
                dt_orden.Rows.Add("", 0, 0, 0, "Ninguna", 0, 0, 0, 0);
                BtnSave.Content = "Guardar";
                BtnExit.Content = "Cancelar";
                Fec_Act.IsEnabled = true;
                Fec_Act.Text = DateTime.Now.ToString();
                Tx_clie.IsEnabled = true;
                Tx_clie.Focus();
                Tx_clie.Text = "";
                Tx_nume.IsEnabled = true;
                Tx_nume.Text = "";
                Fec_ent.IsEnabled = true;
                Fec_ent.Text = DateTime.Now.ToString();

                Tx_User.Text = SiaWin._UserAlias;

                comboBoxSoltador.IsEnabled = true;
                comboBoxLimpiador.IsEnabled = true;
                comboBoxPintor.IsEnabled = true;
                comboBoxRemac.IsEnabled = true;
                comboBoxEntr.IsEnabled = true;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string text = (sender as Button).Content.ToString();

            if (text == "Nuevo")
            {
                controls(2);
            }
            if (text == "Guardar")
            {
                if (validarCamppos() == false)
                {
                    MessageBox.Show("Llene los campos correspondientes", "llenar campos", MessageBoxButton.OK, MessageBoxImage.Question);
                    return;
                }


                if (ValNumOrd(Tx_nume.Text))
                {
                    MessageBox.Show("el numero de orden ingresado ya existe");
                    return;
                }

                foreach (System.Data.DataRow dr in dt_orden.Rows)
                {
                    if (string.IsNullOrEmpty(dr["cod_ref"].ToString()))
                    {
                        MessageBox.Show("existen campos con referencias vacias");
                        return;
                    }
                    decimal cant = Convert.ToDecimal(dr["cantidad"]);

                    if (cant <= 0)
                    {
                        MessageBox.Show("la cantidad debe ser mayor a 0");
                        return;
                    }

                }


                if (MessageBox.Show("Usted desea generar la orden de remachado?", "Orden", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (ExecuteSqlTransaction() == true)
                    {
                        SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, moduloid, -1, -9, "GENERO UNA ORDEN DE REMACHADO #orden:"+ Tx_nume.Text+ "", "");
                        MessageBox.Show("se genero la orden");
                        controls(1);
                    }
                }



            }
        }


        private bool ExecuteSqlTransaction()
        {
            bool flag = false;
            try
            {
                System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(cnEmp);
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
                conn.Open();
                cmd.Connection = conn;
                string query = "";

                string num = Tx_nume.Text.Trim();
                string fec_t = Fec_Act.Text.Trim();
                string cli = Tx_clie.Text.ToString();
                string fec_e = Fec_ent.Text.ToString();
                string sold = comboBoxSoltador.SelectedValue.ToString();
                string limp = comboBoxLimpiador.SelectedValue.ToString();
                string pint = comboBoxPintor.SelectedValue.ToString();
                string rema = comboBoxRemac.SelectedValue.ToString();
                string entr = comboBoxEntr.SelectedValue.ToString();


                foreach (System.Data.DataRow row in dt_orden.Rows)
                {

                    string refe = row["cod_ref"].ToString();

                    double cnrc = Convert.ToDouble(row["can_rec"]);
                    double cnt = Convert.ToDouble(row["cantidad"]);
                    double cnre = Convert.ToDouble(row["cant_nr"]);
                    string obs = row["obs"].ToString();
                    double r_h = Convert.ToDouble(row["r_h"]);
                    double r_r = Convert.ToDouble(row["r_r"]);
                    double r_t = Convert.ToDouble(row["r_t"]);
                    double r_o = Convert.ToDouble(row["r_o"]);

                    query += "insert InOrd_Pro (num_trn,fec_trn,cod_cli,fec_ent,cod_sol,cod_lim,cod_pin,cod_rem,cod_tran,cod_ref,can_rec,cantidad,cant_nr,obs,r_h,r_r,r_t,r_o) values " +
                        "('" + num + "','" + fec_t + "','" + cli + "','" + fec_e + "','" + sold + "','" + limp + "','" + pint + "','" + rema + "','" + entr + "'," +
                        "'" + refe + "'," + cnrc + "," + cnt + "," + cnre + ",'" + obs + "'," + r_h + "," + r_r + "," + r_t + "," + r_o + ");";
                }

                cmd.CommandText = query;
                int value = cmd.ExecuteNonQuery();
                conn.Close();
                if (value > 0) flag = true;
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al generar la orden:" + w);
            }
            return flag;
        }

        public bool validarCamppos()
        {
            bool flag = true;
            if (string.IsNullOrEmpty(Fec_Act.Text)) flag = false;
            if (string.IsNullOrEmpty(Tx_clie.Text)) flag = false;
            if (string.IsNullOrEmpty(Tx_nume.Text)) flag = false;
            if (string.IsNullOrEmpty(Fec_ent.Text)) flag = false;

            if (comboBoxSoltador.SelectedIndex <= -1) flag = false;
            if (comboBoxLimpiador.SelectedIndex <= -1) flag = false;
            if (comboBoxPintor.SelectedIndex <= -1) flag = false;
            if (comboBoxRemac.SelectedIndex <= -1) flag = false;
            if (comboBoxEntr.SelectedIndex <= -1) flag = false;
            if (dt_orden.Rows.Count <= 0) flag = false;

            return flag;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (BtnExit.Content.ToString() == "Cancelar")
            {
                controls(1);
            }
            else
            {
                this.Close();
            }
        }

        private void Tx_clie_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter && !string.IsNullOrEmpty(Tx_clie.Text))
            {
                MoveToNextUIElement(e);
                return;
            } 
            
            

            if (e.Key == Key.F8 || e.Key == Key.Enter)
            {
                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "nom_ter", "idrow", "Maestra de clientes", cnEmp, false, "", idEmp: idemp);
                xx.ShowInTaskbar = false;
                xx.Owner = Application.Current.MainWindow;
                xx.Height = 500;
                xx.ShowDialog();
                idr = xx.IdRowReturn;
                code = xx.Codigo;
                nombre = xx.Nombre;

                if (idr > 0)
                {
                    Tx_clie.Text = code.Trim();
                    Tx_cli_name.Text = nombre.Trim();
                }
            }
        }


        void MoveToNextUIElement(KeyEventArgs e)
        {
            FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;
            TraversalRequest request = new TraversalRequest(focusDirection);
            UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;
            if (elementWithFocus != null)
                if (elementWithFocus.MoveFocus(request)) e.Handled = true;
        }

        private void Tx_clie_LostFocus(object sender, RoutedEventArgs e)
        {


            string code_ter = (sender as TextBox).Text.Trim();
            if (string.IsNullOrEmpty(code_ter)) return;


            DataTable dtTer = SiaWin.Func.SqlDT("select * from comae_ter where cod_ter='" + code_ter + "';", "temporal", idemp);
            if (dtTer.Rows.Count > 0)
            {
                Tx_clie.Text = dtTer.Rows[0]["cod_ter"].ToString().Trim();
                Tx_cli_name.Text = dtTer.Rows[0]["nom_ter"].ToString().Trim();
            }
            else
            {
                MessageBox.Show("el tercero ingresado no existe", "Tercero Inexistente", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                Tx_cli_name.Text = "---";
                Tx_clie.Text = "";
            }

        }

        private void GridConfig_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {

                var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                var rowData = GridConfig.GetRecordAtRowIndex(e.RowColumnIndex.RowIndex);
                string refer = reflector.GetValue(rowData, "cod_ref").ToString();
                var func = refe(refer);

                if (func.Item1)
                {

                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "can_rec")))
                        reflector.SetValue(rowData, "can_rec", 0);

                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cantidad")))
                        reflector.SetValue(rowData, "cantidad", 0);

                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "cant_nr")))
                        reflector.SetValue(rowData, "cant_nr", 0);

                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "obs")))
                        reflector.SetValue(rowData, "obs", "Ninguna");

                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "r_h")))
                        reflector.SetValue(rowData, "r_h", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "r_r")))
                        reflector.SetValue(rowData, "r_r", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "r_t")))
                        reflector.SetValue(rowData, "r_t", 0);
                    if (DBNull.Value.Equals(reflector.GetValue(rowData, "r_o")))
                        reflector.SetValue(rowData, "r_o", 0);


                    GridConfig.UpdateDataRow(e.RowColumnIndex.RowIndex);
                    GridConfig.UpdateLayout();
                    GridConfig.Columns["cod_ref"].AllowEditing = true;
                    GridConfig.Columns["can_rec"].AllowEditing = true;
                    GridConfig.Columns["cantidad"].AllowEditing = true;
                    GridConfig.Columns["cant_nr"].AllowEditing = true;
                    GridConfig.Columns["obs"].AllowEditing = true;
                    GridConfig.Columns["r_h"].AllowEditing = true;
                    GridConfig.Columns["r_r"].AllowEditing = true;
                    GridConfig.Columns["r_t"].AllowEditing = true;
                    GridConfig.Columns["r_o"].AllowEditing = true;
                }
                else
                {
                    reflector.SetValue(rowData, "cod_ref", "");
                    reflector.SetValue(rowData, "can_rec", 0);
                    reflector.SetValue(rowData, "cantidad", 0);
                    reflector.SetValue(rowData, "cant_nr", 0);
                    reflector.SetValue(rowData, "obs", "");
                    reflector.SetValue(rowData, "r_h", 0);
                    reflector.SetValue(rowData, "r_r", 0);
                    reflector.SetValue(rowData, "r_t", 0);
                    reflector.SetValue(rowData, "r_o", 0);

                    GridConfig.UpdateDataRow(e.RowColumnIndex.RowIndex);
                    GridConfig.UpdateLayout();
                    GridConfig.Columns["cod_ref"].AllowEditing = true;
                    GridConfig.Columns["can_rec"].AllowEditing = true;
                    GridConfig.Columns["cantidad"].AllowEditing = true;
                    GridConfig.Columns["cant_nr"].AllowEditing = true;
                    GridConfig.Columns["obs"].AllowEditing = true;
                    GridConfig.Columns["r_h"].AllowEditing = true;
                    GridConfig.Columns["r_r"].AllowEditing = true;
                    GridConfig.Columns["r_t"].AllowEditing = true;
                    GridConfig.Columns["r_o"].AllowEditing = true;
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al editar:" + w);
            }
        }

        public Tuple<bool, DataTable> refe(string refe)
        {
            string select = "select * from inmae_ref where cod_ref='" + refe + "'";
            DataTable dt = new DataTable();
            dt = SiaWin.Func.SqlDT(select, "referencias", idemp);
            var tuple = new Tuple<bool, DataTable>(dt.Rows.Count > 0 ? true : false, dt);
            return tuple;
        }

        private void GridConfig_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                GridColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;
                if (Colum.MappingName == "cod_ref" && e.Key == Key.F8)
                {
                    if (GridConfig.SelectedIndex == -1)
                        this.GridConfig.SelectionController.CurrentCellManager.BeginEdit();

                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar("inmae_ref", "cod_ref", "nom_ref", "cod_ref", "idrow", "Maestra de referencia", SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = System.Windows.Application.Current.MainWindow;
                    winb.Height = 300;
                    winb.Width = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    if (!string.IsNullOrEmpty(code))
                    {
                        var func = refe(code);
                        if (func.Item1)
                        {
                            var reflector = this.GridConfig.View.GetPropertyAccessProvider();
                            int columnIndex = (sender as SfDataGrid).SelectionController.CurrentCellManager.CurrentRowColumnIndex.RowIndex;
                            var rowData = GridConfig.GetRecordAtRowIndex(columnIndex);
                                                        

                            reflector.SetValue(rowData, "cod_ref", func.Item2.Rows[0]["cod_ref"].ToString().Trim());

                            if (DBNull.Value.Equals(reflector.GetValue(rowData, "can_rec")))
                                reflector.SetValue(rowData, "can_rec", 0);

                            if (DBNull.Value.Equals(reflector.GetValue(rowData, "cantidad")))
                                reflector.SetValue(rowData, "cantidad", 0);

                            if (DBNull.Value.Equals(reflector.GetValue(rowData, "cant_nr")))
                                reflector.SetValue(rowData, "cant_nr", 0);

                            reflector.SetValue(rowData, "obs", "Ninguna");

                            if (DBNull.Value.Equals(reflector.GetValue(rowData, "r_h")))
                                reflector.SetValue(rowData, "r_h", 0);
                            if (DBNull.Value.Equals(reflector.GetValue(rowData, "r_r")))
                                reflector.SetValue(rowData, "r_r", 0);
                            if (DBNull.Value.Equals(reflector.GetValue(rowData, "r_t")))
                                reflector.SetValue(rowData, "r_t", 0);
                            if (DBNull.Value.Equals(reflector.GetValue(rowData, "r_o")))
                                reflector.SetValue(rowData, "r_o", 0);                            

                            GridConfig.UpdateDataRow(columnIndex);
                            GridConfig.UpdateLayout();
                            GridConfig.Columns["cod_ref"].AllowEditing = true;
                            GridConfig.Columns["can_rec"].AllowEditing = true;
                            GridConfig.Columns["cantidad"].AllowEditing = true;
                            GridConfig.Columns["cant_nr"].AllowEditing = true;
                            GridConfig.Columns["obs"].AllowEditing = true;
                            GridConfig.Columns["r_h"].AllowEditing = true;
                            GridConfig.Columns["r_r"].AllowEditing = true;
                            GridConfig.Columns["r_t"].AllowEditing = true;
                            GridConfig.Columns["r_o"].AllowEditing = true;
                        }
                    }
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error en el producto f8:" + w);
            }
        }

        public bool ValNumOrd(string num)
        {            
            DataTable dt = SiaWin.Func.SqlDT("select * from InOrd_Pro where NUM_TRN='" + num + "' ", "tabla", idemp);            
            return dt.Rows.Count > 0 ? true : false;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string codbod = SiaWin.Func.cmpCodigo("copventas", "cod_pvt", "cod_bod", pv, idemp).ToString().Trim();
            Tx_Bod.Text = codbod;
        }

        private void Tx_nume_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                string txt = (sender as TextBox).Text.Trim();
                if (ValNumOrd(txt))
                {
                    MessageBox.Show("el numero de orden ingresado ya existe");
                    return;
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error!!:" + w);
            }
        }

        private void GridConfig_CurrentCellActivating(object sender, CurrentCellActivatingEventArgs e)
        {
            if (e.CurrentRowColumnIndex.ColumnIndex == 1 || e.CurrentRowColumnIndex.ColumnIndex == 9)            
                GridConfig.AddNewRowPosition = AddNewRowPosition.Bottom;            
            else            
                GridConfig.AddNewRowPosition = AddNewRowPosition.None;            
            GridConfig.UpdateLayout();
        }




    }
}
