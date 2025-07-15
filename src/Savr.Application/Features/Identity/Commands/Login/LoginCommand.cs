using FluentResults;
using Savr.Application.Abstractions.Messaging;

namespace Savr.Application.Features.Identity.Commands.Login
{
    public record struct LoginCommand(string EmailOrUser, string Password) : ICommand<Result<LoginCommandResult>> { }

}
