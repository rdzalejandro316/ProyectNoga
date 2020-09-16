using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace SiasoftAppExt
{
    //Sia.PublicarPnt(9470,"GeneracionFacComTrans");
    //dynamic ww = ((Inicio)Application.Current.MainWindow).WindowExt(9470,"GeneracionFacComTrans");    
    //ww.ShowInTaskbar=false;
    //ww.Owner = Application.Current.MainWindow;
    //ww.WindowStartupLocation=WindowStartupLocation.CenterScreen;
    //ww.ShowDialog();  
    public partial class GeneracionFacComTrans : Window
    {
        dynamic SiaWin;
        public int idEmp = 0;
        public string idBod = string.Empty;
        public string codpvta = string.Empty;
        string cnEmp = "";
        string codemp = "";
        string nitemp = "";
        string Consecutivo = "";
        DataTable Empresas = new DataTable(); // se usa para traer el id de la empresa
        DataRow foundRow;
        DataTable dt141 = new DataTable();
        StringBuilder _sqlFacturas = new StringBuilder();
        public DataTable dtFacturas = null;
        public GeneracionFacComTrans()
        {
            InitializeComponent();
            SiaWin = Application.Current.MainWindow;
//          idemp = SiaWin._BusinessId; ;
        }
        private void LoadConfig()
        {
            try
            {
                string sqlEmpresas = "select * from business   where businessStatus = 1 ";
                Empresas = SiaWin.SqlDT(sqlEmpresas, "Empresas");
                Empresas.PrimaryKey = new DataColumn[] { Empresas.Columns["BusinessCode"] };
                foundRow = SiaWin.Empresas.Rows.Find(idEmp);
                idEmp = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
                cnEmp = foundRow[SiaWin.CmpBusinessCn].ToString().Trim();
                codemp = foundRow["BusinessCode"].ToString().Trim();
                nitemp = foundRow["BusinessNit"].ToString().Trim();
                string nomempresa = foundRow["BusinessName"].ToString().Trim();
                this.Title = "Generación de Factura y Compras Por Traslados - Empresa:" + codemp + "-" + nomempresa;
                TxtNombreEmpresa.Text= codemp + "-" + nomempresa;
                TxtNobmreBodega.Text = idBod;
                FecIni.Text = DateTime.Now.ToShortDateString();
                if(string.IsNullOrEmpty(codpvta.Trim()))
                {
                    MessageBox.Show("Falta codigo de Punto de venta..");
                    BtnConsult.IsEnabled = false;
                    Guardar.IsEnabled = false;
                    BtnExportar.IsEnabled = false;
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("error en el load" + e.Message);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();
        }
        private void Trae141()
        {
            //ind_cruce n(1),val_uni n(10,2),subtotal n(10,2),val_iva n(10,2),cod_tiva c(1),por_iva n(5,2))
            StringBuilder sql160 = new StringBuilder();
            sql160.Append("select cab_doc.cod_trn,cab_doc.num_trn,cab_doc.fec_trn,cue_doc.cod_ref,mae_ref.nom_ref as nom_ref,cue_doc.cantidad,");
            sql160.Append("	cab_doc.bod_tra,space(3) as trn_tras,space(12) as num_tras,0 as est_cruc,mae_ref.vr_intem as val_uni,mae_ref.vr_intem* cue_doc.cantidad as subtotal,mae_ref.cod_tiva,tiva.por_iva,0 as ind_cruce,iif(tiva.por_iva>0,((mae_ref.vr_intem* cue_doc.cantidad)*tiva.por_iva)/100,000000000.00) as val_iva,mae_ref.cod_tip,isnull(dto.dto_pprv,0) as dto_pprv ");
            sql160.Append(" from incue_doc cue_doc inner join incab_doc cab_doc on cab_doc.idreg = cue_doc.idregcab ");
            sql160.Append(" inner join  inmae_ref as mae_ref  on mae_ref.cod_ref = cue_doc.cod_ref " );
            sql160.Append(" left join inmae_tiva tiva on tiva.cod_tiva=mae_ref.cod_tiva");
            sql160.Append(" inner join inmae_bod bod on bod.cod_bod=cab_doc.bod_tra and bod.tipo_bod=1 ");
            sql160.Append(" left join indto_inte dto on mae_ref.cod_tip=dto.cod_tip  ");
            sql160.Append(" where cab_doc.cod_trn = '160' and convert(date, fec_trn)= '"+FecIni.Text+"' and cab_doc.estado = 9 and cue_doc.cod_bod = '"+idBod+"' and cab_doc.ord_comp = '' and(cab_doc.bod_tra = '003' or");
            sql160.Append(" cab_doc.bod_tra = '007' or cab_doc.bod_tra = '012' or cab_doc.bod_tra = '017' or cab_doc.bod_tra = '050') and cue_doc.cantidad <> 0 ");
            //string sql = SiaWin.DB.RutaTablaBase(sql160.ToString());
            //MessageBox.Show("Alejandor:"+sql);
            //Clipboard.SetText(sql160.ToString());
            dt141.Clear();
            dt141 = SiaWin.Func.SqlDT(sql160.ToString(), "trasl141", idEmp);
            //DataTable dtBod = SiaWin.Func.SqlDT("select top 1 cod_ref,nom_ref from inmae_ref", "inmae_ref", 3);

            //SiaWin.Browse(dt141);
            //SiaWin.Browse(dtBod);
            dataGrid141.ItemsSource = dt141.DefaultView;
            TxtTotal.Text = "0.00";
            Txtiva.Text = "0.00";
            TxtSubtotal.Text = "0.00";
            Suma();
            // valida si tiene valor unitario,  si se va en 0 valor unitario o subtotal sale error en factura electronica
            string evento = "";
            foreach (DataRow o in dt141.Select("subtotal<=0"))
            {
                evento = "Producto con valor en 0:" + o["cod_ref"].ToString();
                MessageBox.Show(evento);
                Guardar.IsEnabled = true;
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idEmp, 2, -1, 0, this.Title + " -Dia:" + this.FecIni.Text.ToString()+" Evento:"+evento, "");

            }

            // cruza


            if (dt141.Rows.Count > 0)
            {
                DataTable dt = dt141.AsEnumerable().GroupBy(r => r.Field<string>("bod_tra")).Select(g => g.First()).CopyToDataTable();
                foreach (DataRow row in dt.Rows)
                {
                    string bodcruzar = row["bod_tra"].ToString().Trim();
                    string bodempresa = SiaWin.Func.cmpCodigo("inmae_bod", "cod_bod", "cod_emp", bodcruzar, 1);
                    string bodcodter = SiaWin.Func.cmpCodigo("inmae_bod", "cod_bod", "cod_ter", bodcruzar, 1);
                    if(string.IsNullOrEmpty(bodcodter.Trim()))
                    {
                        MessageBox.Show("La bodega " + bodcruzar + " no tiene codigo de tercero asignado...");
                        return;
                    }
                    if (string.IsNullOrEmpty(bodempresa.Trim()))
                    {
                        MessageBox.Show("La bodega " + bodcruzar + " no tiene empresa asignada...");
                    }
                    else
                    {
                        int idempresa = EmpresasID(bodempresa);
                        if (idempresa > 0) Cruzar141_051(bodcruzar, idempresa);
                    }
                }
            }
        }
        private void Suma()
        {
            if (dt141.Rows.Count > 0)
            {
                dataGrid141.Focus();
                dataGrid141.SelectedIndex = 0;
                decimal subtotal = Convert.ToDecimal(dt141.Compute("Sum(subtotal)", "").ToString());
                decimal iva = Convert.ToDecimal(dt141.Compute("Sum(val_iva)", "").ToString());
                TxtSubtotal.Text = subtotal.ToString("N2");
                Txtiva.Text = iva.ToString("N2");
                TxtTotal.Text = (subtotal + iva).ToString("N2");
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            _sqlFacturas.Clear();
            if (FecIni.Text.Trim() == "")
            {
                MessageBox.Show("Falta fecha de proceso...");
                FecIni.Focus();
                return;
            }
            Trae141();
            //dt141.ExportToExcel("wilmer");
            // cruza 
            if (dt141.Rows.Count > 0)
            {
                dataGrid141.Focus();
                dataGrid141.SelectedIndex = 0;
                dataGrid141.UpdateLayout();
            }
            this.UpdateLayout();
            FecIni.Focusable = false;
        }
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            
            _sqlFacturas.Clear();
            if (FecIni.Text.Trim() == "")
            {
                MessageBox.Show("Falta fecha de proceso...");
                FecIni.Focus();
                return;
            }
            dt141.Clear();
            Trae141();
            // valida que todos los documentos esten cruzados
            DataRow[] drow = dt141.Select("est_cruc=0");
            if(drow.Length>0)
            {
                MessageBox.Show("Proceso detenido, Existen Traslados sin cruce en la empresa destino....");
                return;
            }
            this.Suma();
            DataTable dt = dt141.AsEnumerable().GroupBy(r => r.Field<string>("bod_tra")).Select(g => g.First()).CopyToDataTable();
            //SiaWin.Browse(dt);
            foreach (DataRow row in dt.Rows)
            {
                string bodcruzar = row["bod_tra"].ToString().Trim();
                string bodempresa = SiaWin.Func.cmpCodigo("inmae_bod", "cod_bod", "cod_emp", bodcruzar, idEmp);
                string bodcodter = SiaWin.Func.cmpCodigo("inmae_bod", "cod_bod", "cod_ter", bodcruzar, idEmp);
                if(string.IsNullOrEmpty(bodcodter.Trim()))
                {
                    MessageBox.Show("Falta codigo de terecero en la bodega:" + bodcruzar + " de la empresa:" + bodempresa);
                    return;
                }
                if (string.IsNullOrEmpty(bodempresa.Trim()))
                {
                    MessageBox.Show("La bodega " + bodcruzar + " no tiene empresa asignada...");
                }
                else
                {
                    int idempresa = EmpresasID(bodempresa);
                    if (idempresa > 0)
                    {
                        if(!Facturar(bodcruzar, idempresa))
                        {
                            MessageBox.Show("Error al crear factura en empresa:" + bodempresa + " Proceso detenido", "Factura");
                        }
                    }
                    //MessageBox.Show(_sqlFacturas.ToString(),"bODCRUZAR:"+bodcruzar+"-BODEMPRESA"+bodempresa+"-nIT:"+bodcodter);
                }
            }
            if(!string.IsNullOrEmpty(_sqlFacturas.ToString()))
            {
                //MessageBox.Show(_sqlFacturas.ToString(),"fINAL");

                //_sqlFacturas.Append(" Select * from @tmp ");
                StringBuilder _sqlFacturaFinal = new StringBuilder();
                _sqlFacturaFinal.Append(@"declare  @tmp table(cod_emp char(3), cod_trn char(3), num_trn char(12), idregcab int, evento char(200)) ");
                _sqlFacturaFinal.Append("begin transaction ");
                _sqlFacturaFinal.Append("BEGIN TRY ");
                _sqlFacturaFinal.Append(_sqlFacturas.ToString());
                _sqlFacturaFinal.Append("select * from @tmp ");
                _sqlFacturaFinal.Append("commit transaction ;");
                _sqlFacturaFinal.Append("END TRY ");
                _sqlFacturaFinal.Append("BEGIN CATCH ");
                _sqlFacturaFinal.Append("select ERROR_NUMBER() AS ErrorNumber , ERROR_MESSAGE() AS ErrorMessage; ");
                _sqlFacturaFinal.Append("IF @@TRANCOUNT > 0 ");
                _sqlFacturaFinal.Append(" begin ");
                _sqlFacturaFinal.Append(" ROLLBACK TRANSACTION; ");
                _sqlFacturaFinal.Append("END ");
                _sqlFacturaFinal.Append("END CATCH ");
                _sqlFacturaFinal.Append("IF @@TRANCOUNT > 0  COMMIT TRANSACTION; ");
                //Clipboard.SetText(_sqlFacturaFinal.ToString());
                DataTable dtFinal = null;
                dtFinal =   SiaWin.Func.SqlDT(_sqlFacturaFinal.ToString(),"dtfinal", idEmp);
                string exp = "cod_trn='005'";
                //Sia.Browse(dt.Select(exp).CopyToDataTable());
                dtFacturas = dtFinal.Select(exp).CopyToDataTable();
                //dtFacturas = null;
                Auditoria(dtFinal, _sqlFacturaFinal.ToString(),idEmp);
                SiaWin.Browse(dtFinal,true);
                dt141.Clear();
                return;
                if (MessageBox.Show("Usted desea imprimir facturas....?","Imprimir Factuas",MessageBoxButton.YesNo,MessageBoxImage.Question,MessageBoxResult.Yes)==MessageBoxResult.Yes)
                {
                    DataRow[] dtFacImprimir = dtFinal.Select("cod_trn='005'");
                    foreach(DataRow dr in dtFacImprimir)
                    {
                        int idregcab = Convert.ToInt32(dr["idregcab"].ToString());
                        MessageBox.Show(idregcab.ToString());
                        //SiaWin.Func.ImprimeFacturaCreditoAutomatico(idregcab, false,idEmp,idBod, codpvta);
                        SiaWin.Func.ImprimeFacturaCreditoAutomatico(idregcab, false, 1500000, idEmp, codemp, codpvta, codpvta);
                    }
                }
                dt141.Clear();
            }
        }
        private bool Facturar(string bodfacturar,int idempresa)
        {
            string bodempresa = SiaWin.Func.cmpCodigo("inmae_bod", "cod_bod", "cod_emp",bodfacturar, idEmp);
            string bodcodter = SiaWin.Func.cmpCodigo("inmae_bod", "cod_bod", "cod_ter", bodfacturar, idEmp);
            // resumen por documento

            DataTable newDtDoc = dt141.AsEnumerable().Where(p => p.Field<string>("bod_tra") == bodfacturar).GroupBy(r => r.Field<string>("num_trn"))
              .Select(g =>
              {
                  var row = dt141.NewRow();
                  row["num_trn"] = g.Key;
                  row["bod_tra"] = g.Max(r => r.Field<string>("bod_tra"));
                  return row;
              }).CopyToDataTable();

            //resumen por bodega y referencia
            DataTable newDt = dt141.AsEnumerable().Where(p => p.Field<string>("bod_tra") == bodfacturar ).GroupBy(r => r.Field<string>("cod_ref"))
              .Select(g =>
              {
                  var row = dt141.NewRow();
                  row["cod_ref"] = g.Key;
                  row["cantidad"] = g.Sum(r => r.Field<decimal>("cantidad"));
                  row["subtotal"] = g.Sum(r => r.Field<decimal>("subtotal"));
                  row["cod_tiva"] = g.Max(r => r.Field<string>("cod_tiva"));
                  row["por_iva"] = g.Max(r => r.Field<decimal>("por_iva"));
                  row["dto_pprv"] = g.Max(r => r.Field<decimal>("dto_pprv"));
                  return row;
              }).CopyToDataTable();
            //MessageBox.Show("registros. totales por referencia:"+newDt.Rows.Count.ToString());
            if (newDt!=null & newDt.Rows.Count>0)
            {
                //_sqlFacturas.Clear();
                Consecutivo = @"; declare @fecdoc__x as datetime;set @fecdoc__x = '"+FecIni.Text.ToString()+"'  ;declare @ini__x as char(4);declare @num__x as varchar(12);declare @iConsecutivo__x char(12) = '' ;declare @iFolioHost__x int = 0;UPDATE COpventas SET fac_credito = ISNULL(fac_credito, 0) + 1  WHERE cod_pvt='" + codpvta + "';SELECT @iFolioHost__x = fac_credito,@ini__x=rtrim(ini_cred) FROM Copventas  WHERE cod_pvt='" + codpvta + "';set @num__x=@iFolioHost__x;select @iConsecutivo__x=rtrim(@ini__x)+rtrim(convert(varchar,@num__x));DECLARE @NewTrn_005 INT;DECLARE @NewTrn_001 INT;DECLARE @NewTrnN_005 INT;DECLARE @NewTrnN_001 INT;";
                string sqlFac = GeneraScript(bodfacturar,newDt, newDtDoc, bodcodter, bodempresa, idempresa);
                //MessageBox.Show(sqlFac);
                if(string.IsNullOrEmpty(sqlFac.Trim()))
                {
                    return false;
                }
                else
                {
                    _sqlFacturas.Append(sqlFac);
                    _sqlFacturas.Replace("__x", "__" + bodfacturar.Trim());
                    _sqlFacturas.Replace("NewTrn_005", "NewTrnDoc_005" + bodfacturar.Trim());
                    _sqlFacturas.Replace("NewTrn_001", "NewTrnDoc_001" + bodfacturar.Trim());
                    _sqlFacturas.Replace("NewTrnN_005", "NewTrnDocN_005" + bodfacturar.Trim());//niif
                    _sqlFacturas.Replace("NewTrnN_001", "NewTrnDocN_001" + bodfacturar.Trim());//niff
                    return true;
                }
            }  
            //foreach (DataRow drow in newDt.Rows)
           // {
             //   //DataRow drow = newDt.Rows[0];
               // MessageBox.Show(drow["cod_ref"].ToString() + "-cnt:"+ drow["cantidad"].ToString() +"-Ref:"+ drow["cod_ref"].ToString() + "-subtotal" + drow["subtotal"].ToString() + "-poriva" + drow["por_iva"].ToString());
                //MessageBox.Show(newDt.Rows.Count.ToString());
                //MessageBox.Show("facturar:" + bodfacturar);
           // }
            return false;
        }

        private string GeneraScript(string bodFactura,DataTable newdt, DataTable DtDoc, string codter,string bodempresa,int idempresa)
        {
            //MessageBox.Show("entra a genera script");
            StringBuilder __sbSql = new StringBuilder();
            try
            {
                // empresa 1

                if (_sqlFacturas.ToString() != "")
                {  

                }
                 
                    string sqlcab005 = @" INSERT INTO incab_doc (cod_trn,fec_trn,cod_cli,num_trn,doc_ref,des_mov,bod_tra,estado,for_pag,dia_pla,fec_ven,cod_ven,suc_cli) values ('005',DATEADD(hour,20,@fecdoc__x),'" + codter.Trim()+"',@iConsecutivo__x,@iConsecutivo__x,'Factura InterEmpresa','" + idBod + "',9,'30',90,dateadd(DAY,90,@fecdoc__x),'95','"+bodFactura+ "');SELECT @NewTrn_005 = SCOPE_IDENTITY();"+Environment.NewLine;
                    sqlcab005 = sqlcab005 + " insert @tmp (cod_emp,cod_trn,num_trn,idregcab,evento) values ('" + codemp + "','005',@iConsecutivo__x,@NewTrn_005,'Factura Venta Traslado InterEmpresa codemp=" + codemp + " Doc: 04-'+@iConsecutivo__x)";
                    string sqlcab001 = @" INSERT INTO incab_doc (cod_trn,num_trn,fec_trn,cod_prv,est_pago,estado,doc_ref,dia_pla,fec_ven,des_mov,bod_tra) values ('001',@iConsecutivo__x,Convert(date,@fecdoc__x),'"+nitemp.Trim()+ "',2,9,@iConsecutivo__x,90,dateadd(DAY,90,@fecdoc__x),'COMPRA AUTOMATICA','" + bodFactura + "');SELECT @NewTrn_001 = SCOPE_IDENTITY();";
                    sqlcab001 = sqlcab001 + " insert @tmp (cod_emp,cod_trn,num_trn,idregcab,evento) values ('" + bodempresa + "','001',@iConsecutivo__x,@NewTrn_001,'Factura Compra Traslado InterEmpresa codemp=" + bodempresa + " Doc: 04-'+@iConsecutivo__x)"; 
                    string sqlInsert005 = string.Empty;
                    string sqlInsert001 = string.Empty;
                    decimal porret = Convert.ToDecimal("2.5");
                    decimal porica = Convert.ToDecimal("1.104");
                    decimal __subtotal = 0;decimal __valiva = 0; decimal __valica = 0, __valret = 0;decimal __costotn = 0; 
                    foreach (System.Data.DataRow dr in newdt.Rows)
                    {
                        //MessageBox.Show(dr["cod_ref"].ToString() + " cantidad:" + dr["cantidad"].ToString());
                        decimal _cantidad = Convert.ToDecimal(dr["cantidad"].ToString());
                        decimal _subtotal = Convert.ToDecimal(dr["subtotal"].ToString());
                        decimal _poriva = Convert.ToDecimal(dr["por_iva"].ToString());

                        //MessageBox.Show("dto_pprv");
                        decimal _dtointer = Convert.ToDecimal(dr["dto_pprv"].ToString());
                        decimal _valuni = Math.Round(_subtotal / _cantidad, 0);
                    //MessageBox.Show("dto_pprv 1");
                    decimal _valunin = Math.Round(_valuni *(1- _dtointer/100),0) ;
                    //MessageBox.Show("dto_pprv 2");
                    decimal _costotn = _cantidad * _valunin;
                    //MessageBox.Show("dto_pprv 3");
                    decimal _valiva = 0;
                        if(_poriva>0) _valiva = (_subtotal * _poriva) / 100;
                        //repl all val_ret with subtotal * 2.5 / 100,val_ica with subtotal * 1.104 / 100,val_requ with 0
                        decimal _valret = (_subtotal * porret) / 100;
                        //decimal _valica = (_subtotal * porica) / 100;
                        decimal _valica = 0;
                        //decimal cosunin = 0;

                        sqlInsert005 = sqlInsert005 + @" INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cod_sub,cantidad,subtotal,val_uni,cod_tiva,por_iva,val_iva,cos_uni,cos_tot,val_ret,val_ica,tot_tot) values (@NewTrn_005,'005',@iConsecutivo__x,'" + dr["cod_ref"].ToString().Trim()+"','"+ idBod + "','001'," +_cantidad.ToString("F", CultureInfo.InvariantCulture) +","+ _subtotal.ToString("F", CultureInfo.InvariantCulture)+","+ _valuni.ToString("F", CultureInfo.InvariantCulture)+",'"+ dr["cod_tiva"].ToString()+"',"+_poriva.ToString("F", CultureInfo.InvariantCulture)+","+ _valiva.ToString("F", CultureInfo.InvariantCulture)+"," + _valuni.ToString("F", CultureInfo.InvariantCulture)+"," + _subtotal.ToString("F", CultureInfo.InvariantCulture)+","+ _valret.ToString("F", CultureInfo.InvariantCulture)+","+ _valica.ToString("F", CultureInfo.InvariantCulture)+","+ (_subtotal+_valiva).ToString("F", CultureInfo.InvariantCulture)+");";
                        sqlInsert001 = sqlInsert001 + @" INSERT INTO incue_doc (idregcab,cod_trn,num_trn,cod_ref,cod_bod,cod_sub,cantidad,cos_uni,cos_tot,cod_tiva,por_iva,val_iva,val_ret,val_ica,cos_unin,cos_totn) values (@NewTrn_001,'001',@iConsecutivo__x,'" + dr["cod_ref"].ToString().Trim() + "','" +bodFactura + "','050'," +_cantidad.ToString("F", CultureInfo.InvariantCulture) +","+ _valuni.ToString("F", CultureInfo.InvariantCulture)+ "," +_subtotal.ToString("F", CultureInfo.InvariantCulture)+",'"+ dr["cod_tiva"].ToString()+"',"+ _poriva.ToString("F", CultureInfo.InvariantCulture)+","+ _valiva.ToString("F", CultureInfo.InvariantCulture)+","+_valret.ToString("F", CultureInfo.InvariantCulture)+",0," + _valunin.ToString("F", CultureInfo.InvariantCulture)+","+_costotn.ToString("F", CultureInfo.InvariantCulture)+"); ";
                        __subtotal = __subtotal + _subtotal;
                        __valiva = __valiva + _valiva;
                        __valret = __valret + _valret;
                        __valica = __valica + _valica;
                        __costotn = __costotn + _subtotal - _costotn;

                }
                __valiva = Math.Round(__valiva, 0);
                __valret = Math.Round(__valret, 0);
                //arma string de contabilidad factura 005 y compra 001
                string sqlcab005co = @" INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,dia_plaz,fec_ven,cod_ven,suc_cli) values ('04',@iConsecutivo__x,@fecdoc__x,90,dateadd(DAY,90,@fecdoc__x),'95','" + bodFactura + "');SELECT @NewTrn_005 = SCOPE_IDENTITY() ; ";
                    sqlcab005co = sqlcab005co + @" INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,dia_plaz,fec_ven,cod_ven,suc_cli) values ('04N',@iConsecutivo__x,@fecdoc__x,90,dateadd(DAY,90,@fecdoc__x),'95','" + bodFactura + "');SELECT @NewTrnN_005 = SCOPE_IDENTITY() ; ";
                    sqlcab005co = sqlcab005co + " insert @tmp (cod_emp,cod_trn,num_trn,idregcab,evento) values ('" + codemp + "','04',@iConsecutivo__x,@NewTrn_005,'Factura Venta Traslado InterEmpresa codemp="+codemp+ " Doc: 04-'+@iConsecutivo__x)";
                    sqlcab005co = sqlcab005co + " insert @tmp (cod_emp,cod_trn,num_trn,idregcab,evento) values ('" + codemp + "','04N',@iConsecutivo__x,@NewTrn_005,'Factura Venta Traslado InterEmpresa codemp=" + codemp + " Doc: 04N-'+@iConsecutivo__x)";
                string sqlcab001co = @" INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,dia_plaz,fec_ven,cod_ven,factura) values ('18',@iConsecutivo__x,@fecdoc__x,90,dateadd(DAY,90,@fecdoc__x),'95',@iConsecutivo__x);SELECT @NewTrn_001 = SCOPE_IDENTITY() ;";
                    sqlcab001co = sqlcab001co + @" INSERT INTO cocab_doc (cod_trn,num_trn,fec_trn,dia_plaz,fec_ven,cod_ven,factura) values ('18N',@iConsecutivo__x,@fecdoc__x,90,dateadd(DAY,90,@fecdoc__x),'95',@iConsecutivo__x);SELECT @NewTrnN_001 = SCOPE_IDENTITY() ;";
                    sqlcab001co = sqlcab001co + " insert @tmp (cod_emp,cod_trn,num_trn,idregcab,evento) values ('" + bodempresa + "','18',@iConsecutivo__x,@NewTrn_005,'Factura Compra Traslado InterEmpresa codemp=" + bodempresa + " Doc: 18-'+@iConsecutivo__x)";
                    sqlcab001co = sqlcab001co + " insert @tmp (cod_emp,cod_trn,num_trn,idregcab,evento) values ('" + bodempresa + "','18N',@iConsecutivo__x,@NewTrn_005,'Factura Compra Traslado InterEmpresa codemp=" + bodempresa + " Doc: 18N-'+@iConsecutivo__x)";

                //005////
                //ingreso
                string sqlcue005co = @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,cre_mov) values (@NewTrn_005,'04',@iConsecutivo__x,'41350601','" + codter.Trim() + "','099','VENTA DE MERCANCIA-'," + __subtotal.ToString("F", CultureInfo.InvariantCulture) + ") ";
                    //iva 
                     sqlcue005co = sqlcue005co+ @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov,cre_mov) values (@NewTrn_005,'04',@iConsecutivo__x,'24080507','" + codter.Trim() + "','','VENTA DE MERCANCIA-'+@iConsecutivo__x," + __subtotal.ToString("F", CultureInfo.InvariantCulture) +","+ __valiva.ToString("F", CultureInfo.InvariantCulture)+ ")  ";
                    //retefte
                    sqlcue005co = sqlcue005co + @"insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov,deb_mov) values (@NewTrn_005,'04',@iConsecutivo__x,'13551505','" + codter.Trim() + "','','VENTA DE MERCANCIA-'+@iConsecutivo__x," + __subtotal.ToString("F", CultureInfo.InvariantCulture) + "," + __valret.ToString("F", CultureInfo.InvariantCulture) + ") ";

                    // xrequ=round(xsubt*0.40/100,0)
                    decimal requ = Convert.ToDecimal("0.40");
                    decimal xrequ = Math.Round((__subtotal *requ) / 100);
                    sqlcue005co = sqlcue005co + @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov,deb_mov) values (@NewTrn_005,'04',@iConsecutivo__x,'13551530','" + codter.Trim() + "','','VENTA DE MERCANCIA-'+@iConsecutivo__x," + __subtotal.ToString("F", CultureInfo.InvariantCulture) + "," + xrequ.ToString("F", CultureInfo.InvariantCulture) + ")";
                    sqlcue005co = sqlcue005co + @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov,cre_mov) values (@NewTrn_005,'04',@iConsecutivo__x,'23657530','" + codter.Trim()+ "','','VENTA DE MERCANCIA-'+@iConsecutivo__x," + __subtotal.ToString("F", CultureInfo.InvariantCulture) + "," + xrequ.ToString("F", CultureInfo.InvariantCulture) + ")";
                //total cartera
 
                sqlcue005co = sqlcue005co + @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,deb_mov,saldo,doc_ref) values (@NewTrn_005,'04',@iConsecutivo__x,'13050505','" + codter.Trim() + "','','VENTA DE MERCANCIA-'+@iConsecutivo__x," + (__subtotal + __valiva - __valret).ToString("F", CultureInfo.InvariantCulture)+","+(__subtotal + __valiva - __valret).ToString("F", CultureInfo.InvariantCulture)+ ",@iConsecutivo__x) ";
                    //001////
                    //ingreso COMPRA
                    string sqlcue001co = @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,deb_mov) values (@NewTrn_001,'18',@iConsecutivo__x,'14350601','" + nitemp + "','099','COMPRA MERCANCIA-'+@iConsecutivo__x," + __subtotal.ToString("F", CultureInfo.InvariantCulture) + ") ";
                    //iva 
                    sqlcue001co = sqlcue001co + @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov,deb_mov) values (@NewTrn_001,'18',@iConsecutivo__x,'24081007','" + nitemp + "','','COMPRA MERCANCIA-'+@iConsecutivo__x," + __subtotal.ToString("F", CultureInfo.InvariantCulture) + "," + __valiva.ToString("F", CultureInfo.InvariantCulture) + ") ";
                    //retefte
                    sqlcue001co = sqlcue001co + @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,bas_mov,cre_mov) values (@NewTrn_001,'18',@iConsecutivo__x,'236540','" + nitemp + "','','COMPRA MERCANCIA-'+@iConsecutivo__x," + __subtotal.ToString("F", CultureInfo.InvariantCulture) + "," + __valret.ToString("F", CultureInfo.InvariantCulture) + ") ";
                    //total CXP
                    sqlcue001co = sqlcue001co + @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,cre_mov,saldo,doc_ref) values (@NewTrn_001,'18',@iConsecutivo__x,'220505','" + nitemp + "','','COMPRA MERCANCIA-'+@iConsecutivo__x," + (__subtotal + __valiva - __valret).ToString("F", CultureInfo.InvariantCulture) + ","  + (__subtotal + __valiva - __valret).ToString("F", CultureInfo.InvariantCulture) + ",@iConsecutivo__x) ";
                    //contabiliza niif 005 cue
                    sqlcue005co= sqlcue005co + @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,cre_mov) values (@NewTrnN_005,'04N',@iConsecutivo__x,'26054502','" + codter.Trim() + "','099','VENTA DE MERCANCIA-'," + __costotn.ToString("F", CultureInfo.InvariantCulture) + ") ";
                    sqlcue005co = sqlcue005co + @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,deb_mov,saldo,doc_ref) values (@NewTrnN_005,'04N',@iConsecutivo__x,'530535','" + codter.Trim() + "','','VENTA DE MERCANCIA-'+@iConsecutivo__x," + (__costotn).ToString("F", CultureInfo.InvariantCulture) + "," + (__subtotal + __valiva - __valret).ToString("F", CultureInfo.InvariantCulture) + ",@iConsecutivo__x) ";
                    //contabiliza niif 001 cue NIIF
                    sqlcue001co =  sqlcue001co + @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,cre_mov) values (@NewTrnN_001,'18N',@iConsecutivo__x,'14350601','" + nitemp + "','099','COMPRA MERCANCIA-'+@iConsecutivo__x," + __costotn.ToString("F", CultureInfo.InvariantCulture) + ") ";
                    sqlcue001co = sqlcue001co + @" insert into cocue_doc (idregcab,cod_trn,num_trn,cod_cta,cod_ter,cod_cco,des_mov,deb_mov,saldo,doc_ref) values (@NewTrnN_001,'18N',@iConsecutivo__x,'220590','" + nitemp + "','','COMPRA MERCANCIA-'+@iConsecutivo__x," + (__costotn).ToString("F", CultureInfo.InvariantCulture) + "," + (__subtotal + __valiva - __valret).ToString("F", CultureInfo.InvariantCulture) + ",@iConsecutivo__x) ";



                StringBuilder sbUpdate160 = new StringBuilder();
                    StringBuilder sbUpdate060 = new StringBuilder();
                    foreach (DataRow dr in DtDoc.Rows)
                    {
                        sbUpdate160.Append(" update incue_doc set cantidad=0,cos_uni=0,cos_tot=0,subtotal=0,val_iva=0,val_ret=0,tot_tot=0 where cod_trn='160' and num_trn='" + dr["num_trn"].ToString()+"' ");
                        sbUpdate060.Append(" update incue_doc set cantidad=0,cos_uni=0,cos_tot=0,subtotal=0,val_iva=0,val_ret=0,tot_tot=0 where cod_trn='060' and num_trn='" + dr["num_trn"].ToString()+"' ");
                    }
                    // arma string 
                    // FACTURA EMPRESA PRINCIPAL
                    __sbSql.Append("USE GRUPOSAAVEDRA_EMP"+codemp+" ");
                    __sbSql.Append(Consecutivo);
                    __sbSql.Append(sqlcab005.ToString());
                    __sbSql.Append(sqlInsert005.ToString());
                    __sbSql.Append(sqlcab005co);
                    __sbSql.Append(sqlcue005co);
                    
                    __sbSql.Append(sbUpdate160.ToString());
                    // ARMA COMPRA 001
                    __sbSql.Append(" USE GRUPOSAAVEDRA_EMP" +bodempresa);
                    __sbSql.Append(sqlcab001.ToString());
                    __sbSql.Append(sqlInsert001.ToString());
                    __sbSql.Append(sqlcab001co);
                    __sbSql.Append(sqlcue001co);
                    __sbSql.Append(sbUpdate060.ToString());
                //MessageBox.Show(__sbSql.ToString());
                    //Clipboard.SetText(__sbSql.ToString());
                    // UPDATE 160-060 A 0
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "TrasladoAutomaticoEmpresa-GeneraSript");
                return string.Empty;
            }
            //MessageBox.Show(__sbSql.ToString(), "Generascrip");
            return __sbSql.ToString();
        }

        private void Cruzar141_051(string bodcruce,int idempresa)
        {
            dataGrid141.UpdateLayout();
            StringBuilder sbsql = new StringBuilder();
            sbsql.Append(" select cab.idreg,cab.cod_trn,cab.num_trn,cue.cod_bod,cab.bod_tra ");
            sbsql.Append(" from incab_doc cab inner join incue_doc cue on cab.idreg = cue.idregcab ");
            sbsql.Append(" where cab.cod_trn = '060' and convert(date, fec_trn,103)= '"+FecIni.Text+"' and estado = 9 and cue.cod_bod = '"+bodcruce+"' and cab.bod_tra = '"+idBod+"' ");
            sbsql.Append(" group by cab.idreg,cab.cod_trn,cab.num_trn,cue.cod_bod,cab.bod_tra ");
            DataTable dtCruzar141_051 = SiaWin.Func.SqlDT(sbsql.ToString(), "trasl141", idempresa);
            foreach(DataRow row in dt141.Rows)
            {
                string numcruzar = row["num_trn"].ToString().Trim();
                //MessageBox.Show("curzar:" + numcruzar);
                DataRow[] drow = dtCruzar141_051.Select("cod_trn='060' and num_trn='" + numcruzar + "'");
                //MessageBox.Show(drow.Length.ToString());
                if(drow.Length>0)
                {
                    row.BeginEdit();
                    row["trn_tras"] = "060";
                    row["num_tras"] = drow[0]["num_trn"].ToString();
                    row["est_cruc"] = 1;
                    row.EndEdit();
                }
            }
        }
        private int EmpresasID(string codemp)
        {
            int idreturn = -1;
            DataRow foundRow = Empresas.Rows.Find(codemp);
            if(foundRow!=null) idreturn = Convert.ToInt32(foundRow["BusinessId"].ToString().Trim());
            if (idreturn < 0) MessageBox.Show("Codiog de empresa no Existe..:" + codemp, "EmpresasID");
            return idreturn;
        }
        private void AuditoriaDoc(int iddoc, string evento, int idemp)
        {
            DataTable dtAud = new DataTable();
            dtAud = SiaWin.Func.SqlDT("select cod_trn,num_trn from incab_doc where idreg=" + iddoc, "tmp", idemp);
            if (dtAud.Rows.Count > 0)
            {
                string __audCodTrn = dtAud.Rows[0]["cod_trn"].ToString();
                string __audNumTrn = dtAud.Rows[0]["num_trn"].ToString();
                string titulo = string.Empty;
                if (__audCodTrn == "005") titulo = " Factura Credito ";

                string _BusinessName = foundRow["BusinessName"].ToString().Trim();
                //SiaWin.Seguridad.Auditor(0, _usercontrol.ProjectId, _usercontrol.UserId, _usercontrol.GroupId, _usercontrol.BusinessId, _usercontrol.ModuleId, -1, 0, evento + " " + titulo + " " + __audCodTrn + "/" + __audNumTrn + " - Modulo:PV" + "-Año:" + Ano + "-Periodo:" + PeriodoName + " Empresa:" + _BusinessName.Trim(), "");

            }

        }
        private void Auditoria(DataTable dt,string sqlevent,int idempresa)
        {
            SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idempresa, 2, -1, 0, this.Title +" -Dia:"+ this.FecIni.Text.ToString()  , "");
            foreach (DataRow row in dt.Rows)
            {
                string numtrn = row["cod_trn"].ToString().Trim();
                string numdoc = row["num_trn"].ToString().Trim();
                string evento = row["evento"].ToString().Trim();
                SiaWin.seguridad.Auditor(0, SiaWin._ProyectId, SiaWin._UserId, SiaWin._UserGroup, idempresa, 2, -1, 0,"Factura InterEmpresa: Dia:"+this.FecIni.Text.ToString()+" -" +evento , "");
            }
        }
    }

}

