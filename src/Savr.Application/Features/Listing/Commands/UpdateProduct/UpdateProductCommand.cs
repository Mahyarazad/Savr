using FluentResults;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.DTOs;

namespace Savr.Application.Features.Products.Commands.UpdateProduct
{
    public record struct UpdateProductCommand(
        long Id,
        string Name,
        string ManufactureEmail,
        string? ManufacturePhone,
        long GroupId,
        Guid UserId
    ) : ICommand<Result<ListingDTO>>;
}
