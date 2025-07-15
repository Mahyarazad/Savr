using Dapper;
using Microsoft.EntityFrameworkCore;
using Savr.Application.Abstractions.Data;
using Savr.Domain.Abstractions.Persistence.Repositories;
using Savr.Domain.Entities;

using Savr.Persistence.Data;
using System.Text.RegularExpressions;

namespace Savr.Persistence.Repositories
{
    public class ListingRepository : Repository<Listing>, IListingRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDapperService _dapper;

        public ListingRepository(ApplicationDbContext context, IDapperService dapper) : base (context)
        {
            _context = context;
            _dapper = dapper;
        }

        public async Task<bool> DoesUserOwnThisListingAsync(long listingId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Listing>()
                .AnyAsync(x => x.Id == listingId && x.UserId == userId, cancellationToken);
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

        public async Task<int> Activate(long groupId, CancellationToken cancellationToken = default)
        {
            var listing = await _context.Set<Listing>().FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken);
            if (listing == null)
            {
                return 0;
            }

            var result = listing.Activate();

            if (result.IsSuccess)
            {
                _context.Set<Listing>().Update(listing);
                return await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                return 0;
            }
        }

        public async Task<int> DeActivate(long groupId, CancellationToken cancellationToken = default)
        {
            var listing = await _context.Set<Listing>().FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken);
            if (listing == null)
            {
                return 0;
            }

            var result = listing.Deactivate();

            if (result.IsSuccess)
            {
                _context.Set<Listing>().Update(listing);
                return await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                return 0;
            }
        }
    }
}
