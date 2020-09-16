using Password;
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
    //Sia.PublicarPnt(9684,"Password");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9684,"Password");    
    //ww.ShowInTaskbar = false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;    
    //ww.ShowDialog();       
    public partial class Password : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        string cod_empresa = "";
        int moduloid = 0;

        public Password()
        {
            InitializeComponent();
            SiaWin = System.Windows.Application.Current.MainWindow;
            idemp = SiaWin._BusinessId; ;
            LoadConfig();
            load();
        }
        private void LoadConfig()
        {
            try
            {
                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                idemp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }

        public void load()
        {
            try
            {
                string query = "select * from Seg_User";
                DataTable dt = SiaWin.Func.SqlDT(query, "usu", 0);
                GridUser.ItemsSource = dt.DefaultView;
            }
            catch (Exception w)
            {
               MessageBox.Show("error al cargar usuarios:"+w);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TxUSer.Text))
                {
                    MessageBox.Show("ingrese un usuario");
                }

                string query = "select UserAlias,UserKey,UserName,Tag,Tag1,Tag2,ImageId,UserIniScreen,BusinessId,UserId,IsRDP,seg_group.GroupCode from Seg_User inner join seg_group on seg_user.GroupId=seg_group.GroupId where UserAlias='" + TxUSer.Text + "'";

                DataTable dt = SiaWin.Func.SqlDT(query, "usu", 0);
                
                string __keyDB = "";
                if (dt.Rows.Count>0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        __keyDB = dr["UserKey"].ToString().Trim();
                    }
                    
                    string descr = Seguridad.Decryption(__keyDB);
                    MessageBox.Show(descr);
                }
                else
                {
                    MessageBox.Show("no existe");
                }

            }
            catch (Exception w)
            {
                MessageBox.Show("error al ver contraseña:" + w);
            }
        }

        private void GridUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataRowView row = (DataRowView)GridUser.SelectedItems[0];
                TxUSer.Text = row["UserAlias"].ToString();

            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
