

namespace Savr.Domain.Abstractions.Persistence.Repositories
{
    
    namespace Savr.Domain.Abstractions.Persistence.Repositories
    {
        public interface IGroupRepository
        {
            Task AddAsync(Group group, CancellationToken cancellationToken = default);

            Task<Group?> GetByIdAsync(long groupId, CancellationToken cancellationToken = default);

            Task<IEnumerable<Group>> GetUserGroupsAsync(Guid userId, CancellationToken cancellationToken = default);

            Task UpdateAsync(Group group, CancellationToken cancellationToken = default);

            Task<int> DeleteAsync(long groupId, CancellationToken cancellationToken = default);

            Task<bool> AnyAsync(long groupId, CancellationToken cancellationToken = default);

            Task<bool> DoesUserOwnGroupAsync(long groupId, Guid userId, CancellationToken cancellationToken = default);
        }
    }

}
