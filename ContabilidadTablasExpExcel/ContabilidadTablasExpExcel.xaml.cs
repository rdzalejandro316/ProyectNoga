using ContabilidadTablasExpExcel;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
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

    //    Sia.PublicarPnt(9616,"ContabilidadTablasExpExcel");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9616,"ContabilidadTablasExpExcel");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();

    public partial class ContabilidadTablasExpExcel : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        public ContabilidadTablasExpExcel()
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
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Exportar " + cod_empresa + "-" + nomempresa;               
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = (sender as Button).Name.ToString().Trim();

                switch (name)
                {
                    case "BtnTerceros":
                        TabItemExt tabItemExt1 = new TabItemExt();
                        tabItemExt1.Header = "Terceros";
                        ControlTercero userCon = new ControlTercero(idemp);
                        tabItemExt1.Content = userCon;
                        TabControl1.Items.Add(tabItemExt1);
                       break;
                    case "BtnBancos":
                        TabItemExt tabItemExt2 = new TabItemExt();
                        tabItemExt2.Header = "Bancos";
                        generico gen = new generico(idemp,"2","Maestra de bancos");
                        tabItemExt2.Content = gen;
                        TabControl1.Items.Add(tabItemExt2);
                        break;
                    case "BtnCcosto":
                        TabItemExt tabItemExt3 = new TabItemExt();
                        tabItemExt3.Header = "C COSTO";
                        generico gen3 = new generico(idemp, "3", "Maestra de centro de costos");
                        tabItemExt3.Content = gen3;
                        TabControl1.Items.Add(tabItemExt3);
                        break;
                    case "Btnciudad":
                        TabItemExt tabItemExt4 = new TabItemExt() { Header = "CIUDADES" };                                 
                        tabItemExt4.Content = new generico(idemp, "4", "Maestra de ciudades"); ;
                        TabControl1.Items.Add(tabItemExt4);
                        break;
                    case "BtnDepa":
                        TabItemExt tabItemExt5 = new TabItemExt() { Header = "Departamento" };
                        tabItemExt5.Content = new generico(idemp, "5", "Maestra de Departamento"); ;
                        TabControl1.Items.Add(tabItemExt5);
                        break;
                    case "BtnPais":
                        TabItemExt tabItemExt6 = new TabItemExt() { Header = "Paises" };
                        tabItemExt6.Content = new generico(idemp, "6", "Maestra de Paises"); ;
                        TabControl1.Items.Add(tabItemExt6);
                        break;
                    case "BtnTalonarios":
                        TabItemExt tabItemExt7 = new TabItemExt() { Header = "Talonarios" };
                        tabItemExt7.Content = new generico(idemp, "7", "Maestra de Talonarios"); ;
                        TabControl1.Items.Add(tabItemExt7);
                        break;
                    case "BtnDocumentos":
                        TabItemExt tabItemExt8 = new TabItemExt() { Header = "Documentos Contables" };
                        tabItemExt8.Content = new genericoDocument(idemp, "8"); ;
                        TabControl1.Items.Add(tabItemExt8);
                        break;


                }

                

            }
            catch (Exception w)
            {
                MessageBox.Show("error alabrir:"+w);
            }
        }





      
    }
}
