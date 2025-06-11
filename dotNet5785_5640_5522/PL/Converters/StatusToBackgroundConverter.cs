using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BO; // Make sure BO.TypeOfDistance is accessible

namespace PL.Converters;
public class StatusToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TypeOfDistance status)
        {
            return status switch
            {
                TypeOfDistance.Aerial => Brushes.LightBlue,
                TypeOfDistance.Driving => Brushes.LightGreen,
                TypeOfDistance.Walking => Brushes.IndianRed,
                _ => Brushes.White
            };
        }
        return Brushes.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}