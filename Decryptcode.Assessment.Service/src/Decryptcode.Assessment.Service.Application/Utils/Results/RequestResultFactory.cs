namespace Decryptcode.Assessment.Service.Application.Utils.Results;

public static class RequestResultFactory<T>
{
    public static RequestResult<T> Ok(T content)
    {
        return new RequestResult<T>
        {
            Content = content,
            StatusCode = 200,
            Error = null
        };
    }

    public static RequestResult<T> Created(T content)
    {
        return new RequestResult<T>
        {
            Content = content,
            StatusCode = 201,
            Error = null
        };
    }

    public static RequestResult<T> NoContent()
    {
        return new RequestResult<T>
        {
            Content = default,
            StatusCode = 204,
            Error = null
        };
    }

    public static RequestResult<T> BadRequest(params string[] errors)
    {
        var list = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
        var error = (list != null && list.Any()) ? string.Join("; ", list) : null;
        return new RequestResult<T>
        {
            Content = default,
            StatusCode = 400,
            Error = error
        };
    }

    public static RequestResult<T> BadRequest(IEnumerable<string> errors)
    {
        var list = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList();
        var error = (list != null && list.Any()) ? string.Join("; ", list) : null;
        return new RequestResult<T>
        {
            Content = default,
            StatusCode = 400,
            Error = error
        };
    }

    public static RequestResult<T> NotFound(string? message = null)
    {
        var error = string.IsNullOrWhiteSpace(message) ? null : message!.Trim();

        return new RequestResult<T>
        {
            Content = default,
            StatusCode = 404,
            Error = error
        };
    }

    public static RequestResult<T> Unauthorized(string? message = null)
    {
        var error = string.IsNullOrWhiteSpace(message) ? null : message!.Trim();
        return new RequestResult<T>
        {
            Content = default,
            StatusCode = 401,
            Error = error
        };
    }

    public static RequestResult<T> Forbid(string? message = null)
    {
        var error = string.IsNullOrWhiteSpace(message) ? null : message!.Trim();
        return new RequestResult<T>
        {
            Content = default,
            StatusCode = 403,
            Error = error
        };
    }

    public static RequestResult<T> Conflict(string? message = null)
    {
        var error = string.IsNullOrWhiteSpace(message) ? null : message!.Trim();
        return new RequestResult<T>
        {
            Content = default,
            StatusCode = 409,
            Error = error
        };
    }
}
