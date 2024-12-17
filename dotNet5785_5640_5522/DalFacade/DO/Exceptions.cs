namespace DO;
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}
[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}
[Serializable]
public class DalDependencyNotInitializedException : Exception
{
    public DalDependencyNotInitializedException(string message) : base(message) { }
}
[Serializable]
public class DalInvalidException : Exception
{
    public DalInvalidException(string message) : base(message) { }
}
[Serializable]
public class DalMissingDataException : Exception
{
    public DalMissingDataException(string message) : base(message) { }
}
[Serializable]
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string message) : base(message) { }
}