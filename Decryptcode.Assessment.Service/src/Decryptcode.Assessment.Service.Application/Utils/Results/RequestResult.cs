namespace Decryptcode.Assessment.Service.Application.Utils.Results;

public sealed class RequestResult<T> : IRequestResult<T>
{
    public T? Content { get; set; }

    public int StatusCode { get; set; }

    public string? Error { get; set; }
}
