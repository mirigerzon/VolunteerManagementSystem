using System;
using System.Globalization;
using System.Windows.Data;
using BO;

namespace PL.Converters;

public class StatusToIsUpdatableConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CallStatus status)
        {
            return status == CallStatus.Open ||
                   status == CallStatus.InTreatment;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
