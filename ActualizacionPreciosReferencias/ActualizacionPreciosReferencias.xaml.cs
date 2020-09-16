using Microsoft.Win32;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
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
    //    Sia.PublicarPnt(9594,"ActualizacionPreciosReferencias");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9594,"ActualizacionPreciosReferencias");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

     
    public partial class ActualizacionPreciosReferencias : Window
    {

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        DataTable dt = new DataTable();
        DataTable dt_errores = new DataTable();

        public ActualizacionPreciosReferencias()
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
                this.Title = "Actualizacion de precios masiva Masiva de referencias - " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }
        
        private void BtnImportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                impotar();
            }
            catch (Exception w)
            {
                MessageBox.Show("error  al importar:" + w);
            }
        }

        public bool validarArchioExcel(DataTable dt)
        {
            bool flag = true;
            if (dt.Columns.Contains("Cod_ref") == false || dt.Columns.IndexOf("Cod_ref") != 0) flag = false;
            if (dt.Columns.Contains("Cos_usd") == false || dt.Columns.IndexOf("Cos_usd") != 1) flag = false;
            if (dt.Columns.Contains("Vrunc") == false || dt.Columns.IndexOf("Vrunc") != 2) flag = false;
            if (dt.Columns.Contains("Val_ref") == false || dt.Columns.IndexOf("Val_ref") != 3) flag = false;
            if (dt.Columns.Contains("Vr_intem") == false || dt.Columns.IndexOf("Vr_intem") != 4) flag = false;
            if (dt.Columns.Contains("Val_ref2") == false || dt.Columns.IndexOf("Val_ref2") != 5) flag = false;
            if (dt.Columns.Contains("Estado") == false || dt.Columns.IndexOf("Estado") != 6) flag = false;
            return flag;
        }

        public async void impotar()
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

            if (validarArchioExcel(dt) == false)
            {
                MessageBox.Show("La plantilla importada no corresponde a la que permite el sistema por favor verifique con la plantilla que genera esta pantalla", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
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
            sfBusyIndicator.IsBusy = false;
            Tx_total_err.Text = dt_errores.Rows.Count.ToString();
            Tx_total.Text = ((DataTable)slowTask.Result).Rows.Count.ToString();

        }

        private DataTable Process(DataTable dt)
        {
            try
            {                
                dt.Columns.Add("COD_ANT");             
                dt.Columns.Add("COS_USD_REF", typeof(decimal));
                dt.Columns.Add("VRUNC_REF", typeof(decimal));
                dt.Columns.Add("VAL_REF_REF", typeof(decimal));
                dt.Columns.Add("VR_INTEM_REF", typeof(decimal));
                dt.Columns.Add("VAL_REF2_REF", typeof(decimal));

                int i = 1;
                decimal val; int esta;
                foreach (DataRow dr in dt.Rows)
                {
                    // referencia --------------------------
                    string cod_bod = dr["Cod_ref"].ToString();
                    DataTable dt_ref = SiaWin.Func.SqlDT("select * from inmae_ref where cod_ref='" + cod_bod + "'  ", "referencia", idemp);
                    if (dt_ref.Rows.Count > 0)
                    {
                        dr["COD_REF"] = dt_ref.Rows[0]["cod_ref"].ToString();
                        dr["COD_ANT"] = dt_ref.Rows[0]["cod_ant"].ToString();

                        //valores anteriores
                        dr["COS_USD_REF"] = Convert.ToDecimal(dt_ref.Rows[0]["COS_USD"] == DBNull.Value || decimal.TryParse(dt_ref.Rows[0]["COS_USD"].ToString(), out val) == false ? 0 : dt_ref.Rows[0]["COS_USD"]);
                        dr["VRUNC_REF"] = Convert.ToDecimal(dt_ref.Rows[0]["VRUNC"] == DBNull.Value || decimal.TryParse(dt_ref.Rows[0]["VRUNC"].ToString(), out val) == false ? 0 : dt_ref.Rows[0]["VRUNC"]);
                        dr["VAL_REF_REF"] = Convert.ToDecimal(dt_ref.Rows[0]["VAL_REF"] == DBNull.Value || decimal.TryParse(dt_ref.Rows[0]["VAL_REF"].ToString(), out val) == false ? 0 : dt_ref.Rows[0]["VAL_REF"]);
                        dr["VR_INTEM_REF"] = Convert.ToDecimal(dt_ref.Rows[0]["VR_INTEM"] == DBNull.Value || decimal.TryParse(dt_ref.Rows[0]["VR_INTEM"].ToString(), out val) == false ? 0 : dt_ref.Rows[0]["VR_INTEM"]);
                        dr["VAL_REF2_REF"] = Convert.ToDecimal(dt_ref.Rows[0]["VAL_REF2"] == DBNull.Value || decimal.TryParse(dt_ref.Rows[0]["VAL_REF2"].ToString(), out val) == false ? 0 : dt_ref.Rows[0]["VAL_REF2"]);


                        //valores que llegan y se validan si son numeros
                        dr["COS_USD"] = Convert.ToDecimal(dr["COS_USD"] == DBNull.Value || decimal.TryParse(dr["COS_USD"].ToString(), out val) == false ? 0 : dr["COS_USD"]);
                        dr["VRUNC"] = Convert.ToDecimal(dr["VRUNC"] == DBNull.Value || decimal.TryParse(dr["VRUNC"].ToString(), out val) == false ? 0 : dr["VRUNC"]);
                        dr["VAL_REF"] = Convert.ToDecimal(dr["VAL_REF"] == DBNull.Value || decimal.TryParse(dr["VAL_REF"].ToString(), out val) == false ? 0 : dr["VAL_REF"]);
                        dr["VR_INTEM"] = Convert.ToDecimal(dr["VR_INTEM"] == DBNull.Value || decimal.TryParse(dr["VR_INTEM"].ToString(), out val) == false ? 0 : dr["VR_INTEM"]);
                        dr["VAL_REF2"] = Convert.ToDecimal(dr["VAL_REF2"] == DBNull.Value || decimal.TryParse(dr["VAL_REF2"].ToString(), out val) == false ? 0 : dr["VAL_REF2"]);

                        dr["ESTADO"] = Convert.ToBoolean(dr["ESTADO"] == DBNull.Value || Int32.TryParse(dr["ESTADO"].ToString(), out esta) == false ? false : true);
                    } 
                    else { DataRow row = dt_errores.NewRow(); row["fila"] = i; row["error"] = "la referencia " + dr["Cod_ref"] + " no existe"; dt_errores.Rows.Add(row); }
                    
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
                    worksheet.Range["B1"].Text = "COS_USD";
                    worksheet.Range["C1"].Text = "VRUNC";
                    worksheet.Range["D1"].Text = "VAL_REF";
                    worksheet.Range["E1"].Text = "VR_INTEM";
                    worksheet.Range["F1"].Text = "VAL_REF2";
                    worksheet.Range["G1"].Text = "ESTADO";
                    worksheet.Range["A1:G1"].CellStyle.Font.Bold = true;

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

        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridRefe.ItemsSource == null) return;
            if (dataGridRefe.View.Records.Count <= 0) return;

            try
            {
                string query = "";

                foreach (DataRow item in dt.Rows)
                {
                    string cod_ref = string.IsNullOrEmpty(item["COD_REF"].ToString().Trim()) ? " " : item["COD_REF"].ToString().Trim();

                    int error = dt_errores.Rows.Count;

                    if (error > 0)
                    {
                        MessageBox.Show("la importacion contiene errores debe de estar todo correcto", "alerta", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    };

                    if (error==0)
                    {
                        decimal cos_usd = Convert.ToDecimal(item["COS_USD"]) == 0 ?
                            Convert.ToDecimal(item["COS_USD_REF"]) : Convert.ToDecimal(item["COS_USD"]);

                        decimal vrunc = Convert.ToDecimal(item["VRUNC"]) == 0 ?
                            Convert.ToDecimal(item["VRUNC_REF"]) : Convert.ToDecimal(item["VRUNC"]);

                        decimal val_ref = Convert.ToDecimal(item["VAL_REF"]) == 0 ?
                            Convert.ToDecimal(item["VAL_REF_REF"]) : Convert.ToDecimal(item["VAL_REF"]);

                        decimal vr_intem = Convert.ToDecimal(item["VR_INTEM"]) == 0 ?
                            Convert.ToDecimal(item["VR_INTEM"]) : Convert.ToDecimal(item["VR_INTEM"]);

                        decimal val_ref2 = Convert.ToDecimal(item["VAL_REF2"]) == 0 ?
                            Convert.ToDecimal(item["VAL_REF2_REF"]) : Convert.ToDecimal(item["VAL_REF2"]);

                        int estado = Convert.ToBoolean(item["ESTADO"]) == true ? 1 : 0;    
                                                
                        query += "update inmae_ref set cos_usd="+ cos_usd.ToString("F", CultureInfo.InvariantCulture) + ",vrunc=" + vrunc.ToString("F", CultureInfo.InvariantCulture) + ",val_ref=" + val_ref.ToString("F", CultureInfo.InvariantCulture) + ",vr_intem=" + vr_intem.ToString("F", CultureInfo.InvariantCulture) + ",val_ref2=" + val_ref2.ToString("F", CultureInfo.InvariantCulture) + ",estado="+ estado + "  where cod_ref='" + cod_ref + "';";
                    }
                }



                if (MessageBox.Show("usted desea actualizar los precios de las referencias importadas", "Alerta", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {

                   // MessageBox.Show("query:"+ query);

                    if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                    {
                        MessageBox.Show("el proceso se ejecuto exitosamente");
                    }
                    else
                    {
                        MessageBox.Show("fallo el proceso por favor verifique los campos");
                    }
                    dataGridRefe.ItemsSource = null;
                    Tx_total.Text = "0";
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("ERROR AL EJECUTAR EL PROCESO:" + w);
            }

        }

        private void BtnErrores_Click(object sender, RoutedEventArgs e)
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
