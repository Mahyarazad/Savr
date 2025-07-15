using FluentResults;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.DTOs;

namespace Savr.Application.Features.Products.Commands.CreateProduct
{
    public record CreateProductCommand(
        string Name,
        string ManufactureEmail,
        string? ManufacturePhone,
        long GroupId,
        Guid UserId
    ) : ICommand<Result<ListingDTO>>;
}
