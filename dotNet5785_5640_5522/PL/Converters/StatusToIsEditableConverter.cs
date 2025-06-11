using System;
using System.Globalization;
using System.Windows.Data;
using BO;

namespace PL.Converters;

public class StatusToIsEditableConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CallStatus status)
            return status == CallStatus.Open;
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
