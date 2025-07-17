using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;
namespace Savr.Application.Features.Listings.Commands
{
    public record struct CreateListingResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Listing { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? Errors { get; set; }
        public Guid RequestId => Guid.NewGuid();
        public bool IsSuccessful => Errors == null;
        public HttpStatusCode StatusCode { get; set; }
    }

}
