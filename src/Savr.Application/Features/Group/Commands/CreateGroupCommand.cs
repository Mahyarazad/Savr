using FluentResults;
using Savr.Application.Abstractions.Messaging;


namespace Savr.Application.Features.Group.Commands
{
    public record struct CreateGroupCommand(string Title, string Description) : ICommand<Result<GroupDTO>>;
}
