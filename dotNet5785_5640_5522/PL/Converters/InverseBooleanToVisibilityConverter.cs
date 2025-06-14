using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                // If boolValue is true (IsNewCall is true), collapse it.
                // If boolValue is false (IsNewCall is false), show it (visible).
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible; // Default or fallback
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue; // Not used for two-way binding
        }
    }
}