using ChonkyApp.Commands;
using ChonkyApp.Models;
using ChonkyApp.Services;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;

using Xamarin.Forms;

namespace ChonkyApp.ViewModels
{
    public class UserProfileViewModel : INotifyPropertyChanged
    {
        private ICommand saveProfileCommand;

        public UserDataStore DataStore = new UserDataStore();
        
        private bool isEnabled;
        private UserProfile currentProfile;

        public ICommand SaveProfileCommand
        {
            get => saveProfileCommand;
            set { SetProperty(ref saveProfileCommand, value); }
        }
        public bool IsEnabled
        {
            get => isEnabled;
            set { SetProperty(ref isEnabled, value); }
        }

        public UserProfile CurrentProfile
        {
            get => currentProfile;
            set { SetProperty(ref currentProfile, value); }
        }

        public bool ShouldGain
        {
            get => CurrentProfile.Goal == UserGoal.Gain;
            set
            {
                if (value)
                    CurrentProfile.Goal = UserGoal.Gain;
            }
        }

        public bool ShouldMaintain
        {
            get => false;
            set { }
        }

        public bool ShouldLose
        {
            get => CurrentProfile.Goal == UserGoal.Lose;
            set
            {
                if (value)
                    CurrentProfile.Goal = UserGoal.Lose;
            }
        }

        public String FullName
        {
            get => $"{CurrentProfile.FirstName} {CurrentProfile.LastName}";
        }


        public UserProfileViewModel()
        {
            SaveProfileCommand = new SaveProfileCommand(this);
            IsEnabled = true;
            CurrentProfile = DataStore.GetCurrentProfile().Result;
        }

        public void ReloadUser()
        {
            var user = DataStore.GetCurrentProfile().Result;
            CurrentProfile.EmailAddress = user.EmailAddress;
            CurrentProfile.BirthDate = user.BirthDate;
            CurrentProfile.CreatedAt =  user.CreatedAt;
            CurrentProfile.FirstName = user.FirstName;
            CurrentProfile.LastName = user.LastName;
            CurrentProfile.Goal = user.Goal;
            CurrentProfile.Height = user.Height;
            CurrentProfile.IsImperial = user.IsImperial;
            CurrentProfile.Sex = user.Sex;
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

        public void SaveCurrentProfile()
        {
            DataStore.UpdateItemAsync(CurrentProfile).Wait();
            ReloadUser();
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
