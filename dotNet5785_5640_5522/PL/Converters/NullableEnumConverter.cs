using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows; // נדרש עבור DependencyProperty.UnsetValue

namespace PL.Converters;

public class NullableEnumConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return "All"; // מציג "All" עבור ערכי Enum שהם null
        }
        // ממיר את ערך ה-Enum למחרוזת (למשל, "Open" עבור CallStatus.Open)
        return value.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // המרה הפוכה לרוב אינה נחוצה אם אתה קושר ישירות ל-SelectedItem/SelectedValue
        // של ComboBox המכיל ערכי Enum
        if (value is string s && s == "All")
        {
            return null; // מחזיר null אם נבחר "All"
        }

        // אם הטיפוס היעד הוא Nullable<Enum>, WPF כבר יודע לטפל בזה עבור רוב המקרים.
        // אם בכל זאת נדרשת המרה ידנית, זה יהיה מורכב יותר ויתבקש לוגיקה ספציפית לטיפוס ה-Enum.
        // כרגע נחזיר DependencyProperty.UnsetValue כדי לציין שאין טיפול ספציפי כאן.
        return DependencyProperty.UnsetValue;
    }
}