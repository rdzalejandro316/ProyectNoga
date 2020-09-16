using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
    //Sia.PublicarPnt(9702,"BodegaProductoNoConforme");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9702,"BodegaProductoNoConforme");
    //ww.punto_v = "010";
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //ww.ShowDialog();

    public partial class BodegaProductoNoConforme : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa;
        public string punto_v = "";
        public string name_pv = "";

        public BodegaProductoNoConforme()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                tx_Pv.Text = punto_v;
                tx_namepv.Text = name_pv;

                SiaWin = Application.Current.MainWindow;
                if (idemp <= 0) idemp = SiaWin._BusinessId;

                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "TRASLADO A BODEGA PRODUCTO NO CONFORME  - Empresa:" + cod_empresa + "-" + nomempresa;
                loadBod();

                Dp_Fecini.Text = DateTime.Now.AddMonths(-1).ToString();
                Dp_Fecfin.Text = DateTime.Now.ToString();

            }
            catch (Exception w)
            {
                MessageBox.Show("error en el load" + w);
            }
        }

        private void BtnGet_Click(object sender, RoutedEventArgs e)
        {
            CargarNotas();
        }

        public async void CargarNotas()
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                sfBusyIndicator.IsBusy = true;
                dataGridCabeza.ItemsSource = null;

                string pv = punto_v;
                string fec_ini = Dp_Fecini.Text;
                string fec_fin = Dp_Fecfin.Text;
                string emp = cod_empresa;


                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(pv, emp, fec_ini, fec_fin, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridCabeza.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    tx_reg.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                }
                else
                {
                    MessageBox.Show("No tienen devoluciones por producto no conforme", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    tx_reg.Text = "0";
                }

                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("error cal cargar notas;" + w);
            }
        }

        public void loadBod()
        {
            try
            {
                DataTable dt = SiaWin.Func.SqlDT("select cod_bod,rtrim(cod_bod)+'-'+rtrim(nom_bod) as nom_bod from InMae_bod where tipo_bod='6' and bod_nconforme ='" + punto_v + "' ", "bod", idemp);
                if (dt.Rows.Count > 0)
                {
                    comboBoxBodega.ItemsSource = dt.DefaultView;
                    comboBoxBodega.SelectedIndex = 0;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar bodegas:" + w);
            }
        }


        private DataSet LoadData(string pv, string emp, string fec_ini, string fec_fin, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpBodegaProductoNoConforme", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cod_pvt", pv);
                cmd.Parameters.AddWithValue("@fecini", fec_ini);
                cmd.Parameters.AddWithValue("@fecfin", fec_fin);
                cmd.Parameters.AddWithValue("@codemp", emp);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                if (row == null) return;
                int id = Convert.ToInt32(row["idreg"]);
                SiaWin.TabTrn(0, idemp, true, id, 2, WinModal: true);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir devolucion:" + w);
            }
        }





        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridCabeza.SelectedIndex >= 0)
                {
                    int id = ExecuteSqlTransaction();
                    if (id != -1)
                    {
                        dataGridCabeza.ItemsSource = null;
                        SiaWin.TabTrn(0, idemp, true, id, 2, WinModal: true);
                    }
                }
                else
                {
                    MessageBox.Show("seleccione un nota credito para realizar el traslado a bodega de producto no conforme");
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar transaccion:" + w);
            }
        }


        private int ExecuteSqlTransaction()
        {
            int bandera = -1;

            if (MessageBox.Show("Usted desea genarar el traslado a (" + comboBoxBodega.SelectedValue.ToString() + " - bodega de producto no conforme)?" + comboBoxBodega.SelectedValue.ToString(), "Guardar Traslado", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                DataRowView row = (DataRowView)dataGridCabeza.SelectedItems[0];
                string nota_cre = row["num_trn"].ToString().Trim();
                string _bodOrigen = row["bod_tra"].ToString().Trim();
                string _bodDestino = comboBoxBodega.SelectedValue.ToString();
                DataTable dt_cuerpo = SiaWin.Func.SqlDT("select * from incue_doc where idregcab='" + row["idreg"].ToString().Trim() + "' ", "bod", idemp);

                string TipoConsecutivo = "sal_trasl";
                string codtrn = "141";
                string codtrncontra = "051";


                using (SqlConnection connection = new SqlConnection(cnEmp))
                {
                    StringBuilder errorMessages = new StringBuilder();
                    try
                    {
                        string sqlcabContra = "";
                        string sqlcab = "";
                        string puntov = punto_v + "T";


                        string sqlConsecutivo = @"declare @fecdoc as datetime;set @fecdoc = getdate();";
                        sqlConsecutivo = sqlConsecutivo + "declare @fecdocsecond as datetime;set @fecdocsecond = DATEADD(second,1,GETDATE()); ";
                        sqlConsecutivo = sqlConsecutivo + "declare @ini as char(4);declare @num as varchar(12); ";
                        sqlConsecutivo = sqlConsecutivo + "declare @iConsecutivo char(12) = '' ;declare @iFolioHost int = 0; ";
                        sqlConsecutivo = sqlConsecutivo + "UPDATE COpventas SET " + TipoConsecutivo + "=ISNULL(" + TipoConsecutivo + ", 0) + 1  WHERE cod_pvt='" + punto_v + "'; ";
                        sqlConsecutivo = sqlConsecutivo + "SELECT @iFolioHost = " + TipoConsecutivo + ",@ini=rtrim('" + puntov + "') FROM Copventas  WHERE cod_pvt='" + punto_v + "'; set @num=@iFolioHost; ";
                        sqlConsecutivo = sqlConsecutivo + "select @iConsecutivo=rtrim(@ini)+'-'+REPLICATE ('0',11-len(rtrim(@ini))-len(rtrim(convert(varchar,@num))))+rtrim(convert(varchar,@num));";



                        sqlcab = sqlConsecutivo + @"INSERT INTO incab_doc (cod_trn,fec_trn,num_trn,doc_ref,des_mov,bod_tra,UserId) values ('" + codtrn + "',@fecdoc,@iConsecutivo,@iConsecutivo,'SALIDA POR PRODUCTO NO CONFORME','" + _bodDestino + "'," + SiaWin._UserId + ");DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                        sqlcabContra = @"INSERT INTO incab_doc (cod_trn,fec_trn,num_trn,doc_ref,des_mov,bod_tra,UserId) values ('" + codtrncontra + "',@fecdocsecond,@iConsecutivo,@iConsecutivo,'ENTRADA POR PRODUCTO NO CONFORME','" + _bodOrigen + "'," + SiaWin._UserId + ");DECLARE @NewIDContra INT;SELECT @NewIDContra = SCOPE_IDENTITY();";

                        string sql = "";
                        string sqlcontra = "";



                        foreach (DataRow item in dt_cuerpo.Rows)
                        {
                            decimal cantidad = Convert.ToInt32(item["cantidad"]);
                            string cod_ref = item["cod_ref"].ToString().Trim();

                            if (cantidad > 0)
                            {
                                sql = sql + @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cantidad,fecha_aded,doc_cruc) values (@NewID,'" + codtrn + "',@iConsecutivo,'" + cod_ref + "','" + _bodOrigen + "'," + cantidad.ToString("F", CultureInfo.InvariantCulture) + ",@fecdoc,'" + nota_cre + "');";

                                sqlcontra = sqlcontra + @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cantidad,fecha_aded) values (@NewIDContra,'" + codtrncontra + "',@iConsecutivo,'" + cod_ref + "','" + _bodDestino + "'," + cantidad.ToString("F", CultureInfo.InvariantCulture) + ",@fecdocsecond);";
                            }
                        }

                        //command.CommandText = sqlcab + sql + sqlcabContra + sqlcontra + @"select CAST(@NewId AS int);";


                        connection.Open();
                        SqlCommand command = connection.CreateCommand();
                        command.Connection = connection;
                        StringBuilder xx = new StringBuilder();
                        xx.Append("begin transaction ");
                        xx.Append("BEGIN TRY ");
                        xx.Append("DECLARE @IdentityValue BIGINT ");
                        xx.Append("SELECT @IdentityValue = IDENT_CURRENT('incab_doc');");
                        xx.Append(sqlcab + sql + ";");
                        xx.Append(";insert into InCue_DocCtrl(tipo,cod_ref,cod_bod,cantidad,idregcab) SELECT 2,incue_doc.cod_ref,incue_doc.cod_bod,sum(cantidad) as cantidad,@NewId from incue_doc inner join [GrupoSaavedra_Emp010].[dbo].[InMae_ref] as ref on ref.cod_ref=incue_doc.cod_ref and ref.ind_cant<>1 where idregcab=@NewId group by incue_doc.cod_ref,cod_bod order by incue_doc.cod_ref,cod_bod ;");
                        xx.Append(sqlcabContra + sqlcontra + @";select CAST(@NewId AS int);");
                        xx.Append("commit transaction;");
                        xx.Append("END TRY ");
                        xx.Append("BEGIN CATCH ");
                        xx.Append("select ERROR_NUMBER() AS ErrorNumber , ERROR_MESSAGE() AS ErrorMessage; ");
                        xx.Append("IF @@TRANCOUNT > 0 ");
                        xx.Append(" begin ");
                        xx.Append("ROLLBACK TRANSACTION; ");
                        xx.Append("DBCC CHECKIDENT('incab_doc', RESEED, @IdentityValue); ");
                        xx.Append("END ");
                        xx.Append("END CATCH ");
                        xx.Append("IF @@TRANCOUNT > 0  COMMIT TRANSACTION; ");
                        command.CommandText = xx.ToString();

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        int rows_returned = sda.Fill(dt);

                        if (rows_returned > 0)
                        {
                            if (dt != null)
                            {
                                if (dt.Columns.Count > 1) // es error
                                {
                                    connection.Close();
                                    string msg = dt.Rows[0][1].ToString();
                                    SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, msg, "");
                                    MessageBox.Show(msg, "Error - Productos sin Stock");
                                    return -1;
                                }
                                if (dt.Columns.Count == 1) // tiene registro
                                {
                                    connection.Close();
                                    //MessageBox.Show(dt.Rows[0][0].ToString());
                                    //_usercontrol.Seg.Auditor(0,_usercontrol.ProjectId,idUser,_usercontrol.GroupId,idEmp,_usercontrol.ModuleId,_usercontrol.AccesoId,0,xx.ToString(),"");
                                    return Convert.ToInt32(dt.Rows[0][0].ToString());
                                }

                            }

                        }


                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Append("c Error:#" + ex.Message.ToString());
                        MessageBox.Show(errorMessages.ToString());
                        bandera = -1;
                    }
                }
            }
            else
            {
                bandera = -1;
            }

            return bandera;
        }





        /////----------------------------        
        ////modificacion por : alejandro #08

        //public void ExecuteSqlTransactionContable(string idreg)
        //{
        //    try
        //    {

        //        //datos
        //        string centro_costo = "";
        //        string numero_trn = "";
        //        string tercero = "";
        //        string vendedor = "";
        //        //cuenta
        //        string cod_trnVenta = "";
        //        string cod_trnContable = "";

        //        //cuentas rentas
        //        //retenta debito
        //        string cta_renta_deb = "";
        //        double por_cta_renta_deb = 0;
        //        //renta credito
        //        string cta_renta_cre = "";
        //        double por_cta_renta_cre = 0;
        //        //cxc
        //        //string cta_cxc = "";

        //        decimal debito = 0;
        //        decimal credito = 0;


        //        #region Datos factura

        //        //valores de la venta, obtencion de retenciones del cliente,cuenta ica por la ciudad del cliente
        //        string query = "select InCab_doc.cod_trn,InCab_doc.num_trn,InMae_trn.cod_tdo,InCab_doc.cod_cli,InMae_mer.cod_cco,InCab_doc.trn_anu,InCab_doc.cod_ven ";
        //        query += "from InCab_doc ";
        //        query += "inner join CoMae_ter on CoMae_ter.cod_ter = InCab_doc.cod_cli ";
        //        query += "inner join InMae_mer on InMae_mer.cod_mer = InCab_doc.cod_ven ";
        //        query += "inner join InMae_trn on InCab_doc.cod_trn = InMae_trn.cod_trn ";
        //        query += "where InCab_doc.idreg='" + idreg + "' ";

        //        DataTable dtCont = ((Inicio)Application.Current.MainWindow).DB.SqlDT(query, "Table1", idEmp);
        //        //((Inicio)Application.Current.MainWindow).Browse(dtCont);

        //        tercero = dtCont.Rows[0]["cod_cli"].ToString().Trim();
        //        numero_trn = dtCont.Rows[0]["num_trn"].ToString().Trim();
        //        centro_costo = dtCont.Rows[0]["cod_cco"].ToString().Trim();
        //        cod_trnVenta = dtCont.Rows[0]["cod_trn"].ToString().Trim();
        //        cod_trnContable = dtCont.Rows[0]["cod_tdo"].ToString().Trim();
        //        vendedor = dtCont.Rows[0]["cod_ven"].ToString().Trim();

        //        string trn_anu_fact = dtCont.Rows[0]["trn_anu"].ToString().Trim();

        //        #endregion

        //        #region cuentas


        //        var rc_deb = ((Inicio)Application.Current.MainWindow).Func.GetPropiedadesCuenta("cta_rcdeb", "001", idEmp);
        //        cta_renta_deb = rc_deb.Item1;
        //        por_cta_renta_deb = rc_deb.Item2;

        //        var rc_cre = ((Inicio)Application.Current.MainWindow).Func.GetPropiedadesCuenta("cta_rccre", "001", idEmp);
        //        cta_renta_cre = rc_cre.Item1;
        //        por_cta_renta_cre = rc_cre.Item2;


        //        if (cod_trnVenta != "004" && ConfigCSource.trnAnu != "004")
        //        {
        //            var rc_cxc = ((Inicio)Application.Current.MainWindow).Func.GetPropiedadesCuenta("cta_cxc", "001", idEmp);
        //            ConfigCSource.cta_cxc = rc_cxc.Item1;
        //        }
        //        else
        //        {
        //            ConfigCSource.cta_cxc = "";
        //        }

        //        #endregion

        //        #region traer compra y formar los valores para agregar a la contabilidad


        //        //valores de reteica,retefuente,reteiva
        //        DataTable dtCompra = ((Inicio)Application.Current.MainWindow).DB.SqlDT("select * from incue_doc where idregcab='" + idreg + "' ", "Compra", idEmp);
        //        decimal _subtotal = 0, _valretFue = 0, _valretica = 0, _val_ren_deb = 0, _val_ren_cre = 0;
        //        System.Data.DataTable dtServicio = new DataTable();
        //        dtServicio.Columns.Add("cod_ref");
        //        dtServicio.Columns.Add("cod_cta");
        //        dtServicio.Columns.Add("valor", typeof(double));
        //        dtServicio.Columns.Add("tipo", typeof(bool));

        //        foreach (DataRow item in dtCompra.Rows)
        //        {
        //            string referencia = item["cod_ref"].ToString().Trim();

        //            decimal subtotal = referencia == "BOLSA_PLASTICA" ? 0 : Convert.ToDecimal(item["subtotal"]);


        //            System.Data.DataRow[] result = ServiciosRef.Select("cod_ref='" + referencia + "' ");


        //            decimal val_ret_fue = 0;
        //            decimal val_ret_ica = 0;

        //            if (result.Length <= 0)//si no son servicios
        //            {
        //                val_ret_fue = referencia == "BOLSA_PLASTICA" ? 0 : Convert.ToDecimal(item["val_ret"]);
        //                val_ret_ica = referencia == "BOLSA_PLASTICA" ? 0 : Convert.ToDecimal(item["val_ica"]);
        //            }
        //            else
        //            {
        //                double subt_ser = Convert.ToDouble(item["subtotal"]);
        //                foreach (System.Data.DataRow row in result)
        //                {
        //                    if (!string.IsNullOrEmpty(row["cta_rtf"].ToString()))
        //                    {
        //                        double prc_rft = Convert.ToDouble(row["por_cta_rtf"]) > 0 ? Convert.ToDouble(row["por_cta_rtf"]) : 0;
        //                        double valorSerRtf = (subt_ser * prc_rft) / 100;
        //                        dtServicio.Rows.Add(row["cod_ref"].ToString(), row["cta_rtf"].ToString(), valorSerRtf, true);
        //                    }
        //                    if (!string.IsNullOrEmpty(row["cta_ica"].ToString()))
        //                    {
        //                        double prc_rica = Convert.ToDouble(row["por_cta_rica"]) > 0 ? Convert.ToDouble(row["por_cta_rica"]) : 0;
        //                        double valorSerRica = (subt_ser * prc_rica) / 100;
        //                        dtServicio.Rows.Add(row["cod_ref"].ToString(), row["cta_ica"].ToString(), valorSerRica, false);
        //                    }
        //                }
        //            }

        //            //decimal val_ret_fue = item["cod_ref"].ToString().Trim() == "BOLSA_PLASTICA" ? 0 : Convert.ToDecimal(item["val_ret"]);
        //            //decimal val_ret_ica = item["cod_ref"].ToString().Trim() == "BOLSA_PLASTICA" ? 0 : Convert.ToDecimal(item["val_ica"]);

        //            decimal val_ren_deb = (subtotal * Convert.ToDecimal(por_cta_renta_deb)) / 100;
        //            decimal val_ren_cre = (subtotal * Convert.ToDecimal(por_cta_renta_cre)) / 100;

        //            //retefuente
        //            _valretFue += val_ret_fue;
        //            //reteica
        //            _valretica += val_ret_ica;
        //            //renta debito
        //            _val_ren_deb += val_ren_deb;
        //            //renta credito
        //            _val_ren_cre += val_ren_cre;
        //            //subtotal 
        //            _subtotal += subtotal;
        //        }

        //        //iva agrupacion de valores y sus cuentas
        //        string ivaAgrupa = "select sum(val_iva) as val_iva_total,InCue_doc.por_iva,InCue_doc.cod_tiva,InMae_tiva.cod_ctav ";
        //        ivaAgrupa += "from InCue_doc ";
        //        ivaAgrupa += "left join InMae_tiva ON InCue_doc.cod_tiva = InMae_tiva.cod_tiva ";
        //        ivaAgrupa += "where idregcab='" + idreg + "' and cod_trn='" + cod_trnVenta + "' and InCue_doc.cod_ref<>'BOLSA_PLASTICA' ";
        //        ivaAgrupa += "group by InCue_doc.por_iva,InCue_doc.cod_tiva,InMae_tiva.cod_ctav ";

        //        DataTable dtIva = ((Inicio)Application.Current.MainWindow).DB.SqlDT(ivaAgrupa, "IvaAgrupado", idEmp);

        //        string cuentaIvaArmable = "";

        //        string cuentaReteIvaArmable = "";
        //        decimal totalLinea = 0;
        //        decimal totalIva = 0;

        //        decimal _val_ret_iva = 0;

        //        foreach (DataRow item in dtIva.Rows)
        //        {

        //            decimal ivaVal = Convert.ToDecimal(item["val_iva_total"]);
        //            ivaVal = Math.Round(ivaVal, 0);
        //            if (ivaVal != 0)
        //            {
        //                string debito_credito = cod_trnVenta == "004" || cod_trnVenta == "005" ? "cre_mov" : "deb_mov";
        //                string descripcion_nc = cod_trnVenta == "004" || cod_trnVenta == "005" ? "IVA-" + tercero : ConfigCSource.TipoAnu + "-" + ConfigCSource.numAnu;

        //                cuentaIvaArmable += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov," + debito_credito + ") values (@NewTrn_001,'" + cod_trnContable + "','" + numero_trn + "','" + item["cod_ctav"] + "','" + tercero + "','" + centro_costo + "','" + descripcion_nc + "'," + _subtotal.ToString("F", CultureInfo.InvariantCulture) + "," + ivaVal.ToString("F", CultureInfo.InvariantCulture) + "); ";
        //                totalIva += ivaVal;
        //            }
        //        }


        //        totalIva = Math.Round(totalIva);               

        //        //linea 
        //        string subtLinea = "select sum(subtotal) as subtotal,InMae_tip.cta_ing from InCue_doc  ";
        //        subtLinea += "inner join InMae_ref on InCue_doc.cod_ref = InMae_ref.cod_ref ";
        //        subtLinea += "inner join InMae_tip on InMae_ref.cod_tip = InMae_tip.cod_tip ";
        //        subtLinea += "where idregcab='" + idreg + "' ";
        //        subtLinea += "group by InMae_tip.cta_ing ";

        //        DataTable dtSubLinea = ((Inicio)Application.Current.MainWindow).DB.SqlDT(subtLinea, "LineaTot", idEmp);
        //        string cuentainv = "";
        //        foreach (DataRow item in dtSubLinea.Rows)
        //        {
        //            string debito_credito = cod_trnVenta == "004" || cod_trnVenta == "005" ? "cre_mov" : "deb_mov";
        //            decimal sub_totalLinea = Convert.ToDecimal(item["subtotal"]);

        //            string descripcion_nc = cod_trnVenta == "004" || cod_trnVenta == "005" ? "VENTA MERCANCIA A:" + tercero : ConfigCSource.TipoAnu + "-" + ConfigCSource.numAnu;

                                       
        //            cuentainv += @"insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov," + debito_credito + ") values (@NewTrn_001,'" + cod_trnContable + "','" + numero_trn + "','" + item["cta_ing"] + "','" + tercero + "','" + centro_costo + "','" + descripcion_nc + "'," + _subtotal.ToString("F", CultureInfo.InvariantCulture) + "," + sub_totalLinea.ToString("F", CultureInfo.InvariantCulture) + "); ";
        //            totalLinea += sub_totalLinea;
        //        }

        //        #endregion

        //        #region sumar debitos y creditos y redondear                

        //        _valretFue = Math.Round(_valretFue);
        //        _valretica = Math.Round(_valretica);
        //        _val_ren_deb = Math.Round(_val_ren_deb);
        //        _val_ren_cre = Math.Round(_val_ren_cre);


        //        if (ConfigCSource.maneja_iva == true) credito += totalIva;
        //        if (ConfigCSource.retencion_fue == true) debito += _valretFue;
        //        if (ConfigCSource.retencion_ica == true) debito += _valretica;
        //        if (ConfigCSource.retencion_iva == true && ConfigCSource.maneja_iva == true) debito += _val_ret_iva;
        //        debito += _val_ren_deb;
        //        credito += _val_ren_cre;

        //        //credito = Math.Round(credito);
        //        //debito = Math.Round(debito);

        //        //decimal totalcxc = credito - debito;
        //        //totalcxc = Math.Round(totalcxc);

        //        #endregion

        //        #region generar el documento contable



        //        if (ConfigCSource.cod_trn.IsBetween("004", "009"))
        //        {
        //            using (SqlConnection connection = new SqlConnection(CnEmp))
        //            {

        //                connection.Open();
        //                StringBuilder errorMessages = new StringBuilder();
        //                SqlCommand command = connection.CreateCommand();
        //                SqlTransaction transaction;

        //                transaction = connection.BeginTransaction("Transaction");
        //                command.Connection = connection;
        //                command.Transaction = transaction;


        //                string sqlConsecutivo = @"declare @fecdoc as datetime;
        //                set @fecdoc = getdate();declare @ini as char(4);DECLARE @NewTrn_001 INT;";

        //                string sqlcab001co = sqlConsecutivo + @" INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,dia_plaz,fec_ven,factura,cod_ven,UserId) values ('" + cod_trnContable + "','" + numero_trn + "',@fecdoc,90,dateadd(DAY,90,@fecdoc),'" + numero_trn + "','" + vendedor + "'," + _usercontrol.UserId + ");SELECT @NewTrn_001 = SCOPE_IDENTITY();";

        //                //servicios con cuentas
        //                System.Data.DataTable ServAgruCta = new System.Data.DataTable();
        //                if (dtServicio.Rows.Count > 0)
        //                {
        //                    ServAgruCta = dtServicio.AsEnumerable()
        //                        .GroupBy(a => a["cod_cta"])
        //                        .Select(c =>
        //                        {
        //                            var row = dtServicio.NewRow();
        //                            row["cod_cta"] = c.Key;
        //                            row["tipo"] = c.Max(a => a.Field<bool>("tipo"));
        //                            row["valor"] = c.Sum(a => a.Field<double>("valor"));
        //                            return row;
        //                        }).CopyToDataTable();
        //                }

        //                //ingreso
        //                string sqlcue001co = cuentainv;
        //                //iva 
        //                if (totalIva > 0 && ConfigCSource.maneja_iva == true)
        //                    sqlcue001co += cuentaIvaArmable;


        //                if (totalIva > 0)
        //                    sqlcue001co += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,deb_mov) values (@NewTrn_001,'" + cod_trnContable + "','" + numero_trn + "','531520','" + tercero + "','" + centro_costo + "',''," + totalIva.ToString("F", CultureInfo.InvariantCulture) + ")";

        //                if (totalLinea>0)
        //                    sqlcue001co += @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,deb_mov) values (@NewTrn_001,'" + cod_trnContable + "','" + numero_trn + "','530535','" + tercero + "','" + centro_costo + "',''," + totalLinea.ToString("F", CultureInfo.InvariantCulture) + ")";


        //                command.CommandText = sqlcab001co + sqlcue001co + @"select CAST(@NewTrn_001 AS int);";
        //                var r = new object();
        //                r = command.ExecuteScalar();
        //                transaction.Commit();
        //                connection.Close();
        //            }
        //        }
        //        #endregion

        //    }
        //    catch (Exception e)
        //    {
        //        ((Inicio)Application.Current.MainWindow).Func.SiaExeptionGobal(e);
        //        MessageBox.Show("Error al generar documento contable:" + e);
        //    }
        //}







    }
}
