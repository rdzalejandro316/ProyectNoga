using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
    //    Sia.PublicarPnt(9704,"Desencriptar");
    //    dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9704,"Desencriptar");
    //    ww.ShowInTaskbar = false;
    //    ww.Owner = Application.Current.MainWindow;
    //    ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;        
    //    ww.ShowDialog();
    public partial class Desencriptar : Window
    {
        dynamic SiaWin;
        public int idemp = 0;        

        public Desencriptar()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SiaWin = Application.Current.MainWindow;
        }


        public static string CompressString(string text)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(text);
                var memoryStream = new MemoryStream();
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gZipStream.Write(buffer, 0, buffer.Length);
                }
                memoryStream.Position = 0;
                var compressedData = new byte[memoryStream.Length];
                memoryStream.Read(compressedData, 0, compressedData.Length);
                var gZipBuffer = new byte[compressedData.Length + 4];
                Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
                Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
                buffer = null; memoryStream = null; compressedData = null;
                return Convert.ToBase64String(gZipBuffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CompressString");
                return "";
            }
        }

        public string DecompressString(string compressedText)
        {
            try
            {
                if (compressedText == string.Empty) return string.Empty;
                byte[] gZipBuffer = Convert.FromBase64String(compressedText);
                using (var memoryStream = new MemoryStream())
                {
                    int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                    memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                    var buffer = new byte[dataLength];
                    memoryStream.Position = 0;
                    using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                    {
                        gZipStream.Read(buffer, 0, buffer.Length);
                    }
                    gZipBuffer = null;

                    return Encoding.UTF8.GetString(buffer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "DecompressString");
                return "";
            }
        }


        public class ClosableTab : TabItem
        {
            public ClosableTab()
            {
                this.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.VerticalAlignment = VerticalAlignment.Stretch;
                this.Header = "dll";
            }
            private void DeshabilitaControles(int depth, object obj)
            {
                foreach (object child in LogicalTreeHelper.GetChildren(obj as DependencyObject))
                {
                    DeshabilitaControles(depth + 1, child);
                }
            }
            /// <summary>
            /// Property - Set the Title of the Tab
            /// </summary>
            /// Maestra parametros
            public string Maestra = string.Empty;
            public string CmpCodigo = string.Empty;
            public string CmpNombre = string.Empty;
            public int CmpReturn = 0;
            /// 
            /// Estado 0=Se puede cerrar,1 Creando,2 Modificando,3 Ejectutando proceso pero se puede cerrar,4 Ejecutando proceso pero no se puede cerrar
            public bool MultiTab = false;
            public bool GuardarEnMemoriaAlCerrar = false;
            public bool CerrarInactivo = false;
            public int CodeScreen = 0;
            public int Estado = 0;
            public int EstadoActivoInactivo = 0;
            public bool EstadoVisible = true;
            public bool ImgVer = false;
            public int TypeScreen = 0;
            public bool CancelaTarea = false;
            public bool ProgresoEstado = false;
            public int idemp = 0;
            public string PathImgBusiness { get; set; }
            public bool VisibleButtonClose { get; set; } = true;
            public DataSet dsDoc { get; set; }
            public void Logo(int img, string type) { }
            public string Title { get; set; }
        }


        private void BtnDesco_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] _CodeMvVm = null;
                string NameClassExt = "";
                DataTable dt = SiaWin.Func.SqlDT("select isext,mvvm_zip,fileext,Date_Edit_Screen,Cache_Screen,isWindow from Screens where Id_Screen='" + TxIdScreen.Text+ "'", "tabla", 0);
                if (dt.Rows.Count>0)
                {
                    if (dt.Rows[0]["MvVm_zip"] != DBNull.Value)
                    {
                        _CodeMvVm = Convert.FromBase64String(DecompressString((string)dt.Rows[0]["MvVm_zip"]));
                        NameClassExt = dt.Rows[0]["fileext"].ToString().Trim();
                        var dll = Assembly.Load(_CodeMvVm);
                        var class1Type = dll.GetType("SiasoftAppExt." + NameClassExt);

                        int isWindow = Convert.ToInt32(dt.Rows[0]["isWindow"]);
                        if (isWindow==1)
                        {
                            dynamic c = Activator.CreateInstance(class1Type);
                            Window control = (Window)c; ;
                            control.Show();
                        }
                        else
                        {                            
                            ClosableTab tab = new ClosableTab();
                            dynamic c = Activator.CreateInstance(class1Type, tab);
                            UserControl control = (UserControl)c; ;
                            tab.Content = control;
                            GridMain.Items.Add(tab);
                        }                        
                    }
                }
                else
                {
                    MessageBox.Show("nada");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al desencriptar:"+w);
            }
        }



        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = @"C:\SiasoftApp\SiasoftGSNueva\SiasoftGSV2019\SiasoftGSV2019_actual\Library\InlistCli.dll";
                var dll = Assembly.LoadFile(path);                
                var class1Type = dll.GetType("SiasoftAppExt." + dll.GetName().Name);
                dynamic c = Activator.CreateInstance(class1Type);
                

                Window control = (Window)c; ;
                control.Show();
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar asemby"+w);
            }
        }


        public SyntaxTree Parse(string text, string filename = "", CSharpParseOptions options = null)
        {
            var stringText = SourceText.From(text, Encoding.UTF8);
            return SyntaxFactory.ParseSyntaxTree(stringText, options, filename);
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] _CodeMvVm = null;
                string NameClassExt = "";
                DataTable dt = SiaWin.Func.SqlDT("select isext,mvvm_zip,fileext,Date_Edit_Screen,Cache_Screen,isWindow from Screens where Id_Screen='" + TxIdScreen.Text + "'", "tabla", 0);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["MvVm_zip"] != DBNull.Value)
                    {
                        _CodeMvVm = Convert.FromBase64String(DecompressString((string)dt.Rows[0]["MvVm_zip"]));
                        NameClassExt = dt.Rows[0]["fileext"].ToString().Trim();
                        var dll = Assembly.Load(_CodeMvVm);
                        var class1Type = dll.GetType("SiasoftAppExt." + NameClassExt);
                        string ArchivoRequest = dll.GetName().Name+".dll";

                        dynamic c = Activator.CreateInstance(class1Type);
                        var parsedSyntaxTree = Parse(c, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp6));
                        var compilation = CSharpCompilation.Create(ArchivoRequest, new SyntaxTree[] { parsedSyntaxTree }, references, DefaultCompilationOptions);

                    }
                }
                else
                {
                    MessageBox.Show("nada");
                }


            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:"+w);
            }
        }


    }
}
