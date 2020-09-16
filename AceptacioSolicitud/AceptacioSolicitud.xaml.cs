using AceptacioSolicitud;
using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using Syncfusion.UI.Xaml.Grid;
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
    //Sia.PublicarPnt(9650,"AceptacioSolicitud");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9650,"AceptacioSolicitud");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //ww.ShowDialog();


    public partial class AceptacioSolicitud : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public string cod_pvt = "";
        public string name_user = "";

        public AceptacioSolicitud()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;                      
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
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                string alias = foundRow["BusinessAlias"].ToString().Trim();
                this.Title = "Aceptacion de solicitud " + cod_empresa + "-" + nomempresa;

                cargargrid();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void cargargrid()
        {
            dataGrid.ItemsSource = null;
            string query = "select idrow,concepto,convert(varchar,fecha_solic,103) as fecha_solic,valor from solicitudDineros  where cod_pvt='" + cod_pvt + "' and estado=0";
            //MessageBox.Show(query);

            System.Data.DataTable dt = SiaWin.Func.SqlDT(query, "tabla", idemp);

            if (dt.Rows.Count > 0)
            {
                dataGrid.ItemsSource = dt.DefaultView;
            }
            else
            {
                dataGrid.ItemsSource = null;
                MessageBox.Show("no tiene solicitudes de dinero en este momento", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public string usuario ()
        {            
            System.Data.DataTable dt = SiaWin.Func.SqlDT("select * from Seg_User where UserId='"+ SiaWin._UserId + "'", "tabla", 0);
            return dt.Rows.Count > 0 ? dt.Rows[0]["UserName"].ToString().Trim() : "----";
        }
          

        GridRowSizingOptions gridRowResizingOptions = new GridRowSizingOptions();
        double autoHeight = 20;
        List<string> excludeColumns = new List<string>() { "concepto", "UserAlias", "fecha_solic", "valor"};

        private void dataGridCxC_QueryRowHeight(object sender, Syncfusion.UI.Xaml.Grid.QueryRowHeightEventArgs e)
        {
            if (this.dataGrid.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out autoHeight))
            {
                if (autoHeight > 24)
                {
                    e.Height = autoHeight;
                    e.Handled = true;
                }

                if (e.RowIndex == 0)
                {
                    e.Height = 30;
                    e.Handled = true;
                }
            }
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                if (dataGrid.SelectedIndex>=0)
                {
                    Nota win = new Nota();
                    win.ShowInTaskbar = false;
                    win.Owner = Application.Current.MainWindow;
                    win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    win.ShowDialog();

                    if (win.flag == false)
                    {
                        MessageBox.Show("debe de escribir algo en la nota de aceptacion", "alerta", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }                    

                    DataRowView row = (DataRowView)dataGrid.SelectedItems[0];
                    string id = row["idrow"].ToString();
                    string query = "update solicitudDineros set fecha_acept=GETDATE(),usu_acept='"+ SiaWin._UserId + "',descripc_acept='"+ win.descripcion.Trim() + "',estado=1,estado_soli='ACEPTADO POR:"+ usuario() + "' where idrow='"+id+"' ";

                    if (MessageBox.Show("Usted desea aceptar la solicitud de dinero", "Aceptar Solicitud", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                        {
                            MessageBox.Show("aceptacion exitosa", "alerta", MessageBoxButton.OK, MessageBoxImage.Information);
                            formato(id);
                            cargargrid();
                        }
                    }

                    


                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al aceptar solicitud:"+w);
            }
        }

        public void formato( string id,string format= "Solicitud")
        {
            try
            {
                List<ReportParameter> parameters = new List<ReportParameter>();

                ReportParameter paramcodemp = new ReportParameter();
                paramcodemp.Values.Add(id);
                paramcodemp.Name = "id";
                parameters.Add(paramcodemp);
                
                ReportParameter paramEmpresa = new ReportParameter();
                paramEmpresa.Values.Add(cod_empresa);
                paramEmpresa.Name = "codemp";
                parameters.Add(paramEmpresa);

                string repnom = @"/Contabilidad/SolicitudDinero";
                string TituloReport = format;
                SiaWin.Reportes(parameters, repnom, TituloReporte: TituloReport, Modal: true, idemp: idemp, ZoomPercent: 50);
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el reporte:" + w);
            }
        }

        private void BtnConsutlar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "select solicitudDineros.idrow,solicitudDineros.cod_pvt,Copventas.nom_pvt, ";
                query += "solicitudDineros.concepto,solicitudDineros.valor,solicitudDineros.fecha_solic,solicitudDineros.usu_solicitud,usu_sol.username as usu_sol, ";
                query += "solicitudDineros.usu_acept,usu_acept.username as usu_acept,solicitudDineros.fecha_acept,solicitudDineros.descripc_acept,solicitudDineros.estado_soli ";
                query += "from solicitudDineros ";
                query += "inner join Copventas on solicitudDineros.cod_pvt = Copventas.cod_pvt ";
                query += "left join GrupoSaavedra_SiaApp.dbo.Seg_User usu_sol  on usu_sol.UserId = solicitudDineros.usu_solicitud ";
                query += "left join GrupoSaavedra_SiaApp.dbo.Seg_User usu_acept  on usu_acept.UserId = solicitudDineros.usu_acept ";
                query += "where solicitudDineros.cod_pvt = '" + cod_pvt + "' ";                

                DataTable dt = SiaWin.Func.SqlDT(query, "table", idemp);

                if (dt.Rows.Count>0)
                {
                    dataGridHistorial.ItemsSource = dt.DefaultView;

                }
                else
                {
                    MessageBox.Show("no tiene hitorial de solicitudes");
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
                var excelEngine = dataGridHistorial.ExportToExcel(dataGridHistorial.View, options);
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
                SiaWin.Func.SiaExeptionGobal(w);
                MessageBox.Show("error al exportar");
            }
        }

        private void BtnReimprimir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridHistorial.SelectedIndex>0)
                {                    
                    DataRowView row = (DataRowView)dataGridHistorial.SelectedItems[0];
                    string id = row["idrow"].ToString();
                    formato(id, "fromato Reimpreso");
                }
               
            }
            catch (Exception w)
            {
                MessageBox.Show("error en la solicitud:"+w);
            }
        }





    }
}
