using Fletes;
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
    //    Sia.PublicarPnt(9556,"Fletes");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9556,"Fletes");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class Fletes : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        DataTable dt_doc = new DataTable();
        public Fletes()
        {
            InitializeComponent();

            SiaWin = Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId;            
            loadColumns();
            BtnSave.Focus();

            //if (SiaWin._UserId != 21)
            //{
            //    MessageBox.Show("esta pantalla esta en mantenimineto por favor espere");
            //    this.IsEnabled = false;
            //}            
        }

        public void loadColumns()
        {
            dt_doc.Columns.Add("cod_trn");
            dt_doc.Columns.Add("num_trn");
            dt_doc.Columns.Add("cod_cli");
            dt_doc.Columns.Add("nom_ter");
            dt_doc.Columns.Add("cod_ven");
            dt_doc.Columns.Add("fec_trn");
            dt_doc.Columns.Add("cantidad");
            dt_doc.Columns.Add("subtotal", typeof(decimal));
            dt_doc.Columns.Add("val_iva", typeof(decimal));
            dt_doc.Columns.Add("tot_tot", typeof(decimal));
            dataGridRefe.ItemsSource = dt_doc;
        }

        private void LoadConfig()
        {
            try
            {
                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Fletes " + cod_empresa + "-" + nomempresa;
                TX_fecha.Text = DateTime.Now.ToString();

                string llave = idemp.ToString() + "-" + 5;

                BtnConsulta.IsEnabled = SiaWin.Acc.ContainsKey(llave + "-204") == true ? true : false;
                BtnSave.IsEnabled = SiaWin.Acc.ContainsKey(llave + "-205") == true ? true : false;
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
            }
            catch (Exception w)
            {
                MessageBox.Show("error:" + w);
            }


        }

        public void clean()
        {
            TX_fecha.Text = DateTime.Now.ToString();
            TX_empresa.Text = "";
            TX_name_empresa.Text = "";
            Tx_guia.Text = "";
            TextFlete.Text = "0";
            TextSeguro.Text = "0";
            TextPeso.Text = "0";
            TX_cliente.Text = "";
            TX_name_cliente.Text = "";
            Tx_vendedor.Text = "";
            Total.Text = "0";
            Cbx_Fpag.SelectedIndex = -1;
            Cbx_envio.SelectedIndex = -1;
            if (dt_doc.Rows.Count > 0)
            {
                dt_doc.Clear();
            }
            GridDocument.Visibility = Visibility.Hidden;
            GridDocument.IsEnabled = true;
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {



                if (BtnSave.Content.ToString() == "Nuevo")
                {
                    clean();
                    BtnSave.Content = "Guardar";
                    GridTxt.IsEnabled = true;
                    BtnCancel.Content = "Cancelar";
                    TX_empresa.Focus();
                }
                else
                {

                    var tag = ((ComboBoxItem)Cbx_envio.SelectedItem).Tag.ToString();
                    #region validaciones                

                    if (string.IsNullOrWhiteSpace(TX_empresa.Text))
                    {
                        MessageBox.Show("llene el campo de empresa de envio");
                        return;
                    }
                    if (string.IsNullOrWhiteSpace(Tx_guia.Text))
                    {
                        MessageBox.Show("llene el campo de guia");
                        return;
                    }

                    if (Cbx_Fpag.SelectedIndex == -1)
                    {
                        MessageBox.Show("seleccione una forma de pago");
                        return;
                    }
                    if (Cbx_envio.SelectedIndex == -1)
                    {
                        MessageBox.Show("seleccione una clase de envio");
                        return;
                    }




                    if (tag == "M" || tag == "E")
                    {
                        if (dt_doc.Rows.Count == 0 || dt_doc == null)
                        {
                            MessageBox.Show("agregue por los menos un documento");
                            return;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(TX_cliente.Text))
                        {
                            MessageBox.Show("agregue el campo nit");
                            return;
                        }
                    }

                    bool flag = validarGuia();
                    if (flag)
                    {
                        MessageBox.Show("la guia ingresada:" + Tx_guia.Text.Trim() + " ya esta registrada");
                        return;
                    }

                    #endregion

                    bool merca = tag == "M" || tag == "E" ? true : false;
                    bool tbl_fle = tableFletes(merca);

                    var tag_fpag = ((ComboBoxItem)Cbx_Fpag.SelectedItem).Tag.ToString();
                    bool tbl_contable = false;
                    if (tag_fpag == "con" || tag_fpag == "fac")
                    {
                        tbl_contable = DocuContable();
                    }

                    if (tbl_fle == true)
                    {
                        if (tag_fpag == "con" || tag_fpag == "fac")
                        {
                            if (tbl_contable)
                            {
                                MessageBox.Show("flete registrado exitosamente");
                            }
                        }
                        else MessageBox.Show("flete registrado exitosamente");

                        Clear();
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar" + w);
            }
        }


        public bool tableFletes(bool isMerca)
        {
            bool flag = false;
            try
            {
                string query = "";

                if (isMerca)
                {
                    // SiaWinSiaWin.Browse(dt_doc);                    
                    decimal total_factura = Convert.ToDecimal(dt_doc.Compute("Sum(subtotal)", ""));

                    int numer_docs = dt_doc.Rows.Count;
                    foreach (DataRow item in dt_doc.Rows)
                    {
                        decimal sub_factura = Convert.ToDecimal(item["subtotal"]);
                        decimal valor_flete = 0;
                        decimal valor_seguro = 0;
                        decimal flete = Convert.ToDecimal(TextFlete.Value);
                        decimal seguro = Convert.ToDecimal(TextSeguro.Value);


                        //mari
                        if (sub_factura == 0)
                        {
                            valor_flete = flete / numer_docs;
                            valor_seguro = seguro / numer_docs; 
                        }
                        else
                        {
                            valor_flete = (sub_factura / total_factura) * flete;
                            valor_seguro = (sub_factura / total_factura) * seguro;
                        }



                        //det_fle
                        int tipo = Cbx_envio.SelectedIndex + 1;
                        query += "insert into indet_fle (cod_prv, n_guia, n_fra, vr_flete, vr_seguro,fecha,tipo) values('" + TX_empresa.Text.Trim() + "','" + Tx_guia.Text.Trim() + "','" + item["num_trn"].ToString().Trim() + "'," + Math.Round(valor_flete) + "," + Math.Round(valor_seguro) + ",GETDATE(),'" + tipo + "');";

                        //actualizacion de fechas de entrega
                        query += "update incab_doc set fec_envi='" + TX_fecha.Text + "' where cod_trn='" + item["cod_trn"].ToString().Trim() + "' and num_trn='" + item["num_trn"].ToString().Trim() + "'; ";
                    }

                    //flete             
                    int ind_cau = Cbx_Fpag.SelectedIndex == 0 ? 0 : 1;
                    int fpag = Cbx_Fpag.SelectedIndex + 1;
                    string n_cau = ind_cau == 1 ? Tx_guia.Text : "";

                    query += "insert into infletes (n_guia,cod_prv,vr_flete,vr_seguro,cod_ven,cod_cli,fec_env,ind_acu,n_cau,f_pag,peso) values ('" + Tx_guia.Text.Trim() + "','" + TX_empresa.Text.Trim() + "'," + TextFlete.Value + "," + TextSeguro.Value + ",'" + dt_doc.Rows[0]["cod_ven"].ToString().Trim() + "','" + dt_doc.Rows[0]["cod_cli"].ToString().Trim() + "','" + TX_fecha.Text + "'," + ind_cau + ",'" + n_cau + "','" + fpag + "'," + TextPeso.Value + ");";

                }
                else
                {

                    #region consecutivo

                    string fecha = DateTime.Now.ToString("dd/MM/yyyy");
                    string select = "select n_fra from indet_fle where fecha between '" + fecha + "' and '" + fecha + " 23:59:59' and tipo='2' order by n_fra desc;";
                    DataTable dt = SiaWin.Func.SqlDT(select, "tabla", idemp);
                    string consecutivo = "";
                    if (dt.Rows.Count > 0)
                    {

                        string nfra = dt.Rows[0]["n_fra"].ToString().Trim();
                        string con = nfra.Substring(nfra.Length - 1, 1);
                        byte[] bytes = Encoding.ASCII.GetBytes(con);
                        int ascii = 0;
                        foreach (byte b in bytes) ascii = b;
                        string str = char.ConvertFromUtf32(ascii + 1);
                        consecutivo = "D" + DateTime.Now.ToString("ddMMyyy") + str;
                    }
                    else
                    {
                        consecutivo = "D" + DateTime.Now.ToString("ddMMyyy") + "a";
                    }
                    #endregion

                    int tipo = Cbx_envio.SelectedIndex + 1;
                    query += "insert into indet_fle (cod_prv, n_guia, n_fra, vr_flete, vr_seguro,fecha,tipo) values('" + TX_empresa.Text.Trim() + "','" + Tx_guia.Text + "','" + consecutivo.Trim() + "'," + TextFlete.Value + "," + TextSeguro.Value + ",GETDATE(),'" + tipo + "');";

                    int ind_cau = Cbx_Fpag.SelectedIndex == 0 ? 0 : 1;
                    int fpag = Cbx_Fpag.SelectedIndex + 1;

                    query += "insert into infletes (cod_prv,vr_flete,vr_seguro,cod_ven,cod_cli,fec_env,ind_acu,n_cau,f_pag,peso) values ('" + TX_empresa.Text.Trim() + "'," + TextFlete.Value + "," + TextSeguro.Value + ",'" + Tx_vendedor.Text + "','" + TX_cliente.Text + "','" + TX_fecha.Text + "'," + ind_cau + ",'" + Tx_guia.Text + "','" + fpag + "'," + TextPeso.Value + ");";
                }

                //if (SiaWin._UserId == 21)
                //{
                //    MessageBox.Show("mensaje de prueba:" + query);
                //}

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    flag = true;
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el flete:" + w);
            }
            return flag;
        }


        public string GetIndCons()
        {
            System.Data.DataTable dt = SiaWin.Func.SqlDT("select ind_con from Comae_trn where cod_trn='05A'", "tabla", idemp);
            return dt.Rows.Count > 0 ? dt.Rows[0]["ind_con"].ToString().Trim() : "";
        }

        public bool DocuContable()
        {
            bool flag = false;
            using (SqlConnection connection = new SqlConnection(cnEmp))
            {
                connection.Open();
                StringBuilder errorMessages = new StringBuilder();
                SqlCommand command = connection.CreateCommand();
                SqlTransaction transaction;
                transaction = connection.BeginTransaction("Transaction");
                command.Connection = connection;
                command.Transaction = transaction;

                try
                {
                    string ind_con = GetIndCons().Trim();
                    string sqlConsecutivo = " ";

                    if (ind_con == "3")
                    {
                        DateTime fecha = Convert.ToDateTime(TX_fecha.Text);

                        sqlConsecutivo += "declare @ini  char(8);declare @lonnum as int = 12;declare @iFolioHost int = 0;declare @iConsecutivo char(12) = '';";
                        sqlConsecutivo += "SELECT @iFolioHost = num_" + fecha.ToString("MM") + "+1,@lonnum=lon_num, ";
                        sqlConsecutivo += "@ini=rtrim(inicial)+'" + fecha.ToString("MM") + fecha.ToString("yyyy").Substring(2, 2) + "' FROM comae_trn  WHERE cod_trn = '05A'; ";
                        sqlConsecutivo += "UPDATE Comae_trn SET num_" + fecha.ToString("MM") + "= ISNULL(num_" + fecha.ToString("MM") + ", 0) + 1  WHERE cod_trn='05A';";
                        sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+REPLICATE('0', iif(@lonnum<3,12,@lonnum) - len(rtrim(@ini)) - len(rtrim(convert(varchar, @iFolioHost)))) + rtrim(convert(varchar, @iFolioHost)); ";
                    }
                    else
                    {
                        sqlConsecutivo += "declare @ini as char(4);declare @num as varchar(12);declare @iConsecutivo char(12) = '' ;";
                        sqlConsecutivo += "declare @iFolioHost int = 0;";
                        sqlConsecutivo += "UPDATE Comae_trn SET num_act= ISNULL(num_act, 0) + 1  WHERE cod_trn='05A';";
                        sqlConsecutivo += "SELECT @iFolioHost = num_act,@ini=rtrim(inicial) FROM Comae_trn  WHERE cod_trn='05A';";
                        sqlConsecutivo += "set @num=@iFolioHost;";
                        sqlConsecutivo += "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));";
                    }

                    string sqlcab = sqlConsecutivo + @"INSERT INTO cocab_doc (cod_trn,fec_trn,num_trn,detalle,factura,fec_ven) values ('05A','" + TX_fecha.Text + "',@iConsecutivo,'Flete','" + Tx_guia.Text + "','" + TX_fecha.Text + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                    string sql = "";
                    var tag = ((ComboBoxItem)Cbx_envio.SelectedItem).Tag.ToString();
                    string cuenta_A = tag == "M" ? "523550" : "523540";
                    string cuenta_B = "233545";
                    string tercero = TX_empresa.Text;
                    //string tercero_contra = tag == "M" || tag == "E" ? dt_doc.Rows[0]["cod_cli"].ToString().Trim() : TX_empresa.Text;

                    decimal val1 = Convert.ToDecimal(TextFlete.Value);
                    decimal val2 = Convert.ToDecimal(TextSeguro.Value);
                    decimal total = val1 + val2;
                    decimal valor_total = total;

                    sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,doc_ref,doc_cruc,bas_mov,deb_mov,cre_mov) values (@NewID,'05A',@iConsecutivo,'" + cuenta_A + "','" + tercero + "','" + getCco(tercero).Trim() + "','Causacion remesas Fra Q1','','',0," + valor_total.ToString("F", CultureInfo.InvariantCulture) + ",0);";
                    //contra
                    sql = sql + @"INSERT INTO cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,des_mov,doc_ref,doc_cruc,bas_mov,deb_mov,cre_mov) values (@NewID,'05A',@iConsecutivo,'" + cuenta_B + "','" + tercero + "','Causacion remesas Fra Q1',@iConsecutivo,'',0,0," + valor_total.ToString("F", CultureInfo.InvariantCulture) + ");";

                    command.CommandText = sqlcab + sql + @"select CAST(@NewId AS int);";
                    //if (SiaWin._UserId == 21) MessageBox.Show(command.CommandText);
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    flag = true;
                    //return Convert.ToInt32(r.ToString());
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append(" SQL-Index #" + i + "\n" + "Message: " + ex.Errors[i].Message + "\n" + "LineNumber: " + ex.Errors[i].LineNumber + "\n" + "Source: " + ex.Errors[i].Source + "\n" + "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                    //return -1;
                }
                catch (Exception ex)
                {
                    errorMessages.Append("Error:" + ex.StackTrace + "-" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                    //return -1;
                }
            }

            return flag;
        }

        public string getCco(string cod_ter)
        {
            string cco = "";
            string query = "select cod_ter,cod_ven,InMae_mer.cod_cco ";
            query += "from Comae_ter ";
            query += "left join InMae_mer on Comae_ter.cod_ven = InMae_mer.cod_mer ";
            query += "where cod_ter='" + cod_ter + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "bod", idemp);
            if (dt.Rows.Count > 0) cco = dt.Rows[0]["cod_cco"].ToString();
            return cco;
        }



        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {

            if (BtnCancel.Content.ToString() == "Salir")
            {
                this.Close();
            }
            else
            {
                if (MessageBox.Show("Usted desea cancelar el proceso..?", "alerta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Clear();
                    BtnSave.Content = "Nuevo";
                    BtnCancel.Content = "Salir";
                    GridTxt.IsEnabled = false;
                }
            }

        }

        public void Clear()
        {
            try
            {
                TX_empresa.Text = "";
                TX_name_empresa.Text = "";
                Tx_guia.Text = "";
                TextFlete.Value = 0;
                TextSeguro.Value = 0;
                TextPeso.Value = 0;
                Cbx_Fpag.SelectedIndex = -1;
                Cbx_envio.SelectedIndex = -1;
                dt_doc.Clear();
                TX_cliente.Text = "";
                TX_name_cliente.Text = "";
                Tx_documentos.Text = "";
                Tx_vendedor.Text = "";
            }
            catch (Exception w)
            {
                MessageBox.Show("error al limpiar:" + w);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Tx_documentos.Text))
                {
                    MessageBox.Show("el campo de documento esta vacio");
                    return;
                }

                string select = "select cab.cod_trn,cab.num_trn,cab.cod_cli,ter.nom_ter,cab.cod_ven,cab.fec_trn,sum(cue.cantidad) as cantidad,sum(cue.subtotal) as subtotal,sum(cue.val_iva) as val_iva,sum(cue.tot_tot) as tot_tot From incab_doc as cab ";
                select += "inner join incue_doc as cue on cab.idreg = cue.idregcab ";
                select += "left join comae_ter as ter on ter.cod_ter = cab.cod_cli ";
                select += "where cab.num_trn='" + Tx_documentos.Text + "' and cab.cod_trn in('004','005','145','141') ";
                select += "group by  cab.cod_trn,cab.num_trn,cab.cod_cli,ter.nom_ter,cab.cod_ven,cab.fec_trn; ";
                //mierda
                //MessageBox.Show(select);

                System.Data.DataTable dt = SiaWin.Func.SqlDT(select, "tabla", idemp);

                decimal n;
                if (dt.Rows.Count > 0)
                {
                    string cod_cli = dt.Rows[0]["cod_cli"].ToString().Trim();
                    string nom_ter = dt.Rows[0]["nom_ter"].ToString().Trim();
                    string cod_ven = dt.Rows[0]["cod_ven"].ToString().Trim();
                    string cod_trn = dt.Rows[0]["cod_trn"].ToString().Trim();


                    //si son documentos 145 o 141 no tienen cod_cli por ende nos vamos al tercero que tiene la bodega 
                    if (string.IsNullOrEmpty(cod_cli))
                    {
                        if (cod_trn == "141" || cod_trn == "145")
                        {
                            string getcli = "select InCab_doc.bod_tra,InMae_bod.cod_ter,comae_ter.nom_ter,comae_ter.cod_ven ";
                            getcli += "From InCab_doc ";
                            getcli += "inner join InMae_bod on InMae_bod.cod_bod = InCab_doc.bod_tra ";
                            getcli += "inner join comae_ter on comae_ter.cod_ter = inmae_bod.cod_ter ";
                            getcli += "where num_trn='" + Tx_documentos.Text + "' and (cod_trn='145' or cod_trn='141') ";
                            System.Data.DataTable dtcli = SiaWin.Func.SqlDT(getcli, "tabla", idemp);
                            if (dtcli.Rows.Count > 0)
                            {
                                cod_cli = dtcli.Rows[0]["cod_ter"].ToString().Trim();
                                nom_ter = dtcli.Rows[0]["nom_ter"].ToString().Trim();
                                cod_ven = dtcli.Rows[0]["cod_ven"].ToString().Trim();
                            }
                        }
                    }


                    dt_doc.Rows.Add
                        (
                            dt.Rows[0]["cod_trn"].ToString(),
                            dt.Rows[0]["num_trn"].ToString(),
                            cod_cli,
                            nom_ter,
                            cod_ven,
                            dt.Rows[0]["fec_trn"].ToString(),
                            Convert.ToDecimal(dt.Rows[0]["cantidad"] == DBNull.Value || decimal.TryParse(dt.Rows[0]["cantidad"].ToString(), out n) == false ? 0 : dt.Rows[0]["cantidad"]),
                            Convert.ToDecimal(dt.Rows[0]["subtotal"] == DBNull.Value || decimal.TryParse(dt.Rows[0]["subtotal"].ToString(), out n) == false ? 0 : dt.Rows[0]["subtotal"]),
                            Convert.ToDecimal(dt.Rows[0]["val_iva"] == DBNull.Value || decimal.TryParse(dt.Rows[0]["val_iva"].ToString(), out n) == false ? 0 : dt.Rows[0]["val_iva"]),
                            Convert.ToDecimal(dt.Rows[0]["tot_tot"] == DBNull.Value || decimal.TryParse(dt.Rows[0]["tot_tot"].ToString(), out n) == false ? 0 : dt.Rows[0]["tot_tot"])
                        );

                    Tx_documentos.Focus();
                }
                else
                {
                    MessageBox.Show("el documento: " + Tx_documentos.Text.Trim() + " no existe o no es una factura");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al adicionar:" + w);
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TX_empresa.Text) && (e.Key == Key.F8 || e.Key == Key.Enter))
            {
                try
                {
                    int idr = 0; string code = ""; string nombre = "";
                    dynamic xx = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "nom_ter", "idrow", "Maestra de clientes", cnEmp, false, "", idEmp: idemp);
                    xx.ShowInTaskbar = false;
                    xx.Owner = Application.Current.MainWindow;
                    xx.Height = 400;
                    xx.ShowDialog();
                    idr = xx.IdRowReturn;
                    code = xx.Codigo;
                    nombre = xx.Nombre;
                    xx = null;
                    if (idr > 0)
                    {
                        if ((sender as TextBox).Name == "TX_empresa")
                        {
                            TX_empresa.Text = code;
                            TX_name_empresa.Text = nombre;
                        }

                        if ((sender as TextBox).Name == "TX_cliente")
                        {
                            TX_cliente.Text = code;
                            TX_name_cliente.Text = nombre;
                            string query = "select * from comae_ter where cod_ter='" + code + "';";
                            System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                            if (dt.Rows.Count > 0) Tx_vendedor.Text = dt.Rows[0]["cod_ven"].ToString().Trim();
                        }

                    }
                }
                catch (Exception w)
                {
                    MessageBox.Show("error al buscar:" + w);
                }
            }
        }

        private void TX_tercero_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty((sender as TextBox).Text)) return;


            string query = "select * from comae_ter where cod_ter='" + (sender as TextBox).Text.Trim() + "'; ";

            System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);

            if (dt.Rows.Count > 0)
            {
                (sender as TextBox).Text = dt.Rows[0]["cod_ter"].ToString();

                if ((sender as TextBox).Name == "TX_empresa")
                    TX_name_empresa.Text = dt.Rows[0]["nom_ter"].ToString().Trim();

                if ((sender as TextBox).Name == "TX_cliente")
                {
                    TX_name_cliente.Text = dt.Rows[0]["nom_ter"].ToString().Trim();
                    Tx_vendedor.Text = dt.Rows[0]["cod_ven"].ToString().Trim();
                }

            }
            else
            {
                MessageBox.Show("el tercero ingresado no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                (sender as TextBox).Text = "";
                if ((sender as TextBox).Name == "TX_empresa") TX_name_empresa.Text = "";
                if ((sender as TextBox).Name == "TX_cliente") TX_name_cliente.Text = "";
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Cbx_envio.SelectedIndex == -1) return;

            var tag = ((ComboBoxItem)(sender as ComboBox).SelectedItem).Tag.ToString();
            GridEnvio.Visibility = tag == "M" || tag == "E" ? Visibility.Hidden : Visibility.Visible;

            if (tag == "M")
            {
                GridDocumentBloq.Visibility = Visibility.Hidden;
                GridDocument.Visibility = Visibility.Visible;
            }
            else
            {
                GridDocumentBloq.Visibility = Visibility.Visible;
                GridDocument.Visibility = Visibility.Hidden;
            }
        }

        private void BtnDelDoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Usted desea elimainar el documento ingresado..?", "alerta", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DataRowView dr = (DataRowView)dataGridRefe.SelectedItems[0];
                    dr.Delete();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar");
            }
        }


        private void TextUpdateTotal_LostFocus(object sender, RoutedEventArgs e)
        {
            decimal val1 = Convert.ToDecimal(TextFlete.Value);
            decimal val2 = Convert.ToDecimal(TextSeguro.Value);
            decimal total = val1 + val2;
            Total.Text = total.ToString("C");
        }




        private void Tx_guia_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Tx_guia.Text))
                {
                    MessageBox.Show("la guia no tiene que esta vacia");
                }
                else
                {
                    bool flag = validarGuia();
                    if (flag == true)
                    {
                        MessageBox.Show("la guia ingresada " + Tx_guia.Text.Trim() + " ya existe debe de registrar otra guia");
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al validar:" + w);
            }
        }

        public bool validarGuia()
        {
            bool flag = false;
            if (string.IsNullOrWhiteSpace(Tx_guia.Text.Trim())) flag = true;
            if (flag == false)
            {
                string query = "select * from infletes where n_guia='" + Tx_guia.Text.Trim() + "' ";
                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                if (dt.Rows.Count > 0) flag = true;
            }
            return flag;
        }

        private void BtnConsulta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnSave.Content = "Nuevo";
                GridTxt.IsEnabled = false;
                Consultar ventana = new Consultar();
                ventana.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                ventana.ShowInTaskbar = false;
                ventana.Owner = Application.Current.MainWindow;
                ventana.ShowDialog();

                if (ventana.flag)
                {
                    string tipo = ventana.tipo;

                    if (tipo == "M")
                    {
                        //select* from indet_fle where n_guia = ''
                        string query = "select fle.cod_prv,ter.nom_ter,n_guia,n_fra,vr_flete,vr_seguro,fecha,tipo ";
                        query += "from indet_fle as fle ";
                        query += "inner join Comae_ter as ter on ter.cod_ter = fle.cod_prv ";
                        query += "where n_fra = '" + ventana.guia_doc + "' ";

                        //MessageBox.Show("query :"+ query);

                        DataTable tabla = SiaWin.Func.SqlDT(query, "Buscar", idemp);
                        if (tabla.Rows.Count > 0)
                        {
                            TX_empresa.Text = tabla.Rows[0]["cod_prv"].ToString();
                            TX_name_empresa.Text = tabla.Rows[0]["nom_ter"].ToString();
                            Tx_guia.Text = tabla.Rows[0]["n_guia"].ToString();

                            //TextFlete.Text = tabla.Rows[0]["vr_flete"].ToString();                            
                            //TextSeguro.Text = tabla.Rows[0]["vr_seguro"].ToString();



                            Cbx_envio.SelectedIndex = Convert.ToInt32(tabla.Rows[0]["tipo"]) - 1;



                            #region infletes

                            string select = "select fec_env,cod_cli,ind_acu,n_cau,f_pag,peso,vr_flete,vr_seguro from infletes where cod_prv='" + TX_empresa.Text + "' and n_guia='" + Tx_guia.Text + "' ";
                            DataTable fletes = SiaWin.Func.SqlDT(select, "Buscar", idemp);
                            if (fletes.Rows.Count > 0)
                            {
                                TextFlete.Value = Convert.ToDecimal(fletes.Rows[0]["vr_flete"]);
                                TextSeguro.Value = Convert.ToDecimal(fletes.Rows[0]["vr_seguro"]);

                                TX_fecha.Text = fletes.Rows[0]["fec_env"].ToString();
                                TextPeso.Text = fletes.Rows[0]["peso"].ToString();
                                Cbx_Fpag.SelectedIndex = Convert.ToInt32(fletes.Rows[0]["f_pag"]) - 1;
                                if (Cbx_envio.SelectedIndex == 1 || Cbx_envio.SelectedIndex == 2) TX_cliente.Text = fletes.Rows[0]["cod_cli"].ToString();
                            }
                            #endregion

                            #region documentos


                            string queryDoc = "select cab.cod_trn,cab.num_trn,cab.cod_cli,ter.nom_ter,cab.cod_ven,cab.fec_trn,sum(cue.cantidad) as cantidad, ";
                            queryDoc += "sum(cue.subtotal) as subtotal,sum(cue.val_iva) as val_iva,sum(cue.tot_tot) as tot_tot ";
                            queryDoc += "from indet_fle as fle ";
                            queryDoc += "inner join InCab_doc as cab on cab.num_trn = fle.n_fra ";
                            queryDoc += "inner join incue_doc as cue on cab.idreg = cue.idregcab ";
                            queryDoc += "inner join comae_ter as ter on ter.cod_ter = cab.cod_cli  ";
                            queryDoc += "where fle.cod_prv='" + TX_empresa.Text.Trim() + "' and n_guia='" + Tx_guia.Text + "' ";
                            queryDoc += "group by  cab.cod_trn,cab.num_trn,cab.cod_cli,ter.nom_ter,cab.cod_ven,cab.fec_trn; ";

                            System.Data.DataTable dt = SiaWin.Func.SqlDT(queryDoc, "tabla", idemp);
                            if (dt.Rows.Count > 0)
                            {
                                if (dt_doc.Rows.Count > 0) dt_doc.Clear();

                                foreach (DataRow item in dt.Rows)
                                {
                                    dt_doc.Rows.Add
                                    (
                                        item["cod_trn"].ToString(),
                                        item["num_trn"].ToString(),
                                        item["cod_cli"].ToString(),
                                        item["nom_ter"].ToString(),
                                        item["cod_ven"].ToString(),
                                        item["fec_trn"].ToString(),
                                        Convert.ToDecimal(item["cantidad"]),
                                        Convert.ToDecimal(item["subtotal"]),
                                        Convert.ToDecimal(item["val_iva"]),
                                        Convert.ToDecimal(item["tot_tot"])
                                    );
                                }
                            }

                            #endregion

                            decimal val1 = Convert.ToDecimal(TextFlete.Value);
                            decimal val2 = Convert.ToDecimal(TextSeguro.Value);
                            decimal total = val1 + val2;
                            Total.Text = total.ToString("C");
                        }
                        else
                        {
                            MessageBox.Show("no se encontro el documento: " + ventana.guia_doc);
                            Total.Text = "0";
                        }
                    }

                    if (tipo == "D")
                    {
                        string query = "select fle.cod_prv,ter.nom_ter,n_guia,n_fra,vr_flete,vr_seguro,fecha,tipo ";
                        query += "from indet_fle as fle ";
                        query += "inner join Comae_ter as ter on ter.cod_ter = fle.cod_prv ";
                        query += "where n_guia = '" + ventana.guia_doc + "' ";

                        DataTable tabla = SiaWin.Func.SqlDT(query, "Buscar", idemp);
                        if (tabla.Rows.Count > 0)
                        {
                            TX_empresa.Text = tabla.Rows[0]["cod_prv"].ToString();
                            TX_name_empresa.Text = tabla.Rows[0]["nom_ter"].ToString();
                            Tx_guia.Text = tabla.Rows[0]["n_guia"].ToString();                            

                            Cbx_envio.SelectedIndex = Convert.ToInt32(tabla.Rows[0]["tipo"]) - 1;

                            #region infletes

                            string select = "select fec_env,cod_cli,ind_acu,n_cau,f_pag,peso,vr_flete,vr_seguro from infletes where cod_prv='" + TX_empresa.Text + "' and n_guia='" + Tx_guia.Text + "' ";
                            DataTable fletes = SiaWin.Func.SqlDT(select, "Buscar", idemp);
                            if (fletes.Rows.Count > 0)
                            {
                                TX_fecha.Text = fletes.Rows[0]["fec_env"].ToString();
                                TextPeso.Text = fletes.Rows[0]["peso"].ToString();
                                Cbx_Fpag.SelectedIndex = Convert.ToInt32(fletes.Rows[0]["f_pag"]) - 1;
                                TextFlete.Value = Convert.ToDecimal(fletes.Rows[0]["vr_flete"]);
                                TextSeguro.Value = Convert.ToDecimal(fletes.Rows[0]["vr_seguro"]);

                                if (Cbx_envio.SelectedIndex == 1 || Cbx_envio.SelectedIndex == 2) TX_cliente.Text = fletes.Rows[0]["cod_cli"].ToString();
                            }
                            #endregion

                            if (Cbx_envio.SelectedIndex == 0)
                            {
                                #region documentos

                                string queryDoc = "select cab.cod_trn,cab.num_trn,cab.cod_cli,cab.cod_ven,cab.fec_trn,sum(cue.cantidad) as cantidad, ";
                                queryDoc += "sum(cue.subtotal) as subtotal,sum(cue.val_iva) as val_iva,sum(cue.tot_tot) as tot_tot ";
                                queryDoc += "from indet_fle as fle ";
                                queryDoc += "inner join InCab_doc as cab on cab.num_trn = fle.n_fra ";
                                queryDoc += "inner join incue_doc as cue on cab.idreg = cue.idregcab ";                                
                                queryDoc += "where fle.cod_prv='" + TX_empresa.Text.Trim() + "' and n_guia='" + Tx_guia.Text + "' and cab.cod_trn in ('141','145','004','005') ";
                                queryDoc += "group by  cab.cod_trn,cab.num_trn,cab.cod_cli,cab.cod_ven,cab.fec_trn; ";



                                System.Data.DataTable dt = SiaWin.Func.SqlDT(queryDoc, "tabla", idemp);
                                if (dt.Rows.Count > 0)
                                {
                                    if (dt_doc.Rows.Count > 0) dt_doc.Clear();
                                    decimal inp;

                                    foreach (DataRow item in dt.Rows)
                                    {
                                        if (item["cod_trn"].ToString().Trim() != "055")
                                        {
                                            dt_doc.Rows.Add
                                            (
                                                item["cod_trn"].ToString(),
                                                item["num_trn"].ToString(),
                                                item["cod_cli"].ToString(),
                                                "",//nom_ter
                                                item["cod_ven"].ToString(),
                                                item["fec_trn"].ToString(),
                                                Convert.ToDecimal(item["cantidad"]),
                                                Convert.ToDecimal(
                                                    item["subtotal"] == DBNull.Value || decimal.TryParse(item["subtotal"].ToString(), out inp) == false ?
                                                    0 : item["subtotal"]
                                                    ),
                                                Convert.ToDecimal(item["val_iva"]),
                                                Convert.ToDecimal(                                                                                                        
                                                    item["tot_tot"] == DBNull.Value || decimal.TryParse(item["tot_tot"].ToString(),out inp) == false ?
                                                    0: item["tot_tot"]
                                                    )
                                            );
                                        }
                                    }
                                }

                                #endregion
                            }

                            decimal val1 = Convert.ToDecimal(TextFlete.Value);
                            decimal val2 = Convert.ToDecimal(TextSeguro.Value);
                            decimal total = val1 + val2;
                            Total.Text = total.ToString("C");
                        }
                        else
                        {
                            MessageBox.Show("no se encontro la guia: " + ventana.guia_doc);
                            Total.Text = "0";
                        }

                    }

                    GridDocument.IsEnabled = false;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Tx_guiaDel.Text.Trim()) || string.IsNullOrEmpty(Tx_terDel.Text.Trim()))
                {
                    MessageBox.Show("debe de ingresar el numero de la guia junto con el nit de la empresa");
                    return;
                }

                string query = "select * from infletes  WHERE n_guia='" + Tx_guiaDel.Text.Trim() + "' and cod_prv='" + Tx_terDel.Text.Trim() + "';";
                //MessageBox.Show("A1:"+query);
                DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
                if (dt.Rows.Count > 0)
                {
                    int fpag = Convert.ToInt32(dt.Rows[0]["f_pag"]);
                    if (fpag == 2 || fpag == 3)
                    {
                        string forma = fpag == 2 ? "Contado" : "Factura Credito";
                        MessageBox.Show("la guia ingresada:" + Tx_guiaDel.Text + " tiene la forma de pago:" + forma + " por lo tanto tiene registros en contabilidad y no se puede borrar", "Alert", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return;
                    }
                    else
                    {
                        string delete = "delete infletes WHERE n_guia='" + Tx_guiaDel.Text.Trim() + "' and cod_prv='" + Tx_terDel.Text.Trim() + "';";
                        delete += "delete from indet_fle WHERE n_guia = '" + Tx_guiaDel.Text.Trim() + "' and cod_prv = '" + Tx_terDel.Text.Trim() + "';";

                        //MessageBox.Show("A2:" + delete);
                        if (SiaWin.Func.SqlCRUD(delete, idemp) == true)
                        {
                            MessageBox.Show("eliminacion exitosa de la guia:" + Tx_guiaDel.Text.ToString());
                            SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, 5, -1, -9, "Elimino la guia:" + Tx_guiaDel.Text.Trim(), "");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("no existe ningun registro con los campos ingresados");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al eliminar:" + w);
            }
        }

        private void Cbx_Fpag_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null)
                {
                    keyboardFocus.MoveFocus(tRequest);
                }

                e.Handled = true;
            }
        }



    }
}
