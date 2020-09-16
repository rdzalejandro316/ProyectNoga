using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class SaldosBodegas : Window
    {
        dynamic SiaWin;
        string _conexion;
        DataTable bodCND = new DataTable();
        DataTable bodPv = new DataTable();
        //string codigo, string nombre, int idrow, string conexion, string idbod, int idemp
        public SaldosBodegas(string codigo, string nombre, int idrow, string conexion, string idbod, int idemp)
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            TxtCodigo.Text = codigo;
            TxtNombre.Text = nombre;
            _conexion = conexion;
        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
                e.Handled = true;
            }
        }
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
        }


    }
}
