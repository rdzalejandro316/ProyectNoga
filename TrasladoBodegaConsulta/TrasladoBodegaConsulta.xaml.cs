using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.ScrollAxis;
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

    //    Sia.PublicarPnt(9638,"TrasladoBodegaConsulta");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9638,"TrasladoBodegaConsulta");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();
    public partial class TrasladoBodegaConsulta : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        DataSet ds = new DataSet();
        public string cod_pvt = "";
        public TrasladoBodegaConsulta()
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
                this.Title = "Consulta de trasladdos " + cod_empresa + "-" + nomempresa;

                FechaIni.Text = DateTime.Now.ToShortDateString();
                FechaFin.Text = DateTime.Now.ToShortDateString();
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

        private void CmbTipoCons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (CmbTipoCons.SelectedIndex >= 0)
            {
                switch (CmbTipoCons.SelectedIndex)
                {
                    case 0:
                        COLMval_uni.IsHidden = true;
                        COLMsubtotal.IsHidden = true;
                        COLMpor_des.IsHidden = true;
                        COLMtot_tot.IsHidden = true;
                        dataGridSF.ItemsSource = null;
                        TX_Total.Text = "-";
                        break;
                    case 1:
                        COLMval_uni.IsHidden = false;
                        COLMsubtotal.IsHidden = false;
                        COLMpor_des.IsHidden = false;
                        COLMtot_tot.IsHidden = false;
                        dataGridSF.ItemsSource = null;
                        TX_Total.Text = "-";
                        break;
                    case 2:
                        COLMval_uni.IsHidden = true;
                        COLMsubtotal.IsHidden = true;
                        COLMpor_des.IsHidden = true;
                        COLMtot_tot.IsHidden = true;
                        dataGridSF.ItemsSource = null;
                        TX_Total.Text = "-";
                        break;
                    default:
                        break;
                }

            }

        }


        private void Ejecutar_Click(object sender, RoutedEventArgs e)
        {

            if (CmbTipoCons.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione el tipo de transaccion");
                return;
            }
            dataGridSF.ClearFilters();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                string TipoTrn = "";
                switch (CmbTipoCons.SelectedIndex)
                {
                    case 0:
                        TipoTrn = "141";
                        break;
                    case 1:
                        TipoTrn = "145";
                        break;
                    case 2:
                        TipoTrn = "146";
                        break;
                }

                StringBuilder _sql = new StringBuilder();
                ds.Clear();
                ds.Tables.Clear();
                _sql.Append("select InCab_doc.idreg,InCab_doc.cod_trn,InCab_doc.num_trn,InCab_doc.fec_trn,InCab_doc.bod_tra as cod_boddes,bodegaDes.nom_bod as bodegades,InCue_doc.cod_bod as cod_bodorg,bodegaOrigen.nom_bod as bodegaorigen, ");
                _sql.Append("InCue_doc.cod_ref,InMae_ref.nom_ref,InCue_doc.cantidad,InCue_doc.val_uni,InCue_doc.por_des,subtotal,tot_tot ");
                _sql.Append("from InCab_doc ");
                _sql.Append("inner join InCue_doc on InCab_doc.idreg = InCue_doc.idregcab ");
                _sql.Append("inner join InMae_bod as bodegaDes on InCab_doc.bod_tra = bodegaDes.cod_bod ");
                _sql.Append("inner join inmae_bod as bodegaOrigen on InCue_doc.cod_bod = bodegaOrigen.cod_bod ");
                _sql.Append("inner join InMae_ref on InCue_doc.cod_ref = InMae_ref.cod_ref ");
                _sql.Append("where InCab_doc.cod_trn='" + TipoTrn + "' and incab_doc.fec_trn between '" + FechaIni.Text + "' and '" + FechaFin.Text + " 23:59:59' order by InCab_doc.fec_trn,InCab_doc.num_trn");


                ds.Tables.Add(SiaWin.DB.SqlDT(_sql.ToString(), "Traslados", idemp));

                dataGridSF.ItemsSource = ds.Tables["Traslados"];
                TX_Total.Text = ds.Tables["Traslados"].Rows.Count.ToString();
                if (ds.Tables["Traslados"].Rows.Count > 0)
                {


                    dataGridSF.Focus();

                    dataGridSF.SelectedItem = 1;
                    dataGridSF.UpdateLayout();
                    //int id1x = dg.SelectedIndex;
                    dataGridSF.MoveCurrentCell(new RowColumnIndex(1, 1), false);


                }
            }
            catch (Exception ex)
            {
                SiaWin.Func.SiaExeptionGobal(ex);
                MessageBox.Show(ex.Message, "-PvTrasladosBodega-LoadData");
            }
        }

        private void ExportaXLS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new Syncfusion.UI.Xaml.Grid.Converter.ExcelExportingOptions();
                options.ExcelVersion = ExcelVersion.Excel2013;
                var excelEngine = dataGridSF.ExportToExcel(dataGridSF.View, options);
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
                throw;
            }
        }

        private void ReImprimir_Click(object sender, RoutedEventArgs e)
        {
            DataRowView row = (DataRowView)dataGridSF.SelectedItems[0];
            if (row == null)
            {
                MessageBox.Show("Registro sin datos");
                return;
            }
            int numtrn = (int)row["idreg"];
            
            ImprimeDocumentoTraslado((string)row["cod_trn"], (string)row["num_trn"], 1, false);            
        }

        private void ImprimeDocumentoTraslado(string codtrn, string numtrn, int Reimprimir, bool traslado)
        {

            if (string.IsNullOrEmpty(codtrn)) return;
            if (string.IsNullOrEmpty(numtrn)) return;


            if (traslado == false)
            {
                if (dataGridSF.SelectedIndex < 0)
                {
                    MessageBox.Show("seleccione el documento a imprimir");
                    return;
                }

                if (ds.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("No hay registros para exportar..");
                    return;
                }
            }

            try
            {

                List<ReportParameter> parameters = new List<ReportParameter>();
                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(cod_empresa);
                paramcodemp.Name = "codemp";
                parameters.Add(paramcodemp);
                ReportParameter paramtrn = new ReportParameter();
                paramtrn.Name = "codtrn";
                paramtrn.Values.Add(codtrn);
                parameters.Add(paramtrn);
                ReportParameter paramnum = new ReportParameter();
                paramnum.Values.Add(numtrn);
                paramnum.Name = "numtrn";
                parameters.Add(paramnum);
                ReportParameter paramReim = new ReportParameter();
                paramReim.Values.Add(Reimprimir.ToString());
                paramReim.Name = "Reimprime";
                parameters.Add(paramReim);
                if (codtrn != "141")
                {
                    int impvalores = 1;
                    if (MessageBox.Show("Imprime Valores", "Siasoft", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        impvalores = 0;
                    }
                    ReportParameter paramValores = new ReportParameter();
                    paramValores.Values.Add(impvalores.ToString());
                    paramValores.Name = "ImprimeValores";
                    parameters.Add(paramValores);

                }

                string TipoReporte = @"/Otros/FrmDocumentos/PvTrasladosBodega141";
                if (codtrn == "145") TipoReporte = @"/Otros/FrmDocumentos/PvTrasladosBodega145";
                if (codtrn == "146") TipoReporte = @"/Otros/FrmDocumentos/PvTrasladosBodega145";
                string TituloAuditoria = "Traslado de Bodega:";
                if (codtrn == "145") TituloAuditoria = "Traslado Bodega Consignacion";
                if (codtrn == "146") TituloAuditoria = "Traslado Bodega Consignacion - Anulacion";

                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idemp, 0, 0, 0, TituloAuditoria + ":" + codtrn + "-" + numtrn, "");                
                SiaWin.Reportes(parameters, TipoReporte, Modal: true);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }






    }
}
