using FluentResults;
using Savr.Domain.Entities;

namespace Savr.Domain.Abstractions.Persistence.Repositories
{
    public interface IListingRepository : IRepository<Listing>
    {

        Task AddWithTagsAsync(Listing listing, List<Tag> tags, CancellationToken cancellationToken = default);

        Task UpdateWithTags(Listing listing, List<Tag> tags);

        Task<int> Activate(long groupId, CancellationToken cancellationToken = default);

        Task<int> DeActivate(long groupId, CancellationToken cancellationToken = default);

        Task<bool> DoesUserOwnThisListingAsync(long listingId, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Listing>> GetListingListAsync(
            int pageNumber,
            int pageSize,
            string? nameFilter,
            string? manufactureEmailFilter,
            string? phoneFilter);
    }
}
