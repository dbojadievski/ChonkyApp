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
}
