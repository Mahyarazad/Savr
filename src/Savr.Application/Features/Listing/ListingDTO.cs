namespace Savr.Application.DTOs
{
    public record struct ListingDTO(
        long Id,
        string Name,
        string ManufactureEmail,
        string? ManufacturePhone,
        bool IsAvailable,
        DateTime ProductDate,
        long GroupId,
        Guid UserId
    );
}
