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

namespace NotasEmpleados
{

    public partial class NotaAdd : Window
    {

        dynamic SiaWin;
        int idemp = 0;

        public string empleado = "";
        public bool actualizo = false;

        public NotaAdd()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string val = validacion();
                if (!string.IsNullOrEmpty(val))
                {
                    MessageBox.Show(val);
                    return;
                }

                string query = "insert into CoMae_terNota (cod_ter,fecha,usuario,nota,title) values (" + empleado + ",getdate(),'" + SiaWin._UserId + "','" + TX_descr.Text + "','" + Tx_tit.Text + "')";

                if (SiaWin.Func.SqlCRUD(query, idemp) == true)
                {
                    actualizo = true;
                    this.Close();
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al guardar:" + w);
            }
        }

        public string validacion()
        {
            string val = string.Empty;
            if (string.IsNullOrEmpty(Tx_tit.Text)) val = "ingrese un titulo";
            if (string.IsNullOrEmpty(TX_descr.Text)) val = "ingrese la nota a digitar";
            return val;
        }







    }
}
