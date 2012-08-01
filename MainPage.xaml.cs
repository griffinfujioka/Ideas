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
using Microsoft.Phone.Shell;
using Ideas.ViewModels;
using Ideas.Models;
using System.Threading;
using System.Collections.ObjectModel;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Controls; 


namespace Ideas
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region A list of different ways a user can share a wish: Social networks, email or SMS
        /* -------------------------------------------------------------
         * There's got to be a better way to do this but this is what I did.... 
         * 
         * This way, we can set the ItemsSource property of a ListPicker 
         * to the shareOptions list, then create a template and bind the items
         * to OptionName thus displaying all of the options for sharing a wish 
         ----------------------------------------------------------------*/
        List<ShareOption> shareOptions = new List<ShareOption>
        {
            new ShareOption
            { 
                OptionName = ""
            }, 

            new ShareOption
            {
                OptionName = "Social networks"
            }, 

            new ShareOption 
            {
                OptionName = "Email"
            }, 

            new ShareOption
            {
                OptionName = "SMS" 
            }
        };
        #endregion

        #region Constructor
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
            this.IdeasListBox.ItemsSource = App.ViewModel.Ideas;
            this.defaultListPicker.ItemsSource = shareOptions; 
            //this.menu.Opened += new RoutedEventHandler(menu_Opened); 
        }
        #endregion

        #region Add new idea
        private void newItemAppBarButton_Click(object sender, EventArgs e)
        {
            (DataContext as IdeaViewModel).AddNewIdea();
            NavigationService.Navigate(new Uri("/Views/IdeaPage.xaml", UriKind.Relative));
        }
        #endregion

        void menu_Opened(object sender, RoutedEventArgs e)
        {
            //add some content here
        }

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
            this.DataContext = App.ViewModel;
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


        #region Feedback button
        private void feedbackButton_Click(object sender, EventArgs e)
        {
            EmailComposeTask emailTask = new EmailComposeTask();

            emailTask.To = "wsuwpdg@hotmail.com";
            emailTask.Subject = "Ideas";
            emailTask.Show();
        }
        #endregion


        #region HubTile Hold
        private void HubTile_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var ideaName = (sender as HubTile).DataContext;
            App.ViewModel.SelectedIdea = (ideaName as Idea);
            this.DataContext = App.ViewModel;
        }
        #endregion 



        #region Delete idea click 
        private void DeleteIdea_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("Are you sure you want to delete this idea?", "", MessageBoxButton.OKCancel);
            if (m == MessageBoxResult.Cancel)
            {
                return;
            }

            Idea idea = App.ViewModel.SelectedIdea;

            string title = (DataContext as IdeaViewModel).SelectedIdea.Title;
            string orginalTitle = title;
            title = title.Replace(" ", "-");    // Replace white spaces with -
            int id = (DataContext as IdeaViewModel).SelectedIdea.Id;

            // Look to see whether the Tile already exists and if so, don't try to create it again.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=FromTile_" + id + "_" + title));

            if (TileToFind != null)      // this idea is pinned, delete it 
                TileToFind.Delete();

            (DataContext as IdeaViewModel).removeAllRequirements();
            (DataContext as IdeaViewModel).removeAllUseCases();
            (DataContext as IdeaViewModel).deleteIdea();

        }
        #endregion 

        #region Pin idea to start
        private void PinToStart_Click(object sender, RoutedEventArgs e)
        {

            string title = (DataContext as IdeaViewModel).SelectedIdea.Title;
            string orginalTitle = title;
            title = title.Replace(" ", "-");    // Replace white spaces with -
            int id = (DataContext as IdeaViewModel).SelectedIdea.Id;

            // Look to see whether the Tile already exists and if so, don't try to create it again.
            ShellTile TileToFind = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=FromTile_" + id + "_" + title));


            // Create the tile if we didn't find it 
            if (TileToFind == null)
            {

                // Create the tile object and set some initial properties for the tile.
                StandardTileData NewTileData = new StandardTileData
                {
                    Title = (DataContext as IdeaViewModel).SelectedIdea.Title,
                    BackgroundImage = new Uri("Tile.png", UriKind.Relative),


                    BackTitle = (DataContext as IdeaViewModel).SelectedIdea.Title,
                    BackBackgroundImage = null,
                    BackContent = (DataContext as IdeaViewModel).SelectedIdea.Overview,
                };

                ShellTile.Create(new Uri("/Views/IdeaPage.xaml?DefaultTitle=FromTile_" + id + "_" + title, UriKind.Relative), NewTileData);
            }
            else
            {
                string message = orginalTitle + " is already pinned.";
                MessageBox.Show(message);
            }
        }
        #endregion 

        #region Share idea click; Open the list picker
        private void shareIdea_Click(object sender, EventArgs e)
        {
            defaultListPicker.Open();
        }
        #endregion 

        #region List picker selection changed
        private void defaultListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (defaultListPicker.SelectedIndex)
            {
                case 0:             // blank, nothing 
                    break;
                case 1:             // social networks 
                    shareIdeaSocialNetworks_Click(sender, e);
                    break;
                case 2:             // email 
                    shareIdeaEmail_Click(sender, e);
                    break;
                case 3:             // sms 
                    shareIdeaSMS_Click(sender, e);
                    break;
            }
            defaultListPicker.SelectedIndex = 0;
        }
        #endregion 

        #region Share to social networks
        private void shareIdeaSocialNetworks_Click(object sender, EventArgs e)
        {
            ShareStatusTask shareStatusTask = new ShareStatusTask();

            shareStatusTask.Status = "Saved my " + (DataContext as IdeaViewModel).SelectedIdea.Title + " idea via Ideas http://www.windowsphone.com/en-US/apps/f18161dd-b3cf-4b7b-baf1-e161454939c1?fb_ref=wpcwam";
            shareStatusTask.Status += ". You interested?";
            shareStatusTask.Show();
        }
        #endregion

        #region Share via email
        private void shareIdeaEmail_Click(object sender, EventArgs e)
        {
            // Compose an email 
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            string Idea = "";

            Idea += (DataContext as IdeaViewModel).SelectedIdea.Title + "\n";
            emailComposeTask.Subject = "My " + Idea + " idea";

            if ((DataContext as IdeaViewModel).SelectedIdea.Overview != "")
                Idea += "\nOverview: " + (DataContext as IdeaViewModel).SelectedIdea.Overview;    // the overview 

            if ((DataContext as IdeaViewModel).SelectedIdea.SystemRequirements.Count != 0)
            {
                Idea += "\nSystem Requirements: ";
                foreach (SystemRequirement sr in (DataContext as IdeaViewModel).SelectedIdea.SystemRequirements)
                    Idea += "\n - " + sr.Requirement;
            }

            if ((DataContext as IdeaViewModel).SelectedIdea.UseCases.Count != 0)
            {
                Idea += "\nUse Cases: ";
                foreach (UseCase uc in (DataContext as IdeaViewModel).SelectedIdea.UseCases)
                    Idea += "\n - " + uc.UCase;
            }

            if ((DataContext as IdeaViewModel).SelectedIdea.Notes != "")
                Idea += "\nOther notes: " + (DataContext as IdeaViewModel).SelectedIdea.Notes;

            emailComposeTask.Body = Idea + "\n\nvia Ideas for Windows Phone";

            emailComposeTask.Show();
        }
        #endregion

        #region Share via sms
        private void shareIdeaSMS_Click(object sender, EventArgs e)
        {
            SmsComposeTask smsTask = new SmsComposeTask();

            string Idea = "I have an idea for " + (DataContext as IdeaViewModel).SelectedIdea.Title;

            if ((DataContext as IdeaViewModel).SelectedIdea.Overview != "")
                Idea += ". Basically, " + (DataContext as IdeaViewModel).SelectedIdea.Overview;    // the overview 

            smsTask.Body = Idea + ". Would you want to discuss it?";

            smsTask.Show();
        }
        #endregion 

    }
}