namespace BBPix.Errors;

public class BbApiException : PixException
{
    public BbApiException(string error, string errorDescription) : base(errorDescription, error)
    {
        Error = error;
        ErrorDescription = errorDescription;
    }

    /// <summary>
    ///     The error message returned by the API.
    /// </summary>
    public string Error { get; }

    /// <summary>
    ///     Gets the information of this exception.
    /// </summary>
    public string ErrorDescription { get; }

    /// <summary>
    ///     Creates a new [PixException] instance for an API error with the given [error].
    /// </summary>
    public static PixException ApiError(Dictionary<string, dynamic> errorMap)
    {
        string error = errorMap.ContainsKey("error") ? errorMap["error"] : "uncaughtError";
        string errorDescription = errorMap.ContainsKey("error_description")
            ? errorMap["error_description"]
            : errorMap.ToString();

        return new BbApiException(error, errorDescription);
    }

    public override string ToString()
    {
        return $"BBApiException: error: {Error}, errorData: {ErrorDescription}";
    }
}