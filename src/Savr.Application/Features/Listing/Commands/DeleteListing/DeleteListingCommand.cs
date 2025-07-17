using FluentResults;
using Savr.Application.Abstractions.Messaging;

namespace Savr.Application.Features.Listings.Commands
{
    public record struct DeleteListingCommand(long Id) : ICommand<Result>
    {
    }
}
