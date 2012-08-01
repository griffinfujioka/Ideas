using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Collections.ObjectModel;
using Ideas.Models;
using Ideas.ViewModels;

namespace Ideas.Models
{
    [Table(Name = "Ideas")]             // Define a table of ideas 
    public class Idea : INotifyPropertyChanged, INotifyPropertyChanging     // Inherits PropertyChanged and PropertyChanging
    {
        #region Idea attributes: Title, Overview, Notes, System Requirements, Use Cases
        private string _Title;
        private string _Overview;
        private string _Notes;
        private EntitySet<SystemRequirement> _SystemRequirements;
        private EntitySet<UseCase> _UseCases;
        private ObservableCollection<SystemRequirement> _SysReqsOC;
        private ObservableCollection<UseCase> _UseCaseOC;
        #endregion

        #region Idea() default Constructor
        public Idea()
        {
            Title = "";
            Overview = "";
            Notes = "";
            this._SystemRequirements = new EntitySet<SystemRequirement>(this.OnSystemRequirementAdded, this.OnSystemRequirementRemoved);
            this._UseCases = new EntitySet<UseCase>(this.OnUseCaseAdded, this.OnUseCaseRemoved);
            _SysReqsOC = new ObservableCollection<SystemRequirement>();
            _UseCaseOC = new ObservableCollection<UseCase>();

        }
        #endregion

        #region ID, Title, Overview, Notes column definitions
        // IdeaId is a primary key in the Ideas table, and a foreign key in the System Requirements table 
        // The foreign key bug which was giving me problems stemmed from setting the AutoSync property to AutoSync.OnInsert
        [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false)]
        public int Id { get; set; }


        [Column]        // Define Title as a column 
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value)
                    return;
                NotifyPropertyChanging("Title");
                _Title = value;
                NotifyPropertyChanged("Title");
            }
        }

        [Column]
        public string Overview
        {
            get { return _Overview; }
            set
            {
                NotifyPropertyChanging("Overview");
                _Overview = value;
                NotifyPropertyChanged("Overview");

            }
        }

        [Column]
        public string Notes
        {
            get { return _Notes; }
            set
            {
                NotifyPropertyChanging("Notes");
                _Notes = value;
                NotifyPropertyChanged("Notes");
            }
        }


        #endregion

        #region Define the assocation to the SystemRequirements table and Observable collection
        // Relational database using an EntitySet & an AssociationAttribute
        // One idea has multiple system requirements (one-to-many relationship)  ---- EntitySet = one-to-many, EntityRef = one-to-one



        /* ThisKey refers to the internal integer within the Idea class. OtherKey refers to the public method within the SystemRequiremenst class which will return an integer SRId */
        [Association(Name = "FK_Idea_SystemRequirements", Storage = "_SystemRequirements", ThisKey = "Id", OtherKey = "IdeaId")]
        public EntitySet<SystemRequirement> SystemRequirements
        {
            get { return this._SystemRequirements; }
            set
            {
                NotifyPropertyChanging("SystemRequirements");
                _SystemRequirements.Assign(value);
                NotifyPropertyChanged("SystemRequirements");

            }
        }

        public ObservableCollection<SystemRequirement> SysReqsOC
        {
            get { return _SysReqsOC; }
            set
            {
                _SysReqsOC = value;
                NotifyPropertyChanged("SysReqsOC");
            }

        }

        #endregion

        #region Define the UseCase table
        //Define the association to the UseCase table 
        [Association(Name = "FK_Idea_UseCases", Storage = "_UseCases", ThisKey = "Id", OtherKey = "IdeaId")]
        public EntitySet<UseCase> UseCases
        {
            get { return this._UseCases; }
            //set
            //{
            //    //NotifyPropertyChanging("SystemRequirements");
            //    this._UseCases.Assign(value);
            //    //NotifyPropertyChanged("SystemRequirements");

            //}
        }

        public ObservableCollection<UseCase> UseCaseOC
        {
            get { return _UseCaseOC; }
            set
            {
                _UseCaseOC = value;
                NotifyPropertyChanged("UseCaseOC");
            }

        }
        #endregion

        #region SysReq and Use Case attach and detach methods
        // Called during add operation 
        private void attach_SysReq(SystemRequirement newRequirement)
        {
            NotifyPropertyChanging("Idea");
        }

        // Called during a remove operation 
        private void detach_SysReq(SystemRequirement req)
        {
            NotifyPropertyChanging("Idea");
        }

        private void attach_UseCase(UseCase newUseCase)
        {
            NotifyPropertyChanging("Idea");
        }

        private void detach_UseCase(UseCase uc)
        {
            NotifyPropertyChanging("Idea");
        }
        #endregion

        #region System requirement added/removed
        // Are these ever even evoked?
        private void OnSystemRequirementAdded(SystemRequirement sysRequirement)
        {
            sysRequirement.Idea = this;
        }

        private void OnSystemRequirementRemoved(SystemRequirement sysRequirement)
        {
            sysRequirement.Idea = null;
        }
        #endregion

        #region Use Case added/removed
        private void OnUseCaseAdded(UseCase uc)
        {
            uc.Idea = this;
        }

        private void OnUseCaseRemoved(UseCase uc)
        {
            uc.Idea = null;
        }
        #endregion

        public void clearSystemRequirements()
        {
            
        }

        #region PropertyChanging, PropertyChanged overrides
        public event PropertyChangingEventHandler PropertyChanging;

        private void NotifyPropertyChanging(string info)
        {
            if (this.PropertyChanging != null)
                this.PropertyChanging(this, new PropertyChangingEventArgs(info));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

    }

    #region IdeasDataContext
    public class IdeasDataContext : DataContext
    {
        public IdeasDataContext(string connectionString) : base(connectionString) { }

        public Table<Idea> Ideas;
        public Table<UseCase> UseCases;
        public Table<SystemRequirement> SystemRequirements;
    }
    #endregion
}