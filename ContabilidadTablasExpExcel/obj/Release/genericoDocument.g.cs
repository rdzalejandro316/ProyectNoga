﻿#pragma checksum "..\..\genericoDocument.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "C2DFCDB1883CA83D522F8611C18DCD71B79055CA13AC9C6D9EFFC7F6E40CABE5"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using ContabilidadTablasExpExcel;
using Syncfusion;
using Syncfusion.UI.Xaml.Controls.DataPager;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.RowFilter;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.Windows;
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


namespace ContabilidadTablasExpExcel {
    
    
    /// <summary>
    /// genericoDocument
    /// </summary>
    public partial class genericoDocument : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 50 "..\..\genericoDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tx_transacion;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\genericoDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker fec_ini;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\genericoDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker fec_fin;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\genericoDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BTNconsultar;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\genericoDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.Windows.Controls.Notification.SfBusyIndicator sfBusyIndicator;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\genericoDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Syncfusion.UI.Xaml.Grid.SfDataGrid dataGrid;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\genericoDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock Txreg;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\genericoDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnExportar;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\genericoDocument.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnPdf;
        
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
            System.Uri resourceLocater = new System.Uri("/ContabilidadTablasExpExcel;component/genericodocument.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\genericoDocument.xaml"
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
            this.tx_transacion = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.fec_ini = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 3:
            this.fec_fin = ((System.Windows.Controls.DatePicker)(target));
            return;
            case 4:
            this.BTNconsultar = ((System.Windows.Controls.Button)(target));
            
            #line 58 "..\..\genericoDocument.xaml"
            this.BTNconsultar.Click += new System.Windows.RoutedEventHandler(this.BTNconsultar_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.sfBusyIndicator = ((Syncfusion.Windows.Controls.Notification.SfBusyIndicator)(target));
            return;
            case 6:
            this.dataGrid = ((Syncfusion.UI.Xaml.Grid.SfDataGrid)(target));
            return;
            case 7:
            this.Txreg = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 8:
            this.BtnExportar = ((System.Windows.Controls.Button)(target));
            
            #line 81 "..\..\genericoDocument.xaml"
            this.BtnExportar.Click += new System.Windows.RoutedEventHandler(this.BtnExportar_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.BtnPdf = ((System.Windows.Controls.Button)(target));
            
            #line 82 "..\..\genericoDocument.xaml"
            this.BtnPdf.Click += new System.Windows.RoutedEventHandler(this.BtnPdf_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
