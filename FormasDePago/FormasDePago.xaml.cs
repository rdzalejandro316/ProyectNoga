using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace SiasoftAppExt
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class FormasDePago : Window
    {
        dynamic SiaWin;
        DataTable dtCue = new DataTable();
        DataTable dtBan = new DataTable();
        int idemp = 0;
        public string NomCliente = string.Empty;
        decimal totalPagar = 0;
        public FormasDePago()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
            idemp = SiaWin._BusinessId;
            loadInfo();
            if (SiaWin.ValReturn!=null) totalPagar =  Convert.ToDecimal(SiaWin.ValReturn.ToString());
            TxtTotalRecaudo.Text = totalPagar.ToString("C2");
            dataGrid.UpdateLayout();
            dataGrid.SelectedIndex = 0;
            dataGrid.Focus();
           
           // MessageBox.Show(dtCue.Rows.Count.ToString());
            //this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 1), true);

            // find the ColumnIndex for that row.
            //var RowIndex = this.dataGrid.ResolveToRowIndex(1);
            //var ColumnIndex = this.dataGrid.ResolveToScrollColumnIndex(1);

            // CurrentCell is set if MappingName is EmployeeID
            //this.dataGrid.MoveCurrentCell(new RowColumnIndex(0, 1));
        }
        private void loadInfo()
        {
            dtBan = SiaWin.Func.SqlDT("select cod_ban,cod_ban+'-'+nom_ban as nom_ban,cod_cta from comae_ban  order by cod_ban", "comae_ban", idemp);
            dtBan.PrimaryKey = new System.Data.DataColumn[] { dtBan.Columns["cod_ban"] };
            // establecer paths
         
            //ComboBanco.ItemsSource = dtBan.DefaultView;
            //ComboBanco.MappingName = "cod_ban";
            //ComboBanco.DisplayMemberPath = "nom_ban";
            //ComboBanco.SelectedValuePath = "cod_ban";
            dtCue = dtBan.Copy();
            dtCue.Columns.Add("valor", typeof(Decimal)).DefaultValue=0;
            dtCue.Columns.Add("dias", typeof(Int32)).DefaultValue = 0;
            dtCue.Columns.Add("fechaven", typeof(DateTime));
            dtCue.Columns.Add("documento", typeof(string)).DefaultValue = string.Empty;
            dataGrid.ItemsSource = dtCue.DefaultView;
            this.UpdateLayout();
            dataGrid.SelectedIndex = 0;
            //this.dataGrid.Columns.Add(new GridComboBoxColumn() { HeaderText = "Product Name", MappingName = "ProductName", ItemsSource = viewModel.ComboItems });
            //CmbBan.ItemsSource = dtBan.DefaultView;
            //CmbBan.DisplayMemberPath = "nom_ban";
            //CmbBan.SelectedValuePath = "cod_ban";
            //          MessageBox.Show(_Parameter[1].ToString());
            //int idrowtrnrr = Convert.ToInt32(((UserControl1)param).Param1[0].ToString());
            //ConfigCSource.idregcab = Convert.ToInt32(((UserControl1)param).Param1[0].ToString());
        }
        private void dataGrid_CurrentCellEndEdit(object sender, CurrentCellEndEditEventArgs e)
        {   
            GridColumn colum = ((SfDataGrid) sender).CurrentColumn as GridColumn;
            if (colum.MappingName == "valor")
            {
                //GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
                decimal totalabono = 0;
                decimal.TryParse(dtCue.Compute("Sum(valor)", "").ToString(), out totalabono);
                System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
//                decimal _abono = Convert.ToDecimal(dr["valor"].ToString());
                if (totalabono > totalPagar)
                {
                    MessageBox.Show("El valor pagado es mayor al saldo...");
                    dr.BeginEdit();
                    dr["valor"] = 0;
                    dr.EndEdit();
                }
                dataGrid.UpdateLayout();
                sumaAbonos();
            }
        }
        private void dataGrid_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F8)
                {
                    GridNumericColumn Colum = ((SfDataGrid)sender).CurrentColumn as GridNumericColumn;
                    if (Colum.MappingName == "valor")
                    {
                        decimal totalabono = 0;
                        decimal.TryParse(dtCue.Compute("Sum(valor)", "").ToString(), out totalabono);
                        System.Data.DataRow dr = dtCue.Rows[dataGrid.SelectedIndex];
                        dr.BeginEdit();
                        dr["valor"] = (totalPagar - totalabono);
                        dr.EndEdit();
                        e.Handled = true;
                    }
                    dataGrid.UpdateLayout();
                    sumaAbonos();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void sumaAbonos()
        {
            decimal totalabono = 0;
            decimal.TryParse(dtCue.Compute("Sum(valor)", "").ToString(), out totalabono);
            //TxtTotalRecaudo.Text = totalabono.ToString("C2");
            TxtTotalPagado.Text = totalabono.ToString("C2");
            TxtTotalRecaudo.Text = Convert.ToDecimal(totalPagar - totalabono).ToString("C2"); ;
            //double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=4").ToString(), out abonoCxCAnt);
            //double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=1").ToString(), out abonoCxP);
            //double.TryParse(dtCue.Compute("Sum(abono)", "tip_apli=2").ToString(), out abonoCxPAnt);
            //            TextCxCAbono.Text = abonoCxC.ToString("C");
            //           TextCxCAntAbono.Text = abonoCxCAnt.ToString("C");
            //            TextCxPAbono.Text = abonoCxP.ToString("C");
            //            TextCxPAntAbono.Text = abonoCxPAnt.ToString("C");
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Double abono = 0;
            //double.TryParse(dtCue.Compute("Sum(valor)","").ToString(), out abono);
            //if (abono<= 0) e.Cancel = true;
            //SiaWin.ValReturn = dtCue;
            //this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            decimal abono = 0;
            decimal.TryParse(dtCue.Compute("Sum(valor)", "").ToString(), out abono);
            if (abono <= 0 || abono!= totalPagar)
            {
                MessageBox.Show("Digita Valor a pagar o valor a abono es diferente al valor a pagar");
                dataGrid.SelectedIndex = 0;
                dataGrid.Focus();
                return;
            }
            SiaWin.ValReturn = dtCue;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SiaWin.ValReturn = null;
            this.Close();
        }

        private void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            this.dataGrid.MoveCurrentCell(new RowColumnIndex(1, 1), true);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.F5)
            {
                if (e.Key == System.Windows.Input.Key.F5)
                {
                   BtnGrabar.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                   return;
                }
            }
        }
    }
}
