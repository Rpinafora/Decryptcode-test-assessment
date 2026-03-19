namespace Decryptcode.Assessment.Service.Application.Utils.Results;

public interface IRequestResult
{
    string? Error { get; set; }

    int StatusCode { get; set; }
}

public interface IRequestResult<T> : IRequestResult
{
    T? Content { get; set; }
}
