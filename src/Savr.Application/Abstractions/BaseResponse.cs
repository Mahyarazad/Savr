using System.Net;

namespace Savr.Application.Abstractions
{
    public abstract class BaseResponse()
    {
        protected HttpStatusCode StatusCode { get; init; }
    }
}
