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
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using Syncfusion.UI.Xaml.Grid;
using System.Windows.Forms;
using System.Windows.Controls.Primitives;
using System.Text.RegularExpressions;


namespace ReportesCierrePv
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Botrcprovi : Window
    {        
      dynamic SiaWin;
      public int idEmp = 0;
      public string idBod = string.Empty;
      public string codpvta = string.Empty;
      int idemp = 0;
 
        DataTable dtCue = new DataTable();
      DataTable dtini = new DataTable();

        public Botrcprovi()
      {
               InitializeComponent();
               SiaWin = System.Windows.Application.Current.MainWindow;
               idemp = SiaWin._BusinessId;
               Loadinfo();
               dataGridpvrcprovi.UpdateLayout();
               dataGridpvrcprovi.SelectedIndex = 0;
               dataGridpvrcprovi.Focus();
            //          idemp = SiaWin._BusinessId; ;
        }

        private void Loadinfo()
        {
            dtini = SiaWin.Func.SqlDT("IF (SELECT COUNT(*) FROM pvrcprovi) = 0 BEGIN INSERT INTO pvrcprovi (nrc,cl,valor) VALUES ('0',' ',0) END else BEGIN select * from pvrcprovi END", "pvrcprovi", idemp);
            dtini = SiaWin.Func.SqlDT("select nrc,frc,cl,valor from pvrcprovi", "pvrcprovi", idemp);
            dtCue = dtini.Copy();
            dataGridpvrcprovi.ItemsSource = dtCue.DefaultView;
            this.UpdateLayout();
            dataGridpvrcprovi.SelectedIndex = 0;

        }
        private void Button_ClickINI(object sender, RoutedEventArgs e)
        {
 
            if (Iniciarr.Content.ToString() == "GRABAR DOCUMENTO")
            {
                if (string.IsNullOrEmpty(this.Recibo_.Text))
                {
                    System.Windows.MessageBox.Show("Falta El Numero del Recibo");
                    return;
                }

                if (string.IsNullOrEmpty(this.Fecha_.Text))
                {
                    System.Windows.MessageBox.Show("Falta La fecha del Documento");
                    return;
                }
                if (string.IsNullOrEmpty(this.Cliente_.Text))
                {
                    System.Windows.MessageBox.Show("Falta El nombre del cliente");
                    return;
                }
                if (string.IsNullOrEmpty(this.Valor_.Text))
                {
                    System.Windows.MessageBox.Show("Falta El valor del documento");
                    return;
                }

                Iniciarr.Content = "ADICIONAR DOCUMENTO";
                dtini = SiaWin.Func.SqlDT("IF (SELECT nrc FROM pvrcprovi where nrc='0') = 1 delete from pvrcprovi where nrc='0' ", "pvrcprovi", idemp);
                dtini = SiaWin.Func.SqlDT("IF (SELECT COUNT(*) FROM pvrcprovi where nrc='0') = 1 BEGIN delete from pvrcprovi where nrc='0' END", "pvrcprovi", idemp);

                dtini = SiaWin.Func.SqlDT("insert into pvrcprovi (nrc,frc,cl,valor) values ('" + Recibo_.Text+"','"+Fecha_.Text+"','"+Cliente_.Text+"',"+Valor_.Text+ ") ", "pvrcprovi", idemp);
                dtini = SiaWin.Func.SqlDT("select nrc,frc,cl,valor from pvrcprovi", "pvrcprovi", idemp);
                dtCue = dtini.Copy();
                dataGridpvrcprovi.ItemsSource = dtCue.DefaultView;
                this.UpdateLayout();
                dataGridpvrcprovi.SelectedIndex = 0;
                this.Recibo_.Text = "";
                this.Fecha_.Text = "";
                this.Cliente_.Text = "";
                this.Valor_.Text = "";
                this.Recibo_.IsEnabled = false;
                this.Fecha_.IsEnabled = false;
                this.Cliente_.IsEnabled = false;
                this.Valor_.IsEnabled = false;
                this.Iniciarr.Focus();
            }
            else
            {
                this.Iniciarr.Content = "GRABAR DOCUMENTO";
                this.Recibo_.IsEnabled = true;
                this.Fecha_.IsEnabled = true;
                this.Fecha_.Text = DateTime.Now.Date.ToString();
                this.Cliente_.IsEnabled = true;
                this.Valor_.IsEnabled = true;
                this.Valor_.Text = "";
                this.Recibo_.Focus();
            }
        }

        private void Button_ClickINS(object sender, RoutedEventArgs e)
        {
            if (dataGridpvrcprovi.SelectedIndex >= 0 && dataGridpvrcprovi.SelectedIndex >= 0)
            {
                this.Recibo_.Text = "";
                this.Fecha_.Text = "";
                this.Cliente_.Text = "";
                this.Valor_.Text = "";
                DataRowView row = (DataRowView)dataGridpvrcprovi.SelectedItems[0];
                this.Recibo_.Text = row["nrc"].ToString().Trim();
                this.Fecha_.Text = row["frc"].ToString().Trim();
                this.Cliente_.Text = row["cl"].ToString().Trim();
                this.Valor_.Text = row["valor"].ToString().Trim();
                this.Iniciarr.Content = "GRABAR DOCUMENTO";
                this.Recibo_.IsEnabled = true;
                this.Fecha_.IsEnabled = true;
                this.Cliente_.IsEnabled = true;
                this.Valor_.IsEnabled = true;
                this.Recibo_.Focus();
            }
            else
            {
                System.Windows.MessageBox.Show("seleccione el documento a  editar");
            }

        }

        private void dataGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {
            GridColumn colum = ((SfDataGrid)sender).CurrentColumn as GridColumn;
            {
                System.Data.DataRow dr = dtCue.Rows[dataGridpvrcprovi.SelectedIndex];
                dataGridpvrcprovi.UpdateLayout();
            }
        }

        private void Button_ClickSAL(object sender, RoutedEventArgs e)
        {
            SiaWin.ValReturn = null;
            this.Close();
        }

        private void ValidacionNumeros(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab)
            {
                e.Handled = false;
            }
            else
            {
                System.Windows.MessageBox.Show("este campo solo admite valores numericos");
                e.Handled = true;
            }
        }
    }
}

