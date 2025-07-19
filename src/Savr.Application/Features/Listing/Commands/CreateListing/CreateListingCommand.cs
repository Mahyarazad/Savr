using FluentResults;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Features.Listing;

namespace Savr.Application.Features.Listings.Commands
{
    public record struct CreateListingCommand(
        string Name,
        string Description,
        string Location,
        decimal PreviousPrice,
        decimal CurrentPrice,
        decimal PriceWithPromotion,
        Guid UserId,
        long GroupId,
        List<string> TagNames
    ) : ICommand<Result<ListingDTO>>;

}
