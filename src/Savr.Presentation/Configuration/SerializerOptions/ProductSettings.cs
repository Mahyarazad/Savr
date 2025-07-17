using Savr.Application.Features.Listings.Commands;

using System.Text.Json.Serialization;

namespace Savr.Presentation.Configuration.SerializerOptions
{
    [JsonSerializable(typeof(CreateListingCommand))]
    [JsonSerializable(typeof(CreateListingResponse))]
    public partial class ListingSettings : JsonSerializerContext
    {
    }
}
