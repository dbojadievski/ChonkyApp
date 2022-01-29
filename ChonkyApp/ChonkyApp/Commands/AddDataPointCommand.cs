using ChonkyApp.Models;
using ChonkyApp.ViewModels;

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ChonkyApp.Commands
{
    class AddDataPointCommand : ICommand
    {
        private DataPointViewModel viewModel;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return
            (
                viewModel != null
                && viewModel.BodyWeight.Value != 0
                && viewModel.BodyFat.Value != 0
            );
        }

        public void Execute(object parameter)
        {
            viewModel.SaveDataPoint();
        }

        public AddDataPointCommand(DataPointViewModel vm)
        {
            viewModel = vm;
        }
    }

    class SaveProfileCommand: ICommand
    {
        private UserProfileViewModel viewModel;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return (viewModel != null) && viewModel.IsEnabled;
        }

        public void Execute(object parameter)
        {
            viewModel.SaveCurrentProfile();
        }

        public SaveProfileCommand(UserProfileViewModel vm)
        {
            viewModel = vm;
        }
    }

    class SaveDailyGoalCommand: ICommand
    {
        public  event EventHandler CanExecuteChanged;
        private DataPointViewModel viewModel;
        
        public bool CanExecute(object parameter)
        {
            return (viewModel != null);
        }

        public void Execute(object parameter)
        {
            var dailyGoal = (Guid) parameter;
            if (dailyGoal != null)
            {
               var hasAchieved = viewModel.HasAchievedGoal(dailyGoal).Result;
                if (hasAchieved)
                {
                    // Unachieve
                    viewModel.MarkGoalAsUnachieved(dailyGoal).Wait();
                }
                else
                {
                    // achieve.
                    viewModel.MarkGoalAsAchieved(dailyGoal).Wait();
                }
            }
        }

        public SaveDailyGoalCommand(DataPointViewModel vm)
        {
            viewModel = vm;
        }
    }
}
