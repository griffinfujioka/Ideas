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
using System.Collections.ObjectModel;
using Ideas.Models;
using Ideas.ViewModels;
using Microsoft.Phone.Shell; 
namespace Ideas.Views
{
    public partial class IdeaPage : PhoneApplicationPage
    {
        #region A list of different ways a user can share a wish: Social networks or email
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
            } 
        };
        #endregion

        public ObservableCollection<SystemRequirement> SysReqsforBinding;
        public IdeaPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
            SysReqsforBinding = new ObservableCollection<SystemRequirement>((DataContext as IdeaViewModel).SelectedIdea.SysReqsOC);

            #region Initialize the ShellTile + an exception handler
            ShellTile secondaryTile =
                ShellTile.ActiveTiles.FirstOrDefault(
                x => x.NavigationUri
                .ToString()
                .Contains("TileID=Secondary"));
            #endregion


        }

        private void SaveItemAppBarButton_Click(object sender, EventArgs e)
        {
            // Confirm a title is provided 
            if (titleTextBox.Text.Length == 0)
            {
                MessageBox.Show("Please give your idea a name.");
                return;
            }
            else
            {

                (DataContext as IdeaViewModel).SelectedIdea.Title = titleTextBox.Text;          // Save title 
                (DataContext as IdeaViewModel).SelectedIdea.Overview = overviewTextBox1.Text;       //  overview
                (DataContext as IdeaViewModel).SelectedIdea.Notes = notesTextBox.Text;              // and notes 

                //// If the system requirements textbox isn't empty, save that
                if (systemReqsTextBox.Text != "")
                {
                    string reqTitle = systemReqsTextBox.Text;
                    var req = new SystemRequirement() { Requirement = reqTitle };
                    (DataContext as IdeaViewModel).AddRequirement(req);
                }

                // same thing for use cases 
                if (useCaseTextBox.Text != "")
                {
                    string ucTitle = useCaseTextBox.Text;
                    var uc = new UseCase() { UCase = ucTitle };
                    (DataContext as IdeaViewModel).AddUseCase(uc);
                }

            }

            (DataContext as IdeaViewModel).SaveIdeasToDB();

            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }

        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (DataContext != null)
            {
                if (((DataContext as IdeaViewModel).SelectedIdea.Title == null) && ((DataContext as IdeaViewModel).SelectedIdea.Overview == null))
                {
                    // If both the name and why field are null, delete the friggin' thing 
                    (DataContext as IdeaViewModel).deleteIdea();

                }
            }

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Fill up the system requirements listbox 
            if (DataContext != null)
            {
                if ((DataContext as IdeaViewModel).SelectedIdea.Title != null)
                {
                    titleTextBox.Text = (DataContext as IdeaViewModel).SelectedIdea.Title;
                }

                if ((DataContext as IdeaViewModel).SelectedIdea.Overview != null)
                    overviewTextBox1.Text = (DataContext as IdeaViewModel).SelectedIdea.Overview;
                if ((DataContext as IdeaViewModel).SelectedIdea.Notes != null)
                    notesTextBox.Text = (DataContext as IdeaViewModel).SelectedIdea.Notes;

            }



            base.OnNavigatedTo(e);
        }

        private void deleteIdeaButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("Are you sure you want to delete this idea?", "", MessageBoxButton.OKCancel);
            if (m == MessageBoxResult.Cancel)
            {
                return;
            }
            // Cast the parameter as a button.
            (DataContext as IdeaViewModel).deleteIdea();

            // Put the focus back to the main page.
            NavigationService.Navigate(new Uri("MainPage.xaml", UriKind.Relative));
        }

        // Define functions for: adding/removing a system requirement 
        private void addSysReqButton_Click(object sender, RoutedEventArgs e)
        {
            // Add the contents of systemReqsTextbox to the Table<SystemRequirements>
            string reqTitle = systemReqsTextBox.Text;
            // (DataContext as MainPageViewModel).SelectedIdea.SystemRequirements.Add(newSysReq);
            if (reqTitle != "")
            {
                var req = new SystemRequirement() { Requirement = reqTitle };

                (DataContext as IdeaViewModel).AddRequirement(req);             // Add to Datatable, EntitySet & Observable collection 

            }

            (DataContext as IdeaViewModel).SaveIdeasToDB();
            systemReqsTextBox.Text = "";

        }

        private void addUseCaseButton_Click(object sender, RoutedEventArgs e)
        {
            string ucTitle = useCaseTextBox.Text;
            if (ucTitle != "")
            {
                var uc = new UseCase() { UCase = ucTitle };
                (DataContext as IdeaViewModel).AddUseCase(uc);
            }
            useCaseTextBox.Text = "";
        }

        private void deleteSysReqButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            //App.ViewModel.SelectedReq = button.DataContext as SystemRequirement; 

            if (button != null)
            {
                // This code is totally repetitive! 
                // Get a handle for the system requirement 
                var reqForDelete = button.DataContext as SystemRequirement;


                (DataContext as IdeaViewModel).RemoveSelectedReq(reqForDelete);
                button = null;
            }
            return;

        }

        private void deleteUseCaseButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                // Get a handle for the system requirement 
                var ucForDelete = button.DataContext as UseCase;
                //(DataContext as IdeaViewModel).SelectedUseCase = ucForDelete;
                (DataContext as IdeaViewModel).RemoveSelectedUseCase(ucForDelete);
                button = null;
            }
            return;

        }

        private void shareIdeaSocialNetworks_Click(object sender, EventArgs e)
        {
            ShareStatusTask shareStatusTask = new ShareStatusTask();

            shareStatusTask.Status = "One of my ideas is " + (DataContext as IdeaViewModel).SelectedIdea.Title;
            shareStatusTask.Status += ". Would you like to hear more?";
            shareStatusTask.Show();
        }

        private void shareIdeaEmail_Click(object sender, EventArgs e)
        {
            // Compose an email 
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            string Idea = "";

            Idea += (DataContext as IdeaViewModel).SelectedIdea.Title;
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

            emailComposeTask.Body = Idea + "\n\nvia Ideas for Windows Phone";

            emailComposeTask.Show();
        }

        private void pinToStartButton_Click(object sender, EventArgs e)
        {
            ShellTile ApplicationTile = ShellTile.ActiveTiles.First();

            if (ApplicationTile != null)
            {
                StandardTileData NewTitleData = new StandardTileData
                {
                    Title = (DataContext as IdeaViewModel).SelectedIdea.Title,
                    BackgroundImage = new Uri("Tile.png", UriKind.Relative),

                    BackTitle = (DataContext as IdeaViewModel).SelectedIdea.Title,
                    BackBackgroundImage = null,
                    BackContent = (DataContext as IdeaViewModel).SelectedIdea.Overview,
                };

                ApplicationTile.Update(NewTitleData);


            }
        }
    }
}