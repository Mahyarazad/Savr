using FluentResults;
using Savr.Application.Abstractions.Messaging;


namespace Savr.Application.Features.Group.Commands
{
    public record struct DeleteGroupCommand(long Id) : ICommand<Result>;
}
