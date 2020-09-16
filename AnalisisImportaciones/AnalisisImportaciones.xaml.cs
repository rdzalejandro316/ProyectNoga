using AnalisisImportaciones;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9592,"AnalisisImportaciones");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9592,"AnalisisImportaciones");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class AnalisisImportaciones : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public DataTable dt_contable = new DataTable();
        public DataTable dt_inv = new DataTable();
        public DataTable listAgrupDocUpdate = new DataTable();

        public AnalisisImportaciones()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            //idemp = SiaWin._BusinessId; ;            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
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
                this.Title = "Analisis de Importaciones " + cod_empresa + "-" + nomempresa;

                if (cod_empresa == "010" || cod_empresa == "030" || cod_empresa == "060" || cod_empresa == "070")
                {
                    MessageBox.Show("este empresa no maneja importaciones", "alert", MessageBoxButton.OK, MessageBoxImage.Stop);
                    this.IsEnabled = false;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }



        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Buscar view = new Buscar(idemp);
                view.ShowInTaskbar = false;
                view.Owner = Application.Current.MainWindow;
                view.PntImportacion = true;
                view.ShowDialog();

                if (view.selecciono)
                {
                    DataRowView rowgrid = view.row;
                    string n_importacion = rowgrid["n_imp"].ToString().Trim();
                    Tx_Impor.Text = n_importacion;

                    Buscar viewDoc = new Buscar(idemp);
                    viewDoc.ShowInTaskbar = false;
                    viewDoc.Owner = Application.Current.MainWindow;
                    viewDoc.PntImportacion = false;
                    viewDoc.n_importacion = n_importacion;
                    viewDoc.ShowDialog();

                    documentImportacion(n_importacion, cod_empresa);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir la busqueda");
            }
        }


        public async void documentImportacion(string importacion, string empresa)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;

                string imp = importacion;
                string emp = empresa;
                GridMain.Opacity = 0.5;

                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(imp, emp, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dt_inv = ((DataSet)slowTask.Result).Tables[0];
                    dt_contable = ((DataSet)slowTask.Result).Tables[1];
                    listAgrupDocUpdate = ((DataSet)slowTask.Result).Tables[2];

                    if (dt_contable.Rows.Count<=0)
                    {
                        MessageBox.Show("la importacion no contiene documentos contables registrados para poder realizar el proceso de importacion","alert",MessageBoxButton.OK,MessageBoxImage.Error);
                        this.sfBusyIndicator.IsBusy = false;
                        GridMain.Opacity = 1;
                        return;
                    }

                    bool flag = false;
                    foreach (DataRow item in dt_inv.Rows) if (Convert.ToInt32(item["estado"]) == 1) flag = true;


                    if (flag == true)
                    {
                        BtnCalcular.IsEnabled = true;
                        BtnCerrar.IsEnabled = false;
                        BtnCalcular.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                        actualizarValores(dt_inv);                        
                    }
                    else
                    {
                        BtnCerrar.IsEnabled = true;
                        BtnCalcular.IsEnabled = true;
                    }

                    if (flag) MessageBox.Show("la importacion ya se liquido", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);

                    dataGridImpor.ItemsSource = dt_inv;
                    dataGridConta.ItemsSource = dt_contable;

                    decimal deb = Convert.ToDecimal(dt_contable.Compute("Sum(deb_mov)", ""));
                    decimal cre = Convert.ToDecimal(dt_contable.Compute("Sum(cre_mov)", ""));

                    Tx_debito.Text = deb.ToString("C");
                    Tx_credito.Text = cre.ToString("C");


                    Tx_cnt.Text = dt_inv.Compute("Sum(cantidad)", "").ToString();

                    decimal cos_uni = Convert.ToDecimal(dt_inv.Compute("Sum(cos_uni)", ""));
                    decimal cos_tot = Convert.ToDecimal(dt_inv.Compute("Sum(cos_tot)", ""));
                    Tx_unPesUni.Text = cos_uni.ToString("C");
                    Tx_unPesTot.Text = cos_tot.ToString("C");

                }

                GridMain.Opacity = 1;
                this.sfBusyIndicator.IsBusy = false;
            }
            catch (Exception ex)
            {
                this.sfBusyIndicator.IsBusy = false;
                MessageBox.Show(ex.Message);
            }
        }



        public void actualizarValores(DataTable dt_cue)
        {
            foreach (DataRow item in dt_cue.Rows)
            {
                decimal c = Convert.ToDecimal(item["p_c"]);
                decimal a = Convert.ToDecimal(item["p_a"]);
                decimal ni = Convert.ToDecimal(item["p_ni"]);
                decimal g_impo = Convert.ToDecimal(item["gasto_importacion"]);

                string query = "update InCue_doc set pc_aranc=" + c.ToString("F", CultureInfo.InvariantCulture) + ",pa_aranc="+ a.ToString("F", CultureInfo.InvariantCulture) + ",pni_aranc="+ a.ToString("F", CultureInfo.InvariantCulture) + ",gas_impo="+ g_impo.ToString("F", CultureInfo.InvariantCulture) + " where idreg='" +item["idregcu"].ToString() +"' and cod_trn='980' ";
                
                if (SiaWin.Func.SqlCRUD(query, idemp) == false)
                {
                    MessageBox.Show("error en la actualizacion de datos para el formato de impresion","alerta",MessageBoxButton.OK,MessageBoxImage.Error);
                }                
            }
        }



        private DataSet LoadData(string impo, string empresa, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpImportacion980", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@num_import", impo);
                cmd.Parameters.AddWithValue("@codemp", empresa);
                da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
                con.Close();
                return ds;

            }
            catch (Exception e)
            {
                MessageBox.Show("error#" + e.Message);
                return null;
            }
        }

        private void BtnCalcular_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //MessageBox.Show("a1");
                #region % C


                decimal suma_cost = Convert.ToDecimal(dt_inv.Compute("Sum(cos_tot)", ""));


                decimal total_con = Convert.ToDecimal(dt_contable.Compute("Sum(deb_mov)", "n_ar='' and doc_980=''"));
                //MessageBox.Show("total_con:" + total_con);

                foreach (System.Data.DataRow dr in dt_inv.Rows)
                {

                    dr.BeginEdit();
                    decimal valor_por = (Convert.ToDecimal(dr["cos_tot"]) / suma_cost) * 100;
                    dr["p_c"] = valor_por;

                    decimal valor = Math.Round(valor_por, 6) * total_con / 100;
                    //MessageBox.Show("operacion:(" + Math.Round(valor_por, 2) + "/" + total_con + ")*100");
                    //MessageBox.Show("valor:" + valor);
                    dr["p_c_valor"] = valor;
                    dr.EndEdit();
                }
                dataGridImpor.UpdateLayout();



                #endregion

                //MessageBox.Show("a2");
                #region % A


                DataTable dt_I = new DataTable();
                DataTable dt_C = new DataTable();
                DataTable dt_temp_inv = dt_inv;
                DataTable dt_temp_cont = dt_contable;

                //agrupacion inventario por arancel
                if (dt_temp_inv.Rows.Count > 0)
                {
                    dt_I = dt_temp_inv.AsEnumerable()
                        .GroupBy(a => a["agrupacion"].ToString().Trim())
                        .Select(c =>
                        {
                            var row = dt_temp_inv.NewRow();
                            row["agrupacion"] = c.Key;
                            row["cos_tot"] = c.Sum(a => a.Field<decimal>("cos_tot"));
                            return row;
                        }).CopyToDataTable();
                }


                //agrupacion contable por arancel
                if (dt_temp_cont.Rows.Count > 0)
                {
                    dt_C = dt_temp_cont.AsEnumerable()
                        .GroupBy(a => a["n_ar"].ToString().Trim())
                        .Select(c =>
                        {
                            var row = dt_temp_cont.NewRow();
                            row["n_ar"] = c.Key;
                            row["deb_mov"] = c.Sum(a => a.Field<decimal>("deb_mov"));
                            return row;
                        }).CopyToDataTable();
                }



                //guarda en una lista los aranceles de los documentos
                var valores = new List<string>();
                foreach (DataRow item in dt_C.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(item["n_ar"].ToString().Trim()))
                        valores.Add(item["n_ar"].ToString().Trim());
                }

                //buscar el arancel si esta dentro de la lista y realiza el calculo
                foreach (System.Data.DataRow dr in dt_inv.Rows)
                {
                    string agrupacion = dr["agrupacion"].ToString().Trim();
                    if (valores.Contains(agrupacion, StringComparer.OrdinalIgnoreCase))
                    {
                        DataRow[] dataIn = dt_I.Select("agrupacion='" + agrupacion + "'");
                        DataRow[] dataCo = dt_C.Select("n_ar='" + agrupacion + "'");
                        foreach (DataRow row in dataIn)
                        {
                            decimal valor_agru_in = Convert.ToDecimal(row["cos_tot"]);
                            decimal valor_agru_co = 0;
                            foreach (var row_co in dataCo) valor_agru_co = Convert.ToDecimal(row_co["deb_mov"]);
                            dr.BeginEdit();
                            decimal valor_por = (Convert.ToDecimal(dr["cos_tot"]) / valor_agru_in) * 100;
                            dr["p_a"] = valor_por;
                            decimal valor = (Math.Round(valor_por, 6) * valor_agru_co) / 100;
                            dr["p_a_valor"] = valor;
                            dr.EndEdit();
                        }
                        dataGridImpor.UpdateLayout();
                        dataGridImpor.Focus();
                        dataGridImpor.SelectedIndex = 0;
                    }
                }


                #endregion

                //MessageBox.Show("a3");
                #region % NI


                DataTable dt_I_NI = new DataTable();
                DataTable dt_C_NI = new DataTable();
                DataTable dt_temp_inv_NI = dt_inv;
                DataTable dt_temp_cont_NI = dt_contable;

                //agrupacion inventario por documento 980
                if (dt_temp_inv_NI.Rows.Count > 0)
                {
                    dt_I_NI = dt_temp_inv_NI.AsEnumerable()
                        .GroupBy(a => a["documento"].ToString().Trim())
                        .Select(c =>
                        {
                            var row = dt_temp_inv_NI.NewRow();
                            row["documento"] = c.Key;
                            row["cos_tot"] = c.Sum(a => a.Field<decimal>("cos_tot"));
                            return row;
                        }).CopyToDataTable();
                }

                //agrupacion contable por documento 980
                if (dt_temp_cont_NI.Rows.Count > 0)
                {
                    dt_C_NI = dt_temp_cont_NI.AsEnumerable()
                        .GroupBy(a => a["doc_980"].ToString().Trim())
                        .Select(c =>
                        {
                            var row = dt_temp_cont_NI.NewRow();
                            row["doc_980"] = c.Key;
                            row["deb_mov"] = c.Sum(a => a.Field<decimal>("deb_mov"));
                            return row;
                        }).CopyToDataTable();
                }



                //guarda en una lista los aranceles de los documentos
                var valores_NI = new List<string>();
                foreach (DataRow item in dt_C_NI.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(item["doc_980"].ToString().Trim()))
                        valores.Add(item["doc_980"].ToString().Trim());
                }

                //buscar el arancel si esta dentro de la lista y realiza el calculo
                foreach (System.Data.DataRow dr in dt_inv.Rows)
                {
                    string document = dr["documento"].ToString().Trim();
                    if (valores.Contains(document, StringComparer.OrdinalIgnoreCase))
                    {
                        DataRow[] dataIn = dt_I_NI.Select("documento='" + document + "'");
                        DataRow[] dataCo = dt_C_NI.Select("doc_980='" + document + "'");
                        foreach (DataRow row in dataIn)
                        {
                            decimal valor_agru_in = Convert.ToDecimal(row["cos_tot"]);
                            decimal valor_agru_co = 0;
                            foreach (var row_co in dataCo) valor_agru_co = Convert.ToDecimal(row_co["deb_mov"]);
                            dr.BeginEdit();
                            decimal valor_por = (Convert.ToDecimal(dr["cos_tot"]) / valor_agru_in) * 100;
                            dr["p_ni"] = valor_por;
                            decimal valor = (Math.Round(valor_por, 6) * valor_agru_co) / 100;
                            dr["p_ni_valor"] = valor;
                            dr.EndEdit();
                        }
                        dataGridImpor.UpdateLayout();
                        dataGridImpor.Focus();
                        dataGridImpor.SelectedIndex = 0;
                    }
                }


                #endregion

                //MessageBox.Show("a4");
                #region Gasto de importacion


                decimal val = 0;
                foreach (System.Data.DataRow dr in dt_inv.Rows)
                {
                    decimal p_c_valor = Convert.ToDecimal(dr["p_c_valor"]);
                    decimal p_a_valor = Convert.ToDecimal(dr["p_a_valor"]);
                    decimal p_ni_valor = Convert.ToDecimal(dr["p_ni_valor"]);

                    decimal valor = p_c_valor + p_a_valor + p_ni_valor;

                    dr.BeginEdit();
                    dr["gasto_importacion"] = valor;

                    decimal total = Convert.ToDecimal(dr["cos_tot"]) + Convert.ToDecimal(dr["gasto_importacion"]);
                    dr["imp_tot"] = total;
                    dr["imp_uni"] = total / Convert.ToDecimal(dr["cantidad"]);
                    dr.EndEdit();
                }

                dataGridImpor.UpdateLayout();
                dataGridImpor.Focus();
                dataGridImpor.SelectedIndex = 0;

                #endregion

                //MessageBox.Show("a5");
                #region totales


                decimal valorGIM = 0;
                decimal valorTPro = 0;
                foreach (System.Data.DataRow dr in dt_inv.Rows)
                {
                    valorGIM += Convert.ToDecimal(dr["gasto_importacion"]);
                    valorTPro += Convert.ToDecimal(dr["imp_tot"]);
                }

                

                Tx_GsImp.Text = Math.Round(valorGIM).ToString("C");
                Tx_prod.Text = valorTPro.ToString("C");


                decimal gs_impo = Convert.ToDecimal(valorGIM);
                decimal fac = (gs_impo / suma_cost) * 100;
                decimal facRed = Math.Round(fac, 4);

                F_importacion.Text = facRed.ToString();
                #endregion

                actualizarValores(dt_inv);


            }
            catch (Exception w)
            {
                MessageBox.Show("Error al calcular:" + w);
            }
        }



        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PntGuardar view = new PntGuardar(idemp);
                view.ShowInTaskbar = false;
                view.Owner = Application.Current.MainWindow;
                view.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                view.ShowDialog();

                bool flag = view.guardar;


                if (flag)
                {
                    var ret = view.val_ret;
                    int doc = GenerateDocument(consecutivo: ret.Item1, fec_tras: ret.Item2, bodega: ret.Item3);
                    if (doc > 0)
                    {
                        MessageBox.Show("transaccion exitosa");
                        BtnCerrar.IsEnabled = false;
                        BtnCalcular.IsEnabled = false;
                        //BtnCalcular.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));                        
                        //BtnCalcular.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));                        
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error:" + w);
            }
        }



        public int GenerateDocument(string consecutivo, string fec_tras, string bodega)
        {
            int doc_generado = 0;

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
                    string sqlcab144 = "";
                    string sqlcab054 = "";

                    string cue144 = "";
                    string cue054 = "";

                    string update980 = "";
                    foreach (DataRow dr in listAgrupDocUpdate.Rows)
                        update980 += "update incab_doc set estado='1' where idreg='" + dr["idreg"].ToString().Trim() + "';";

                    sqlcab144 = @"INSERT INTO incab_doc (cod_trn,fec_trn,num_trn,bod_tra) values ('144','" + fec_tras + "','" + consecutivo + "','" + bodega + "');DECLARE @NewID INT;SELECT @NewID = SCOPE_IDENTITY();";
                    sqlcab054 = @"INSERT INTO incab_doc (cod_trn,fec_trn,num_trn,bod_tra) values ('054','" + fec_tras + "','" + consecutivo + "','900');DECLARE @NewIDContra INT;SELECT @NewIDContra = SCOPE_IDENTITY();";

                    foreach (DataRow dr in dt_inv.Rows)
                    {
                        decimal cantidad = Convert.ToDecimal(dr["cantidad"]);
                        decimal cos_uni_054 = Convert.ToDecimal(dr["imp_uni"]);
                        decimal cos_tot_054 = Convert.ToDecimal(dr["imp_tot"]);
                        decimal cos_uni_144 = Convert.ToDecimal(dr["cos_uni"]);
                        decimal cos_tot_144 = Convert.ToDecimal(dr["cos_tot"]);

                        cue144 += @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_bod,cod_ref,cantidad,cos_uni,cos_tot,fecha_aded) values (@NewID,'144','" + consecutivo + "', '900' ,'" + dr["cod_ref"].ToString().Trim() + "'," + cantidad.ToString("F", CultureInfo.InvariantCulture) + "," + cos_uni_144.ToString("F", CultureInfo.InvariantCulture) + "," + cos_tot_144.ToString("F", CultureInfo.InvariantCulture) + ",GETDATE());";
                        cue054 += @"INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_bod,cod_ref,cantidad,cos_uni,cos_tot,fecha_aded) values (@NewIDContra,'054','" + consecutivo + "',  '" + bodega + "', '" + dr["cod_ref"].ToString().Trim() + "'," + cantidad.ToString("F", CultureInfo.InvariantCulture) + "," + cos_uni_054.ToString("F", CultureInfo.InvariantCulture) + "," + cos_tot_054.ToString("F", CultureInfo.InvariantCulture) + ",GETDATE());";
                    }



                    command.CommandText = sqlcab144 + cue144 + sqlcab054 + cue054 + update980 + @"select CAST(@NewId AS int);";
                    //MessageBox.Show(command.CommandText.ToString());
                    var r = new object();
                    r = command.ExecuteScalar();
                    transaction.Commit();
                    connection.Close();
                    doc_generado = Convert.ToInt32(r.ToString());
                }
                catch (Exception ex)
                {
                    SiaWin.Func.SiaExeptionGobal(ex);
                    errorMessages.Append("c Error:#" + ex.Message.ToString());
                    transaction.Rollback();
                    MessageBox.Show(errorMessages.ToString());
                }

                return doc_generado;
            }

        }





        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;

                var excelEngine = dataGridImpor.ExportToExcel(dataGridImpor.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;
                workBook.ActiveSheet.Columns[13].NumberFormat = "#.#";


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

                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:" + w);
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Imprimir view = new Imprimir(idemp);
                view.doc_impo = Tx_Impor.Text;
                view.fac_impo = Convert.ToDecimal(F_importacion.Text);
                view.ShowInTaskbar = false;
                view.Owner = Application.Current.MainWindow;
                view.ShowDialog();
                

            }
            catch (Exception w)
            {
                MessageBox.Show("error al imprimir:" + w);                
            }
        }



        











    }
}
