using System;
using System.Collections.Generic;
using System.Text;

using ChonkyApp.Models;
namespace ChonkyApp.ViewModels
{
    public class AddCustomMeasurementViewModel: ViewModelBase
    {
        private MeasurementKindEntry measurementKind;

        public MeasurementKindEntry MeasurementKind
        {
            get => measurementKind;
            set => SetProperty(ref measurementKind, value);
        }

        public AddCustomMeasurementViewModel()
        {
            MeasurementKind = new MeasurementKindEntry();
        }
    }
}
