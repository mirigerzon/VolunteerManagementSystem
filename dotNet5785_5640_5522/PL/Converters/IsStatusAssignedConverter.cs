using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BO;
namespace PL.Converters;

public class IsStatusAssignedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CallStatus status)
        {
            if (status == CallStatus.InTreatment || status == CallStatus.InTreatmentAtRisk)
            {
                return Visibility.Visible;
            }
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}