namespace BO;


//DO Exceptions
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string? message, Exception? innerException) :
                                                    base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string? message, Exception? innerException) :
                                                    base(message, innerException)
    { }
}

[Serializable]
public class BlDeletionImpossibleException : Exception
{
    public BlDeletionImpossibleException(string? message) : base(message) { }
    public BlDeletionImpossibleException(string? message, Exception? innerException) :
                                                    base(message, innerException)
    { }
}

[Serializable]
public class BlNoAccessRightsException : Exception
{
    public BlNoAccessRightsException(string? message) : base(message) { }
    public BlNoAccessRightsException(string? message, Exception? innerException) :
                                                    base(message, innerException)
    { }
}

//BL Exceptions
[Serializable]
public class  BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

[Serializable]
public class BlArgumentNullException : Exception
{
    public BlArgumentNullException(string? message) : base(message) { }
}

[Serializable]
public class BlBadInputDataException : Exception
{
    public BlBadInputDataException(string? message) : base(message) { }
}

