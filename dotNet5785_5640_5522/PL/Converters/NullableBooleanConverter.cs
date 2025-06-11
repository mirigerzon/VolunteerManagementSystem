using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows; // נדרש עבור DependencyProperty.UnsetValue

namespace PL.Converters;

public class NullableBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return "All"; // מציג "All" כאשר הערך הבוליאני הוא null
        }
        if (value is bool b)
        {
            return b ? "Active" : "Inactive"; // מציג "Active" או "Inactive" בהתאם לערך
        }
        return DependencyProperty.UnsetValue; // מציין שאין המרה
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            if (s == "All")
            {
                return null; // מחזיר null עבור בחירת "All"
            }
            if (s == "Active")
            {
                return true;
            }
            if (s == "Inactive")
            {
                return false;
            }
        }
        return DependencyProperty.UnsetValue; // מציין שאין המרה
    }
}