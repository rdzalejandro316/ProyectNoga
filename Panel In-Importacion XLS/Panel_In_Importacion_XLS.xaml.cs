using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.XlsIO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9500,"Panel_In_Importacion_XLS");    
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9500, "Panel_In_Importacion_XLS");
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog(); 

    //https://www.microsoft.com/es-es/download/details.aspx?id=23734
    public class Referencias : IDataErrorInfo
    {

        public string Cod_ref { get; set; }
        public string Cod_bod { get; set; }
        public string Nom_ref { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Cos_unit { get; set; }
        public decimal Cos_Tot { get; set; }
        public string Cod_tiva { get; set; }
        public decimal Por_iva { get; set; }

        [Display(AutoGenerateField = false)]
        public string Error { get; set; }

        public string this[string columnName]
        {
            get
            {
                Panel_In_Importacion_XLS principal = new Panel_In_Importacion_XLS();

                if (columnName == "Cod_ref")
                {
                    var tpl = principal.GetTableValRef(Cod_ref, "cod_ref");

                    if (tpl.Item1 == false)
                    {
                        Error = "la referencia no existe: " + this.Cod_ref;
                        return "la referencia no existe: " + this.Cod_ref;
                    }
                    else
                    {
                        //Nom_ref = tpl.Item2;
                        Nom_ref = "---";
                    }
                }

                if (columnName == "Cod_bod")
                {
                    if (principal.GetTableVal(Cod_bod, "cod_bod") == false)
                    {
                        Error = "la bodega no existe: " + this.Cod_bod;
                        return "la bodega no existe: " + this.Cod_bod;
                    }
                }

                return string.Empty;
            }
        }

        public Referencias(string cod_ref, string nom_ref,string cod_bod, decimal cant, decimal cos_unit, decimal cos_tot,string cod_tiva, decimal por_iva)
        {
            Cod_ref = cod_ref;
            Nom_ref = nom_ref;
            Cod_bod = cod_bod;
            Cantidad = cant;
            Cos_unit = cos_unit;
            Cos_Tot = cos_tot;
            Cod_tiva = cod_tiva;
            Por_iva = por_iva;
        }


    }



    public partial class Panel_In_Importacion_XLS : System.Windows.Window
    {

        private ObservableCollection<Referencias> _Refere;
        public ObservableCollection<Referencias> Refere
        {
            get { return _Refere; }
            set { _Refere = value; }
        }

        public System.Data.DataTable tablaXLS = new System.Data.DataTable();

        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public Boolean bandera = false;

        public Panel_In_Importacion_XLS()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
            loadColumns();
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
                this.Title = "Importar XLS" + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void loadColumns()
        {
            tablaXLS.Columns.Add("cod_ref");
            tablaXLS.Columns.Add("nom_ref");
            tablaXLS.Columns.Add("cod_bod");
            tablaXLS.Columns.Add("cantidad");
            tablaXLS.Columns.Add("cos_uni");
            tablaXLS.Columns.Add("cos_tot");
            tablaXLS.Columns.Add("cod_tiva");
            tablaXLS.Columns.Add("por_iva");
        }

        public bool GetTableVal(string valor, string column)
        {
            bool flag = false;
            var valores = ditribucion(column);
            string select = "select " + valores.Item2 + "  from  " + valores.Item1 + " where  " + valores.Item2 + "='" + valor.Trim() + "'  ";
            System.Data.DataTable dt = SiaWin.Func.SqlDT(select, "tabla", idemp);
            flag = dt.Rows.Count > 0 ? true : false;

            return flag;
        }

        public Tuple<bool, string> GetTableValRef(string valor, string column)
        {
            bool flag = false;
            var valores = ditribucion(column);
            string select = "select " + valores.Item2 + "  from  " + valores.Item1 + " where  " + valores.Item2 + "='" + valor.Trim() + "'  ";
            System.Data.DataTable dt = SiaWin.Func.SqlDT(select, "tabla", idemp);
            flag = dt.Rows.Count > 0 ? true : false;
            return new Tuple<bool, string>(flag, dt.Rows.Count > 0 ? dt.Rows[0]["nom_ref"].ToString() : "-");
        }

        public Tuple<string, string> ditribucion(string column)
        {
            string tabla = ""; string campo = "";
            switch (column)
            {
                case "cod_ref":
                    tabla = "inmae_ref"; campo = "cod_ref";
                    break;
                case "cod_bod":
                    tabla = "inmae_bod"; campo = "cod_bod";
                    break;
            }
            var tuple = new Tuple<string, string>(tabla, campo);
            return tuple;
        }

        private void Generar_Click(object sender, RoutedEventArgs e)
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
                    //worksheet.Range["B1"].Text = "NOM_REF";
                    worksheet.Range["B1"].Text = "COD_BOD";
                    worksheet.Range["C1"].Text = "CANTIDAD";
                    worksheet.Range["D1"].Text = "COS_UNI";
                    worksheet.Range["E1"].Text = "COS_TOT";
                    worksheet.Range["A1:E1"].CellStyle.Font.Bold = true;

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

        private void Button_Click(object sender, RoutedEventArgs e)
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

        public void impotar()
        {

            _Refere = new ObservableCollection<Referencias>();

            OpenFileDialog openfile = new OpenFileDialog();
            openfile.DefaultExt = ".xlsx";
            openfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            var browsefile = openfile.ShowDialog();
            string root = openfile.FileName;

            if (string.IsNullOrEmpty(root)) return;

            DataTable dt = ConvertExcelToDataTable(root);

            if (validarArchioExcel(dt) == false)
            {
                MessageBox.Show("La plantilla importada no corresponde a la que permite el sistema por favor verifique con la plantilla que genera esta pantalla", "alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }



            decimal n;
            foreach (System.Data.DataRow row in dt.Rows)
            {
                var referencia = ReturnName(row[0].ToString().Trim());

                _Refere.Add(new Referencias(
                    row[0].ToString(),
                    referencia.Item1,
                    row[1].ToString(),                    
                    Convert.ToDecimal(row[2] == DBNull.Value || decimal.TryParse(row[2].ToString(), out n) == false ? 0 : row[2]),
                    Convert.ToDecimal(row[3] == DBNull.Value || decimal.TryParse(row[3].ToString(), out n) == false ? 0 : row[3]),
                    Convert.ToDecimal(row[4] == DBNull.Value || decimal.TryParse(row[4].ToString(), out n) == false ? 0 : row[4]),
                    referencia.Item2,
                    referencia.Item3
                    ));
            }

            dtGrid.ItemsSource = Refere;
            TotalReg.Text = dt.Rows.Count.ToString();
        }

        public bool validarArchioExcel(DataTable dt)
        {
            bool flag = true;
            if (dt.Columns.Contains("Cod_ref") == false || dt.Columns.IndexOf("Cod_ref") != 0) flag = false;
            //if (dt.Columns.Contains("Nom_ref") == false || dt.Columns.IndexOf("Nom_ref") != 1) flag = false;
            if (dt.Columns.Contains("Cod_bod") == false || dt.Columns.IndexOf("Cod_bod") != 1) flag = false;
            if (dt.Columns.Contains("Cantidad") == false || dt.Columns.IndexOf("Cantidad") != 2) flag = false;
            if (dt.Columns.Contains("Cos_uni") == false || dt.Columns.IndexOf("Cos_uni") != 3) flag = false;
            if (dt.Columns.Contains("Cos_Tot") == false || dt.Columns.IndexOf("Cos_Tot") != 4) flag = false;
            return flag;
        }

        public Tuple<string,string,decimal> ReturnName(string code)
        {
            string query = "select InMae_ref.nom_ref,InMae_ref.cod_tiva,InMae_tiva.por_iva from InMae_ref  ";
            query += "inner join InMae_tiva on InMae_tiva.cod_tiva = InMae_ref.cod_tiva  ";
            query += "where cod_ref='" + code + "' ";
            System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);
            //return dt.Rows.Count > 0 ? dt.Rows[0]["nom_ref"].ToString() : "-";

            return new Tuple<string, string, decimal>
                (
                    dt.Rows.Count > 0 ? dt.Rows[0]["nom_ref"].ToString() : "-",
                    dt.Rows.Count > 0 ? dt.Rows[0]["cod_tiva"].ToString() : "-",
                    dt.Rows.Count > 0 ? Convert.ToDecimal(dt.Rows[0]["por_iva"]) : 0
                ); 
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

        private void BTNvalidar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in Refere)
                {
                    System.Data.DataRow row = tablaXLS.NewRow();
                    row["cod_ref"] = item.Cod_ref;                    
                    row["cod_bod"] = item.Cod_bod;
                    row["nom_ref"] = item.Nom_ref;
                    row["cantidad"] = item.Cantidad;
                    row["cos_uni"] = item.Cos_unit;
                    row["cos_tot"] = item.Cos_Tot;
                    row["cod_tiva"] = item.Cod_tiva;
                    row["por_iva"] = item.Por_iva;
                    tablaXLS.Rows.Add(row);
                }


                //SiaWin.Browse(tablaXLS);
                if (tablaXLS.Rows.Count > 0)
                {
                    bandera = true;
                    this.Close();
                }
                          


                //tablaXLS = Refere;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al validar:"+w);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }





    }
}