using FluentResults;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.DTOs;

namespace Savr.Application.Features.Products.Commands.UpdateProduct
{
    public record struct UpdateListingCommand(
        long Id,
        string Name,
        string Description,
        string Location,
        decimal PreviousPrice,
        decimal CurrentPrice,
        decimal PriceWithPromotion,
        long GroupId,
        Guid UserId,
         List<string> TagNames
    ) : ICommand<Result>;

}
