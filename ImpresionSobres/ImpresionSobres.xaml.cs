using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
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

    //Sia.PublicarPnt(9551,"ImpresionSobres");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9551, "ImpresionSobres");
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();
    public partial class ImpresionSobres : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";

        public ImpresionSobres()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();

            Tx_codter.Focus();


            this.MaxHeight = 400;
            this.MinHeight = 400;
            this.MaxWidth = 600;
            this.MinWidth = 600;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }
        private void LoadConfig()
        {
            try
            {
                if (idemp <= 0) idemp = SiaWin._BusinessId;
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                //idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());               
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Impresion de sobres " + cod_empresa + "-" + nomempresa;
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            string var = validarCampos();
            if (!string.IsNullOrEmpty(var))
            {
                MessageBox.Show(var);
                return;
            }

            //List<ReportParameter> parameters = new List<ReportParameter>();
            //ReportParameter paramcodemp = new ReportParameter();
            //paramcodemp.Values.Add(cod_empresa);
            //paramcodemp.Name = "codemp";
            //parameters.Add(paramcodemp);
            List<ReportParameter> parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("Nombre", Tx_nomter.Text.Trim()));
            parameters.Add(new ReportParameter("Nit", Tx_codter.Text.Trim()));
            parameters.Add(new ReportParameter("direccion", Tx_Dir.Text.Trim()));
            parameters.Add(new ReportParameter("telefono", Tx_tel.Text.Trim()));
            parameters.Add(new ReportParameter("concepto", Tx_conc.Text.Trim()));
            parameters.Add(new ReportParameter("factura", Tx_Fact.Text.Trim()));
            parameters.Add(new ReportParameter("valor", Tx_Desc.Text.Trim()));

            string repnom = @"/Inventarios/ImpresionSobres";
            string TituloReport = " - impresion de sobres -";
            SiaWin.Reportes(parameters, repnom, TituloReporte: TituloReport, Modal: true, idemp: idemp, ZoomPercent: 50);

        }


        public string validarCampos()
        {
            string text = "";
            if (string.IsNullOrEmpty(Tx_Fact.Text)) text = "ingrese el numero de la factura";
            if (string.IsNullOrEmpty(Tx_conc.Text)) text = "ingrese el concepto";
            if (string.IsNullOrEmpty(Tx_codter.Text)) text = "ingrese el tercero";                        
            return text;
        }

        private void Tx_codter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F8 || (e.Key == Key.Enter && string.IsNullOrEmpty(Tx_codter.Text)))
            {
                int idr = 0; string code = ""; string nom = "";
                dynamic winb = SiaWin.WindowBuscar("comae_ter", "cod_ter", "nom_ter", "cod_ter", "idrow", "Maestra de Tercero", SiaWin.Func.DatosEmp(idemp), false, "", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = System.Windows.Application.Current.MainWindow;
                winb.Height = 300;
                winb.Width = 400;
                winb.ShowDialog();
                idr = winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                Tx_codter.Text = code;
                Tx_nomter.Text = nom;
            }

        }

        private void Tx_codter_LostFocus(object sender, RoutedEventArgs e)
        {
            string code_ter = (sender as TextBox).Text.Trim();

            if (string.IsNullOrEmpty(code_ter)) {
                Tx_nomter.Text = "";
                return;
            }
            

            DataTable dtTer = SiaWin.Func.SqlDT("select * from comae_ter where cod_ter='" + code_ter + "';", "temporal", idemp);
            if (dtTer.Rows.Count > 0)
            {
                Tx_codter.Text = dtTer.Rows[0]["cod_ter"].ToString().Trim();
                Tx_nomter.Text = dtTer.Rows[0]["nom_ter"].ToString().Trim();

                getInfo(Tx_codter.Text);
            }
            else
            {
                MessageBox.Show("el tercero ingresado no existe", "Tercero Inexistente", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                Tx_codter.Text = "---";
                Tx_nomter.Text = "";
                Tx_Suc.Focus();

            }
        }


        public void getInfo(string ter)
        {
            string select = "select comae_ter.nom_ter,comae_ter.dir,comae_ciu.nom_ciu,comae_ter.tel1 ";
            select += "from comae_ter ";
            select += "inner join comae_ciu on comae_ter.cod_ciu = comae_ciu.cod_ciu ";
            select += "where comae_ter.cod_ter='" + ter + "' ";
            DataTable dtTer = SiaWin.Func.SqlDT(select, "temporal", idemp);

            if (dtTer.Rows.Count > 0)
            {
                DataTable dt = SiaWin.Func.SqlDT("select * from inmae_suc where cod_ter='" + ter + "'", "temporal", idemp);
                if (dt.Rows.Count > 0)
                {

                    int code = sucursal(ter);

                    string cadena= "select inmae_suc.dir,inmae_suc.tel,isnull(comae_ciu.nom_ciu,'') as nom_ciu,inmae_suc.nom_suc from inmae_suc ";
                    cadena += "left join comae_ciu on inmae_suc.cod_ciu = comae_ciu.cod_ciu ";
                    cadena += "where inmae_suc.idrow='"+code+"' ";

                    DataTable dtsuc = SiaWin.Func.SqlDT(cadena, "temporal", idemp);

                    if (dtsuc.Rows.Count>0)
                    {
                        Tx_nomter.Text = dtTer.Rows[0]["nom_ter"].ToString().Trim();
                        Tx_Suc.Text = dtsuc.Rows[0]["nom_suc"].ToString().Trim();
                        Tx_Dir.Text = dtsuc.Rows[0]["dir"].ToString().Trim();
                        Tx_tel.Text = dtsuc.Rows[0]["tel"].ToString().Trim();
                        Tx_ciud.Text = dtsuc.Rows[0]["nom_ciu"].ToString().Trim();
                    }                    
                }
                else
                {
                    Tx_nomter.Text = dtTer.Rows[0]["nom_ter"].ToString().Trim();
                    Tx_Suc.Text = "";
                    Tx_Dir.Text = dtTer.Rows[0]["dir"].ToString().Trim();
                    Tx_tel.Text = dtTer.Rows[0]["tel1"].ToString().Trim();
                    Tx_ciud.Text = dtTer.Rows[0]["nom_ciu"].ToString().Trim();
                }
            }

        }


        public int sucursal(string cod_ter)
        {
            int idr = 0; string code = ""; string nom = "";
            try
            {
                
                dynamic winb = SiaWin.WindowBuscar("inmae_suc", "cod_suc", "nom_suc", "cod_suc", "idrow", "Maestra de Sucursales", SiaWin.Func.DatosEmp(idemp), true, "cod_ter='" + cod_ter + "'", idEmp: idemp);
                winb.ShowInTaskbar = false;
                winb.Owner = System.Windows.Application.Current.MainWindow;
                winb.Height = 300;
                winb.Width = 400;
                winb.ShowDialog();
                idr =  winb.IdRowReturn == null ? 0 : winb.IdRowReturn;
                code = winb.Codigo;
                nom = winb.Nombre;
                
            }
            catch (Exception W)
            {
                MessageBox.Show("ERROR:"+W);
            }
            return idr;

        }

        
    }
}
