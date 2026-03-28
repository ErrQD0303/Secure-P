namespace SecureP.Service.Abstraction.Results;

public class ValidateObjectAnnotationResult : IResult
{
    public bool IsSuccess { get; }

    public IReadOnlyList<Error> Errors { get; }

    private ValidateObjectAnnotationResult(bool isSuccess, IReadOnlyList<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static ValidateObjectAnnotationResult Success() => new(true, []);
    public static ValidateObjectAnnotationResult Failure(IReadOnlyList<Error> errors) => new(false, errors);
}