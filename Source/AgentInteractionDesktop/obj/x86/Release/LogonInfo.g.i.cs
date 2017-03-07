﻿#pragma checksum "..\..\..\LogonInfo.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B33BB04AA703C2D308807B3075E5FBD1"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Agent.Interaction.Desktop.CustomControls;
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


namespace Agent.Interaction.Desktop {
    
    
    /// <summary>
    /// LogonInfo
    /// </summary>
    public partial class LogonInfo : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 22 "..\..\..\LogonInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblTitle;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\LogonInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox gbVoiceChannel;
        
        #line default
        #line hidden
        
        
        #line 27 "..\..\..\LogonInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox chkbxVoice;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\LogonInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblQueue;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\LogonInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbQueue;
        
        #line default
        #line hidden
        
        
        #line 43 "..\..\..\LogonInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblAgentLogin;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\LogonInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblAgentLoginID;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\LogonInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblAgentPassword;
        
        #line default
        #line hidden
        
        
        #line 46 "..\..\..\LogonInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox txtAgentPassword;
        
        #line default
        #line hidden
        
        
        #line 49 "..\..\..\LogonInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnOk;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\LogonInfo.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCancel;
        
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
            System.Uri resourceLocater = new System.Uri("/Agent.Interaction.Desktop;component/logoninfo.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\LogonInfo.xaml"
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
            
            #line 7 "..\..\..\LogonInfo.xaml"
            ((Agent.Interaction.Desktop.LogonInfo)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\LogonInfo.xaml"
            ((Agent.Interaction.Desktop.LogonInfo)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.Window_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.lblTitle = ((System.Windows.Controls.Label)(target));
            
            #line 23 "..\..\..\LogonInfo.xaml"
            this.lblTitle.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.lblTitle_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.gbVoiceChannel = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 4:
            this.chkbxVoice = ((System.Windows.Controls.CheckBox)(target));
            
            #line 27 "..\..\..\LogonInfo.xaml"
            this.chkbxVoice.Unchecked += new System.Windows.RoutedEventHandler(this.chkbxVoice_Unchecked);
            
            #line default
            #line hidden
            
            #line 27 "..\..\..\LogonInfo.xaml"
            this.chkbxVoice.Checked += new System.Windows.RoutedEventHandler(this.chkbxVoice_Checked);
            
            #line default
            #line hidden
            return;
            case 5:
            this.lblQueue = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.cbQueue = ((System.Windows.Controls.ComboBox)(target));
            
            #line 41 "..\..\..\LogonInfo.xaml"
            this.cbQueue.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.PreviewKeyUp);
            
            #line default
            #line hidden
            
            #line 41 "..\..\..\LogonInfo.xaml"
            this.cbQueue.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.cbQueue_SelectionChanged);
            
            #line default
            #line hidden
            
            #line 42 "..\..\..\LogonInfo.xaml"
            this.cbQueue.KeyUp += new System.Windows.Input.KeyEventHandler(this.cbQueue_KeyUp);
            
            #line default
            #line hidden
            return;
            case 7:
            this.lblAgentLogin = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.lblAgentLoginID = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.lblAgentPassword = ((System.Windows.Controls.Label)(target));
            return;
            case 10:
            this.txtAgentPassword = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 46 "..\..\..\LogonInfo.xaml"
            this.txtAgentPassword.PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.PreviewKeyUp);
            
            #line default
            #line hidden
            return;
            case 11:
            this.btnOk = ((System.Windows.Controls.Button)(target));
            
            #line 49 "..\..\..\LogonInfo.xaml"
            this.btnOk.Click += new System.Windows.RoutedEventHandler(this.btnOk_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            this.btnCancel = ((System.Windows.Controls.Button)(target));
            
            #line 51 "..\..\..\LogonInfo.xaml"
            this.btnCancel.Click += new System.Windows.RoutedEventHandler(this.btnCancel_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

