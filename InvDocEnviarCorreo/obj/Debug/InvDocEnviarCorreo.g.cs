﻿#pragma checksum "..\..\InvDocEnviarCorreo.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "B725406479CCB76526C8F40F7A1F638355C3FB28F481C7AA050BECF575A55850"
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
    /// InvDocEnviarCorreo
    /// </summary>
    public partial class InvDocEnviarCorreo : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 41 "..\..\InvDocEnviarCorreo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxDocum;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\InvDocEnviarCorreo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox tx_coore;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\InvDocEnviarCorreo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox tx_pass;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\InvDocEnviarCorreo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Tx_des;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\InvDocEnviarCorreo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cob_smpt;
        
        #line default
        #line hidden
        
        
        #line 58 "..\..\InvDocEnviarCorreo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Tx_Asu;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\InvDocEnviarCorreo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnClick;
        
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
            System.Uri resourceLocater = new System.Uri("/InvDocEnviarCorreo;component/invdocenviarcorreo.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\InvDocEnviarCorreo.xaml"
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
            
            #line 9 "..\..\InvDocEnviarCorreo.xaml"
            ((SiasoftAppExt.InvDocEnviarCorreo)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TxDocum = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.tx_coore = ((System.Windows.Controls.TextBox)(target));
            return;
            case 4:
            this.tx_pass = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 5:
            this.Tx_des = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.cob_smpt = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.Tx_Asu = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.BtnClick = ((System.Windows.Controls.Button)(target));
            
            #line 61 "..\..\InvDocEnviarCorreo.xaml"
            this.BtnClick.Click += new System.Windows.RoutedEventHandler(this.BtnClick_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

