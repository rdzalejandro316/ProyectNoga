using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IO;
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
    //    Sia.PublicarPnt(9593,"CreacionMasivaReferencias");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9593,"CreacionMasivaReferencias");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class CreacionMasivaReferencias : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        DataTable dt = new DataTable();
        DataTable dt_errores = new DataTable();
        
        public CreacionMasivaReferencias()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            LoadConfig();
            dt_errores.Columns.Add("fila", typeof(int));
            dt_errores.Columns.Add("error");
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
                this.Title = "Creacion Masiva de referencias " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".xlsx";
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.Title = "Guardar Plantilla como...";
                saveFileDialog.ShowDialog();
                string ruta = saveFileDialog.FileName;

                if (string.IsNullOrEmpty(ruta)) return;

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2010;

                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];


                    worksheet.IsGridLinesVisible = true;

                    worksheet.Range["A1"].Text = "COD_REF";
                    worksheet.Range["B1"].Text = "NOM_REF";
                    worksheet.Range["C1"].Text = "COD_TIP";
                    worksheet.Range["D1"].Text = "COD_PRV";                    
                    worksheet.Range["E1"].Text = "VRUNC";
                    worksheet.Range["F1"].Text = "VAL_REF";                    
                    worksheet.Range["G1"].Text = "COD_GRU";                    
                    worksheet.Range["H1"].Text = "COD_ANT";
                    worksheet.Range["I1"].Text = "VR_INTEM";
                    worksheet.Range["J1"].Text = "VAL_REF2";
                    worksheet.Range["K1"].Text = "COD_SGR";
                    worksheet.Range["A1:K1"].CellStyle.Font.Bold = true;

                    if (string.IsNullOrEmpty(ruta))
                        MessageBox.Show("Por favor, seleccione una ruta para guardar la plantilla");
                    else
                    {
                        workbook.SaveAs(ruta);
                        MessageBox.Show("Documento Guardado");
                    }

                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }

        public bool validarArchioExcel(DataTable dt)
        {
            bool flag = true;
            if (dt.Columns.Contains("Cod_ref") == false || dt.Columns.IndexOf("Cod_ref") != 0) flag = false;
            if (dt.Columns.Contains("Nom_ref") == false || dt.Columns.IndexOf("Nom_ref") != 1) flag = false;
            if (dt.Columns.Contains("Cod_tip") == false || dt.Columns.IndexOf("Cod_tip") != 2) flag = false;
            if (dt.Columns.Contains("Cod_prv") == false || dt.Columns.IndexOf("Cod_prv") != 3) flag = false;
            if (dt.Columns.Contains("Vrunc") == false || dt.Columns.IndexOf("Vrunc") != 4) flag = false;
            if (dt.Columns.Contains("Val_ref") == false || dt.Columns.IndexOf("Val_ref") != 5) flag = false;
            if (dt.Columns.Contains("Cod_gru") == false || dt.Columns.IndexOf("Cod_gru") != 6) flag = false;
            if (dt.Columns.Contains("Cod_ant") == false || dt.Columns.IndexOf("Cod_ant") != 7) flag = false;
            if (dt.Columns.Contains("Vr_intem") == false || dt.Columns.IndexOf("Vr_intem") != 8) flag = false;
            if (dt.Columns.Contains("Val_ref2") == false || dt.Columns.IndexOf("Val_ref2") != 9) flag = false;
            if (dt.Columns.Contains("Cod_sgr") == false || dt.Columns.IndexOf("Cod_sgr") != 10) flag = false;
            return flag;
        }

        public DataTable Limpiar(DataTable dt)
        {
            DataTable dt1 = dt.Clone(); //copy the structure 
            for (int i = 0; i <= dt.Rows.Count - 1; i++) //iterate through the rows of the source
            {
                System.Data.DataRow currentRow = dt.Rows[i];  //copy the current row 
                foreach (var colValue in currentRow.ItemArray)//move along the columns 
                {
                    if (!string.IsNullOrEmpty(colValue.ToString())) // if there is a value in a column, copy the row and finish
                    {
                        dt1.ImportRow(currentRow);
                        break; //break and get a new row                        
                    }
                }
            }
            return dt1;
        }

        public static System.Data.DataTable ConvertExcelToDataTable(string FileName)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2013;
                IWorkbook workbook = application.Workbooks.Open(FileName);
                IWorksheet worksheet = workbook.Worksheets[0];
                System.Data.DataTable customersTable = worksheet.ExportDataTable(worksheet.UsedRange, ExcelExportDataTableOptions.ColumnNames);
                return customersTable;
            }
        }

        private async void BtnImportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openfile = new OpenFileDialog();
                openfile.DefaultExt = ".xlsx";
                openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                var browsefile = openfile.ShowDialog();
                string root = openfile.FileName;

                if (string.IsNullOrEmpty(root)) return;

                sfBusyIndicator.IsBusy = true;
                dt.Clear(); dt_errores.Clear();
                dt = ConvertExcelToDataTable(root);
                dt = Limpiar(dt);

                //valida si la plantilla esta bien 
                if (validarArchioExcel(dt) == false)
                {
                    MessageBox.Show("La plantilla importada no corresponde a la que permite el sistema por favor verifique con la plantilla que genera esta pantalla", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                    sfBusyIndicator.IsBusy = false;
                    return;
                }

                if (dt.Rows.Count <= 0)
                {
                    MessageBox.Show("La plantilla importada no contiene ningun dato para importar", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                    sfBusyIndicator.IsBusy = false;
                    return;
                }

                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                var slowTask = Task<DataTable>.Factory.StartNew(() => Process(dt), source.Token);
                await slowTask;

                if (((DataTable)slowTask.Result).Rows.Count > 0)
                {
                    dataGridRefe.ItemsSource = ((DataTable)slowTask.Result).DefaultView;
                }

                Tx_totalCrea.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();
                Tx_totalErro.Text = dt_errores.Rows.Count.ToString();
                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("error  al importar:" + w);
            }
        }


        private DataTable Process(DataTable dt)
        {
            try
            {                                
                int i = 1;
                foreach (DataRow dr in dt.Rows)
                {

                    #region linea                     
                    string cod_tip = dr["COD_TIP"].ToString().Trim();
                    DataTable dt_tip = SiaWin.Func.SqlDT("select * from inmae_tip where cod_tip='" + cod_tip+ "'  ", "linea", idemp);
                    if (dt_tip.Rows.Count <= 0) { DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "la linea:'" + cod_tip+ "' no existe"; dt_errores.Rows.Add(row); }
                    #endregion

                    #region provedor
                    string cod_prv = dr["COD_PRV"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cod_prv))
                    {
                        DataTable dt_prv = SiaWin.Func.SqlDT("select * from inmae_prv where cod_prv='" + cod_prv + "'  ", "provedor", idemp);
                        if (dt_prv.Rows.Count <= 0) { DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "el codigo de provedor:'" + cod_prv + "' no existe"; dt_errores.Rows.Add(row); }
                    }                    
                    #endregion

                    #region grupo
                    string cod_gru = dr["COD_GRU"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cod_gru))
                    {
                        DataTable dt_gru = SiaWin.Func.SqlDT("select * from InMae_gru where cod_gru='" + cod_gru + "'  ", "grupo", idemp);
                        if (dt_gru.Rows.Count <= 0) { DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "el codigo de grupo:'" + cod_gru + "' no existe"; dt_errores.Rows.Add(row); }
                    }
                    
                    #endregion

                    #region sub grupo
                    string cod_sgr = dr["COD_SGR"].ToString().Trim();
                    if (!string.IsNullOrEmpty(cod_sgr))
                    {
                        DataTable dt_sgr = SiaWin.Func.SqlDT("select * from InMae_sgr where Cod_sgr='" + cod_sgr + "'  ", "subgrupo", idemp);
                        if (dt_sgr.Rows.Count <= 0) { DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "el codigo de sub grupo:'" + cod_sgr + "' no existe"; dt_errores.Rows.Add(row); }
                    }

                    #endregion


                    decimal output;

                    #region VRUNC                      
                    if (dr["VRUNC"] == DBNull.Value || decimal.TryParse(dr["VRUNC"].ToString(), out output) == false)
                    {
                        DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "el campo VRUNC :'" + dr["VRUNC"] + "' tiene que ser numerico"; dt_errores.Rows.Add(row);
                    }
                    #endregion

                    #region VAL_REF
                    if (dr["VAL_REF"] == DBNull.Value || decimal.TryParse(dr["VAL_REF"].ToString(), out output) == false)
                    {
                        DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "el campo VAL_REF:'" + dr["VAL_REF"] + "' tiene que ser numerico"; dt_errores.Rows.Add(row);
                    }
                    #endregion

                    #region VR_INTEM
                    if (dr["VR_INTEM"] == DBNull.Value || decimal.TryParse(dr["VR_INTEM"].ToString(), out output) == false)
                    {
                        DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "el campo VR_INTEM:'" + dr["VR_INTEM"] + "' tiene que ser numerico"; dt_errores.Rows.Add(row);
                    }
                    #endregion

                    #region VAL_REF2
                    if (dr["VAL_REF2"] == DBNull.Value || decimal.TryParse(dr["VAL_REF2"].ToString(), out output) == false)
                    {
                        DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "el campo VAL_REF2:'" + dr["VAL_REF2"] + "' tiene que ser numerico"; dt_errores.Rows.Add(row);
                    }
                    #endregion

                    //validacion si existe en la maestra
                    string cod_ref = dr["COD_REF"].ToString().Trim();
                    DataTable dt_ref = SiaWin.Func.SqlDT("select * from inmae_ref where cod_ref = '" + cod_ref + "'", "referencia", idemp);                    
                    if (dt_ref.Rows.Count > 0)
                    {
                        DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "la referencia :'" + dr["COD_REF"] + "' ya existe"; dt_errores.Rows.Add(row);
                    }

                    i++;
                }
                return dt;
            }
            catch (Exception e)
            {
                MessageBox.Show("en la consulta:" + e.Message);
                return null;
            }
        }


        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {

            if (dataGridRefe.ItemsSource == null || dataGridRefe.View.Records.Count <= 0)
            {
                MessageBox.Show("la grilla de importacion esta vacia importe su plantilla para poder crear referencias");
                return;
            } 
            


            try
            {
                string query = "";


                foreach (DataRow item in dt.Rows)
                {
                    int error = dt_errores.Rows.Count;

                    if (error > 0)
                    {
                        MessageBox.Show("la importacion contiene errores debe de estar todo correcto", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    };

                    if (error == 0)
                    {                  
                        string cod_ref = item["COD_REF"].ToString().Trim(); 
                        string nom_ref = item["NOM_REF"].ToString().Trim();
                        string cod_tip = item["COD_TIP"].ToString().Trim();
                        string cod_prv = item["COD_PRV"].ToString().Trim();
                        decimal vrunc = Convert.ToDecimal(item["VRUNC"]);
                        decimal val_ref = Convert.ToDecimal(item["VAL_REF"]);
                        string cod_gru = item["COD_GRU"].ToString().Trim();                                                
                        string cod_ant = item["COD_ANT"].ToString().Trim();                                                
                        decimal vr_intem = Convert.ToDecimal(item["VR_INTEM"]);
                        decimal val_ref2 = Convert.ToDecimal(item["VAL_REF2"]);
                        string cod_sgr = item["COD_SGR"].ToString().Trim();

                        //string val_ref2 = item.Val_ref2.ToString("F", CultureInfo.InvariantCulture);

                        query += "insert into inmae_ref (cod_ref,nom_ref,cod_tip,cod_prv,cos_usd,Vrunc,val_ref,pos_ara,cod_gru,Cod_mar,cod_ant,vr_intem,val_ref2,cod_sgr,tipo_prv,cod_tiva,tip_ref,cod_med,ind_iva,estado,fec_crea) " +
                                                   "values ('" + cod_ref + "','" + nom_ref + "','" + cod_tip + "','" + cod_prv + "',0," + vrunc.ToString("F", CultureInfo.InvariantCulture) + "," + val_ref.ToString("F", CultureInfo.InvariantCulture) + ",'','" + cod_gru + "'" +
                                                   ",'' ,'" + cod_ant + "', " + vr_intem.ToString("F", CultureInfo.InvariantCulture) + "," + val_ref2.ToString("F", CultureInfo.InvariantCulture) + ",'" + cod_sgr + "',1,'C','3','UNI'" +
                                                   ",'1','1','" + DateTime.Now.ToString("dd/MM/yyyy").ToString() + "');";


                    }
                }



                if (MessageBox.Show("usted desea subir las referencias importadas a la maestra de referencias", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {

                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        MessageBox.Show("el proceso se ejecuto exitosamente");
                    }
                    else
                    {
                        MessageBox.Show("fallo el proceso por favor verifique los campos");
                    }

                    dt.Clear();
                    dt_errores.Clear();
                    dataGridRefe.ItemsSource = null;
                    Tx_totalCrea.Text = "0";
                    Tx_totalErro.Text = "0";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL EJECUTAR EL PROCESO:" + w);
            }
        }

        private void BtnExpError_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SiaWin.Browse(dt_errores);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir la lista de errores:" + w);
            }
        }






    }
    
}
