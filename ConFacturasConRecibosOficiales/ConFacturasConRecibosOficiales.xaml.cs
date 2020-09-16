using ConFacturasConRecibosOficiales;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

    //    Sia.PublicarPnt(9634,"ConFacturasConRecibosOficiales");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9634,"ConFacturasConRecibosOficiales");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();
    public partial class ConFacturasConRecibosOficiales : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int moduloid = 0;

        public ConFacturasConRecibosOficiales()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
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
                this.Title = "Factura relacionadas Recibos de caja " + cod_empresa + "-" + nomempresa;

                System.Data.DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='CO'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }

        private void BtnConsultar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Tx_factura.Text))
                {
                    MessageBox.Show("ingrese una factura", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                
                DataTable dt = SiaWin.Func.SqlDT("select * from incab_doc where num_trn='"+Tx_factura.Text+"' and cod_trn='005' ", "factura", idemp);
                if (dt.Rows.Count>0)
                {
                    
                    string query = "select cab.idreg,cab.cod_trn,cab.num_trn,cab.fec_trn,cab.cod_ven,cue.cod_cta,cue.cod_ter,des_mov,cue.deb_mov as valor,cue.cre_mov as abono from Cocue_doc as cue ";
                    query += "inner join CoCab_doc as cab on cab.idreg = cue.idregcab and cab.cod_trn = cue.cod_trn and cab.num_trn = cue.num_trn ";
                    query += "where cue.cod_ter = '"+ dt.Rows[0]["cod_cli"].ToString() + "' and doc_ref = '" + Tx_factura.Text + "' ";
                    query += "order by deb_mov desc ";

                    DataTable dt_abonos = SiaWin.Func.SqlDT(query, "recibos", idemp);
                    if (dt_abonos.Rows.Count>0)
                    {
                        dataGridCxCD.ItemsSource = dt_abonos.DefaultView;
                        Tx_Rows.Text = dt_abonos.Rows.Count.ToString();
                        double valor = Convert.ToDouble(dt_abonos.Compute("Sum(valor)", ""));
                        Tx_valor.Text = valor.ToString("C");
                        double abono = Convert.ToDouble(dt_abonos.Compute("Sum(abono)", ""));
                        Tx_abono.Text = abono.ToString("C");
                    }
                    else
                    {
                        MessageBox.Show("no tiene abonos");
                        Tx_Rows.Text = "0";
                    }
                }
                else
                {
                    MessageBox.Show("la factura "+ Tx_factura.Text.Trim() + " ingresada no existe","alerta",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                }

                

            }
            catch (Exception w)
            {
                MessageBox.Show("error al consultar:"+w);
            }
        }

        private void BtnExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;

                var excelEngine = dataGridCxCD.ExportToExcel(dataGridCxCD.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];
                workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;

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

                    //Message box confirmation to view the created workbook.
                    if (MessageBox.Show("Usted quiere abrir el archivo en excel?", "Ver archvo",
                                        MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                        System.Diagnostics.Process.Start(sfd.FileName);
                    }
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al exportar:" + w);
            }
        }


        private void BtnDetalleD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridCxCD.SelectedItems[0];
                if (row == null) return;
                int idreg = Convert.ToInt32(row["idreg"]);
                if (idreg <= 0) return;
                SiaWin.TabTrn(0, idemp, true, idreg, 1, WinModal: true);
            }
            catch (Exception w)
            {
                System.Windows.MessageBox.Show("Error ...." + w.Message);
            }
        }

        private void BtnDetalleFpag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)dataGridCxCD.SelectedItems[0];
                Fpago win = new Fpago();
                win.factura = row["num_trn"].ToString();
                win.ShowInTaskbar = false;
                win.Owner = Application.Current.MainWindow;
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                win.ShowDialog();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al ver formas de pago:"+w);
            }
        }



    }
}
