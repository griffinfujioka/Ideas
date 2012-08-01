using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ideas.Models;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Ideas.ViewModels
{
    public class IdeaViewModel : INotifyPropertyChanged
    {


        private Idea _Model;                   // an IdeaViewModel references an idea 
        private IdeasDataContext _DB;               // data context includes tables for: Ideas, System Requirements and Use Cases 

        private ObservableCollection<Idea> _Ideas;         // a collection of ideas 
        // The selected idea should be an IdeaViewModel... maybe? 
        private Idea _SelectedIdea;                            // currently selected idea 

        private ObservableCollection<SystemRequirement> _SystemRequirements;        // each idea has a collection of sys reqs 
        private SystemRequirement _SelectedReq;                             // selected requirement (NOT CURRENTLY IN USE! will be used to edit? ) 

        private ObservableCollection<UseCase> _UseCases;                        // and a collection of use cases 
        private UseCase _SelectedUseCase;


        public Idea Model
        {
            get { return _Model; }
            set
            {
                _Model = value;
                NotifyPropertyChanged("Model");
            }
        }

        public IdeasDataContext DB
        {
            get { return _DB; }
        }

        #region 3 constructors... TODO: WTF is going on in these? This is  not right.
        // Overload 1
        public IdeaViewModel(IdeasDataContext db)
        {
            _DB = db;
            _Model = new Idea();
            _SystemRequirements = new ObservableCollection<SystemRequirement>(_Model.SystemRequirements);
            //_UseCases = new ObservableCollection<UseCase>(SelectedIdea.UseCases);
            //_SelectedReq = null;
            //_SelectedUseCase = null;
        }

        // Overload 2 
        public IdeaViewModel(Idea model, IdeasDataContext db)
        {
            _DB = db;
            _Model = model;
            // Populate the SystemRequirements and UseCases observable collections
            _SystemRequirements = new ObservableCollection<SystemRequirement>(_Model.SystemRequirements);
            //_UseCases = new ObservableCollection<UseCase>(SelectedIdea.UseCases);
        }

        // Overload 3 
        public IdeaViewModel(string dbConnectionString)
        {
            //_DB = new IdeasDataContext(dbConnectionString);
            //_Model = new Idea(); 
            //if (SelectedIdea != null)
            //{
            //    _SystemRequirements = new ObservableCollection<SystemRequirement>(_Model.SystemRequirements);
            //    //_UseCases = new ObservableCollection<UseCase>(_Model.UseCases);
            //}
            _DB = new IdeasDataContext(dbConnectionString);
        }

        #endregion

        public ObservableCollection<Idea> Ideas
        {
            get { return _Ideas; }
            set
            {
                _Ideas = value;
                NotifyPropertyChanged("Ideas");
            }

        }

        public Idea SelectedIdea
        {
            get { return _SelectedIdea; }
            set
            {
                _SelectedIdea = value;
                NotifyPropertyChanged("SelectedIdea");
            }
        }

        #region Title, Overview, SystemRequirements, UseCases, Notes
        public string Title
        {
            get { return SelectedIdea.Title; }
            set
            {
                if (SelectedIdea.Title != value)
                {
                    SelectedIdea.Title = value;
                }
            }
        }

        public string Overview
        {
            get { return _Model.Overview; }
            set
            {
                if (_Model.Overview != value)
                {
                    _Model.Overview = value;
                }
            }

        }

        public ObservableCollection<SystemRequirement> SystemRequirements
        {
            get { return _SystemRequirements; }
            set
            {
                _SystemRequirements = value;
                NotifyPropertyChanged("SystemRequirements");
            }

        }

        public ObservableCollection<UseCase> UseCases
        {
            get { return _UseCases; }
            set
            {
                _UseCases = value;
                NotifyPropertyChanged("UseCases");
            }
        }

        public string Notes
        {
            get { return _Model.Notes; }
            set
            {
                if (_Model.Notes != value)
                {
                    _Model.Notes = value;
                }
            }
        }
        #endregion


        public SystemRequirement SelectedReq
        {
            get { return _SelectedReq; }
            set
            {
                _SelectedReq = value;
                NotifyPropertyChanged("SelectedReq");
            }

        }

        public UseCase SelectedUseCase
        {
            get { return _SelectedUseCase; }
            set
            {
                _SelectedUseCase = value;
                NotifyPropertyChanged("SelectedUseCase");
            }
        }




        #region database operations: LoadIdeasFromDB(), AddNewIdea(), deleteIdea(), saveChangestoDB()



        public void LoadIdeasFromDB()
        {

            // Specify the query for all ideas in the database.
            var IdeasInDB = from Idea idea in _DB.Ideas select idea;



            // Load all of the ideas into an observable collection - is this loading the next level of the database correctly? 
            Ideas = new ObservableCollection<Idea>();
            foreach (var ideaInDB in IdeasInDB)
            {
                Ideas.Add(ideaInDB);
            }


        }

        public void AddNewIdea()
        {
            var newIdea = new Idea();       // create the idea 
            SelectedIdea = newIdea;         // set to the relevant datacontext

            _DB.Ideas.InsertOnSubmit(newIdea);      // Add to the database 
            _DB.SubmitChanges();                    // Save changes to the database  
            Ideas.Add(newIdea);                     // Add to the observable collection 

        }

        public void deleteIdea()
        {
            var ideaForDelete = this.SelectedIdea;

            ideaForDelete.SystemRequirements.Clear();
            ideaForDelete.SystemRequirements = null; 
            // Remove from the observable collection 
            Ideas.Remove(ideaForDelete);

            // Remove from the data context 
            _DB.Ideas.DeleteOnSubmit(ideaForDelete);

            // Save the database 
            _DB.SubmitChanges();
        }


        public void SaveIdeasToDB()
        {
            _DB.SubmitChanges();
        }


        #endregion




        #region Add/Remove System Requirements
        public void AddRequirement(SystemRequirement req)
        {
            /* Set the idea context to the appropriate idea 
            critical for multidimensional DB, if you don't do this 
            the req doesn't know which Idea it's attached to. Maybe done internally when adding to the entity set? */
            req.Idea = SelectedIdea;
            // Add to the entity set 
            SelectedIdea.SystemRequirements.Add(req);

            // Add to SystemRequirements datatable 
            _DB.SystemRequirements.InsertOnSubmit(req);

            // Add to the observable collection 
            SelectedIdea.SysReqsOC.Add(req);

            // Save the data context 
            _DB.SubmitChanges();

        }


        public void RemoveSelectedReq(SystemRequirement reqForDelete)
        {

            // Remove from the datatable 
            _DB.SystemRequirements.DeleteOnSubmit(reqForDelete);
            _DB.SubmitChanges();

            // Remove from the entity set
            SelectedIdea.SystemRequirements.Remove(reqForDelete);

            // Remove from the observable collection 
            SelectedIdea.SysReqsOC.Remove(SelectedReq);
        }
        #endregion

        public void removeAllRequirements()
        {
            if(SelectedIdea != null)
            {
            _DB.SystemRequirements.DeleteAllOnSubmit<SystemRequirement>(SelectedIdea.SystemRequirements);
            _DB.SubmitChanges();
            }
            return;
        }

        public void removeAllUseCases()
        {
            if (SelectedIdea != null)
            {
                _DB.UseCases.DeleteAllOnSubmit<UseCase>(SelectedIdea.UseCases);
                _DB.SubmitChanges(); 
            }
            return; 
        }

        public void AddUseCase(UseCase uc)
        {
            // Set the idea context to the appropriate idea 
            uc.Idea = SelectedIdea;

            // Add to the entity set 
            SelectedIdea.UseCases.Add(uc);

            // Add to UseCases datatable 
            _DB.UseCases.InsertOnSubmit(uc);

            // Add to the observable collection 
            SelectedIdea.UseCaseOC.Add(uc);

            // Save the data context 
            _DB.SubmitChanges();

        }

        public void RemoveSelectedUseCase(UseCase ucForDelete)
        {
            // Remove from the data table 
            _DB.UseCases.DeleteOnSubmit(ucForDelete);
            _DB.SubmitChanges();

            // Remove from the entity set 
            SelectedIdea.UseCases.Remove(ucForDelete);

            // Remove from the observable collection 
            SelectedIdea.UseCaseOC.Remove(ucForDelete);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
