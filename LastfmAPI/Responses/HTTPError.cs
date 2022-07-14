using System.Net;

namespace LastfmAPI.Responses;

public class HTTPError : IResponse
{
    public HttpStatusCode StatusCode { get; private set; }

    public HTTPError(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }
}