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
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace SiasoftAppExt
{

    /// Sia.PublicarPnt(9667,"Co_BalanceAno");
    /// Sia.TabU(9667);
    /// 
    public partial class Co_BalanceAno : UserControl
    {

        dynamic SiaWin;
        dynamic tabitem;
        public int idemp = 0;
        string codemp = string.Empty;
        int moduloid = 0;
        string cnEmp = "";
        bool loaded = false;

        public Co_BalanceAno(dynamic tabitem1)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            tabitem = tabitem1;
            tabitem.MultiTab = true;
            if (tabitem.idemp > 0) idemp = tabitem.idemp;
            if (tabitem.idemp <= 0) idemp = SiaWin._BusinessId;
        }

        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                codemp = foundRow["BusinessCode"].ToString().Trim();
                tabitem.Logo(idLogo, ".png");
                tabitem.Title = "Balance Año(" + aliasemp + ")";
                TituloBalance.Text = "Empresa:" + codemp + "-" + foundRow["BusinessName"].ToString().Trim();
                DateTime fechatemp = DateTime.Today;
                fechatemp = new DateTime(fechatemp.Year, 1, 1);
                C1.Text = "1";
                C2.Text = "9";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (loaded == true) return;
            loaded = true;
            System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='CO'");
            if (drmodulo == null) this.IsEnabled = false;
            moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());
            LoadConfig();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.F8 || e.Key == Key.Tab || e.Key == Key.OemComma)
            {
                e.Handled = false;
            }
            else
            {
                MessageBox.Show("este campo solo admite valores numericos", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                e.Handled = true;
            }
            try
            {
                if (e.Key == System.Windows.Input.Key.F8)
                {
                    string idTab = ((TextBox)sender).Tag.ToString();
                    if (idTab.Length > 0)
                    {
                        string tag = ((TextBox)sender).Tag.ToString();
                        string cmptabla = ""; string cmpcodigo = ""; string cmpnombre = ""; string cmporden = ""; string cmpidrow = ""; string cmptitulo = ""; string cmpconexion = ""; bool mostrartodo = true; string cmpwhere = "";
                        if (string.IsNullOrEmpty(tag)) return;

                        //if (tag == "comae_cta1")
                        //{
                        cmptabla = "comae_cta";
                        cmpcodigo = "cod_cta";
                        cmpnombre = "UPPER(nom_cta)";
                        cmporden = "cod_cta";
                        cmpidrow = "cod_cta";
                        cmptitulo = "Maestra de Cuentas";
                        cmpconexion = cnEmp; mostrartodo = true;
                        cmpwhere = "";
                        int idr = 0; string code = ""; string nom = "";
                        dynamic winb = SiaWin.WindowBuscar(cmptabla, cmpcodigo, cmpnombre, cmporden, cmpidrow, cmptitulo, cnEmp, mostrartodo, cmpwhere, idEmp: idemp);
                        winb.ShowInTaskbar = false;
                        winb.Owner = Application.Current.MainWindow;
                        winb.ShowDialog();

                        idr = winb.IdRowReturn;
                        code = winb.Codigo;
                        nom = winb.Nombre;
                        winb = null;
                        if (idr > 0)
                        {
                            ((TextBox)sender).Text = code.Trim();
                            var uiElement = e.OriginalSource as UIElement;
                            uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        }
                        e.Handled = true;
                    }
                    if (e.Key == Key.Enter)
                    {
                        var uiElement = e.OriginalSource as UIElement;
                        uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Cuen_LostFocus(object sender, RoutedEventArgs e)
        {
            string tag = ((TextBox)sender).Tag.ToString();

            if (tag == "comae_cta1")
            {
                F8_1.Visibility = Visibility.Hidden;

            }
            if (tag == "comae_cta2")
            {
                F8_2.Visibility = Visibility.Hidden;
            }
        }

        private void Cuen_GotFocus(object sender, RoutedEventArgs e)
        {
            string tag = ((TextBox)sender).Tag.ToString();

            if (tag == "comae_cta1")
            {
                F8_1.Visibility = Visibility.Visible;
            }
            if (tag == "comae_cta2")
            {
                F8_2.Visibility = Visibility.Visible;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(C1.Text.Trim()))
                {
                    MessageBox.Show("Falta codigo de cuenta inicial..", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    C1.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(C2.Text.Trim()))
                {
                    MessageBox.Show("Falta codigo de cuenta final..", "Alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    C2.Focus();
                    return;
                }

                int __TipoBalNiif = TipoBalNiif.SelectedIndex;
                if (__TipoBalNiif < 0)
                {
                    MessageBox.Show("Seleccione un tipo de Balance Fiscal o Niif", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    TipoBalNiif.Focus();
                    return;
                }



                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;
                GridConfiguracion.IsEnabled = false;
                sfBusyIndicator.IsBusy = true;
                dataGridConsulta.ItemsSource = null;
                BtnEjecutar.IsEnabled = false;

                source.CancelAfter(TimeSpan.FromSeconds(1));

                tabitem.Progreso(true);
                DateTime fec = Convert.ToDateTime(Fec.Value.ToString());
                int ano = fec.Year;
                string c1 = C1.Text.Trim();
                string c2 = C2.Text.Trim();
                string _TipoBalNiif = TipoBalNiif.SelectedIndex.ToString();

                dataGridConsulta.ClearFilters();
                var slowTask = Task<DataSet>.Factory.StartNew(() => LoadData(ano, c1, c2, _TipoBalNiif, codemp, source.Token), source.Token);
                await slowTask;


                BtnEjecutar.IsEnabled = true;
                tabitem.Progreso(false);
                if (((DataSet)slowTask.Result).Tables[0].Rows.Count > 0)
                {
                    dataGridConsulta.ItemsSource = ((DataSet)slowTask.Result).Tables[0];
                    Total.Text = ((DataSet)slowTask.Result).Tables[0].Rows.Count.ToString();
                    TabControl1.SelectedIndex = 2;
                    TabControl1.SelectedIndex = 1;
                }
                this.sfBusyIndicator.IsBusy = false;
                GridConfiguracion.IsEnabled = true;
                this.dataGridConsulta.GridColumnSizer.ResetAutoCalculationforAllColumns();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Opacity = 1;
            }
        }

        private DataSet LoadData(int ano, string C1, string C2, string _TipoBalNiif, string codemp, CancellationToken cancellationToken)
        {
            try
            {
                SqlConnection con = new SqlConnection(SiaWin._cn);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                cmd = new SqlCommand("_EmpSpCoBalanceAno", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ano", ano);
                cmd.Parameters.AddWithValue("@ctaini", C1);
                cmd.Parameters.AddWithValue("@ctafin", C2);
                cmd.Parameters.AddWithValue("@balanceniif", _TipoBalNiif);
                cmd.Parameters.AddWithValue("@codEmp", codemp);
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


        private static void CellExportingHandler(object sender, GridCellExcelExportingEventArgs e)
        {
            try
            {
                e.Range.CellStyle.Font.Size = 10;
                e.Range.CellStyle.Font.FontName = "Segoe UI";
                if (e.ColumnName == "sal_ini" || e.ColumnName == "debitos" || e.ColumnName == "creditos" || e.ColumnName == "sal_fin" || e.ColumnName == "sal_00" || e.ColumnName == "deb_01" 
                    || e.ColumnName == "cre_01" || e.ColumnName == "sal_01" || e.ColumnName == "deb_02"
                    || e.ColumnName == "cre_02" || e.ColumnName == "sal_02" || e.ColumnName == "deb_03" || e.ColumnName == "cre_03" || e.ColumnName == "sal_03"
                    || e.ColumnName == "deb_04" || e.ColumnName == "cre_04" || e.ColumnName == "sal_04" || e.ColumnName == "deb_05" || e.ColumnName == "cre_05" 
                    || e.ColumnName == "sal_05" || e.ColumnName == "deb_06" || e.ColumnName == "cre_06" || e.ColumnName == "sal_06"
                    || e.ColumnName == "deb_07" || e.ColumnName == "cre_07" || e.ColumnName == "sal_07" || e.ColumnName == "deb_08" || e.ColumnName == "cre_08" || e.ColumnName == "sal_08"
                    || e.ColumnName == "deb_09" || e.ColumnName == "cre_09" || e.ColumnName == "sal_09" || e.ColumnName == "deb_10" || e.ColumnName == "cre_10" || e.ColumnName == "sal_10"
                    || e.ColumnName == "deb_11" || e.ColumnName == "cre_11" || e.ColumnName == "sal_11" || e.ColumnName == "deb_12" || e.ColumnName == "cre_12" || e.ColumnName == "sal_12" || e.ColumnName == "deb_13" || e.ColumnName == "cre_13" || e.ColumnName == "sal_13"

                    )
                {
                    double value = 0;
                    if (double.TryParse(e.CellValue.ToString(), out value))
                    {
                        e.Range.Number = value;
                    }
                    e.Handled = true;
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:"+w);
            }
        }


        private void BTNexpo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                //options.ExportMode = ExportMode.Value;
                options.ExcelVersion = ExcelVersion.Excel2013;
                options.CellsExportingEventHandler = CellExportingHandler;
                var excelEngine = dataGridConsulta.ExportToExcel(dataGridConsulta.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

                //workBook.ActiveSheet.Columns[4].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[5].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[6].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[7].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[8].NumberFormat = "0.0";                
                //workBook.ActiveSheet.Columns[9].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[10].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[11].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[12].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[13].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[14].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[15].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[16].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[17].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[18].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[19].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[20].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[21].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[22].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[23].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[24].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[25].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[26].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[27].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[28].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[29].NumberFormat = "0.0";
                //workBook.ActiveSheet.Columns[30].NumberFormat = "0.0";



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
            catch (Exception ex)
            {
                MessageBox.Show("errro al exportar:" + ex);
            }
        }







    }
}
