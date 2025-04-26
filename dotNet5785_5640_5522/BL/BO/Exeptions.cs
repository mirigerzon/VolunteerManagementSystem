using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO;
internal class Exeptions
{
    [Serializable]
    public class BlDoesNotExistException : Exception
    {
        public BlDoesNotExistException(string? message) : base(message) { }
        public BlDoesNotExistException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public class BlAlreadyExistsException : Exception
    {
        public BlAlreadyExistsException(string? message) : base(message) { }
        public BlAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public class BlDependencyNotInitializedException : Exception
    {
        public BlDependencyNotInitializedException(string? message) : base(message) { }
        public BlDependencyNotInitializedException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public class BlInvalidException : Exception
    {
        public BlInvalidException(string? message) : base(message) { }
        public BlInvalidException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public class BlMissingDataException : Exception
    {
        public BlMissingDataException(string? message) : base(message) { }
        public BlMissingDataException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    [Serializable]
    public class BlXMLFileLoadCreateException : Exception
    {
        public BlXMLFileLoadCreateException(string? message) : base(message) { }
        public BlXMLFileLoadCreateException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// חריגות פנימיות בשכבת BL (לא קשורות ל-DAL ישירות)
    [Serializable]
    public class BlNullPropertyException : Exception
    {
        public BlNullPropertyException(string? message) : base(message) { }
    }

    [Serializable]
    public class BlInvalidDatesException : Exception
    {
        public BlInvalidDatesException(string? message) : base(message) { }
    }

}

