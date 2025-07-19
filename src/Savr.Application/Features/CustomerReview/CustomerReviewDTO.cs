namespace Savr.Application.Features.CustomerReview
{
    public readonly record struct CustomerReviewDTO(
        long Id,
        Guid UserId,
        string UserName,
        string Comment,
        int Rating,
        DateTime CreatedAt,
        int HelpfulCount,
        long ListingId
    );
}
