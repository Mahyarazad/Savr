using Savr.Application.Features.Products.Commands.CreateProduct;
using System.Text.Json.Serialization;

namespace Savr.Presentation.Configuration.SerializerOptions
{
    [JsonSerializable(typeof(CreateProductCommand))]
    [JsonSerializable(typeof(CreateProductResponse))]
    public partial class ProductSettings : JsonSerializerContext
    {
    }
}
