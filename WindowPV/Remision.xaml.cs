using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowPV
{

    public partial class Remision : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public string cod_bodPV = "";

        public DataTable temporal = new DataTable();
        public string tercero = "";
        public string bodegaRemision = "";

        public int idregcabReturn = -1;
        public string codtrn = string.Empty;
        public string numtrn = string.Empty;

        public int PntTip = 0;
        public int idremision = 0;


        public DataTable dt_cue = new DataTable();
        public Remision(int idEmpresa)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = idEmpresa;
            LoadConfig();
            pantalla();
            loadColumns();
        }

        public void loadColumns()
        {
            dt_cue.Columns.Add("cod_ref");
            dt_cue.Columns.Add("nom_ref");
            dt_cue.Columns.Add("cantidadprincipal", typeof(decimal));
            dt_cue.Columns.Add("cantidadfacturada", typeof(decimal));
            dt_cue.Columns.Add("cantidadreal", typeof(decimal));
            dt_cue.Columns.Add("val_uni");
            dt_cue.Columns.Add("subtotal");
            dt_cue.Columns.Add("por_des");
            dt_cue.Columns.Add("tot_tot");
            dt_cue.Columns.Add("num_trn");
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //BodCod.IsFocused = true;
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
                this.Title = "Remision - Empresa:" + cod_empresa + "-" + nomempresa;

                BodCod.Focus();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void pantalla()
        {
            this.MinHeight = 550;
            this.MaxHeight = 550;
            this.MinWidth = 1000;
            this.MaxWidth = 1000;
        }

        private void Txt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                string idTab = ((TextBox)sender).Tag.ToString();

                if (idTab.Length > 0)
                {
                    string tag = ((TextBox)sender).Tag.ToString();
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                    if (string.IsNullOrEmpty(tag)) return;

                    if (tag == "inmae_bod")
                    {
                        cmptabla = tag; cmpcodigo = "cod_bod"; cmpnombre = "UPPER(nom_bod)"; cmporden = "cod_bod"; cmpidrow = "cod_bod"; cmptitulo = "Maestra de Bodegas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "tipo_bod='4' and bod_rem=0 ";
                    }

                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 400;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (idr > 0)
                    {
                        if (tag == "inmae_bod")
                        {
                            BodCod.Text = code;
                            BodNom.Text = nom;
                        }
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        BTNconsultar.Focus();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BTNconsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BTNconsultar.Tag = "1";
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                dataGridCabeza.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                dataGridCabeza.ItemsSource = null;
                dataGridCuerpo.ItemsSource = null;
                dt_cue.Clear();

                string empres = cod_empresa;
                string where = "and bod_tra = '" + BodCod.Text + "' and cuerpo.cod_bod = '" + cod_bodPV + "'";
                string fi = DateTime.Today.AddMonths(-6).ToString("dd/MM/yyyy");
                string ff = DateTime.Now.ToString("dd/MM/yyyy");



                var slowTask = Task<DataSet>.Factory.StartNew(() => CargarConsultaRemi(empres, where, fi, ff, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridCabeza.ItemsSource = ((DataSet)slowTask.Result).Tables[0].DefaultView;
                    Total.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();


                }



                dataGridCabeza.IsEnabled = true;
                sfBusyIndicator.IsBusy = false;
                //SiaWin.Browse(dt_cue);
                //cargarConsulta();
                BTNconsultar.Tag = "0";
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridCabeza.SelectedIndex = 0;
                    dataGridCabeza.Focus();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("erro al cargar:" + w);
            }

        }

        public DataSet CargarConsultaRemi(string empresaP, string where, string fe_ini, string fec_fin, CancellationToken cancellationToken)
        {
            SqlConnection con = new SqlConnection(SiaWin._cn);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();
            cmd = new SqlCommand("_EmpPvConsultaRemision", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@cod_trn", "145");
            cmd.Parameters.AddWithValue("@_codemp", empresaP);
            cmd.Parameters.AddWithValue("@where", where);
            cmd.Parameters.AddWithValue("@fechaIni", fe_ini);
            cmd.Parameters.AddWithValue("@fechaFin", fec_fin);
            da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            con.Close();

            foreach (System.Data.DataRow item in ds.Tables[0].Rows)
            {

                string num_trn = item["num_trn"].ToString().Trim();
                string idreg = item["idreg"].ToString().Trim();

                if (!string.IsNullOrWhiteSpace(num_trn) && !string.IsNullOrWhiteSpace(idreg))
                {
                    SqlConnection con1 = new SqlConnection(SiaWin._cn);
                    SqlCommand cmd1 = new SqlCommand();
                    SqlDataAdapter da1 = new SqlDataAdapter();
                    DataTable dt1 = new DataTable();
                    cmd1 = new SqlCommand("_EmpPvCuerpoConsignacion", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@idreg", idreg);
                    cmd1.Parameters.AddWithValue("@num_trn", num_trn);
                    cmd1.Parameters.AddWithValue("@_codemp", empresaP);
                    da1 = new SqlDataAdapter(cmd1);
                    da1.Fill(dt1);
                    con1.Close();
                    if (dt1.Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow dr_cu in dt1.Rows)
                        {
                            dt_cue.Rows.Add
                            (
                                dr_cu["cod_ref"].ToString(),
                                dr_cu["nom_ref"].ToString(),
                                Convert.ToDecimal(dr_cu["cantidadprincipal"]),
                                Convert.ToDecimal(dr_cu["cantidadfacturada"]),
                                Convert.ToDecimal(dr_cu["cantidadreal"]),
                                dr_cu["val_uni"].ToString(),
                                dr_cu["subtotal"].ToString(),
                                dr_cu["por_des"].ToString(),
                                dr_cu["tot_tot"].ToString(),
                                dr_cu["num_trn"].ToString()
                            );
                        }
                    }
                }
            }

            return ds;
        }


        private void dataGridCabeza_SelectionChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangedEventArgs e)
        {
            try
            {
                if (BTNconsultar.Tag.ToString() == "1") return;

                DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                string idreg = row["idreg"].ToString();
                string num_trn = row["num_trn"].ToString();

                DataTable dt_temporal = new DataTable();
                dt_temporal.Columns.Add("cod_ref");
                dt_temporal.Columns.Add("nom_ref");
                dt_temporal.Columns.Add("cantidadprincipal");
                dt_temporal.Columns.Add("cantidadfacturada");
                dt_temporal.Columns.Add("cantidadreal");
                dt_temporal.Columns.Add("val_uni");
                dt_temporal.Columns.Add("subtotal");
                dt_temporal.Columns.Add("por_des");
                dt_temporal.Columns.Add("tot_tot",typeof(decimal));
                dt_temporal.Columns.Add("num_trn");


                foreach (System.Data.DataRow item in dt_cue.Rows)
                {
                    string num = item["num_trn"].ToString();

                    if (num == num_trn)
                    {
                        dt_temporal.Rows.Add
                            (
                                item["cod_ref"].ToString(),
                                item["nom_ref"].ToString(),
                                Convert.ToDecimal(item["cantidadprincipal"]),
                                Convert.ToDecimal(item["cantidadfacturada"]),
                                Convert.ToDecimal(item["cantidadreal"]),
                                item["val_uni"].ToString(),
                                item["subtotal"].ToString(),
                                item["por_des"].ToString(),
                                item["tot_tot"].ToString(),
                                item["num_trn"].ToString()
                            );
                    }
                }

                temporal.Clear();
                temporal = dt_temporal;
                dataGridCuerpo.ItemsSource = dt_temporal.DefaultView;

                if (dataGridCabeza.SelectedIndex>=0 && dt_temporal.Rows.Count>0)
                {
                    //SiaWin.Browse(dt_temporal);
                    double total = Convert.ToDouble(dt_temporal.Compute("Sum(tot_tot)", "").ToString());
                    TotalRef.Text = total.ToString("C");
                }
                else
                {
                    TotalRef.Text = "0";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al carcar el detalle del cuerpo:" + w);
            }
        }

        private void BTNfacturar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridCabeza.SelectedIndex < 0)
                {
                    MessageBox.Show("no se puede facturar por que no ha seleccionado ningun documento");
                }

                if (temporal.Rows.Count > 0)
                {
                    DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                    tercero = row["cod_cli"].ToString();
                    bodegaRemision = BodCod.Text.Trim();
                    PntTip = 3;
                    idremision = Convert.ToInt32(row["idreg"]);
                    codtrn = row["cod_trn"].ToString();
                    numtrn = row["num_trn"].ToString();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("La consignacion seleccionada no contiene tiene items por q ya se facturaros","alerta",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Seleccione un Documento");
            }

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.F5)
                {
                    if (dataGridCabeza.SelectedIndex < 0)
                    {
                        MessageBox.Show("no se puede facturar por que no ha seleccionado ningun documento");
                    }
                    else
                    {
                        BTNfacturar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("@888" + w);
            }
        }




    }
}

