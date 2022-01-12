using ChonkyApp.Models;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Xamarin.Forms;

namespace ChonkyApp.Converter
{
    public class IsIntEqualEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null || value == null) 
                return false;

            if (parameter.GetType().IsEnum && value is int @int)
                return (int)parameter == @int;
            
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GoalToBoolConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(bool) && (value is UserGoal))
            {
                UserGoal goal = (UserGoal) value;
                return (goal == UserGoal.Lose);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && targetType == typeof(UserGoal))
            {
                return (bool)value ? UserGoal.Lose : UserGoal.Gain;
            }

            return UserGoal.Gain;
        }
    }
}
