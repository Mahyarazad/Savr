using Dapper;
using Microsoft.EntityFrameworkCore;
using Savr.Application.Abstractions.Data;
using Savr.Domain.Abstractions.Persistence.Repositories;
using Savr.Domain.Entities;

using Savr.Persistence.Data;

namespace Savr.Persistence.Repositories
{
    public class ListingRepository(ApplicationDbContext context, IDapperService dapper) : IListingRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IDapperService _dapper = dapper;

        public async Task AddAsync(Listing value, CancellationToken cancellationToken = default)
        {
            await _context.Set<Listing>().AddAsync(value, cancellationToken);
        }

        public async Task<Listing?> GetByIdAsync(long listingId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Listing>()
                .FirstOrDefaultAsync(p => p.Id == listingId, cancellationToken);
        }

        public async Task<int> DeleteAsync(long listingId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Listing>()
                .Where(p => p.Id == listingId)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public Task UpdateAsync(Listing value, CancellationToken cancellationToken = default)
        {
            _context.Set<Listing>().Update(value);
            return Task.CompletedTask;
        }

        public Task<bool> DoesUserOwnThisListingAsync(long listingId, Guid userId, CancellationToken cancellationToken = default)
        {
            return _context.Set<Listing>()
                .AnyAsync(p => p.Id == listingId && p.UserId == userId, cancellationToken);
        }

        public Task<bool> AnyAsync(long listingId, CancellationToken cancellationToken = default)
        {
            return _context.Set<Listing>()
                .AnyAsync(x => x.Id == listingId, cancellationToken);
        }

        public async Task<IEnumerable<Listing>> GetListingListAsync(
            int pageNumber, int pageSize,
            string? nameFilter, string? manufactureEmailFilter, string? phoneFilter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("PageNumber", pageNumber, System.Data.DbType.Int16, System.Data.ParameterDirection.Input);
            parameters.Add("PageSize", pageSize, System.Data.DbType.Int16, System.Data.ParameterDirection.Input);
            parameters.Add("NameFilter", nameFilter, System.Data.DbType.String, System.Data.ParameterDirection.Input);
            parameters.Add("ManufactureEmailFilter", manufactureEmailFilter, System.Data.DbType.String, System.Data.ParameterDirection.Input);
            parameters.Add("PhoneFilter", phoneFilter, System.Data.DbType.String, System.Data.ParameterDirection.Input);

            using var connection = _dapper.CreateConnection();
            return await connection.QueryAsync<Listing>(
                "GetFilteredDataWithPagination",
                parameters,
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
