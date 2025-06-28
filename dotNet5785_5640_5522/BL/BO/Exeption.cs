namespace BO;

/// חריגה לוגית - הישות המבוקשת לא קיימת.
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }

    public BlDoesNotExistException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// חריגה לוגית - הישות כבר קיימת.
[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }

    public BlAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// חריגה לוגית - תלות פנימית בשכבת הנתונים לא מאותחלת.
[Serializable]
public class BlDependencyNotInitializedException : Exception
{
    public BlDependencyNotInitializedException(string? message) : base(message) { }

    public BlDependencyNotInitializedException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// חריגה לוגית - מידע לא תקין שהועבר.
[Serializable]
public class BlInvalidException : Exception
{
    public BlInvalidException(string? message) : base(message) { }

    public BlInvalidException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// חריגה לוגית - חסר מידע הכרחי.
[Serializable]
public class BlMissingDataException : Exception
{
    public BlMissingDataException(string? message) : base(message) { }

    public BlMissingDataException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// חריגה לוגית - תקלה בטעינה/יצירת קובץ XML.
[Serializable]
public class BlXMLFileLoadCreateException : Exception
{
    public BlXMLFileLoadCreateException(string? message) : base(message) { }

    public BlXMLFileLoadCreateException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// חריגה פנימית - תכונה הכרחית מכילה ערך null.
[Serializable]
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

/// חריגה פנימית - תאריכים לא תקינים או סדר זמנים שגוי.
[Serializable]
public class BlInvalidDateRangeException : Exception
{
    public BlInvalidDateRangeException(string? message) : base(message) { }
}

/// חריגה כללית לשגיאה לוגית שלא זוהתה.
[Serializable]
public class BlGeneralException : Exception
{
    public BlGeneralException(string? message) : base(message) { }

    public BlGeneralException(string message, Exception innerException)
        : base(message, innerException) { }
}

/// חריגה לוגית - פעולה לא זמינה זמנית (למשל בזמן סימולציה)
[Serializable]
public class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException(string? message) : base(message) { }

    public BLTemporaryNotAvailableException(string message, Exception innerException)
        : base(message, innerException) { }
}
