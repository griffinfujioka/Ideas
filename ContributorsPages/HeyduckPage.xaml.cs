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
    public partial class HeyduckPage : PhoneApplicationPage
    {
        public HeyduckPage()
        {
            InitializeComponent();
        }

        private void twitterBtn_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask browserTask = new WebBrowserTask();

            browserTask.URL = "https://twitter.com/DavidHeyduck";
            browserTask.Show(); 
        }

        private void websiteBtn_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask browserTask = new WebBrowserTask();

            browserTask.URL = "http://davidheyduck.weebly.com/";
            browserTask.Show(); 
        }
    }
}