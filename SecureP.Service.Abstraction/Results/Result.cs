namespace SecureP.Service.Abstraction.Results;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public IReadOnlyList<Error> Errors { get; }

    private Result(bool isSuccess, T? value, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;



        Value = value;
        Errors = errors;
    }

    public static Result<T> Success(T value) => new(true, value, []);
    public static Result<T> Failure(IReadOnlyList<Error> errors) => new(false, default, errors);
}