using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Converters;

public class AddUpdateToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s && s == "Update")
            return Visibility.Visible; // Visible for Update mode
        return Visibility.Collapsed; // Collapsed otherwise (e.g., Add mode)
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}