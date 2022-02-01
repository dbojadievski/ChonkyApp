using ChonkyApp.ViewModels;

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ChonkyApp.Commands
{
    public class RecordPulseEntryCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var viewModel = parameter as PulseViewModel;
            if (viewModel != null)
            {
                // Gather data, record entry.
                viewModel.RecordEntry();
            }
        }
    }
}
