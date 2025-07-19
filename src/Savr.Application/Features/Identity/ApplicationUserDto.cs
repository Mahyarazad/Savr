namespace Savr.Application.Features.Identity
{
    public record struct ApplicationUserDto(
        string Id,
        string? UserName,
        string? Email,
        bool EmailConfirmed,
        string? PhoneNumber,
        bool PhoneNumberConfirmed,
        bool TwoFactorEnabled,
        string? Firstname,
        string? Lastname,
        List<string>? Roles
    );

}
