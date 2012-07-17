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

namespace Ideas.Views
{
    public partial class AboutUsPage : PhoneApplicationPage
    {
        public AboutUsPage()
        {
            InitializeComponent();
        }

        #region View the contributors page 
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/ContributorsPage.xaml", UriKind.Relative));
        }
        #endregion 
    }
}