namespace BBPix.Errors;

public class BbHttpException : PixException
{
    public BbHttpException(string error, string errorDescription) : base(errorDescription, error)
    {
        Error = error;
        ErrorDescription = errorDescription;
    }

    public override string Error { get; }

    public override string ErrorDescription { get; }

    public static PixException HttpException(object exception)
    {
        return new BbHttpException("network-error", exception.ToString());
    }

    public override string ToString()
    {
        return $"BBHttpException: error: {Error}, errorData: {ErrorDescription}";
    }
}