using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9505,"AnulacionPedidos");
    //dynamic w = ((Inicio)Application.Current.MainWindow).WindowExt(9505,"AnulacionPedidos");
    //w.ShowInTaskbar = false;
    //w.Owner = Application.Current.MainWindow;
    //w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //w.ShowDialog(); 

    public partial class AnulacionPedidos : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public AnulacionPedidos()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId; ;                        
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (idemp <= 0) idemp = SiaWin._BusinessId;
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
                this.Title = "Anulacion de Pedidos - Empresa:" + cod_empresa + "-" + nomempresa;
                pantalla();
                tx_fecini.Text = DateTime.Now.AddMonths(-1).ToString();
                tx_fecfin.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void pantalla()
        {
            this.MinHeight = 500;
            this.MaxHeight = 500;
            this.MinWidth = 1000;
            this.MaxWidth = 1000;
        }

        private void TX_documento_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter || e.Key == Key.F8)
                {
                    string tag = ((TextBox)sender).Tag.ToString();
                    string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                    if (string.IsNullOrEmpty(tag)) return;

                    if (tag == "incab_doc")
                    {
                        cmptabla = "incab_doc"; cmpcodigo = "cod_trn"; cmpnombre = "num_trn"; cmporden = "idreg"; cmpidrow = "idreg"; cmptitulo = "Documentos"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "cod_trn='505' ";
                    }
                    if (tag == "incab_docConsulta")
                    {
                        cmptabla = "incab_doc"; cmpcodigo = "cod_trn"; cmpnombre = "num_trn"; cmporden = "idreg"; cmpidrow = "idreg"; cmptitulo = "Documentos"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "cod_trn='505' ";
                    }

                    int code = 0; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);

                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Width = 500;
                    winb.Height = 350;
                    winb.ShowDialog();
                    code = winb.IdRowReturn;
                    nom = winb.Nombre;
                    winb = null;
                    if (code > 0)
                    {
                        if (tag == "incab_doc")
                        {
                            TX_idreg.Text = code.ToString();
                            TX_documento.Text = nom.Trim();
                        }
                        if (tag == "incab_docConsulta")
                        {
                            TX_idregConsulta.Text = code.ToString();
                            TX_documentoConsulta.Text = nom.Trim();
                        }
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;

                    if (e.Key == Key.Enter)
                    {
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                }


            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show("ERRO EN PREVI:" + ex);
            }

        }


        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F8)
            {
                string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                cmptabla = "incab_doc"; cmpcodigo = "cod_trn"; cmpnombre = "num_trn"; cmporden = "idreg"; cmpidrow = "idreg"; cmptitulo = "Documentos"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "cod_trn='505' ";
                int code = 0; string nom = "";
                dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);

                winb.ShowInTaskbar = false;
                winb.Owner = Application.Current.MainWindow;
                winb.Width = 500;
                winb.Height = 350;
                winb.ShowDialog();
                code = winb.IdRowReturn;
                nom = winb.Nombre;
                winb = null;
                if (code > 0)
                {
                    TX_idreg.Text = code.ToString();
                    TX_documento.Text = nom.Trim();


                    BTNconsultar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    var uiElement = e.OriginalSource as UIElement;

                }
                e.Handled = true;

                if (e.Key == Key.Enter)
                {
                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }
            }

            //if (e.Key == Key.F4)
            //{
            //    if (MessageBox.Show("Usted desea anular todo el pedido ", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            //    {
            //        BtnAnular.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            //    }
            //}
        }

        private void TX_documento_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;

                string idTag = ((TextBox)sender).Tag.ToString();
                string val = ((TextBox)sender).Text.ToString();

                string cadena = "select idreg,num_trn from InCab_doc where num_trn='" + val + "' and cod_trn='505' ";
                //MessageBox.Show("cadena:"+ cadena);
                //MessageBox.Show("idemp:"+ idemp);

                DataTable tabla = SiaWin.Func.SqlDT(cadena, "Buscar", idemp);

                if (tabla.Rows.Count > 0)
                {
                    if (idTag == "incab_doc")
                    {
                        ((TextBox)sender).Text = tabla.Rows[0]["num_trn"].ToString();
                        TX_idreg.Text = tabla.Rows[0]["idreg"].ToString();
                    }
                    else
                    {
                        ((TextBox)sender).Text = tabla.Rows[0]["num_trn"].ToString();
                        TX_idregConsulta.Text = tabla.Rows[0]["idreg"].ToString();
                    }
                }
                else
                {
                    MessageBox.Show("el Documento ingresado no existe");
                    ((TextBox)sender).Text = "";
                    if (idTag == "incab_doc")
                    {
                        TX_idreg.Text = "";
                    }
                    else
                    {
                        TX_idregConsulta.Text = ""; ;
                    }
                }


            }
            catch (Exception W)
            {
                SiaWin.Func.SiaExeptionGobal(W);
                MessageBox.Show("ERROR EN LA LOST:" + W);
            }
        }

        private void BTNconsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clearControls();
                dataGridCxC.ItemsSource = null;
                if (string.IsNullOrEmpty(TX_documento.Text) || string.IsNullOrEmpty(TX_idreg.Text)) { MessageBox.Show("Ingrese un documento para consultar"); return; }


                string sql_cab = " select cabeza.idreg,cabeza.num_trn,cabeza.bod_tra,convert(varchar, cabeza.fec_trn, 120) as fec_trn,cabeza.cod_cli as cod_cli,tercero.nom_ter,ciudad.nom_ciu,cabeza.cod_trn,cabeza.cod_ven,vendedor.nom_mer,cabeza.cod_prv,provedor.nom_prv,cabeza.des_mov ";
                sql_cab = sql_cab + "from InCab_doc as cabeza ";
                sql_cab = sql_cab + "left join Comae_ter as tercero on cabeza.cod_cli = tercero.cod_ter ";
                sql_cab = sql_cab + "left join InMae_mer as vendedor on cabeza.cod_ven = vendedor.cod_mer ";
                sql_cab = sql_cab + "left join comae_ciu as ciudad on tercero.cod_ciu = ciudad.cod_ciu ";
                sql_cab = sql_cab + "left join InMae_prv as provedor on cabeza.cod_prv = provedor.cod_prv ";
                sql_cab = sql_cab + "where cabeza.cod_trn = '505' ";
                sql_cab = sql_cab + "and cabeza.idreg='" + TX_idreg.Text + "' ";

                //string sql_cab = "select cabeza.idreg,cabeza.num_trn,cabeza.fec_trn,cabeza.cod_cli,tercero.nom_ter from InCab_doc as cabeza ";
                //sql_cab = sql_cab + "inner join comae_ter as tercero on cabeza.cod_cli = tercero.cod_ter ";
                //sql_cab = sql_cab + "where idreg='" + TX_idreg .Text + "' ";

                DataTable dt_cab = SiaWin.Func.SqlDT(sql_cab, "Buscar", idemp);
                string id = "";
                if (dt_cab.Rows.Count > 0)
                {
                    id = dt_cab.Rows[0]["idreg"].ToString().Trim();
                    TXT_documento.Text = dt_cab.Rows[0]["num_trn"].ToString().Trim();
                    TXT_fecha.Text = dt_cab.Rows[0]["fec_trn"].ToString().Trim();
                    TXT_codigo.Text = dt_cab.Rows[0]["cod_cli"].ToString().Trim();
                    TXT_nombre.Text = dt_cab.Rows[0]["nom_ter"].ToString().Trim();
                    TXT_Ven.Text = dt_cab.Rows[0]["nom_mer"].ToString().Trim();
                    TXT_Ciud.Text = dt_cab.Rows[0]["nom_ciu"].ToString().Trim();
                    TXT_obser.Text = dt_cab.Rows[0]["des_mov"].ToString().Trim();
                }


                SqlConnection con1 = new SqlConnection(SiaWin._cn);
                SqlCommand cmd1 = new SqlCommand();
                SqlDataAdapter da1 = new SqlDataAdapter();
                DataTable ds1 = new DataTable();
                cmd1 = new SqlCommand("_EmpPvPedidoAnulacion", con1);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@cod_trn", "505");
                cmd1.Parameters.AddWithValue("@idreg", id);
                cmd1.Parameters.AddWithValue("@num_trn", TXT_documento.Text);
                cmd1.Parameters.AddWithValue("@_codemp", cod_empresa);
                da1 = new SqlDataAdapter(cmd1);
                da1.Fill(ds1);
                con1.Close();

                if (ds1.Rows.Count > 0)
                {
                    dataGridCxC.ItemsSource = ds1.DefaultView;
                    TX_total.Text = ds1.Rows.Count.ToString();

                    var uiElement = e.OriginalSource as UIElement;
                    uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    dataGridCxC.SelectedIndex = 1;
                    dataGridCxC.Focus();
                    dataGridCxC.MoveCurrentCell(new RowColumnIndex(1, 1), false);
                    dataGridCxC.ScrollInView(new RowColumnIndex(1, 1));
                }
                else
                {
                    MessageBox.Show("el pedido no tiene items pendientes");
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("Error nen la consulta ppp:" + w);
            }
        }


        private void dataGridCxC_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                string id = row["idreg"].ToString();
                string est_anu = row["est_anu"].ToString();
                string cod_anu = row["cod_anu"].ToString();

                bool flag = getConcepto(cod_anu);
                if (flag)
                {
                    string query = "update incue_doc set cod_anu='" + cod_anu + "' where idreg='" + id + "';";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("error al actualizar"); }
                }
                else
                {
                    MessageBox.Show("el concepto ingresado no existe");
                    row["cod_anu"] = "";
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al editar");
            }
        }


        public bool getConcepto(string conce)
        {
            bool flag = false;
            if (string.IsNullOrWhiteSpace(conce)) flag = true;
            string query = "select cod_anu,det_anu from InConaped where cod_anu='" + conce.Trim() + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "Buscar", idemp);
            if (dt.Rows.Count > 0) flag = true;
            return flag;
        }

        public void clearControls()
        {
            TXT_documento.Text = "";
            TXT_fecha.Text = "";
            TXT_codigo.Text = "";
            TXT_nombre.Text = "";
        }


        #region antiguo

        //private void Chek_anular_Checked(object sender, RoutedEventArgs e)
        //{
        //    Handle(sender as CheckBox,"a");
        //}
        //private void Chek_anular_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    Handle(sender as CheckBox,"a");
        //}
        //void Handle(CheckBox checkBox,string a)
        //{
        //    try
        //    {

        //        DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
        //        bool flag = checkBox.IsChecked.Value;
        //        string query = "";
        //        if (flag == true)
        //        {
        //            query = "update InCue_doc set est_anu='A' where idreg='" + row["idreg"].ToString() + "' and cod_trn='505'";
        //        }
        //        else
        //        {
        //            query = "update InCue_doc set est_anu='' where idreg='" + row["idreg"].ToString() + "' and cod_trn='505'";
        //        }

        //        if (SiaWin.Func.SqlCRUD(query, idemp) == true)
        //        {
        //            string text = flag == true ? "ANULO REF: "+ row["cod_ref"].ToString().Trim()+" DEL DOCUMENTO:"+ row["idregcab"].ToString().Trim() + " ": "DESMARCO ANULACION REF: " + row["cod_ref"].ToString().Trim() + " DEL DOCUMENTO:" + row["idregcab"].ToString().Trim() + " ";

        //            SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, text, "");
        //            row["est_anu"] = flag == true ? "A" : "";                    
        //        }
        //        else
        //        {
        //            MessageBox.Show("fallo la operacion");
        //        }

        //    }
        //    catch (Exception w)
        //    {
        //        SiaWin.Func.SiaExeptionGobal(w);
        //        MessageBox.Show("seleccione una referencia para anular1");
        //    }
        //}

        //private void Feha_Anu_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (dataGridCxC.SelectedIndex >= 0)
        //        {
        //            DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
        //            if (Feha_Anu.Text.Length <= 0) return;

        //            //DateTime fecha = row["fec_anu"] != null ? Convert.ToDateTime(row["fec_anu"].ToString()) : Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));
        //            //if (Feha_Anu.Text == fecha.ToString("dd/MM/yyyy")) return;

        //            string query = "update InCue_doc set fec_anu='" + Feha_Anu.Text + "' where idreg='" + row["idreg"].ToString() + "' and cod_trn='505' ";

        //            if (SiaWin.Func.SqlCRUD(query, idemp) == true)
        //            {
        //                //MessageBox.Show("query:"+ query);
        //                row["fec_anu"] = Feha_Anu.Text;
        //            }
        //            else
        //            {
        //                MessageBox.Show("fallo la operacion");
        //            }
        //        }


        //    }
        //    catch (Exception w) { MessageBox.Show("seleccione una referencia para anular2:" + w); }

        //}

        //private void comboBoxConcept_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    try
        //    {

        //        if (dataGridCxC.SelectedIndex >= 0)
        //        {
        //            if (comboBoxConcept.SelectedIndex >= 0)
        //            {
        //                DataRowView rowCombo = (DataRowView)comboBoxConcept.Items[comboBoxConcept.SelectedIndex];
        //                DataRowView rowGrilla = (DataRowView)dataGridCxC.SelectedItems[0];

        //                if (rowCombo["det_anu"].ToString().Trim() != rowGrilla["det_anu"].ToString().Trim())
        //                {
        //                    string query = "update InCue_doc set cod_anu='" + comboBoxConcept.SelectedValue + "' where idreg='" + rowGrilla["idreg"].ToString() + "' and cod_trn='505' ";
        //                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
        //                    {
        //                        rowGrilla["cod_anu"] = rowCombo["cod_anu"].ToString();
        //                        rowGrilla["det_anu"] = rowCombo["det_anu"].ToString();
        //                    }
        //                }

        //            }
        //        }


        //    }
        //    catch (Exception w)
        //    {
        //        SiaWin.Func.SiaExeptionGobal(w);
        //        MessageBox.Show("seleccione una referencia para anular3:" + w);
        //    }

        //}

        #endregion


        //****************** segundo tab *****************************************************
        private void BTNconsultarConsulta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dataGridConsulta.ItemsSource = null;
                if (string.IsNullOrEmpty(TX_documentoConsulta.Text) || string.IsNullOrEmpty(TX_idregConsulta.Text)) { MessageBox.Show("Ingrese un documento para consultar"); return; }

                string sql_cab = "select cabeza.idreg,cabeza.num_trn,cabeza.fec_trn,cabeza.cod_cli,tercero.nom_ter from InCab_doc as cabeza ";
                sql_cab = sql_cab + "inner join comae_ter as tercero on cabeza.cod_cli = tercero.cod_ter ";
                sql_cab = sql_cab + "where idreg='" + TX_idregConsulta.Text + "' ";

                DataTable dt_cab = SiaWin.Func.SqlDT(sql_cab, "Buscar", idemp);
                if (dt_cab.Rows.Count > 0)
                {
                    TXT_documento.Text = dt_cab.Rows[0]["num_trn"].ToString().Trim();
                    TXT_fecha.Text = dt_cab.Rows[0]["fec_trn"].ToString().Trim();
                    TXT_codigo.Text = dt_cab.Rows[0]["cod_cli"].ToString().Trim();
                    TXT_nombre.Text = dt_cab.Rows[0]["nom_ter"].ToString().Trim();
                }


                string sql_cue = "select cuerpo.est_anu,CONVERT(VARCHAR(10), cuerpo.fec_anu, 103) as fec_anu,cuerpo.cod_anu,concepto.det_anu,cuerpo.idreg,cuerpo.idregcab,cuerpo.cod_ref,referencia.nom_ref,cuerpo.cantidad,cuerpo.val_uni,cuerpo.subtotal,cuerpo.por_des,cuerpo.tot_tot,referencia.val_ref,cuerpo.val_iva,cuerpo.val_des from InCue_doc as cuerpo ";
                sql_cue += "inner join InMae_ref as referencia on cuerpo.cod_ref = referencia.cod_ref ";
                sql_cue += "inner join InCab_doc as cabeza on cuerpo.idregcab = cabeza.idreg ";
                sql_cue += "left join InConaped as concepto on cuerpo.cod_anu = concepto.cod_anu ";
                sql_cue += "where cuerpo.cod_trn='505' and  cuerpo.idregcab='" + TX_idregConsulta.Text + "'  ";
                DataTable dt_cue = SiaWin.Func.SqlDT(sql_cue, "Buscar", idemp);
                if (dt_cue.Rows.Count > 0)
                {
                    dataGridConsulta.ItemsSource = dt_cue.DefaultView;
                    TotConsulta.Text = dt_cue.Rows.Count.ToString();
                }

            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("Error nen la consulta DE ANULADOS:" + w);
            }
        }


        private void dataGridCxC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (dataGridCxC.SelectedIndex >= 0)
                {
                    DataRowView row = (DataRowView)dataGridCxC.SelectedItems[0];
                    GridColumn colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;

                    if (e.Key == Key.F6)
                    {
                        if (colum.MappingName == "cod_anu")
                        {
                            int idr = 0; string code = ""; string nombre = "";
                            dynamic xx = SiaWin.WindowBuscar("InConaped", "cod_anu", "det_anu", "cod_anu", "cod_anu", "Concepto Anulacion", cnEmp, true, "", idEmp: idemp);
                            xx.ShowInTaskbar = false;
                            xx.Owner = Application.Current.MainWindow;
                            xx.Height = 400;
                            xx.Width = 500;
                            xx.ShowDialog();
                            idr = xx.IdRowReturn;
                            code = xx.Codigo;
                            nombre = xx.Nombre;
                            xx = null;
                            if (!string.IsNullOrEmpty(code))
                            {
                                DataRowView rowtbl = (DataRowView)dataGridCxC.SelectedItems[0];
                                rowtbl["cod_anu"] = code;
                                string query = "update incue_doc set cod_anu='" + rowtbl["cod_anu"].ToString() + "' where idreg='" + rowtbl["idreg"].ToString() + "';";
                                if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("error al actualizar"); }
                            }
                            //}
                        }
                    }

                    if (colum.MappingName == "est_anu")
                    {

                        string a = row["est_anu"].ToString();
                        if (string.IsNullOrEmpty(a))
                        {
                            row["est_anu"] = "A";

                            string query = "update incue_doc set est_anu='" + row["est_anu"].ToString() + "' where idreg='" + row["idreg"].ToString() + "';";
                            if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("error al actualizar"); }
                        }
                        else
                        {
                            row["est_anu"] = "";
                            string query = "update incue_doc set est_anu='" + row["est_anu"].ToString() + "' where idreg='" + row["idreg"].ToString() + "';";
                            if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("error al actualizar"); }
                        }

                        //MessageBox.Show("a:" + a);
                        e.Handled = true;
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar los documentos");
            }
        }

        private void BtnAnular_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Usted desea anular todo el pedido ", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (dataGridCxC.View.Records.Count > 0)
                    {
                        var reflector = this.dataGridCxC.View.GetPropertyAccessProvider();
                        int a = 1;
                        foreach (var row in dataGridCxC.View.Records)
                        {
                            var rowData = dataGridCxC.GetRecordAtRowIndex(a);
                            var id = reflector.GetValue(rowData, "idreg");

                            string query = "update incue_doc set est_anu='A' where idreg='" + id + "';";
                            if (SiaWin.Func.SqlCRUD(query, idemp) == false) { MessageBox.Show("error al actualizar"); }
                            a = a + 1;
                        }

                        BTNconsultar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al actualizar:" + w);
            }
        }

        private void BtnAn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("usted desar anular los pedidos del rango de fecha " + tx_fecini.Text + " al " + tx_fecfin.Text,"Ejecutar Procesis", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    string query = "UPDATE InCue_doc SET InCue_doc.est_anu = 'A' FROM InCue_doc SI	";
                    query += "INNER JOIN  (select InCab_doc.idreg,InCab_doc.num_trn,InCab_doc.cod_trn,InCab_doc.fec_trn ";
                    query += "from InCab_doc inner join InCue_doc on InCab_doc.idreg = InCue_doc.idregcab) RAN ";
                    query += "ON  SI.num_trn = RAN.num_trn and SI.cod_trn = RAN.cod_trn where CONVERT(date,RAN.fec_trn,103) between '" + tx_fecini.Text + "' and '" + tx_fecfin.Text + "' and RAN.cod_trn='505' ";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    { MessageBox.Show("se anulo exitosamente los pedidos","alerta",MessageBoxButton.OK,MessageBoxImage.Information); }                    
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al anular:" + w);
            }
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "select InCue_doc.est_anu,InCab_doc.num_trn,* from InCue_doc  ";
                query += "inner join InCab_doc on InCue_doc.idregcab = InCab_doc.idreg ";
                query += "where CONVERT(date,InCab_doc.fec_trn,103) between '" + tx_fecini.Text + "' and '" + tx_fecfin.Text + "' and InCab_doc.cod_trn='505' ";
                query += "order by InCab_doc.num_trn ";

                DataTable tabla = SiaWin.Func.SqlDT(query, "Buscar", idemp);
                if (tabla.Rows.Count>0)
                {
                    SiaWin.Browse(tabla);
                }                
            }
            catch (Exception w)
            {
                MessageBox.Show("erro al ver los pedidos:"+w);
            }
        }


    }
}
