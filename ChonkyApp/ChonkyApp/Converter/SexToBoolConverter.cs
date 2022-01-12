using ChonkyApp.Models;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Xamarin.Forms;

namespace ChonkyApp.Converter
{
    public class SexToBoolConverter : IValueConverter
    {
        private bool SexToBool(object value)
        {
            return value is Sex result ? (result == Sex.Female) : throw new ArgumentException("Value is not a valid sex", nameof(value));
        }

        private Sex BoolToSex(object value)
        {
            return value is bool result ? (result ? Sex.Female : Sex.Male) : throw new ArgumentException("Value is not a valid boolean", nameof(value));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => SexToBool(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => BoolToSex(value);
    }

    public class SexToInvertedBoolConverter : IValueConverter
    {
        private bool SexToInvertedBool(object value)
        {
            return value is Sex result ? (result == Sex.Male) : throw new ArgumentException("Value is not a valid sex", nameof(value));
        }

        private Sex InvertedBoolToSex(object value)
        {
            return value is bool result ? (result ? Sex.Male : Sex.Female) : throw new ArgumentException("Value is not a valid boolean", nameof(value));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => SexToInvertedBool(value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => InvertedBoolToSex(value);
    }
}