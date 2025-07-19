using FluentResults;
using Savr.Domain.Entities;
using Savr.Application.Abstractions.Persistence.Data;
using Savr.Application.Features.Listing;

namespace Savr.Application.Abstractions.Persistence.Repositories
{
    public interface IListingRepository : IRepository<Listing>
    {

        Task AddWithTagsAsync(Listing listing, List<Tag> tags, CancellationToken cancellationToken = default);

        Task UpdateWithTags(Listing listing, List<Tag> tags);

        Task<int> Activate(long groupId, CancellationToken cancellationToken = default);

        Task<int> DeActivate(long groupId, CancellationToken cancellationToken = default);

        Task<bool> DoesUserOwnThisListingAsync(long listingId, Guid userId, CancellationToken cancellationToken = default);

        Task<Application.Abstractions.PagedResult<ListingDTO>> GetListingListAsync(
            int pageNumber,
            int pageSize,
            IEnumerable<Application.Abstractions.SqlFilter>? filters = null);
    }
}
