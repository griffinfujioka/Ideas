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
using Microsoft.Phone.Shell;
using Ideas.ViewModels;
using Ideas.Models;
using System.Threading;
using System.Collections.ObjectModel;
using Microsoft.Phone.Tasks;


namespace Ideas
{
    public partial class MainPage : PhoneApplicationPage
    {

        #region Constructor
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
            this.IdeasListBox.ItemsSource = App.ViewModel.Ideas;
        }
        #endregion 

        #region Add new idea
        private void newItemAppBarButton_Click(object sender, EventArgs e)
        {
            (DataContext as IdeaViewModel).AddNewIdea();
            NavigationService.Navigate(new Uri("/Views/IdeaPage.xaml", UriKind.Relative));
        }
        #endregion 

        #region View the about page
        private void aboutusMenuButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/AboutPage.xaml", UriKind.Relative));
        }
        #endregion 

        #region Listbox Selection Changed 
        // When the selection is changed, change the data context appropriately and go to the idea page 
        private void IdeaListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var button = (sender as ListBox).SelectedItem as Idea;
            App.ViewModel.SelectedIdea = button;

            if (button != null)
            {
                NavigationService.Navigate(new Uri("/Views/IdeaPage.xaml", UriKind.Relative));
            }
            return;
        }
        #endregion 

        #region On Navigated To 
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            // So the main page IdeaListBox won't have an idea selected 
            App.ViewModel.SelectedIdea = null;
            IdeasListBox.SelectedItem = null; 
            base.OnNavigatedTo(e);
        }
        #endregion 

        #region Share all ideas via email 
        private void shareIdeas_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            string IdeasList = "Here are some of my ideas: \n";

            int i = 1;
            foreach (Idea idea in App.ViewModel.Ideas)
            {
                IdeasList += "\n" + i + ". " + idea.Title + "\n";                                      // Print the idea title
                if (idea.Overview != "")    // if the overview is not empty or null, print that too
                    IdeasList += "Overview: " + idea.Overview;    // the overview 

                if (idea.SystemRequirements.Count != 0)
                {
                    IdeasList += "\nSystem Requirements";
                    foreach (SystemRequirement sr in idea.SystemRequirements)         // Print each of the system requirements 
                        IdeasList += "\n\t- " + sr.Requirement;
                }

                if (idea.UseCases.Count != 0)
                {
                    IdeasList += "\nUse Cases:";
                    foreach (UseCase uc in idea.UseCases)              // Print each of the use cases 
                        IdeasList += "\n\t- " + uc.UCase;
                }

                if (idea.Notes != "")
                    IdeasList += "\nOther notes: " + idea.Notes; 

                IdeasList += "\n";
                i++;

            }
            emailComposeTask.Subject = "My ideas";
            emailComposeTask.Body = IdeasList;

            emailComposeTask.Show();
        }
        #endregion 

        #region TODO: Secondary tile code 
        /// <summary>
        /// Navigate to the page for modifying secondary Tile properties. 
        /// Pass a parameter that lets the SecondaryTile page know that it was navigated to from MainPage.
        /// (DefaultTitle will equal 'FromTile' when the user navigates to the SecondaryTile page from a Tile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonChangeSecondaryTile_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/IdeaPage.xaml?DefaultTitle=FromMain", UriKind.Relative));
        }
        #endregion 
        
        #region Feedback button 
        private void feedbackButton_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailTask = new EmailComposeTask();

            emailTask.To = "wsuwpdg@hotmail.com";
            emailTask.Subject = "Ideas";
            emailTask.Show();
        }
        #endregion 

    }
}