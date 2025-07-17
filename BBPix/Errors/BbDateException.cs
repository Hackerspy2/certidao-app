namespace BBPix.Errors;

public class BbDateException : PixException
{
    private readonly string _error;
    private readonly string _errorDescription;

    public BbDateException(string error, string errorDescription, DateTime initialDate, DateTime finalDate, int difference) : base(errorDescription, error)
    {
        _error = error;
        _errorDescription = errorDescription;
        InitialDate = initialDate;
        FinalDate = finalDate;
        Difference = difference;
    }

    /// <summary>
    ///     The error message returned by the API.
    /// </summary>
    public override string Error => _error;

    /// <summary>
    ///     Gets the information of this exception.
    /// </summary>
    public override string ErrorDescription => ToString();

    /// <summary>
    ///     Exception to represent a date-range-start.
    /// </summary>
    public DateTime InitialDate { get; }

    /// <summary>
    ///     Exception to represent a date-range-end.
    /// </summary>
    public DateTime FinalDate { get; }

    /// <summary>
    ///     Exception to represent a date-range-difference-in-days.
    /// </summary>
    public int Difference { get; }

    /// <summary>
    ///     Factory method for creating an instance of [BBHttpException].
    /// </summary>
    /// <param name="dateTimeRange">The date time range.</param>
    /// <returns>An instance of BBDateException.</returns>
    //public static PixException DifferenceBetweenDatesTooLong(DateTime dateTimeRange)
    //{
    //    return new BBDateException(
    //        "difference-between-dates-too-long",
    //        dateTimeRange.Start,
    //        dateTimeRange.End,
    //        (int)dateTimeRange.Duration.TotalDays
    //    );
    //}

    /// <summary>
    ///     Returns a string representation of this exception.
    /// </summary>
    public override string ToString()
    {
        return
            $"BBDateException: error: {_error}, initialDate: {InitialDate}, finalDate: {FinalDate}, difference: {Difference} days";
    }
}