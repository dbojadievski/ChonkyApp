using SQLite;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChonkyApp.Models
{
    public class BaseModel: INotifyPropertyChanged
    {
        private Guid id;
        private DateTime createdAt;
        private DateTime? lastUpdatedAt;

        [PrimaryKey]
        public Guid ID
        {
            get => id;
            set {  SetProperty(ref id, value); }
        }

        public DateTime CreatedAt
        {
            get => createdAt;
            set { SetProperty(ref createdAt, value);}
        }

        public DateTime? LastUpdatedAt
        {
            get => lastUpdatedAt;
            set { SetProperty(ref lastUpdatedAt, value);}
        }

        public BaseModel()
        {
            CreatedAt = DateTime.Now;
            LastUpdatedAt = null;
            ID = Guid.NewGuid();
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
