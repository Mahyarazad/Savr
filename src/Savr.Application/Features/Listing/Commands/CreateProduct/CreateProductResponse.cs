using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json.Serialization;
namespace Savr.Application.Features.Products.Commands.CreateProduct
{
    public record struct CreateProductResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Product { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? Errors { get; set; }
        public Guid RequestId => Guid.NewGuid();
        public bool IsSuccessful => Errors == null;
        public HttpStatusCode StatusCode { get; set; }
    }

}
