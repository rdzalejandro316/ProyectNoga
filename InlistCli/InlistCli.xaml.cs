using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9519,"InlistCli");
    //dynamic WinDescto = ((Inicio)Application.Current.MainWindow).WindowExt(9519,"InlistCli");
    //WinDescto.ShowInTaskbar = false;
    //WinDescto.Owner = Application.Current.MainWindow;
    //WinDescto.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    //WinDescto.ShowDialog(); 
    public partial class InlistCli : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";



        public InlistCli()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId; ;
            //LoadConfig();

            this.MinHeight = 500;
            this.MaxHeight = 500;
            this.MaxWidth = 1000;
            this.MinWidth = 1000;
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
                this.Title = "Bodegas en consignacion " + cod_empresa + "-" + nomempresa;

                TX_fec_ult.Text = DateTime.Now.ToString();
                TX_fec_venc.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                SiaWin.Func.SiaExeptionGobal(e);
                MessageBox.Show("error en el load" + e.Message);
            }
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
                        cmptabla = tag; cmpcodigo = "cod_bod"; cmpnombre = "UPPER(nom_bod)"; cmporden = "cod_bod"; cmpidrow = "cod_bod"; cmptitulo = "Maestra de Bodegas"; cmpconexion = cnEmp; mostrartodo = true; cmpwhere = "tipo_bod='4' ";
                    }

                    int idr = 0; string code = ""; string nom = "";
                    dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);
                    winb.ShowInTaskbar = false;
                    winb.Owner = Application.Current.MainWindow;
                    winb.Height = 400;
                    winb.Width = 500;
                    winb.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    winb.ShowDialog();
                    idr = winb.IdRowReturn;
                    code = winb.Codigo;
                    nom = winb.Nombre;
                    winb = null;
                    if (idr > 0)
                    {
                        if (tag == "inmae_bod")
                        {
                            TX_CodBod.Text = code;
                            TX_NomBod.Text = nom;

                            cargarList(code);
                        }
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public void returnNit(string cod_bod)
        {
            // string query = "select * from inmae_bod where cod_bod='"+ cod_bod + "';";
            string query = "select InMae_bod.cod_ter,Comae_ter.nom_ter from InMae_bod ";
            query += "inner join Comae_ter on InMae_bod.cod_ter = Comae_ter.cod_ter ";
            query += "where InMae_bod.cod_bod = '" + cod_bod + "' ";

            DataTable dt = SiaWin.Func.SqlDT(query, "Clientes", idemp);
            if (dt.Rows.Count > 0)
            {
                TX_tercero.Text = dt.Rows[0]["cod_ter"].ToString();
                TX_Nametercero.Text = dt.Rows[0]["nom_ter"].ToString();
            }
            else
            {
                TX_tercero.Text = "";
                TX_Nametercero.Text = "";
            }
        }

        public void cargarList(string cod_bod)
        {
            try
            {

                //antiguo valor ISNULL(NULLIF(ltrim(rtrim((lista.Valor))), 0), 0) as valor

                string query = "select lista.cod_bod as cod_bod,lista.cod_ter as cod_ter,tercero.nom_ter,lista.Cod_ref as cod_ref,referencia.nom_ref,lista.val_ref,lista.Por_des as por_des,lista.descto as descto,ISNULL(lista.Valor,0) as valor,lista.iva as iva,lista.val_uni as val_uni,  ";
                query += "lista.fec_ult as fec_ult,lista.fec_venc as fec_venc,lista.cod_ant as cod_ant,tablaIva.por_iva as por_iva,lista.ref_cli ";
                query += "from InList_cli as lista ";
                query += "left join Comae_ter as tercero on lista.Cod_ter=tercero.cod_ter ";
                query += "left join InMae_ref as referencia on lista.Cod_ref=referencia.cod_ref ";
                query += "left join InMae_tiva as tablaIva on referencia.cod_tiva=tablaIva.cod_tiva ";
                query += "where lista.Cod_bod='" + cod_bod + "' ";

                DataTable dt = SiaWin.Func.SqlDT(query, "Clientes", idemp);
                if (dt.Rows.Count > 0)
                {
                    //referencia(cod_bod);
                    DataGridInlistCli.ItemsSource = dt.DefaultView;
                }
                else
                {
                    DataGridInlistCli.ItemsSource = null;
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al cargar lista:" + w);
            }
        }


        //public void referencia(string cod_bod)
        //{
        //    string query = "UPDATE InList_cli SET Val_ref = RAN.val_ref FROM InList_cli SI	 ";
        //    query += "INNER JOIN  (select InMae_ref.cod_ref,InMae_ref.val_ref from InMae_ref) RAN ";
        //    query += "ON  SI.Cod_ref = RAN.cod_ref where SI.Cod_bod='"+cod_bod+"'";

        //    if (SiaWin.Func.SqlCRUD(query, idemp) == false)
        //    {
        //        MessageBox.Show("actualizacion fallida");
        //    }
        //}



        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)DataGridInlistCli.SelectedItems[0];
                string tercero = row["cod_ter"].ToString().Trim();
                string referencia = row["cod_ref"].ToString().Trim();
                string bodega = row["cod_bod"].ToString().Trim();

                if (MessageBox.Show("Usted desea eliminar la referencia:"+referencia, "Eliminar", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {

                    string query = "delete InList_cli where cod_ref='" + referencia + "' and cod_bod='" + bodega + "' ";
                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        MessageBox.Show("Eliminacion exitosa");
                    }
                    cargarList(TX_CodBod.Text.Trim());
                }                
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al eliminar:" + w);
            }
        }

        private void DataGridDesLine_CurrentCellEndEdit(object sender, Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventArgs e)
        {
            try
            {


                //MessageBox.Show("estamos en mantenimineto por favor espere");

                DataRowView row = (DataRowView)DataGridInlistCli.SelectedItems[0];
                decimal n;
                                                

                if (e.RowColumnIndex.ColumnIndex == 10)
                {
                    decimal valref = Convert.ToDecimal(row["val_ref"] == DBNull.Value || decimal.TryParse(row["val_ref"].ToString(), out n) == false ? 0 : row["val_ref"]);                    
                    decimal valuni = Convert.ToDecimal(row["val_uni"] == DBNull.Value || decimal.TryParse(row["val_uni"].ToString(), out n) == false ? 0 : row["val_uni"]);
                    decimal diferencia = valref - valuni;
                    decimal por = (diferencia * 100) / valref;
                    //MessageBox.Show("por:"+ por);
                    row["por_des"] = Convert.ToDecimal(por);
                    DataGridInlistCli.UpdateDataRow(e.RowColumnIndex.RowIndex);
                    DataGridInlistCli.UpdateLayout();
                    DataGridInlistCli.Columns["por_des"].AllowEditing = true;
                }                                              


                decimal Val_ref = Convert.ToDecimal(row["val_ref"] == DBNull.Value || decimal.TryParse(row["val_ref"].ToString(), out n) == false ? 0 : row["val_ref"]);
                decimal Por_des = Convert.ToDecimal(row["por_des"] == DBNull.Value || decimal.TryParse(row["por_des"].ToString(), out n) == false ? 0 : row["por_des"]);
                decimal Descto = Convert.ToDecimal(row["descto"] == DBNull.Value || decimal.TryParse(row["descto"].ToString(), out n) == false ? 0 : row["descto"]);
                decimal Valor = Convert.ToDecimal(row["valor"] == DBNull.Value || decimal.TryParse(row["valor"].ToString(), out n) == false ? 0 : row["valor"]);
                decimal Val_uni = Convert.ToDecimal(row["val_uni"] == DBNull.Value || decimal.TryParse(row["val_uni"].ToString(), out n) == false ? 0 : row["val_uni"]);
                

                string ref_cli = row["ref_cli"].ToString().Trim();
                string bodega = row["cod_bod"].ToString().Trim();
                string tercero = row["cod_ter"].ToString().Trim();
                string referencia = row["cod_ref"].ToString().Trim();


                string query = "update InList_cli set val_ref=" + Val_ref.ToString("F", CultureInfo.InvariantCulture) + ",por_des=" + Por_des.ToString("F", CultureInfo.InvariantCulture) + ",descto=" + Descto.ToString("F", CultureInfo.InvariantCulture) + ",valor=" + Valor.ToString("F", CultureInfo.InvariantCulture) + ",Val_uni=" + Val_uni.ToString("F", CultureInfo.InvariantCulture) + ",ref_cli='" + ref_cli + "' where cod_bod='" + bodega + "' and cod_ref='" + referencia + "' ";

                //string query = "update InTer_tip set Por_des=" + Por_des.ToString("F", CultureInfo.InvariantCulture) + ",des_mos=" + des_mos.ToString("F", CultureInfo.InvariantCulture) + ",des_ppag=" + des_ppag.ToString("F", CultureInfo.InvariantCulture) + " where Cod_ter='" + tercero.Trim() + "' and Cod_tip='" + linea + "' ";                
                //MessageBox.Show(query);

                if (SiaWin.Func.SqlCRUD(query, idemp) == false)
                {
                    MessageBox.Show("Fallo la actualizacion de la tabla");
                    
                }
                else
                {
                    //System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                    //dr["por_des"] = Por_des;                                                            


                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, "ACTUALIZO AL CLIENTE:" + tercero + " DE LA BODEGA:" + bodega + " - INLIST_CLI ", "");
                }
                //SiaWin.Func.SqlCRUD(query, idemp);


            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al editar" + w);
            }
        }

        public void bloquear(bool bloq)
        {
            if (bloq == true)
            {
                GridMain.IsEnabled = bloq;
                returnNit(TX_CodBod.Text);
            }

            if (bloq == false)
            {
                GridMain.IsEnabled = bloq;

                TX_tercero.Text = "";
                TX_referencia.Text = "";
                TX_referencia_ant.Text = "";
                TX_Nametercero.Text = "";
                TX_referenciaName.Text = "";
                TX_refCli.Text = "";
                TX_fec_ult.Text = DateTime.Now.ToString();
                TX_fec_venc.Text = DateTime.Now.ToString();
                TX_valRef.Value = 0;
                TX_Pordect.Value = 0;
                TX_Valor.Value = 0;
                TX_Iva.Value = 0;
                TX_ValUni.Value = 0;
            }
        }

        private void BTNadd_Click(object sender, RoutedEventArgs e)
        {
            if (TX_CodBod.Text == "")
            {
                MessageBox.Show("!selecione la bodega para poder adicionar¡");
                return;
            }
            bloquear(true);
        }

        private void BTNguardar_Click(object sender, RoutedEventArgs e)
        {
            if (TX_tercero.Text == "")
            {
                MessageBox.Show("llene el campo de tercero");
                return;
            }

            if (TX_referencia.Text == "" && TX_referencia_ant.Text == "")
            {
                MessageBox.Show("llene el campo de referencia");
                return;
            }

            if (TX_fec_ult.Text == "" || TX_fec_ult.Text.Length > 10 || TX_fec_venc.Text == "" || TX_fec_venc.Text.Length > 10)
            {
                MessageBox.Show("llene los campo de fechas");
                return;
            }

            insert();
        }

        public void insert()
        {
            try
            {
                if (validarExistencia() == true)
                {
                    MessageBox.Show("La bodega,la referencia y el tercero ya estan registrados en la tabla");
                    return;
                }


                string query = "insert into InList_cli (cod_bod,cod_ter,cod_ref,ref_cli,val_ref,por_des,descto,valor,iva,val_uni,fec_ult,fec_venc,cod_ant,fecha_aded) ";
                query += "values ('" + TX_CodBod.Text.Trim() + "','" + TX_tercero.Text.Trim() + "','" + TX_referencia.Text.Trim() + "','" + TX_refCli.Text.Trim() + "'," + TX_valRef.Value + "," + TX_Pordect.Value + "," + TX_Descto.Value + "," + TX_Valor.Value + "," + TX_Iva.Value + "," + TX_ValUni.Value + ",'" + TX_fec_ult.Text + "','" + TX_fec_venc.Text + "','" + TX_referencia_ant.Text.Trim() + "','" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "') ";
                if (SiaWin.Func.SqlCRUD(query, idemp) == false)
                {
                    MessageBox.Show("Fallo el insertar los datos");
                }
                else
                {
                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, "INSERTO CLIENTE A LA LISTA DE LA BODEGA:" + TX_CodBod.Text, "");
                    cargarList(TX_CodBod.Text.ToString().Trim());
                    bloquear(false);
                }
            }
            catch (Exception w)
            {
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al insert:" + w);
            }

        }

        public bool validarExistencia()
        {

            string query = "select * from InList_cli where cod_ref='" + TX_referencia.Text + "' and cod_bod='" + TX_CodBod.Text + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "Clientes", idemp);
            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }


        private void BTNcancelar_Click(object sender, RoutedEventArgs e)
        {
            bloquear(false);
        }

        private void TX_LostFocus(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(((TextBox)sender).Text)) return;

            string Tag = ((TextBox)sender).Tag.ToString();
            string codigo = "";
            string nombre = "";
            //TX_referencia_ant
            TextBox campoNombre = new TextBox();

            switch (Tag)
            {
                case "comae_ter":
                    codigo = "cod_ter"; nombre = "nom_ter"; campoNombre = (TextBox)this.FindName("TX_Nametercero");
                    break;
                case "inmae_ref":
                    codigo = "cod_ref"; nombre = "nom_ref"; campoNombre = (TextBox)this.FindName("TX_referenciaName");
                    break;
            }

            string cadena = "select * from " + Tag + "  where  " + codigo + "='" + ((TextBox)sender).Text.ToString() + "'  ";

            DataTable tabla = SiaWin.Func.SqlDT(cadena, "Buscar", idemp);
            if (tabla.Rows.Count > 0)
            {
                if (Tag == "inmae_ref")
                {
                    TX_referencia_ant.Text = tabla.Rows[0]["cod_ant"].ToString();
                    TX_valRef.Value = Convert.ToDecimal(tabla.Rows[0]["val_ref"].ToString());
                }

                ((TextBox)sender).Text = tabla.Rows[0][codigo].ToString();
                campoNombre.Text = tabla.Rows[0][nombre].ToString();


            }
            else
            {
                MessageBox.Show("el codigo ingresado no existe");
                ((TextBox)sender).Text = "";
                campoNombre.Text = "";
                if (Tag == "inmae_ref") TX_referencia_ant.Text = "";
            }



        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                string idTab = ((TextBox)sender).Tag.ToString();

                if (e.Key == Key.Enter || e.Key == Key.F8)
                {
                    if (idTab.Length > 0)
                    {
                        string tag = ((TextBox)sender).Tag.ToString();
                        string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                        if (string.IsNullOrEmpty(tag)) return;

                        if (tag == "comae_ter")
                        {
                            cmptabla = tag; cmpcodigo = "cod_ter"; cmpnombre = "nom_ter"; cmporden = "cod_ter"; cmpidrow = "cod_ter"; cmptitulo = "Maestra de Terceros"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                        }
                        if (tag == "inmae_ref")
                        {
                            cmptabla = tag; cmpcodigo = "cod_ref"; cmpnombre = "nom_ref"; cmporden = "cod_ref"; cmpidrow = "idrow"; cmptitulo = "Maestra de Referencias"; cmpconexion = cnEmp; mostrartodo = false; cmpwhere = "";
                        }

                        string code = ""; string nom = "";
                        dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);

                        winb.ShowInTaskbar = false;
                        winb.Owner = Application.Current.MainWindow;
                        winb.Height = 400;
                        winb.Width = 500;
                        winb.ShowDialog();
                        //idr = winb.IdRowReturn;
                        code = winb.Codigo;
                        nom = winb.Nombre;
                        winb = null;
                        if (!string.IsNullOrWhiteSpace(code))
                        {
                            if (tag == "comae_ter")
                            {
                                TX_tercero.Text = code.Trim();
                                TX_Nametercero.Text = nom;
                            }
                            if (tag == "inmae_ref")
                            {
                                TX_referencia.Text = code.Trim();
                                TX_referenciaName.Text = nom;
                                //MessageBox.Show("si");
                                cod_anteriot(code.Trim());
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

            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message.ToString());
            }
        }

        public void cod_anteriot(string codigo)
        {
            string query = "select * from inmae_ref where cod_ref='" + codigo.Trim() + "' ";
            //MessageBox.Show(query);

            DataTable tabla = SiaWin.Func.SqlDT(query, "Buscar", idemp);
            if (tabla.Rows.Count > 0) TX_referencia_ant.Text = tabla.Rows[0]["cod_ant"].ToString();
        }

        private void BTNExpo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;

                var excelEngine = DataGridInlistCli.ExportToExcel(DataGridInlistCli.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;

                SaveFileDialog sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx"
                };

                if (sfd.ShowDialog() == true)
                {
                    using (Stream stream = sfd.OpenFile())
                    {
                        if (sfd.FilterIndex == 1)
                            workBook.Version = ExcelVersion.Excel97to2003;
                        else if (sfd.FilterIndex == 2)
                            workBook.Version = ExcelVersion.Excel2010;
                        else
                            workBook.Version = ExcelVersion.Excel2013;
                        workBook.SaveAs(stream);
                    }

                    //Message box confirmation to view the created workbook.
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:" + w);
            }
        }

        private void BTNMasivo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic ww = SiaWin.WindowExt(9636, "ListaPrecionBodMasivo");
                ww.ShowInTaskbar = false;
                ww.idemp = idemp;
                ww.Owner = Application.Current.MainWindow;
                ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ww.ShowDialog();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir:" + w);
            }
        }

        private void TX_ValUni_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal n;
                decimal Val_ref = Convert.ToDecimal(decimal.TryParse(TX_valRef.Value.ToString(), out n) == false ? 0 : TX_valRef.Value);
                decimal Val_uni = Convert.ToDecimal(decimal.TryParse(TX_ValUni.Value.ToString(), out n) == false ? 0 : TX_ValUni.Value);
                decimal diferencia = Val_ref - Val_uni;
                decimal por = (diferencia * 100) / Val_ref;
                TX_Pordect.Value = Convert.ToDouble(por);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al actualizar el porcentaje:" + w);
            }
        }


    }
}
