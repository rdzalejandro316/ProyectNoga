using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace ConsultaPedidos
{
    public partial class DetallePedidoVenta : Window
    {
        dynamic SiaWin;
        public int idemp = 0;
        string cnEmp = "";
        int moduloid = 0;

        public string n_pedido = string.Empty;
        public string bodega = string.Empty;
        public string referencia = string.Empty;
        public string fecha = string.Empty;
        public DetallePedidoVenta()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                System.Data.DataRow foundRow = SiaWin.Empresas.Rows.Find(idemp);
                int idLogo = Convert.ToInt32(foundRow["BusinessLogo"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                string aliasemp = foundRow["BusinessAlias"].ToString().Trim();
                string cod_empresa = foundRow["BusinessCode"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                DataRow[] drmodulo = SiaWin.Modulos.Select("ModulesCode='IN'");
                if (drmodulo == null) this.IsEnabled = false;
                moduloid = Convert.ToInt32(drmodulo[0]["ModulesId"].ToString());
                Title = "Detalle: " + cod_empresa + "-" + nomempresa;
                Name_Ref2.Text = referencia;

                cargarConsulta(fecha, referencia, bodega);
            }
            catch (Exception)
            {
                MessageBox.Show("error al cargar el Load");
            }
        }

        public void cargarConsulta(string fecha_back, string referencia, string bodegas)
        {
            try
            {

                //ordenes de compra


                string QurOrd = "declare @bod varchar(max) = '" + bodegas + "'; ";
                QurOrd += "select cue.cod_ref,ref.nom_ref,cue.num_trn,sum(cantidad) as can_pedi ";
                QurOrd += "from InCue_doc as cue ";
                QurOrd += "inner join InCab_doc as cab on cue.idregcab = cab.idreg ";
                QurOrd += "inner join inmae_ref as ref on cue.cod_ref = ref.cod_ref ";
                QurOrd += "where cab.cod_trn='505' and cab.num_trn='"+n_pedido+"' and cue.cod_ref='" + referencia + "' ";
                QurOrd += "and cue.cod_bod in (select value from STRING_SPLIT(@bod, ','))  ";
                QurOrd += "group by cue.cod_ref,ref.nom_ref,cue.num_trn;";
                //MessageBox.Show("QurOrd:"+QurOrd);
                DataTable dt_ord = SiaWin.Func.SqlDT(QurOrd, "ordenes", idemp);
                dataGridPedido.ItemsSource = dt_ord.DefaultView;


                string QurVen = "declare @bod varchar(max) = '" + bodegas + "'; ";
                QurVen += "select cue.cod_ref,cue.num_trn,cue.doc_cruc,sum(cantidad) as can_venta ";
                QurVen += "from InCue_doc as cue ";
                QurVen += "inner join InCab_doc as cab on cue.idregcab = cab.idreg ";
                QurVen += "where cue.cod_ref='" + referencia + "' and cab.cod_trn='005' and cue.doc_cruc='"+n_pedido+"' ";
                QurVen += "and cue.cod_bod in (select value from STRING_SPLIT(@bod, ','))  ";
                QurVen += "group by cue.cod_ref,cue.num_trn,cue.doc_cruc order by cue.cod_ref; ";
                //MessageBox.Show("QurVen:" + QurVen);
                DataTable dt_ven = SiaWin.Func.SqlDT(QurVen, "venta", idemp);
                dataGridVenta.ItemsSource = dt_ven.DefaultView;


                string QurRem = "declare @bod varchar(max) = '" + bodegas + "'; ";
                QurRem += "select cue.cod_ref,cue.num_trn,cue.doc_cruc,sum(cantidad) as can_remi ";
                QurRem += "from InCue_doc as cue ";
                QurRem += "inner join InCab_doc as cab on cue.idregcab = cab.idreg ";
                QurRem += "where cue.cod_ref='" + referencia + "' and cab.cod_trn='145' and cue.doc_cruc='" + n_pedido + "'  ";
                QurRem += "and cue.cod_bod in (select value from STRING_SPLIT(@bod, ','))  ";
                QurRem += "group by cue.cod_ref,cue.num_trn,cue.doc_cruc order by cue.cod_ref; ";
                //MessageBox.Show("QurRem:" + QurRem);

                DataTable dt_rem = SiaWin.Func.SqlDT(QurRem, "remision", idemp);
                dataGridRemision.ItemsSource = dt_rem.DefaultView;
            }
            catch (Exception w)
            {
                MessageBox.Show("error al cargar la consulata" + w);
            }
        }

        private void BTNdetalle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tag = (sender as Button).Tag.ToString().Trim();
                string cod_trn = "";
                switch (tag)
                {
                    case "1": cod_trn = "505"; break;
                    case "2": cod_trn = "005"; break;
                    case "3": cod_trn = "145"; break;
                }

                string query = "";
                if (tag == "1")
                {
                    DataRowView row = (DataRowView)dataGridPedido.SelectedItems[0];
                    string numtrn = row["num_trn"].ToString().Trim();
                    query = "select * From incab_doc where num_trn='" + numtrn + "' and cod_trn='" + cod_trn + "' ";
                }

                if (tag == "2")
                {
                    DataRowView row = (DataRowView)dataGridVenta.SelectedItems[0];
                    string numtrn = row["num_trn"].ToString().Trim();
                    query = "select * From incab_doc where num_trn='" + numtrn + "' and cod_trn='" + cod_trn + "' ";
                }


                if (tag == "3")
                {
                    DataRowView row = (DataRowView)dataGridRemision.SelectedItems[0];
                    string numtrn = row["num_trn"].ToString().Trim();
                    query = "select * From incab_doc where num_trn='" + numtrn + "' and cod_trn='" + cod_trn + "' ";
                }


                DataTable dt = SiaWin.Func.SqlDT(query, "documento", idemp);
                if (dt.Rows.Count > 0)
                {
                    int idreg = Convert.ToInt32(dt.Rows[0]["idreg"]);
                    SiaWin.TabTrn(0, idemp, true, idreg, moduloid, WinModal: true);
                }
            }
            catch (Exception w)
            {
                MessageBox.Show("error al abrir el documento:" + w);
            }
        }






    }
}
