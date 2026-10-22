namespace QuestLog.Application.Common;

public class Result<T>
{
    public ErrorType? ErrorType { get; }
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string ErrorMessage { get; } = string.Empty;

    private Result(bool isSuccess, T? data, string errorMessage, ErrorType? errorType)
    {
        this.IsSuccess = isSuccess;
        this.Data = data;
        this.ErrorMessage = errorMessage;
        this.ErrorType = errorType;
    }

    public static Result<T> Success(T data) => new(true, data, string.Empty, null);
    public static Result<T> Failure(ErrorType errorType ,string errorMessage) => new(false, default, errorMessage, errorType);

}


public class Result
{
    public ErrorType? ErrorType { get; }
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }

    private Result(bool isSuccess, string errorMessage, ErrorType? errorType)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static Result Success() => new(true, string.Empty, null);
    public static Result Failure(ErrorType errorType ,string errorMessage) => new(false, errorMessage, errorType);
}
