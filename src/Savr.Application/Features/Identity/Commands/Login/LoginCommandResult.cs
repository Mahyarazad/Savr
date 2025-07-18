namespace Savr.Application.Features.Identity.Commands.Login
{
    public record struct LoginCommandResult(string Id, string UserName, string Email, string Token, string RefereshToken, DateTime RefreshTokenExpiryTime);

}