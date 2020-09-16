using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowPV
{
    public partial class AddReferencia : Window
    {


        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";


        public int idregcab = 0;
        public string numeroDoc = "";
        public string trn = "";
        public string codigo_ter = "";
        public string bodega = "";


        double procentaje_desc;
        double valor_ref;
        public double por_iva;
        string cod_tiva;


        //   CalcularDesc(Tx_CodRef.Text);
        double cantidad = 0;        
        public double val_uni = 0;//val_uni         
        public double subtotal = 0;//subtotal        
        public double valIva = 0;//val_iv
        public int valorIva = 0;
        public double total = 0;//tot_tot                


        public double val_ref = 0;
        //public double por_iva = 0;
        public double val_iva = 0;
        //public double por_ica = 0;
        public double val_ica = 0;
        //public double val_ret = 0;
        public double val_riva = 0;
        public double por_dec = 0;




        public Boolean bandera = false;

        public string campoDesInTer_tip = "";
        public string campoDeslinea = "";

        public bool ValueChange = false;

        public AddReferencia()
        {
            InitializeComponent();

            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;

            LoadConfig();
            pantalla();
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
                this.Title = "Agregar Referencia : " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void pantalla()
        {
            this.MaxHeight = 400;
            this.MinHeight = 400;
            this.MaxWidth = 500;
            this.MinWidth = 500;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TX_documen.Text = numeroDoc;
            TX_trn.Text = trn;
            //TX_idregcabeza.Text = idregcab;
        }

        public void BuscarRerf(string referencia)
        {
            try
            {
                string cadena = "select cod_ref,nom_ref from inmae_ref where cod_ref='" + referencia + "' ";
                DataTable dt = SiaWin.Func.SqlDT(cadena, "Clientes", idemp);

                if (dt.Rows.Count > 0)
                {
                    string Ref = dt.Rows[0]["cod_ref"].ToString().Trim();
                    string nomRef = dt.Rows[0]["nom_ref"].ToString().Trim();
                    Tx_NomRef.Text = nomRef;
                    llenarCampos(Ref);
                }
                else
                {
                    MessageBox.Show("La referencia no existe");
                    limpiarCampos();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al encotrar la referencia:" + w);
            }

        }

        public void limpiarCampos()
        {
            Tx_CodRef.Text = "";
            Tx_NomRef.Text = "";
            Tx_ValUni.Text = "";
            Tx_valIva.Text = "";
            Tx_PorDesc.Text = "";
            Tx_SubTot.Text = "";
            Tx_CodRef.Focus();
        }

        private void Tx_CodRef_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Tx_CodRef.Text != "")
            {
                BuscarRerf(Tx_CodRef.Text);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    dynamic ww = SiaWin.WindowExt(9326, "InBuscarReferencia");
                    ww.Conexion = SiaWin.Func.DatosEmp(idemp);
                    ww.idEmp = idemp;
                    ww.idBod = bodega;
                    //ww.idBod = idBod;
                    ww.ShowInTaskbar = false;
                    ww.Owner = Application.Current.MainWindow;
                    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    ww.ShowDialog();

                    if (!string.IsNullOrEmpty(ww.Codigo))
                    {
                        Tx_CodRef.Text = ww.Codigo;
                        Tx_NomRef.Text = ww.Nombre;
                        llenarCampos(ww.Codigo);
                    }
                    ww = null;
                    e.Handled = true;

                    if (e.Key == Key.Enter & !string.IsNullOrEmpty(((TextBox)sender).Text.Trim()))
                    {
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }        

        private void TX_cantidad_ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (ValueChange == true)
            {
                if (Tx_CodRef.Text == "" || string.IsNullOrEmpty(Tx_CodRef.Text))
                {
                    MessageBox.Show("ingrese una referencia");
                    return;
                }
                llenarCampos(Tx_CodRef.Text);
            }
            ValueChange = true;
        }
               
        public void llenarCampos(string referencia)
        {
            try
            {
                if (string.IsNullOrEmpty(referencia))
                {
                    MessageBox.Show("Vacio");
                    return;
                }

                string cmpval_uni = "inmae_ref.val_ref as val_ref";
                //if (ConfigCSource.IsBusinessGroup == true) cmpval_uni = "inmae_ref.vrunc as val_ref";

                string query = "select inmae_ref.idrow,inmae_ref.cod_ref,inmae_ref.cod_ant,rtrim(nom_ref) as nom_ref,inmae_ref.cod_tip,inmae_ref.cod_tiva, ";
                query = query + "inmae_tiva.por_iva,inmae_ref.val_ref as precioLista," + cmpval_uni + ",isnull(InList_cli.Val_uni,0) as val_refList, ";
                query = query + "nom_tip,nom_prv,inmae_tip."+campoDeslinea+ " as '"+campoDeslinea+"', ";                                
                query = query + "isnull(inter_tip."+campoDesInTer_tip+",0) as '"+campoDesInTer_tip+"', ";
                query = query + "isnull(InList_cli.Por_des,0) as decuentoLista ";
                query = query + "FROM inmae_ref ";
                query = query + "inner join inmae_tiva on inmae_tiva.cod_tiva=inmae_ref.cod_tiva  ";
                query = query + "inner join inmae_tip on inmae_tip.cod_tip=inmae_ref.cod_tip  ";
                query = query + "left join inmae_prv on inmae_prv.cod_prv=inmae_ref.cod_prv  ";
                query = query + "left join inter_tip on inter_tip.Cod_ter='"+codigo_ter.Trim()+"' and inter_tip.cod_tip=inmae_Ref.cod_tip  ";
                query = query + "left join InList_cli on InList_cli.Cod_ter='"+codigo_ter+"' and InList_cli.Cod_ref='"+referencia.Trim()+"'  ";
                query = query + "where  inmae_ref.cod_ref='" + referencia.Trim() + "' ";

                

                SqlDataReader dr = SiaWin.DB.SqlDR(query, idemp);

                while (dr.Read())
                {
                    decimal DecLista = Convert.ToDecimal(dr["val_refList"]);

                    por_iva = Convert.ToDouble(dr["por_iva"]);
                    cod_tiva = dr["cod_tiva"].ToString();


                    if (Convert.ToDouble(dr["decuentoLista"]) > 0)
                    {
                        procentaje_desc = Convert.ToDouble(dr["decuentoLista"]);
                    }
                    else if (Convert.ToDouble(dr[campoDesInTer_tip]) > 0)
                    {
                        procentaje_desc = Convert.ToDouble(dr[campoDesInTer_tip]);
                    }
                    else if (Convert.ToDouble(dr[campoDeslinea]) > 0)
                    {
                        procentaje_desc = Convert.ToDouble(dr[campoDeslinea]);
                    }

                    string valorRef = DecLista > 0 ? "val_refList" : "val_ref";
                    //_desc = 1 - Convert.ToDecimal(procentaje_desc) / 100;
                    val_ref = Convert.ToDouble(dr[valorRef]);

                    if (valorRef == "val_refList")
                    {
                        if (por_iva > 0)
                        {
                            double _valref = Convert.ToDouble(dr[valorRef]) / (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                            val_uni  = Math.Round(_valref, 0);
                            //valIva = _valref * (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                        }
                        if (por_iva == 0)
                        {
                            double _valref = Convert.ToDouble(dr[valorRef]);
                            val_uni = Math.Round(_valref, 0);
                            //valIva = _valref * (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                        }
                        
                    }
                    else
                    {
                        if (por_iva > 0)
                        {
                            double _desc = 1 - (Convert.ToDouble(procentaje_desc)) / 100;
                            double _valref = Convert.ToDouble(dr["val_ref"]) * _desc / (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                            val_uni = Math.Round(_valref, 0);
                            //valIva = _valref * (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                        }
                        if (por_iva == 0)
                        {
                            double _valref = Convert.ToDouble(dr["val_ref"]);
                            val_uni = Math.Round(_valref, 0);
                            //valIva = _valref * (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                        }
                        //ConfigCSource.ValUnitMasIva = _valref * (1 + (Convert.ToDouble(dr["por_iva"]) / 100));
                    }

                    cantidad = Convert.ToDouble(TX_cantidad.Value);                                
                    Tx_PorDesc.Text = procentaje_desc.ToString();
                    por_dec = procentaje_desc;

                    Tx_ValUni.Text = (string.Format(("{0:C}"), val_uni));

                    //Tx_valIva.Text = (string.Format(("{0:C}"), valIva))
                    subtotal = val_uni * cantidad;
                    Tx_SubTot.Text = (string.Format(("{0:C}"), subtotal));

                    valIva = (subtotal * por_iva) / 100;
                    Tx_valIva.Text = (string.Format(("{0:C}"), valIva));

                    //val_iva = Convert.ToDouble(dr[valIva]);

                    total = Math.Round(subtotal + valIva);
                    Tx_TotTal.Text = (string.Format(("{0:C}"), total));

                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        


        private void AddRef_Click(object sender, RoutedEventArgs e)
        {

            if (Tx_CodRef.Text == "" || string.IsNullOrEmpty(Tx_CodRef.Text))
            {
                MessageBox.Show("ingrese una referencia");
                return;
            }

            if (validarExistenciaRefDoc(Tx_CodRef.Text) == true)
            {
                MessageBox.Show("La referencia ingresada ya se encuantra en el documento ");
                return;
            }

            string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


            //MessageBox.Show("idregcab:"+ idregcab);
            //MessageBox.Show("Tx_CodRef.Text:" + Tx_CodRef.Text);
            //MessageBox.Show("bodega:" + bodega);
            //MessageBox.Show("TX_cantidad.Text:" + TX_cantidad.Text);
            //MessageBox.Show("val_uni:" + val_uni);            
            //MessageBox.Show("valorIva:" + valIva);
            //MessageBox.Show("procentaje_desc:" + procentaje_desc);
            //MessageBox.Show("iva:" + iva);
            //MessageBox.Show("subtotal:" + subtotal);
            //MessageBox.Show("cod_tiva:" + cod_tiva);
            //MessageBox.Show("trn:" + trn);            
            //MessageBox.Show("numeroDoc:" + numeroDoc);
            //MessageBox.Show("fecha:" + fecha);
            //MessageBox.Show("total:" + total);

            insertRef(
            idregcab, Tx_CodRef.Text, bodega, 
            Convert.ToDecimal(TX_cantidad.Text), Convert.ToInt32(val_uni), Convert.ToDecimal(valIva),
            Convert.ToDecimal(procentaje_desc), Convert.ToDecimal(por_iva), Convert.ToInt32(subtotal),
            "001", cod_tiva, trn, numeroDoc, fecha, Convert.ToDecimal(total)
            );

        }

        public void insertRef(int idregcab, string cod_ref, string cod_bod, decimal cantidad, int val_uni, decimal val_iva, decimal por_des, decimal por_iva, int subtotal, string cod_sub, string cod_tiva, string cod_trn, string num_trn, string fecha_aded, decimal tot_tot)
        {
            //MessageBox.Show("cantidad:"+cantidad);
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    try
                    {
                        cmd.CommandText = "INSERT INTO InCue_doc (idregcab,cod_ref,cod_bod,cantidad,val_uni,val_iva,por_des,por_iva,por_ica,val_ica,por_ret,val_ret,por_riva,val_riva,subtotal,cod_sub,cod_tiva,cod_trn,num_trn,fecha_aded,tot_tot) values (@idregcab,@cod_ref,@cod_bod,@cantidad,@val_uni,@val_iva,@por_des,@por_iva,@por_ica,@val_ica,@por_ret,@val_ret,@por_riva,@val_riva,@subtotal,@cod_sub,@cod_tiva,@cod_trn,@num_trn,@fecha_aded,@tot_tot)";
                        cmd.Parameters.AddWithValue("@idregcab", idregcab);
                        cmd.Parameters.AddWithValue("@cod_ref", cod_ref);
                        cmd.Parameters.AddWithValue("@cod_bod", cod_bod);
                        cmd.Parameters.AddWithValue("@cantidad", cantidad);
                        cmd.Parameters.AddWithValue("@val_uni", val_uni);
                        cmd.Parameters.AddWithValue("@val_iva", val_iva);
                        cmd.Parameters.AddWithValue("@por_des", por_des);
                        cmd.Parameters.AddWithValue("@por_iva", por_iva);

                        cmd.Parameters.AddWithValue("@por_ica", 0);
                        cmd.Parameters.AddWithValue("@val_ica", 0);
                        cmd.Parameters.AddWithValue("@por_ret", 0);
                        cmd.Parameters.AddWithValue("@val_ret", 0);
                        cmd.Parameters.AddWithValue("@por_riva", 0);
                        cmd.Parameters.AddWithValue("@val_riva", 0);

                        cmd.Parameters.AddWithValue("@subtotal", subtotal);
                        cmd.Parameters.AddWithValue("@cod_sub", cod_sub);
                        cmd.Parameters.AddWithValue("@cod_tiva", cod_tiva);
                        cmd.Parameters.AddWithValue("@cod_trn", cod_trn);
                        cmd.Parameters.AddWithValue("@num_trn", num_trn);
                        cmd.Parameters.AddWithValue("@fecha_aded", fecha_aded);
                        cmd.Parameters.AddWithValue("@tot_tot", tot_tot);
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Registro de referencia al documento: " + num_trn + " exitoso");
                        bandera = true;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error Interno Sia", MessageBoxButton.OK, MessageBoxImage.Stop);

                    }

                }
            }
        }

        public Boolean validarExistenciaRefDoc(string referencia)
        {
            Boolean bandera = false;

            string cadena = "select * from InCue_doc where idregcab='" + idregcab + "' and cod_ref='" + referencia + "' ";
            DataTable tabla = SiaWin.Func.SqlDT(cadena, "Validar", idemp);
            if (tabla.Rows.Count > 0)
            {
                bandera = true;
            }

            return bandera;
        }


    }
}

