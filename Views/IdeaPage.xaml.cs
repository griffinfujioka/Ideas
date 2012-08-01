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

        public Idea lastSelectedIdea;

        #region Constructor
        public IdeaPage()
        {
            InitializeComponent();

            this.defaultListPicker.ItemsSource = shareOptions; 

            if (App.ViewModel != null)
            {
                this.DataContext = App.ViewModel;
            }


        }
        #endregion 
        
        #region Save button click 
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

            lastSelectedIdea = (DataContext as IdeaViewModel).SelectedIdea; 
            (DataContext as IdeaViewModel).SaveIdeasToDB();

            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));

        }
        #endregion 
        
        
        // This code sucks balls. You're adding an idea from the button on the main page... 
        // That's a crappy, disfunctional, hacky way of doing it. Go change the code so it doesn't suck. 
        #region On Navigated From 
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (DataContext != null)
            {
                if ((DataContext as IdeaViewModel).SelectedIdea != null)
                {
                    if (((DataContext as IdeaViewModel).SelectedIdea.Title == "") && ((DataContext as IdeaViewModel).SelectedIdea.Overview == ""))
                    {
                        // If both the name and why field are null, delete the friggin' thing 
                        (DataContext as IdeaViewModel).deleteIdea();

                    }
                }
            }

            base.OnNavigatedFrom(e);
        }
        #endregion 

        #region On Navigated To 
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string TileKey = null; 
            
            base.OnNavigatedTo(e);

            try
            {
                TileKey = NavigationContext.QueryString["DefaultTitle"];     // Ex: TileKey = FromTile_2_Caffeine-Calculator --> FromTile_Id_Three-Letter-Name
            }
            catch (KeyNotFoundException ex)
            {
                // Not navigated to from a secondary tile
            }

            if (TileKey != null)
            {
                

                string[] keyComponents = TileKey.Split('_');    // Would return [FromTile][2][Caffeine-Calculator]
                string title = keyComponents.Last<string>();        // Would return Caffeine-Calculator
                title = title.Replace("-", " ");                        // Would return 'Caffeine Calculator' 
                Idea foundIdea = null;

                // Load the idea with that title
                foreach (Idea idea in App.ViewModel.Ideas)
                {
                    if (idea.Title.Contains(title))
                    {
                        foundIdea = idea;
                        App.ViewModel.SelectedIdea = foundIdea;
                        break;
                    }
                }

                //if (foundIdea == null)
                //{
                //    // An idea with that name was not found in the database. 
                //    MessageBox.Show("We're sorry " + title + " could be found. Perhaps the title was changed since pinning the idea or it has since been deleted.", "Idea not found", MessageBoxButton.OK);
                //    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                //}

                
            }
            

            // Fill up controls 
            if (DataContext != null)
            {
                if (App.ViewModel.SelectedIdea == null)         
                {
                    
                    if (App.ViewModel.Ideas.Contains(lastSelectedIdea))
                        App.ViewModel.SelectedIdea = lastSelectedIdea;
                    else
                    {
                        // An idea with that name was not found in the database. 
                        //MessageBox.Show("We're sorry that idea could be found. Perhaps the title was changed since pinning the idea or it has since been deleted.", "Idea not found", MessageBoxButton.OK);
                        
                    }
                }
                else
                {
                    titleTextBox.Text = App.ViewModel.SelectedIdea.Title;
                }
            }
        }



        #endregion 

        #region Delete click 
        private void deleteIdeaButton_Click(object sender, EventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("Are you sure you want to delete this idea?", "", MessageBoxButton.OKCancel);
            if (m == MessageBoxResult.Cancel)
            {
                return;
            }

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

            this.DataContext = null; 

            // Put the focus back to the main page.
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
        #endregion 

        #region Add system requirement click 
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
        #endregion 

        #region Add use case click 
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
        #endregion 

        #region Delete system requirement click 
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
        #endregion 

        #region Delete use case click 
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
        #endregion 

        #region Share to social networks 
        private void shareIdeaSocialNetworks_Click(object sender, EventArgs e)
        {
            ShareStatusTask shareStatusTask = new ShareStatusTask();

            shareStatusTask.Status = "One of my ideas is " + (DataContext as IdeaViewModel).SelectedIdea.Title + " via Ideas http://www.windowsphone.com/en-US/apps/f18161dd-b3cf-4b7b-baf1-e161454939c1?fb_ref=wpcwam";
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

        #region Pin to start click 
        private void pinToStartButton_Click(object sender, EventArgs e)
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

        #region Share idea click; Open the list picker 
        private void shareIdea_Click(object sender, EventArgs e)
        {
            defaultListPicker.Open();
        }
        #endregion 

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}