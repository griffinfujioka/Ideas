using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Ideas.Models
{
    #region Use Cases class
    // Define a UseCases table
    [Table(Name = "UseCases")]
    public class UseCase : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler PropertyChanging;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false)]
        public int Id { get; set; }




        [Column]
        public string UCase
        {
            get { return this._useCase; }
            set
            {
                if (this._useCase == value)
                    return;
                this.NotifyPropertyChanging("UCase");
                this._useCase = value;
                this.NotifyPropertyChanged("UCase");
            }
        }

        // Associate to the appropriate idea, using it's IdeaId as a foreign key 
        [Association(Name = "FK_Idea_UseCases", Storage = "_idea", ThisKey = "IdeaId", OtherKey = "Id", IsForeignKey = true)]
        public Idea Idea
        {
            get
            {
                return this._idea.Entity;
            }
            set
            {
                Idea previousValue = this._idea.Entity;
                if (((previousValue != value) || (this._idea.HasLoadedOrAssignedValue == false)))
                {
                    if ((previousValue != null))
                    {
                        this._idea.Entity = null;

                        previousValue.UseCases.Remove(this);
                    }
                    this._idea.Entity = value;
                    if ((value != null))
                    {
                        value.UseCases.Add(this);
                        this.IdeaId = value.Id; ;
                    }
                    else
                    {
                        this.IdeaId = default(Nullable<int>);
                    }
                }
            }
        }

        [Column]
        private int? IdeaId { get; set; }               // used get the IdeaId to know where to add the specific system requirement... is this getter working? 
        private string _useCase;
        private EntityRef<Idea> _idea = new EntityRef<Idea>();    // used to reference the idea for which a system requirement is associated with

        #region UseCases NotifyProperty overrides
        private void NotifyPropertyChanging(string info)
        {
            if (this.PropertyChanging != null)
                this.PropertyChanging(this, new PropertyChangingEventArgs(info));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion
    }
    #endregion

}