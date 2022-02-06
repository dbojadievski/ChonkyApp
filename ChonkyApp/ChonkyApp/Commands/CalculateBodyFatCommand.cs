using ChonkyApp.ViewModels;

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ChonkyApp.Commands
{
    public class CalculateBodyFatCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            DataPointViewModel viewModel = parameter as DataPointViewModel;
            if (viewModel != null)
            {
                var measurement = viewModel.EstimateBodyFat();
            }
        }
    }
}
