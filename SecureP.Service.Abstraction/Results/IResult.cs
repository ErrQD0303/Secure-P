namespace SecureP.Service.Abstraction.Results;

public interface IResult
{
    bool IsSuccess { get; }
    IReadOnlyList<Error> Errors { get; }
    static IResult Success() => throw new NotImplementedException();
    static IResult Failure(IReadOnlyList<Error> errors) => throw new NotImplementedException();
}
