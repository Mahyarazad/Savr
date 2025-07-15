

using Savr.Domain.Entities;

namespace Savr.Domain.Abstractions.Persistence.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {

        Task<int> Activate(long groupId, CancellationToken cancellationToken = default);
        Task<int> DeActivate(long groupId, CancellationToken cancellationToken = default);
        Task<bool> DoesUserOwnGroupAsync(long groupId, Guid userId, CancellationToken cancellationToken = default);
    }
}