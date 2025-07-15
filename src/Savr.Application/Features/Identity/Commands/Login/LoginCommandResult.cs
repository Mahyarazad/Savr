namespace Savr.Application.Features.Identity.Commands.Login
{
    public record LoginCommandResult(string Id, string UserName, string Email, string Token);

}