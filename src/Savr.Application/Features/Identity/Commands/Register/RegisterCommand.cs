
using FluentResults;
using Savr.Application.Abstractions.Messaging;

namespace Savr.Application.Features.Identity.Commands.Register
{
    public record struct RegisterCommand(string FirstName, string LastName, string Email, string Password, string Role) 
        : ICommand<Result<RegisterCommandResult>>;

}
