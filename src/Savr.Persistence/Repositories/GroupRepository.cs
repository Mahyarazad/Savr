using Dapper;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Savr.Application.Abstractions.Data;
using Savr.Domain.Abstractions.Persistence.Repositories;
using Savr.Domain.Entities;
using Savr.Persistence.Data;


namespace Savr.Persistence.Repositories
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupRepository(ApplicationDbContext context) : base(context) 
        {
            _context = context;
           
        }


        public Task<bool> DoesUserOwnGroupAsync(long groupId, Guid userId, CancellationToken cancellationToken = default)
        {
            return _context.Set<Group>()
                .AnyAsync(g => g.Id == groupId && g.OwnerUserId == userId, cancellationToken);
        }

        
    }
}
