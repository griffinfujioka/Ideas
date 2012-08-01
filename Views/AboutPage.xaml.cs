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
using Ideas.Models;
using Ideas.ViewModels;
using Microsoft.Phone.Tasks; 

namespace Ideas.Views
{
    public partial class AboutPage : PhoneApplicationPage
    {
        #region Contributor object
        public partial class Contributor
        {
            string name;
            string email;
            string website;
            string twitter;

            public string Name
            {
                get { return name; }
                set { name = value; }
            }
        }
        #endregion

        List<Contributor> contributorsList = new List<Contributor>();

        #region Initialize the contributor list 
        public void InitializeContributorList()
        {

            Contributor Cambell = new Contributor()
            {
                Name = "Tyler Cambell"
            };
            contributorsList.Add(Cambell);

            Contributor Christie = new Contributor()
            {
                Name = "Wade Christie"
            };
            contributorsList.Add(Christie);

            Contributor Fujioka = new Contributor()
            {
                Name = "Griffin Fujioka"
            };
            contributorsList.Add(Fujioka);

            Contributor Heyduck = new Contributor()
            {
                Name = "David Heyduck"
            };
            contributorsList.Add(Heyduck); 

            Contributor Hintzke = new Contributor()
            {
                Name = "Matt Hintzke"
            };
            contributorsList.Add(Hintzke);

            Contributor Karcher = new Contributor()
            {
                Name = "Matt Karcher"
            };
            contributorsList.Add(Karcher);

            Contributor Wadagnolo = new Contributor()
            {
                Name = "Joel Wadagnolo"
            };
            contributorsList.Add(Wadagnolo);


        }
        #endregion 

        #region Constructor
        public AboutPage()
        {
            InitializeComponent();

            InitializeContributorList();

            listBox1.ItemsSource = contributorsList;
        }
        #endregion 

        #region Listbox selection changed
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
                return;

            var selectedName = ((sender as ListBox).SelectedItem as Contributor).Name;
            switch (selectedName)
            {
                case "Matt Hintzke":
                    NavigationService.Navigate(new Uri("/ContributorsPages/HintzkePage.xaml", UriKind.Relative));
                    break;
                case "Griffin Fujioka":
                    NavigationService.Navigate(new Uri("/ContributorsPages/FujiokaPage.xaml", UriKind.Relative));
                    break;
                case "David Heyduck":
                    NavigationService.Navigate(new Uri("/ContributorsPages/HeyduckPage.xaml", UriKind.Relative));
                    break; 
                default:
                    MessageBox.Show("There is no contact information available for that person", "", MessageBoxButton.OK);
                    listBox1.SelectedIndex = -1;
                    break;
            }
        }
        #endregion 

        #region On Navigated To
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            
            listBox1.SelectedIndex = -1;
            base.OnNavigatedTo(e);
        }
        #endregion 

        #region Rate and review button 
        private void ratereviewButton_Click(object sender, EventArgs e)
        {
           
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }
        #endregion 
    }
}