﻿#pragma checksum "..\..\Deprecicion.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "96EBF6EF85964D114FF0B993A182EBF8D46CAD0CE109E82B13E0AB43C08E0951"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using SiasoftAppExt;
using Syncfusion;
using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.RowFilter;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.Windows;
using Syncfusion.Windows.Controls.Input;
using Syncfusion.Windows.Controls.Notification;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace SiasoftAppExt {
    
    
    /// <summary>
    /// Deprecicion
    /// </summary>
    public partial class Deprecicion : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 46 "..\..\Deprecicion.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Controls.Input.SfDatePicker Tx_ano;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\Deprecicion.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Controls.Input.SfDatePicker Tx_periodo;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\Deprecicion.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnConsultar;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\Deprecicion.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnDepreciar;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\Deprecicion.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnExportar;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\Deprecicion.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.UI.Xaml.Grid.SfDataGrid dataGrid;
        
        #line default
        #line hidden
        
        
        #line 114 "..\..\Deprecicion.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Controls.Notification.SfBusyIndicator sfBusyIndicator;
        
        #line default
        #line hidden
        
        
        #line 120 "..\..\Deprecicion.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Tx_toact;
        
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
            System.Uri resourceLocater = new System.Uri("/Deprecicion;component/deprecicion.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Deprecicion.xaml"
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
            
            #line 9 "..\..\Deprecicion.xaml"
            ((SiasoftAppExt.Deprecicion)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Tx_ano = ((Syncfusion.Windows.Controls.Input.SfDatePicker)(target));
            return;
            case 3:
            this.Tx_periodo = ((Syncfusion.Windows.Controls.Input.SfDatePicker)(target));
            return;
            case 4:
            this.BtnConsultar = ((System.Windows.Controls.Button)(target));
            
            #line 53 "..\..\Deprecicion.xaml"
            this.BtnConsultar.Click += new System.Windows.RoutedEventHandler(this.BtnConsultar_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.BtnDepreciar = ((System.Windows.Controls.Button)(target));
            
            #line 54 "..\..\Deprecicion.xaml"
            this.BtnDepreciar.Click += new System.Windows.RoutedEventHandler(this.BtnDepreciar_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.BtnExportar = ((System.Windows.Controls.Button)(target));
            
            #line 55 "..\..\Deprecicion.xaml"
            this.BtnExportar.Click += new System.Windows.RoutedEventHandler(this.BtnExportar_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.dataGrid = ((Syncfusion.UI.Xaml.Grid.SfDataGrid)(target));
            return;
            case 8:
            this.sfBusyIndicator = ((Syncfusion.Windows.Controls.Notification.SfBusyIndicator)(target));
            return;
            case 9:
            this.Tx_toact = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

