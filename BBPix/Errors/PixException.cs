namespace BBPix.Errors;

public class PixException : Exception
{
    /// <summary>
    ///     The error that caused the exception.
    /// </summary>
    private readonly string _error;

    /// <summary>
    ///     The data of the exception.
    /// </summary>
    private readonly string _errorDescription;

    /// <summary>
    ///     Creates a new instance of the <see cref="PixException" /> class with the given error
    ///     and exception data.
    /// </summary>
    /// <param name="errorDescription">The description of the error.</param>
    /// <param name="error">The error message.</param>
    public PixException(string errorDescription, string error)
    {
        _error = error;
        _errorDescription = errorDescription;
    }

    /// <summary>
    ///     Gets the error message associated with this exception.
    /// </summary>
    public virtual string Error => _error;

    /// <summary>
    ///     Gets the information of this exception.
    /// </summary>
    public virtual string ErrorDescription => _errorDescription;

    /// <summary>
    ///     Returns a string representation of this exception.
    /// </summary>
    public override string ToString()
    {
        return $"PixError(error: {_error} errorDescription: {_errorDescription})";
    }
}