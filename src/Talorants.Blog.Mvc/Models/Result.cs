namespace Talorants.Blog.Mvc.Models;


public class Result
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    
    public Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public Result(string? errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }

    public Result(bool isSuccess, string? errorMessage)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public Result() {}
}

public class Result<T> : Result
{
    public Result(string? errorMessage) : base(errorMessage) { }
    public Result(bool isSuccess) : base(isSuccess) { }
    public Result(bool isSuccess, string? errorMessage) : base(isSuccess, errorMessage) { }

    public Result(T data)
    {
        Data = data;
        IsSuccess = true;
    }

    public T? Data { get; set; }
}