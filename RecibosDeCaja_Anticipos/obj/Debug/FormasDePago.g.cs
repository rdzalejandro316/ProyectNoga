﻿#pragma checksum "..\..\FormasDePago.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "623AB5C24B5C1ACBDA85ED0B03C68BD1C95636A1"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using SiasoftAppExt;
using Syncfusion;
using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.RowFilter;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.Windows;
using Syncfusion.Windows.Controls.Grid;
using Syncfusion.Windows.Controls.Notification;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace RecibosDeCaja {
    
    
    /// <summary>
    /// FormasDePago
    /// </summary>
    public partial class FormasDePago : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 37 "..\..\FormasDePago.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox CBpagos;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\FormasDePago.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnAdd;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\FormasDePago.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnDel;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\FormasDePago.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TxtTotalRecaudo;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\FormasDePago.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.UI.Xaml.Grid.SfDataGrid dataGrid;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\FormasDePago.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.UI.Xaml.Grid.GridTextColumn ComboBanco;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\FormasDePago.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnGrabar;
        
        #line default
        #line hidden
        
        
        #line 91 "..\..\FormasDePago.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxtTotalPagado;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/RecibosDeCaja;component/formasdepago.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\FormasDePago.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 9 "..\..\FormasDePago.xaml"
            ((RecibosDeCaja.FormasDePago)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.Window_PreviewKeyDown);
            
            #line default
            #line hidden
            
            #line 9 "..\..\FormasDePago.xaml"
            ((RecibosDeCaja.FormasDePago)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CBpagos = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 3:
            this.BtnAdd = ((System.Windows.Controls.Button)(target));
            
            #line 38 "..\..\FormasDePago.xaml"
            this.BtnAdd.Click += new System.Windows.RoutedEventHandler(this.Btnadd_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.BtnDel = ((System.Windows.Controls.Button)(target));
            
            #line 43 "..\..\FormasDePago.xaml"
            this.BtnDel.Click += new System.Windows.RoutedEventHandler(this.BtnDel_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.TxtTotalRecaudo = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.dataGrid = ((Syncfusion.UI.Xaml.Grid.SfDataGrid)(target));
            
            #line 59 "..\..\FormasDePago.xaml"
            this.dataGrid.Loaded += new System.Windows.RoutedEventHandler(this.dataGrid_Loaded);
            
            #line default
            #line hidden
            
            #line 59 "..\..\FormasDePago.xaml"
            this.dataGrid.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.dataGrid_PreviewKeyDown_1);
            
            #line default
            #line hidden
            
            #line 59 "..\..\FormasDePago.xaml"
            this.dataGrid.CurrentCellEndEdit += new Syncfusion.UI.Xaml.Grid.CurrentCellEndEditEventHandler(this.dataGrid_CurrentCellEndEdit);
            
            #line default
            #line hidden
            return;
            case 7:
            this.ComboBanco = ((Syncfusion.UI.Xaml.Grid.GridTextColumn)(target));
            return;
            case 8:
            this.BtnGrabar = ((System.Windows.Controls.Button)(target));
            
            #line 82 "..\..\FormasDePago.xaml"
            this.BtnGrabar.Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 83 "..\..\FormasDePago.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            case 10:
            this.TxtTotalPagado = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

