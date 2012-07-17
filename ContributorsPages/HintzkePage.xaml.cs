using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks; 

namespace Ideas.ContributorsPages
{
    public partial class HintzkePage : PhoneApplicationPage
    {
        public HintzkePage()
        {
            InitializeComponent();
        }

        private void emailLink_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask smsTask = new EmailComposeTask();

            smsTask.To = "codemaster@daemondeveloper.com";
            smsTask.Subject = "Ideas";
            smsTask.Show(); 
        }

        private void websiteLink_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask browserTask = new WebBrowserTask(); 

            browserTask.URL="http://www.daemondeveloper.com/"; 
            browserTask.Show(); 
        }

        private void twitterLink_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask browserTask = new WebBrowserTask();

            browserTask.URL = "https://twitter.com/DaemonDeveloper";
            browserTask.Show(); 
        }
    }
}