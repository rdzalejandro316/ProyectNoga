using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace ContabilidadTablasExpExcel
{
    
    public partial class genericoDocument : UserControl
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        string tipo = "";

        public genericoDocument(int idEmpresa,string TipoD)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = idEmpresa;
            tipo = TipoD;
            LoadConfig();
        }

        private void LoadConfig()
        {
            try
            {
                DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();

                fec_ini.Text = DateTime.Now.AddMonths(-1).ToString();
                fec_fin.Text = DateTime.Now.ToString();

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public string armarWhere()
        {
            string where = " where cab.fec_trn between '"+fec_ini.Text+ "' and  '" + fec_fin.Text + " 23:59:59' ";
            if (!string.IsNullOrEmpty(tx_transacion.Text)) where += " and  cab.cod_trn='"+tx_transacion.Text+"' ";
            return where;
        }

        private async void BTNconsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                sfBusyIndicator.IsBusy = true;

                string where = armarWhere();
                //MessageBox.Show(where);

                var slowTask = Task<DataSet>.Factory.StartNew(() => CargarConsulta(tipo, where, cod_empresa, source.Token), source.Token);
                await slowTask;

                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGrid.ItemsSource = ((DataSet)slowTask.Result).Tables[0].DefaultView;
                    Txreg.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                }
                else
                {
                    MessageBox.Show("sin registros");
                    dataGrid.ItemsSource = null;
                    Txreg.Text = "0";
                }
                sfBusyIndicator.IsBusy = false;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:" + w);
            }
        }

        public DataSet CargarConsulta(string tipo, string where, string empresa, CancellationToken cancellationToken)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                SqlDataAdapter da = new SqlDataAdapter();
                cmd = new SqlCommand("_EmpWindowsExportar", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@where", where);
                cmd.Parameters.AddWithValue("@codemp", empresa);
                da = new SqlDataAdapter(cmd);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                con.Close();
            }
            catch (Exception w)
            {
                MessageBox.Show("erro en la consulta" + w);
            }
            return ds;
        }


        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGrid.ExportToExcel(dataGrid.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

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
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al exportar");
            }
        }

        private void BtnPdf_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //var options = new PdfExportingOptions();
                //options.FitAllColumnsInOnePage = true;
                //options.ExcludeColumns.Add("cod_ant");
                //options.ExcludeColumns.Add("nom_ter");
                //options.ExcludeColumns.Add("nom_mer");
                //options.ExcludeColumns.Add("nom_tip");
                //var document = dataGrid.ExportToPdf();
                //document.PageSettings.Orientation = PdfPageOrientation.Landscape;                

                //SaveFileDialog sfd = new SaveFileDialog
                //{
                //    Filter = "PDF Files(*.pdf)|*.pdf"
                //};

                //if (sfd.ShowDialog() == true)
                //{
                //    using (Stream stream = sfd.OpenFile())
                //    {
                //        document.Save(stream);
                //    }
                //    if (MessageBox.Show("Do you want to view the Pdf file?", "Pdf file has been created",MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                //    {

                //        System.Diagnostics.Process.Start(sfd.FileName);
                //    }
                //}
            }
            catch (Exception w)
            {
                MessageBox.Show("errror al exportar a pdf" + w);
            }
        }





    }
}
