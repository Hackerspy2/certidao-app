namespace BBPix.Errors;

public class BbUnknownException : PixException
{
   public BbUnknownException(string error, string errorDescription) : base(errorDescription, error)
    {
        Error = error;
        ErrorDescription = errorDescription;
    }

    /// <summary>
    ///     The error message returned.
    /// </summary>
    public dynamic Error { get; }

    /// <summary>
    ///     The data of exception.
    /// </summary>
    public string ErrorDescription { get; }

    /// <summary>
    ///     Creates a new <see cref="PixException" /> from the given <paramref name="e" /> object, wrapping it in a
    ///     <see cref="BBUnknownException" />.
    /// </summary>
    public static PixException UnknownException(dynamic e)
    {
        return new BbUnknownException("unknown", e.ToString());
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"BBUnknownException: error: {Error}, errorData: {ErrorDescription}";
    }
}