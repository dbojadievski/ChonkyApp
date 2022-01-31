using ChonkyApp.ViewModels;

using System;
using System.Windows.Input;

class SaveDailyGoalCommand : ICommand
{
    public event EventHandler CanExecuteChanged;
    private DataPointViewModel viewModel;

    public bool CanExecute(object parameter)
    {
        return (viewModel != null);
    }

    public void Execute(object parameter)
    {
        var dailyGoal = (Guid)parameter;
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