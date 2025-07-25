﻿using Savr.Domain.Entities;

namespace Savr.Application.Abstractions.Persistence.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {

        Task<bool> DoesUserOwnGroupAsync(long groupId, Guid userId, CancellationToken cancellationToken = default);
    }
}