using FluentResults;
using Savr.Domain.Entities;

namespace Savr.Domain.Abstractions.Persistence.Repositories
{
    public interface IListingRepository
    {
        Task AddAsync(Listing value, CancellationToken cancellationToken = default);

        Task<Listing?> GetByIdAsync(long listingId, CancellationToken cancellationToken = default);

        Task<int> DeleteAsync(long listingId, CancellationToken cancellationToken = default);

        Task UpdateAsync(Listing value, CancellationToken cancellationToken = default);

        Task<bool> DoesUserOwnThisListingAsync(long listingId, Guid userId, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(long listingId, CancellationToken cancellationToken = default);


        Task<IEnumerable<Listing>> GetListingListAsync(
            int pageNumber,
            int pageSize,
            string? nameFilter,
            string? manufactureEmailFilter,
            string? phoneFilter);
    }
}
