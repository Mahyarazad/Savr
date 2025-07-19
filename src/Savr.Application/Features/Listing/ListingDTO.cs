namespace Savr.Application.Features.Listing
{


    public record struct ListingDTO(
       long Id,
       string Name,
       DateTime CreationDate,
       DateTime UpdateDate,
       string Description,
       string Location,
       decimal AverageRating,
       bool IsAvailable,
       Guid UserId,
       long GroupId,
       decimal PreviousPrice,
       decimal CurrentPrice,
       decimal PriceWithPromotion,
       double PriceDropPercentage
   );

}
