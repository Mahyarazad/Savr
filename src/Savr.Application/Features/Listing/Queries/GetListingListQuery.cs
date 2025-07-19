using Savr.Application.Abstractions;
using Savr.Application.Abstractions.Messaging;
using Savr.Application.Features.Listing;

namespace Savr.Application.Features.Products.Queries
{
    public record struct GetListingListQuery(int pageNumber, int pageSize,
        IEnumerable<SqlFilter> Filters) : IListQuery<PagedResult<ListingDTO>>
    {
    }
}
