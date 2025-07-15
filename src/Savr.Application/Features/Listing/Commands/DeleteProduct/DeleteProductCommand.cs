using FluentResults;
using Savr.Application.Abstractions.Messaging;

namespace Savr.Application.Features.Products.Commands.DeleteProduct
{
    public record struct DeleteProductCommand(long Id) : ICommand<Result>
    {
    }
}
