using System;
using System.Globalization;
using System.Windows.Data;
using BO;

namespace PL.Converters;

public class IsEditableInUpdateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isNewCall = value is bool b && b;

        // Always editable if new call
        if (isNewCall)
            return true;

        // Editable in update only if parameter is "true"
        return parameter?.ToString()?.ToLower() == "true";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
