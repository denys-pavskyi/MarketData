namespace MarketData.BLL.Models.Responses;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public ErrorResponse? Error { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(ErrorResponse error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(ErrorResponse error) => new(error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<ErrorResponse, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error!);
    }
}

public class Result
{
    public bool IsSuccess { get; }
    public ErrorResponse? Error { get; }

    protected Result(bool isSuccess, ErrorResponse? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new Result(true);

    public static Result Failure(ErrorResponse error) => new Result(false, error);

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<ErrorResponse, TResult> onFailure)
    {
        return IsSuccess ? onSuccess() : onFailure(Error!);
    }
}