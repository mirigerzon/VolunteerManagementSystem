using System;
using System.Globalization;
using System.Windows; // נדרש עבור Visibility
using System.Windows.Data; // נדרש עבור IMultiValueConverter
using BO; // ודא ש-BO (Business Objects) זמין כאן עבור CallStatus

namespace PL.Converters;

public class CanDeleteCallConverter : IMultiValueConverter // *** חובה: IMultiValueConverter עבור MultiBinding ***
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // בודק שיש שני ערכים (CallStatus ו-TotalAssignments) ושהם מהטיפוסים הנכונים
        if (values.Length == 2 &&
            values[0] is CallStatus callStatus &&
            values[1] is int totalAssignments)
        {
            // לוגיקה: אפשר למחוק רק אם השיחה לא בסטטוס "Assigned" ולא "Closed" ואין לה הקצאות
            if (callStatus != CallStatus.InTreatment &&
                callStatus != CallStatus.Closed &&
                totalAssignments == 0)
            {
                return Visibility.Visible;
            }
        }
        return Visibility.Collapsed; // במקרים אחרים, הכפתור יהיה מוסתר
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        // המרה הפוכה בדרך כלל אינה נחוצה עבור קונברטרים של Visibility
        throw new NotImplementedException();
    }
}