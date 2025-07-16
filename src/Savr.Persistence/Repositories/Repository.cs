using Microsoft.EntityFrameworkCore;
using Savr.Domain.Abstractions.Persistence.Repositories;
using System.Linq.Expressions;

namespace Savr.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object?[] { id }, cancellationToken);
        }

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity is null)
                return;

            _dbSet.Remove(entity);
            
        }

        public virtual async Task<bool> AnyAsync(long id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object?[] { id }, cancellationToken) is not null;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

    }
}
