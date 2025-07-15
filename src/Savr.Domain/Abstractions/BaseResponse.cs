using System.Net;

namespace Savr.Domain.Abstractions
{
    public abstract class BaseResponse()
    {
        protected HttpStatusCode StatusCode { get; init; }
    }
}
