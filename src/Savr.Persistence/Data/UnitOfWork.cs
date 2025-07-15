using Savr.Domain.Abstractions.Persistence.Data;

namespace Savr.Persistence.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        // The benefits are: removes the presist changes into the databse responsibility from repositories
        // Use the unit of work that has one sole responsibility which is persist data into database
    }
}
