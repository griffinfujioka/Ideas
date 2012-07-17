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
    public partial class FujiokaPage : PhoneApplicationPage
    {
        public FujiokaPage()
        {
            InitializeComponent();
        }

        #region Email
        private void emailLink_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailCompose = new EmailComposeTask();

            emailCompose.Subject = "Ideas";
            emailCompose.To = "fujiokag@hotmail.com";
            emailCompose.Show(); 
        }
        #endregion 

        #region View website (Tumblr)
        private void websiteLink_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask browserTask = new WebBrowserTask();

            browserTask.URL = "www.griffinfujioka.tumblr.com";
            browserTask.Show(); 
        }
        #endregion 

        #region View twitter
        private void twitterLink_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask browserTask = new WebBrowserTask();

            browserTask.URL = "www.twitter.com/griffinfujioka";
            browserTask.Show();
        }
        #endregion 
    }
}