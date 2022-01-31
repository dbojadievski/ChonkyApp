using ChonkyApp.ViewModels;

using System;
using System.Windows.Input;

class SaveProfileCommand : ICommand
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