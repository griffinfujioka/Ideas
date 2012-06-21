using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Ideas.Models
{
    // Define a SystemRequirements table
    [Table(Name = "SystemRequirements")]
    public class SystemRequirement : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler PropertyChanging;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false)]
        public int Id { get; set; }




        [Column]
        public string Requirement
        {
            get { return this._requirement; }
            set
            {
                if (this._requirement == value)
                    return;
                this.NotifyPropertyChanging("Requirement");
                this._requirement = value;
                this.NotifyPropertyChanged("Requirement");
            }
        }

        // Associate to the appropriate idea, using it's IdeaId as a foreign key 
        [Association(Name = "FK_Idea_SystemRequirements", Storage = "_idea", ThisKey = "IdeaId", OtherKey = "Id", IsForeignKey = true)]
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
                        previousValue.SystemRequirements.Remove(this);
                    }
                    this._idea.Entity = value;
                    if ((value != null))
                    {
                        value.SystemRequirements.Add(this);
                        this.IdeaId = value.Id; ;
                    }
                    else
                    {
                        this.IdeaId = default(Nullable<int>);
                    }
                }
            }
        }

        /* int? = int.HasValue */
        [Column]
        private int? IdeaId { get; set; }               // used get the IdeaId to know where to add the specific system requirement... is this getter working? 
        private string _requirement;
        private EntityRef<Idea> _idea = new EntityRef<Idea>();    // used to reference the idea for which a system requirement is associated with

        #region SysReqs NotifyProperty overrides
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
}