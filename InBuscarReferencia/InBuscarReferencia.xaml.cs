using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    /// 
    public partial class InBuscarReferencia : Window
    {
        //Sia.PublicarPnt(9326,"InBuscarReferencia");
        dynamic SiaWin;
        string cmptabla; string cmpcodigo; string cmpnombre; string cmporden; string cmpIdRow;bool mostrartodo; string where;
        DataTable dt = new DataTable();
        private bool TiboBusqueda = true; //false= codigo,true=nombre
        private string codigo;
        private string nombre;
        private int idrowreturn;
        private int idemp;
        private bool Filtro = false;
        private string idbod;
        private string codemp;
        public string UltBusqueda="";
        public  string Conexion;
        public  DataSet ds1 = new DataSet() ;
        DateTime fechaCreacion;
        public int IdRowReturn
        {
            set { idrowreturn = value; }
            get { return idrowreturn; }
        }
        public string Codigo
        {
            set { codigo = value; }
            get { return codigo; }
        }
        public string Nombre
        {
            set { nombre = value; }
            get { return nombre; }
        }
        public string CmpTabla = "inmae_ref";
        public string CmpCodigo = "cod_ref";
        public string CmpNombre = "cod_ant";
        public string CmpOrden = "cod_ant";
        public string CmpIdRow = "idrow";
        public string CmpTitulo = "Maestra de Referencias";
        public bool MostrarTodo = false;
        public string Where = "";
        public int idEmp = 0;
        public string idBod = "";
        public string ultbusqueda = "";
        public InBuscarReferencia()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idEmp = SiaWin._BusinessId;
            cmptabla = CmpTabla;
            cmpcodigo = CmpCodigo;
            cmpnombre = CmpNombre;
            cmporden = CmpOrden;
            cmpIdRow = CmpIdRow;
            mostrartodo = MostrarTodo;
            where = Where;
            idemp = idEmp;
            idbod = idBod;
            this.Title = CmpTitulo;
            TxtTipoBusqueda.Text = "Busqueda por Nombre";
            //dataGrid.PreviewKeyDown += new KeyEventHandler(mainGrid_PreviewKeyDown);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            dt.Clear();
            try
            {
                if (TxtShear.Text.Trim() == string.Empty) TxtShear.Focus();
                if (TxtShear.Text.Trim() == string.Empty) return;
                string bb = TxtShear.Text.Trim();
                dataGrid.ItemsSource = null;
                string www = string.Empty;
                if (TiboBusqueda)  www = TxtConvertTextSinFiltro(bb);
                if (!TiboBusqueda) www = bb;
                if (Filtro==false) www = TxtConvertTextSinFiltro(bb);
                if (Filtro == true) www = TxtConvertText(bb);
                if (www.Trim() != "") www = " and " + www;
                dt = GetDataTable(" where (inmae_ref.estado=1 ) " + www);
                foreach (System.Data.DataColumn col in dt.Columns) col.ReadOnly = false;
                //dv = new DataView(dt);
                //        dv.Sort = "nom_ref ASC,cod_ant ASC";
                //dv.Sort = "nombre ASC";
                dataGrid.ItemsSource = dt.DefaultView; ;

                //dataGrid.ItemsSource = GetDataTable(" where " + www).DefaultView;
                if (dataGrid.Items.Count == 0) return;
                //dataGrid.SelectedItem = dataGrid.Items[1];
                dataGrid.Focus();
                //dataGrid.SelectedIndex = 0;

                var uiElement = e.OriginalSource as UIElement;
                dataGrid.SelectedItem = dataGrid.Items[0];
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[0]);
                dataGrid.CommitEdit();
                dataGrid.UpdateLayout();
                dataGrid.SelectedIndex = dataGrid.SelectedIndex;
                dataGrid.Focus();

                //mierda
                TxtShear.Focus();
                //foreach (System.Data.DataColumn col in dt.Columns) col.ReadOnly = false;
                //dataGrid.ScrollIntoView(dataGrid.SelectedItem, dataGrid.Columns[0]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        public DataTable GetDataTable(string _where)
        {
            try
            {

                //IIF(RTRIM(COD_ANT) = RTRIM(NOM_REF), NOM_REF, RTRIM(COD_ANT) + ' ' + NOM_REF)

                //string sql = "select top 300 IIF(RTRIM(COD_ANT) = RTRIM(NOM_REF), NOM_REF, RTRIM(COD_ANT) + ' ' + NOM_REF)   as nombre," + cmpcodigo + " as codigo,val_ref,00000000.00 as saldo,cod_prv,inmae_ref.cod_tip,tip.por_des,tip.por_desc  from inmae_ref inner join inmae_tip as tip on tip.cod_tip=inmae_ref.cod_tip "+ _where + " order by nombre" ;
                string sql = "select top 450 IIF(RTRIM(NOM_REF) = RTRIM(COD_ANT), RTRIM(COD_ANT), RTRIM(COD_ANT) + ' /' + RTRIM(NOM_REF))   as nombre," + cmpcodigo + " as codigo,val_ref,00000000.00 as saldo,cod_prv,inmae_ref.cod_tip,tip.por_des,tip.por_desc  from inmae_ref inner join inmae_tip as tip on tip.cod_tip=inmae_ref.cod_tip " + _where + " order by nombre";
                
                //Clipboard.SetText(sql);

                //MessageBox.Show(sql);
                dt = SiaWin.DB.SqlDT(sql, "productos", idemp);
                //SiaWin.Browse(dt);
                     //           SqlConnection conn1 = new SqlConnection(Conexion);
                //SqlCommand cmd1 = new SqlCommand(sql, conn1);
                //conn1.Open();
                //SqlDataReader dr = cmd1.ExecuteReader();
                //dt.Load(dr);
                TxtTotal.Content = "Total registros :" + dt.Rows.Count;
                //conn1.Close();
            }
            catch (SqlException SQLex)
            {
                MessageBox.Show("Error:" + SQLex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message);
            }
            return dt;
        }
        private string TxtConvertText(string txt)
        {
            string s = txt;
            // Split string on spaces.
            int inicount = 0;
            string cadena = "";
            string cadenaOR = "";
            string cadenaOROR = ""; // COD_ANT
            string[] words = s.Split(' ');
            foreach (string word in words)
            {
                if (inicount == 0)
                {
                    cadena = "rtrim(cod_ref) like '%" + word + "%'";
                    cadenaOR = "rtrim(" + cmpnombre + ") like '%" + word + "%'";
                    cadenaOROR = "rtrim(cod_ant) like '%" + word + "%'";
                }
                else
                {
                    cadena = cadena + " and rtrim(cod_ref) like '%" + word + "%'";
                    cadenaOR = cadenaOR + " and rtrim(" + cmpnombre + ") like '%" + word + "%'";
                    cadenaOROR = cadenaOROR + " and rtrim(COD_ANT) like '%" + word + "%'";
                }
                inicount = inicount + 1;
            }
            return "(" + cadena + " or " + cadenaOR + " or " + cadenaOROR + ")";
            //return cadena+" or "+cadenaOR+" or "+cadenaOROR ;
        }
        private string TxtConvertTextSinFiltro(string txt)
        {
            string s = txt;
             //return "rtrim(cod_ant)>='" + s.Trim() + "'";
            //return "(rtrim(cod_ref) like '" + s.Trim() + "%' OR rtrim(NOM_REF)+rtrim(cod_ant) LIKE '"+s.Trim()+"%')";
            return "(rtrim(cod_ref) like '" + s.Trim() + "%' OR rtrim(cod_ant) LIKE '" + s.Trim() + "%')";
            //select cod_ref, nom_ref from inmae_ref where (COD_REF LIKE '4515%' OR NOM_REF LIKE '4515%') AND ESTADO = 1
        }
        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectItem();
            e.Handled = true;
        }
        private void SelectItem()
        {
            DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
            if (row != null)
            {
                //int nPnt = Int32.Parse(row[0].ToString());
                this.Codigo = row[1].ToString();
                this.Nombre = row[0].ToString();
                //this.IdRowReturn = nPnt;
                UltBusqueda = TxtShear.Text;
                if(string.IsNullOrEmpty(UltBusqueda)) SiaWin.Func.Var["_UltimaReferenciaBuscar"] = row["codigo"].ToString().Trim();
                if (!string.IsNullOrEmpty(UltBusqueda)) SiaWin.Func.Var["_UltimaReferenciaBuscar"] = UltBusqueda;
            }
            else
            {
                this.IdRowReturn = -1;
            }
            this.Close();
        }
        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Escape)
            {
                SelectItem();
                e.Handled = true;
            }
            if (e.Key == Key.Left)
            {
                if (mostrartodo == false)
                {
                    
                    TxtShear.SelectAll();
                    TxtShear.Focus();
                    TxtShear.SelectAll();
                    e.Handled = true;
                }
            }
            if (e.Key == Key.F2)
            {
                DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                if (row != null)
                {
                   // int nPnt = Int32.Parse(row[0].ToString());
                    this.Codigo = row[1].ToString();
                    this.Nombre = row[0].ToString();
                    //this.IdRowReturn = nPnt;
                    /// valida si hay saldos en bodega
                    DataTable dtcnd = LoadBodega(codigo, idBod, 1); //cnd
                    if (dtcnd == null) return;
                    DataTable dtpv = LoadBodega(codigo, idBod, 2); //cnd
                    if (dtpv == null) return;
                    //DataTable dtcnd = LoadBodega(codigo, idBod, 21); //cnd
                    //DataSet ds1 = LoadData(codigo, idBod); //cnd
                    if (dtcnd.Rows.Count+ dtpv.Rows.Count == 0 )
                    {
                        MessageBox.Show("Producto:" + codigo + "-" + Nombre.Trim() + " Sin saldos en bodegas..");
                        return;
                    }
                    //MessageBox.Show(ds1.Tables[0].Rows.Count.ToString());
                    SaldosBodegas xx = new SaldosBodegas(this.Codigo, this.Nombre, 0, Conexion, idbod, idemp);
                    xx.TxtLinea.Text= row["cod_tip"].ToString();
                    xx.TxtProveedor.Text = row["cod_prv"].ToString();
                    xx.dataGrid.ItemsSource = dtcnd.DefaultView;
                    xx.dataGridPV.ItemsSource = dtpv.DefaultView;
                    xx.TxtFecCrea.Text = fechaCreacion.ToShortDateString();
                    decimal sumInv = 0;
                    decimal sumImp = 0;
                    //foreach (System.Data.DataColumn col in ds1.Tables[0].Columns) col.ReadOnly = false;
                    foreach (DataRow dr in dtcnd.Rows) // search whole table
                    {
                        decimal saldoin = Convert.ToDecimal(dr["saldo"]);
                        //decimal saldoinImp = Convert.ToDecimal(dr["importacion"]);
                        //dr["saldo"] = saldoin; //change the name
                        sumInv = sumInv + saldoin;
                        //sumImp = sumImp + saldoinImp;
                    }
                    xx.TotalCndExit.Text = sumInv.ToString("N2");
                    //xx.TotalCndImpor.Text = sumImp.ToString("N2");
                    xx.TotalCnd.Text = (sumInv+sumImp).ToString("N2");
                    decimal sumInvPv = 0;
                    decimal sumImpPv = 0;
                    //foreach (System.Data.DataColumn col in ds1.Tables[0].Columns) col.ReadOnly = false;
                    foreach (DataRow dr in dtpv.Rows) // search whole table
                    {
                        decimal saldoinPv = Convert.ToDecimal(dr["saldo"]);
                        //decimal saldoinImpPv = Convert.ToDecimal(dr["importacion"]);
                        //dr["saldo"] = saldoin; //change the name
                        sumInvPv = sumInvPv + saldoinPv;
                        //sumImpPv = sumImpPv;
                            //+ saldoinImpPv;
                    }
                    xx.TotalPvExit.Text = sumInvPv.ToString("N2");
                    //xx.TotalPvImpor.Text = sumImpPv.ToString("N2");
                    xx.TotalPv.Text = (sumInvPv + sumImpPv).ToString("N2");
                    xx.TotalExit.Text = (sumInv+sumInvPv).ToString("N2");
                    //xx.TotalImpor.Text =(sumImp+sumImpPv).ToString("N2");
                    xx.Total.Text = (sumInv + sumInvPv).ToString("N2");
                    //xx.dataGrid.ItemsSource = ds1.Tables[0].DefaultView;
                    xx.ShowInTaskbar = false;
                    xx.Owner = Application.Current.MainWindow;
                    xx.dataGrid.Focus();
                    xx.dataGrid.SelectedItem = dataGrid.Items[0];
                    xx.dataGrid.SelectedIndex = 0;
                    xx.dataGrid.Focus();
                    xx.dataGrid.SelectedIndex = 0;
                    xx.ShowDialog();
                    e.Handled = true;
                }
            }
        }
        private void TxtShear_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnBuscar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                e.Handled = true;
            }
            if (e.Key == Key.Down)
            {
                if (dataGrid.Items.Count == 0) return;
                dataGrid.Focus();
                var uiElement = e.OriginalSource as UIElement;
                dataGrid.SelectedItem = dataGrid.Items[0];
                uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                dataGrid.CurrentCell = new DataGridCellInfo(dataGrid.Items[dataGrid.SelectedIndex], dataGrid.Columns[0]);
                dataGrid.CommitEdit();
                dataGrid.SelectedIndex = dataGrid.SelectedIndex;
                e.Handled = true;
            }
        }
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dt.Rows.Count == 0) return;
            DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
            if (row != null)
            {
                //int nReturn = Int32.Parse(row[0].ToString());
                //if (nReturn < 0) return;
                string codref = row[1].ToString();
                decimal saldoin = SiaWin.Func.SaldoInv(codref, idbod, codemp);
                SaldoInv.Text = saldoin.ToString();
                DataRowView DRV = (DataRowView)dataGrid.SelectedItem;
                DataRow DR = DRV.Row;
                DR.BeginEdit();
                DR["saldo"] = saldoin;
                DR.EndEdit();
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            idbod = idBod;
            idemp = idEmp;
            System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
            int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
            //cnEmp = foundRow["BusinessCn"].ToString().Trim();
            string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
            string nomempresa = foundRow["BusinessName"].ToString().Trim();
            //            tabitem.Logo(idLogo, ".png");
            //          tabitem.Title = "Analisis de Venta(" + aliasemp + ")";
            codemp = foundRow["BusinessCode"].ToString().Trim();
            this.Title = "Saldos de Inventario - Empresa:" + codemp + "-" + nomempresa;
            //GroupId = 0;
            ultbusqueda = UltBusqueda;
            if (TiboBusqueda) TxtTipoBusqueda.Text = "Busqueda por:";
            if (TiboBusqueda==false) TxtTipoBusqueda.Text = "Busqueda por:";
            
            if (MostrarTodo == true)
            {
                if (where != string.Empty)
                {
                    where = " where " + where;
                }
                dataGrid.ItemsSource = GetDataTable(where).DefaultView;
                BtnBuscar.Visibility = Visibility.Collapsed;
                TxtShear.Visibility = Visibility.Collapsed;
                dataGrid.SelectedIndex = 0;
                dataGrid.Focus();
            }
            else
            {
                if (ultbusqueda != string.Empty) TxtShear.Text = ultbusqueda;
                TxtShear.Text = "";
                TxtShear.Focus();
                TxtShear.SelectAll();
            }
            if (SiaWin.Func.Var.ContainsKey("_UltimaReferenciaBuscar")) TxtShear.Text = SiaWin.Func.Var["_UltimaReferenciaBuscar"].ToString().Trim();
            if (!SiaWin.Func.Var.ContainsKey("_UltimaReferenciaBuscar")) SiaWin.Func.Var.Add("_UltimaReferenciaBuscar", "");
            if (!string.IsNullOrEmpty(TxtShear.Text.Trim())) BtnBuscar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

        }
        private DataSet LoadData(string refe, string bod)
        {
            try
            {
                ds1.Clear();
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                //DataSet ds1 = new DataSet();
                //cmd = new SqlCommand("ConsultaCxcCxpAll", con);
                cmd = new SqlCommand("_EmpSaldosInventariosPorReferenciaBodegas", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Ref", refe);//if you have parameters.
                cmd.Parameters.AddWithValue("@Bod", "");//if you have parameters.
                cmd.Parameters.AddWithValue("@codemp", codemp);//if you have parameters.
                //cmd.Parameters.AddWithValue("@Where", where);//if you have parameters.
                da = new SqlDataAdapter(cmd);
                da.Fill(ds1);
                con.Close();
                return ds1;
                //VentasPorProducto.ItemsSource = ds.Tables[0];
                //VentaPorBodega.ItemsSource = ds.Tables[1];
                //VentasPorCliente.ItemsSource = ds.Tables[2];
            }
            catch (SqlException SQLex)
            {
                MessageBox.Show("Error SQL:" + SQLex.Message);

            }
            catch (Exception e)
            {
                MessageBox.Show("Error App:"+e.Message);
            }
            return null;
        }
        private DataTable LoadBodega(string refe, string bod,int tipo)
        {

            DataTable dttipo = new DataTable();
            DataTable dtUltVta = new DataTable();
            try
            {
                StringBuilder sqlUltFechaVta = new StringBuilder();
                string sqlor = "";

                

                if (tipo == 1) sqlor = " or tipo_bod=3 and inmae_bod.cod_emp<>'' and  inmae_bod.cod_bod<>'900'  ";                

                string sql = "select cod_bod,nom_bod,cod_emp,000000000.00 as saldo,0000000000.00 as importacion,00000000000.00 as total,0 as indactual,space(10) as ultfecvta,000000 as dias,space(10) as fec_crea from inmae_bod where tipo_bod="+tipo.ToString()+sqlor+" order by cod_bod";
                //MessageBox.Show(sql);
                dttipo = SiaWin.DB.SqlDT(sql, "SaldosBodega", idemp);
                
                if(dttipo.Rows.Count>0)
                {
                    foreach (DataRow dr in dttipo.Rows) // search whole table
                    {
                        string idbodx = dr["cod_bod"].ToString().Trim();
                        string codemp = dr["cod_emp"].ToString().Trim();
                        if (!string.IsNullOrEmpty(codemp))
                        {
                            decimal saldoin = SiaWin.Func.SaldoInv(refe, idbodx, codemp);
                            //decimal saldoinimp = SiaWin.Func.SaldoInv(refe, "980", codemp);
                            dr["saldo"] = saldoin; //change the name
                                                   //dr["importacion"] = saldoinimp; //change the name
                            dr["total"] = saldoin;
                            if (idbodx == bod) dr["indactual"] = 1;
                            /// trae ultima fecha de venta
                            sqlUltFechaVta.Append("select top 1  convert(date,fec_trn) AS ultfecvta,DATEDIFF(DAY,CAB.FEC_TRN,GETDATE()) AS dias,inmae_ref.fec_crea ");
                            sqlUltFechaVta.Append("from incab_doc as cab inner join incue_doc as cue on cue.idregcab = cab.idreg INNER JOIN INMAE_REF ON INMAE_REF.COD_REF=cue.cod_ref ");
                            sqlUltFechaVta.Append("where cue.cod_ref = '" + refe.Trim() + "' AND COD_BOD = '" + idbodx + "' AND CAB.COD_TRN BETWEEN '004' AND '005' AND CAB.COD_VEN <> '95' ORDER BY CAB.fec_trn DESC ");
                            dtUltVta.Clear();

                            // trae id de la empresa
                            int idempresa = idEmp;
                            if (!string.IsNullOrEmpty(codemp))
                            {
                                DataRow[] result = SiaWin.Empresas.Select("BusinessCode='" + codemp + "'");
                                if (result != null)
                                {
                                    foreach (DataRow row in result)
                                    {
                                        idempresa = (int)row["BusinessId"];
                                        //Console.WriteLine("{0}, {1}", row[0], row[1]);
                                    }

                                }
                            }
                            dtUltVta = SiaWin.DB.SqlDT(sqlUltFechaVta.ToString(), "tbl", idempresa);
                            if (dtUltVta != null && dtUltVta.Rows.Count > 0)
                            {
                                //       MessageBox.Show(codemp + "-" + idbodx+"-"+idempresa.ToString());

                                DateTime dtfechaultvta = Convert.ToDateTime(dtUltVta.Rows[0]["ultfecvta"].ToString()).Date;
                                DateTime dtfechacrea = Convert.ToDateTime(dtUltVta.Rows[0]["fec_crea"].ToString()).Date;
                                fechaCreacion = dtfechacrea.Date;


                                int diasaltura = Convert.ToInt32(dtUltVta.Rows[0]["dias"].ToString());
                                dr["ultfecvta"] = dtfechaultvta.Date;
                                dr["dias"] = diasaltura;
                                //dr["fec_crea"] = dtfechacrea.Date;
                                //     MessageBox.Show(codemp + "-" + idbodx + "-" + idempresa.ToString()+" fecha:"+dtfechaultvta.Date.ToString()+"-"+diasaltura.ToString());

                            }
                        }
                        sqlUltFechaVta.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message,"SaldosBodegas-LoadBodega");
            }
            return dttipo;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.F9)
            {
                if(Filtro==false)
                {
                    Filtro = true;
                    TxtFiltro.Visibility = Visibility.Visible;
                    dt.Clear();
                    TxtShear.Text = "";
                    TxtShear.Focus();
                }
                else
                {
                    Filtro = false;
                    TxtFiltro.Visibility = Visibility.Hidden;
                    TxtShear.Text = "";
                    dt.Clear();
                    TxtShear.Focus();
                }

            }
            return;
            if (e.Key==Key.F8)
            {
                if (TiboBusqueda)
                {
                    TiboBusqueda = false;
                    TxtTipoBusqueda.Text = "Busqueda por Codigo";
                }
                else
                {
                    TiboBusqueda = true;
                    TxtTipoBusqueda.Text = "Busqueda por Nombre";
                }
            }
        }

        private void TxtShear_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
