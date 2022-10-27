using TF2SA.Common.Errors;

namespace TF2SA.Http.Errors;

public class HttpError : Error
{
    public HttpError(string message)
    {
        Message = message;
    }
}
