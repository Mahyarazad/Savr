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
        private readonly IDapperService _dapper;

        public GroupRepository(ApplicationDbContext context, IDapperService dapper) : base(context) 
        {
            _context = context;
            _dapper = dapper;
        }

        public async Task<int> Activate(long groupId, CancellationToken cancellationToken = default)
        {
            var group = await _context.Set<Group>().FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken);
            if (group == null)
            {
                return 0;
            }

            var result = group.Activate();

            if (result.IsSuccess)
            {
                _context.Set<Group>().Update(group);
                return await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                return 0;
            }
        }

        public async Task<int> DeActivate(long groupId, CancellationToken cancellationToken = default)
        {
            var group = await _context.Set<Group>().FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken);
            if (group == null)
            {
                return 0;
            }

            var result = group.Deactivate();

            if (result.IsSuccess)
            {
                _context.Set<Group>().Update(group);
                return await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                return 0;
            }
        }

        public Task<bool> DoesUserOwnGroupAsync(long groupId, Guid userId, CancellationToken cancellationToken = default)
        {
            return _context.Set<Group>()
                .AnyAsync(g => g.Id == groupId && g.OwnerUserId == userId, cancellationToken);
        }

        
    }
}
