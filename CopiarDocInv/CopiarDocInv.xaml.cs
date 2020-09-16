using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

    //    Sia.PublicarPnt(9699,"CopiarDocInv");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9699,"CopiarDocInv");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();


    public partial class CopiarDocInv : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string tabla_cab = "incab_doc";
        string tabla_cue = "incue_doc";
        string tabla_trn = "inmae_trn";

        DataTable trn = new DataTable();

        public CopiarDocInv()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
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
                this.Title = "Copiar Documentos - " + cod_empresa + " - " + nomempresa;



                trn = SiaWin.Func.SqlDT("SELECT rtrim(cod_trn) as cod_trn,rtrim(cod_trn)+'-'+rtrim(nom_trn) as nom_trn FROM " + tabla_trn + " order by cod_trn ", "transacion", idemp);
                t_TrnCop.ItemsSource = trn.DefaultView;
                t_TrnCop.DisplayMemberPath = "nom_trn";
                t_TrnCop.SelectedValuePath = "cod_trn";


                t_TrnNue.ItemsSource = trn.DefaultView;
                t_TrnNue.DisplayMemberPath = "nom_trn";
                t_TrnNue.SelectedValuePath = "cod_trn";

                dp_fecha.Text = DateTime.Now.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }


        public bool GetNewDoc()
        {
            DataTable dt_cab = SiaWin.Func.SqlDT("select * from " + tabla_cab + " where num_trn='" + Tx_NumeroNue.Text + "' and cod_trn='" + t_TrnNue.SelectedValue + "' ", "cabeza", idemp);
            return dt_cab.Rows.Count > 0 ? true : false;
        }



        private void BtnProcesar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region validaciones                

                if (t_TrnCop.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de transaccion a copiar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (string.IsNullOrEmpty(Tx_Numero.Text))
                {
                    MessageBox.Show("ingrese el documento a copiar", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }


                if (GetNewDoc() == true)
                {
                    MessageBox.Show("el documento nuevo a copiar:" + Tx_NumeroNue.Text + " ya existe en inventario ingrese un codigo diferente", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                DataTable dt_cab = SiaWin.Func.SqlDT("select * from " + tabla_cab + " where num_trn='" + Tx_Numero.Text + "' and cod_trn='" + t_TrnCop.SelectedValue + "' ", "cabeza", idemp);
                if (dt_cab.Rows.Count <= 0)
                {
                    MessageBox.Show("el documento ingresado:" + Tx_Numero.Text + " no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                #endregion

                DataTable dt_cue = SiaWin.Func.SqlDT("select * from " + tabla_cue + " where idregcab='" + dt_cab.Rows[0]["idreg"].ToString() + "' ", "cuerpo", idemp);
                if (dt_cue.Rows.Count > 0)
                {
                    using (SqlConnection connection = new SqlConnection(cnEmp))
                    {
                        DateTime f;
                        string query = "INSERT INTO " + tabla_cab + " (cod_trn,num_trn,fec_trn,ano_doc,per_doc,des_mov,doc_ref,bod_tra,cod_ciu,cod_suc,cod_cco,cod_ven,cod_cli,suc_cli,dia_pla,fec_ven,UserId) VALUES (@cod_trn,@num_trn,@fec_trn,@ano_doc,@per_doc,@des_mov,@doc_ref,@bod_tra,@cod_ciu,@cod_suc,@cod_cco,@cod_ven,@cod_cli,@suc_cli,@dia_pla,@fec_ven,@UserId);SELECT CAST(scope_identity() AS int)";
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            DateTime fecha = Convert.ToDateTime(dp_fecha.Text);
                            DateTime fecha_ven = Convert.ToDateTime(
                                dt_cab.Rows[0]["fec_ven"] == DBNull.Value ||
                                DateTime.TryParse(dt_cab.Rows[0]["fec_ven"].ToString(), out f) == false ?
                                DateTime.Now : dt_cab.Rows[0]["fec_ven"]
                                );

                            cmd.Parameters.AddWithValue("@cod_trn", t_TrnNue.SelectedValue);
                            cmd.Parameters.AddWithValue("@num_trn", Tx_NumeroNue.Text);
                            cmd.Parameters.AddWithValue("@fec_trn", fecha.ToString("dd/MM/yyyy"));
                            cmd.Parameters.AddWithValue("@ano_doc", fecha.Year);
                            cmd.Parameters.AddWithValue("@per_doc", fecha.Month);
                            cmd.Parameters.AddWithValue("@des_mov", Tx_DescNue.Text);
                            cmd.Parameters.AddWithValue("@bod_tra", dt_cab.Rows[0]["bod_tra"].ToString());
                            cmd.Parameters.AddWithValue("@doc_ref", dt_cab.Rows[0]["doc_ref"].ToString());
                            cmd.Parameters.AddWithValue("@cod_ciu", dt_cab.Rows[0]["cod_ciu"].ToString());
                            cmd.Parameters.AddWithValue("@cod_suc", dt_cab.Rows[0]["cod_suc"].ToString());
                            cmd.Parameters.AddWithValue("@cod_cco", dt_cab.Rows[0]["cod_cco"].ToString());
                            cmd.Parameters.AddWithValue("@cod_ven", dt_cab.Rows[0]["cod_ven"].ToString());
                            cmd.Parameters.AddWithValue("@cod_cli", dt_cab.Rows[0]["cod_cli"].ToString());
                            cmd.Parameters.AddWithValue("@suc_cli", dt_cab.Rows[0]["suc_cli"].ToString());
                            cmd.Parameters.AddWithValue("@dia_pla", Convert.ToInt32(dt_cab.Rows[0]["dia_pla"]));
                            cmd.Parameters.AddWithValue("@fec_ven", fecha_ven.ToString("dd/MM/yyyy"));
                            cmd.Parameters.AddWithValue("@UserId", SiaWin._UserId);

                            connection.Open();

                            int newID = (int)cmd.ExecuteScalar();
                            decimal v;
                            if (newID == 0) MessageBox.Show("la transacion no fue exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            else
                            {
                                foreach (DataRow item in dt_cue.Rows)
                                {
                                    string query_cu = "INSERT INTO " + tabla_cue + " (idregcab,cod_trn,num_trn,ano_doc,per_doc,cod_sub,cod_tiva,cod_ref,cod_bod,cantidad,val_uni,val_iva,val_ret,val_ica,por_des,val_des,val_tot,valor,cos_uni,cos_tot,por_iva,por_ret,subtotal,por_ica,por_riva,val_riva,fisico,val_flet,cos_usd,tot_tot,cos_unin,cos_totn) values (@idregcab,@cod_trn,@num_trn,@ano_doc,@per_doc,@cod_sub,@cod_tiva,@cod_ref,@cod_bod,@cantidad,@val_uni,@val_iva,@val_ret,@val_ica,@por_des,@val_des,@val_tot,@valor,@cos_uni,@cos_tot,@por_iva,@por_ret,@subtotal,@por_ica,@por_riva,@val_riva,@fisico,@val_flet,@cos_usd,@tot_tot,@cos_unin,@cos_totn)";
                                    using (SqlCommand cmd_cu = new SqlCommand(query_cu, connection))
                                    {
                                        cmd_cu.Parameters.AddWithValue("@idregcab", newID);
                                        cmd_cu.Parameters.AddWithValue("@cod_trn", t_TrnNue.SelectedValue);
                                        cmd_cu.Parameters.AddWithValue("@num_trn", Tx_NumeroNue.Text);
                                        cmd_cu.Parameters.AddWithValue("@ano_doc", fecha.Year);
                                        cmd_cu.Parameters.AddWithValue("@per_doc", fecha.Month < 10 ? "0"+fecha.Month : fecha.Month.ToString());
                                        cmd_cu.Parameters.AddWithValue("@cod_sub", item["cod_sub"].ToString());
                                        cmd_cu.Parameters.AddWithValue("@cod_tiva", item["cod_tiva"].ToString());
                                        cmd_cu.Parameters.AddWithValue("@cod_ref", item["cod_ref"].ToString());
                                        cmd_cu.Parameters.AddWithValue("@cod_bod", item["cod_bod"].ToString());
                                        decimal cantidad = Convert.ToDecimal(item["cantidad"] == DBNull.Value || decimal.TryParse(item["cantidad"].ToString(), out v) == false ? 0 : item["cantidad"]);
                                        cmd_cu.Parameters.AddWithValue("@cantidad", cantidad);

                                        decimal val_uni = Convert.ToDecimal(item["val_uni"] == DBNull.Value || decimal.TryParse(item["val_uni"].ToString(), out v) == false ? 0 : item["val_uni"]);
                                        cmd_cu.Parameters.AddWithValue("@val_uni", val_uni);

                                        decimal val_iva = Convert.ToDecimal(item["val_iva"] == DBNull.Value || decimal.TryParse(item["val_iva"].ToString(), out v) == false ? 0 : item["val_iva"]);
                                        cmd_cu.Parameters.AddWithValue("@val_iva", val_iva);

                                        decimal val_ret = Convert.ToDecimal(item["val_ret"] == DBNull.Value || decimal.TryParse(item["val_ret"].ToString(), out v) == false ? 0 : item["val_ret"]);
                                        cmd_cu.Parameters.AddWithValue("@val_ret", val_ret);

                                        decimal val_ica = Convert.ToDecimal(item["val_ica"] == DBNull.Value || decimal.TryParse(item["val_ica"].ToString(), out v) == false ? 0 : item["val_ica"]);
                                        cmd_cu.Parameters.AddWithValue("@val_ica", val_ica);

                                        decimal por_des = Convert.ToDecimal(item["por_des"] == DBNull.Value || decimal.TryParse(item["por_des"].ToString(), out v) == false ? 0 : item["por_des"]);
                                        cmd_cu.Parameters.AddWithValue("@por_des", por_des);

                                        decimal val_des = Convert.ToDecimal(item["val_des"] == DBNull.Value || decimal.TryParse(item["val_des"].ToString(), out v) == false ? 0 : item["val_des"]);
                                        cmd_cu.Parameters.AddWithValue("@val_des", val_des);

                                        decimal val_tot = Convert.ToDecimal(item["val_tot"] == DBNull.Value || decimal.TryParse(item["val_tot"].ToString(), out v) == false ? 0 : item["val_tot"]);
                                        cmd_cu.Parameters.AddWithValue("@val_tot", val_tot);

                                        decimal valor = Convert.ToDecimal(item["valor"] == DBNull.Value || decimal.TryParse(item["valor"].ToString(), out v) == false ? 0 : item["valor"]);
                                        cmd_cu.Parameters.AddWithValue("@valor", valor);

                                        decimal cos_uni = Convert.ToDecimal(item["cos_uni"] == DBNull.Value || decimal.TryParse(item["cos_uni"].ToString(), out v) == false ? 0 : item["cos_uni"]);
                                        cmd_cu.Parameters.AddWithValue("@cos_uni", cos_uni);

                                        decimal cos_tot = Convert.ToDecimal(item["cos_tot"] == DBNull.Value || decimal.TryParse(item["cos_tot"].ToString(), out v) == false ? 0 : item["cos_tot"]);
                                        cmd_cu.Parameters.AddWithValue("@cos_tot", cos_tot);

                                        decimal por_iva = Convert.ToDecimal(item["por_iva"] == DBNull.Value || decimal.TryParse(item["por_iva"].ToString(), out v) == false ? 0 : item["por_iva"]);
                                        cmd_cu.Parameters.AddWithValue("@por_iva", por_iva);

                                        decimal por_ret = Convert.ToDecimal(item["por_ret"] == DBNull.Value || decimal.TryParse(item["por_ret"].ToString(), out v) == false ? 0 : item["por_ret"]);
                                        cmd_cu.Parameters.AddWithValue("@por_ret", por_ret);

                                        decimal subtotal = Convert.ToDecimal(item["subtotal"] == DBNull.Value || decimal.TryParse(item["subtotal"].ToString(), out v) == false ? 0 : item["subtotal"]);
                                        cmd_cu.Parameters.AddWithValue("@subtotal", subtotal);

                                        decimal por_ica = Convert.ToDecimal(item["por_ica"] == DBNull.Value || decimal.TryParse(item["por_ica"].ToString(), out v) == false ? 0 : item["por_ica"]);
                                        cmd_cu.Parameters.AddWithValue("@por_ica", por_ica);

                                        decimal por_riva = Convert.ToDecimal(item["por_riva"] == DBNull.Value || decimal.TryParse(item["por_riva"].ToString(), out v) == false ? 0 : item["por_riva"]);
                                        cmd_cu.Parameters.AddWithValue("@por_riva", por_riva);

                                        decimal val_riva = Convert.ToDecimal(item["val_riva"] == DBNull.Value || decimal.TryParse(item["val_riva"].ToString(), out v) == false ? 0 : item["val_riva"]);
                                        cmd_cu.Parameters.AddWithValue("@val_riva", val_riva);

                                        decimal fisico = Convert.ToDecimal(item["fisico"] == DBNull.Value || decimal.TryParse(item["fisico"].ToString(), out v) == false ? 0 : item["fisico"]);
                                        cmd_cu.Parameters.AddWithValue("@fisico", fisico);

                                        decimal val_flet = Convert.ToDecimal(item["val_flet"] == DBNull.Value || decimal.TryParse(item["val_flet"].ToString(), out v) == false ? 0 : item["val_flet"]);
                                        cmd_cu.Parameters.AddWithValue("@val_flet", val_flet);

                                        decimal cos_usd = Convert.ToDecimal(item["cos_usd"] == DBNull.Value || decimal.TryParse(item["cos_usd"].ToString(), out v) == false ? 0 : item["cos_usd"]);
                                        cmd_cu.Parameters.AddWithValue("@cos_usd", cos_usd);

                                        decimal tot_tot = Convert.ToDecimal(item["tot_tot"] == DBNull.Value || decimal.TryParse(item["tot_tot"].ToString(), out v) == false ? 0 : item["tot_tot"]);
                                        cmd_cu.Parameters.AddWithValue("@tot_tot", tot_tot);

                                        decimal cos_unin = Convert.ToDecimal(item["cos_unin"] == DBNull.Value || decimal.TryParse(item["cos_unin"].ToString(), out v) == false ? 0 : item["cos_unin"]);
                                        cmd_cu.Parameters.AddWithValue("@cos_unin", cos_unin);

                                        decimal cos_totn = Convert.ToDecimal(item["cos_totn"] == DBNull.Value || decimal.TryParse(item["cos_totn"].ToString(), out v) == false ? 0 : item["cos_totn"]);
                                        cmd_cu.Parameters.AddWithValue("@cos_totn", cos_totn);

                                        cmd_cu.ExecuteScalar();
                                    }
                                }

                                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, SiaWin._BusinessId, -9, -1, -9, "GENERO COPIA DE DOCUMENTOS DE:" + t_TrnCop.SelectedValue + "-" + Tx_Numero.Text+ " / DOCUMENTO NUEVO : "+ t_TrnNue.SelectedValue + "-"+Tx_NumeroNue.Text+"   ", "");
                                SiaWin.TabTrn(0, idemp, true, newID, 2, WinModal: true);
                                MessageBox.Show("copia de documento exitosa", "procesos exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                                clean();
                            }
                        }
                    }


                }
                else
                {
                    MessageBox.Show("el documento ingresado:" + Tx_Numero.Text.Trim() + " no tiene cuerpo consulte con el administrador del sistema", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al procesar:" + w);
            }
        }


        public void clean()
        {
            t_TrnCop.SelectedIndex = -1;
            Tx_Numero.Text = "";

            t_TrnNue.SelectedIndex = -1;
            Tx_NumeroNue.Text = "";
            Tx_DescNue.Text = "";
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int idr = 0; string code = ""; string nombre = "";
                dynamic xx = SiaWin.WindowBuscar(tabla_cab, "cod_trn", "num_trn", "cod_trn", "idreg", "Documentos", cnEmp, false, "", idEmp: idemp);
                xx.ShowInTaskbar = false;
                xx.Owner = Application.Current.MainWindow;
                xx.Height = 400;
                xx.Width = 400;
                xx.ShowDialog();
                idr = xx.IdRowReturn;
                code = xx.Codigo;
                nombre = xx.Nombre;
                xx = null;
                if (idr > 0)
                {
                    Tx_Numero.Text = nombre;
                    selectedTrn(code);
                }
                if (string.IsNullOrEmpty(code)) e.Handled = false;
                if (string.IsNullOrEmpty(code)) return;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al buscar la transaccion:" + w);
            }
        }


        public void selectedTrn(string code)
        {
            string query = "select * from " + tabla_trn + " where cod_trn='" + code + "' ";
            DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);
            if (dt.Rows.Count > 0)
            {
                int i = 0;
                foreach (DataRow item in trn.Rows)
                {
                    if (item["cod_trn"].ToString().Trim() == code.Trim()) t_TrnCop.SelectedIndex = i;
                    i++;
                }
            }

        }

        private void Tx_Numero_LostFocus(object sender, RoutedEventArgs e)
        {
            string document = (sender as TextBox).Text.Trim();
            string tipo = (sender as TextBox).Tag.ToString();

            if (string.IsNullOrEmpty(document)) return;
            if (tipo == "doc_viejo")
            {

                if (t_TrnCop.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el tipo de transaccion", "alerta", MessageBoxButton.OK, MessageBoxImage.Stop);
                    (sender as TextBox).Text = "";
                    return;
                }

                DataTable dt = SiaWin.Func.SqlDT("select * from " + tabla_cab + " where num_trn='" + document + "' and cod_trn='" + t_TrnCop.SelectedValue + "' ", "table", idemp);
                if (dt.Rows.Count > 0)
                {
                    selectedTrn(dt.Rows[0]["cod_trn"].ToString());
                    (sender as TextBox).Foreground = Brushes.Black;
                }
                else
                {
                    MessageBox.Show("el documento ingresado no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                    (sender as TextBox).Foreground = Brushes.Red;
                    return;
                }
            }
        }

        private void BtnDoc_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                DataTable dt = SiaWin.Func.SqlDT("select * from " + tabla_cab + " where num_trn='" + Tx_Numero.Text + "' and cod_trn='" + t_TrnCop.SelectedValue + "' ", "table", idemp);
                if (dt.Rows.Count > 0)
                {
                    int idreg = Convert.ToInt32(dt.Rows[0]["idreg"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, 2, WinModal: true);
                }
                else
                {
                    MessageBox.Show("documento no existe", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al visualisar doc:" + w);
            }
        }



    }
}


