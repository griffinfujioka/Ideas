﻿#pragma checksum "C:\Users\Griffin\Documents\Visual Studio 2010\Projects\Ideas\Ideas\Views\AboutPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E51B36F08FF28711059EB9DEA0EDD65C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Ideas.Views {
    
    
    public partial class AboutPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBlock createdBy;
        
        internal System.Windows.Controls.TextBlock AboutUscontent;
        
        internal System.Windows.Controls.TextBlock versionControl;
        
        internal System.Windows.Controls.Grid ContributorsPanel;
        
        internal System.Windows.Controls.ListBox listBox1;
        
        internal System.Windows.Controls.TextBlock textBlock1;
        
        internal System.Windows.Controls.TextBlock textBlock2;
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem ratereviewButton;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Ideas;component/Views/AboutPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.createdBy = ((System.Windows.Controls.TextBlock)(this.FindName("createdBy")));
            this.AboutUscontent = ((System.Windows.Controls.TextBlock)(this.FindName("AboutUscontent")));
            this.versionControl = ((System.Windows.Controls.TextBlock)(this.FindName("versionControl")));
            this.ContributorsPanel = ((System.Windows.Controls.Grid)(this.FindName("ContributorsPanel")));
            this.listBox1 = ((System.Windows.Controls.ListBox)(this.FindName("listBox1")));
            this.textBlock1 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock1")));
            this.textBlock2 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock2")));
            this.ratereviewButton = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("ratereviewButton")));
        }
    }
}
