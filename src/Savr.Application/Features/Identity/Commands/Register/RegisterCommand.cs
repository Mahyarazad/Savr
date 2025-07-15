
using FluentResults;
using Savr.Application.Abstractions.Messaging;

namespace Savr.Application.Features.Identity.Commands.Register
{
    public record RegisterCommand(string FirstName, string LastName, string Email, string UserName, string Password) 
        : ICommand<Result<RegisterCommandResult>>;

}
