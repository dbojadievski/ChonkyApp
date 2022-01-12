using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Xamarin.Forms;

namespace ChonkyApp.Converter
{
    public class InvertedBoolConverter : IValueConverter
    {
        private bool InvertBool(object value)
        {
            return value is bool result ? !result : throw new ArgumentException("Value is not a valid boolean", nameof(value));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => InvertBool(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => InvertBool(value);
    }
}
