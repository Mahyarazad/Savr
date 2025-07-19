using Dapper;
using Microsoft.EntityFrameworkCore;
using Savr.Application.Abstractions.Data;
using Savr.Domain.Entities;

using Savr.Persistence.Data;
using System.Reflection;
using System.Data;
using Savr.Application.Abstractions;
using Savr.Application.Abstractions.Persistence.Data;
using Savr.Application.Abstractions.Persistence.Repositories;
using Savr.Application.Features.Listing;

namespace Savr.Persistence.Repositories
{
    public class ListingRepository : Repository<Listing>, IListingRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDapperService _dapper;

        public ListingRepository(ApplicationDbContext context, IDapperService dapper) : base(context)
        {
            _context = context;
            _dapper = dapper;
        }

        public async Task<bool> DoesUserOwnThisListingAsync(long listingId, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Listing>()
                .AnyAsync(x => x.Id == listingId && x.UserId == userId, cancellationToken);
        }




        public async Task<Application.Abstractions.PagedResult<ListingDTO>> GetListingListAsync(
            int pageNumber , 
            int pageSize,
            IEnumerable<Application.Abstractions.SqlFilter>? filters = null)
        {
            var query = _context.Set<Listing>().AsQueryable();

            // Apply dynamic filters
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    query = FilterExtensions.ApplyFilter(query, filter);
                }
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Get paginated data
            var items = await query
                .OrderByDescending(x => x.Id) // Default order
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ListingDTO(
                    x.Id,
                    x.Name,
                    x.CreationDate,
                    x.UpdateDate,
                    x.Description,
                    x.Location,
                    x.AverageRating,
                    x.IsAvailable,
                    x.UserId,
                    x.GroupId,
                    x.PreviousPrice,
                    x.CurrentPrice,
                    x.PriceWithPromotion,
                    x.PriceDropPercentage
                ))
                .ToListAsync();

            return new PagedResult<ListingDTO>
            {
                TotalCount = totalCount,
                Items = items
            };
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

        public async Task AddWithTagsAsync(Listing listing, List<Tag> tags, CancellationToken cancellationToken = default)
        {
            foreach (var tag in tags)
            {
                _context.Entry(tag).State = EntityState.Unchanged; // attach existing or
                                                                   // _context.Entry(tag).State = EntityState.Added;  // for new ones
            }

            var listingTagsField = typeof(Listing)
                .GetField("_tags", BindingFlags.Instance | BindingFlags.NonPublic);

            if (listingTagsField != null)
            {
                var tagList = listingTagsField.GetValue(listing) as IList<Tag>;
                foreach (var tag in tags)
                    tagList?.Add(tag);
            }

            await _dbSet.AddAsync(listing, cancellationToken);
        }


        public Task UpdateWithTags(Listing listing, List<Tag> newTags)
        {
            // Attach tags to ensure they are tracked
            AttachTags(newTags);

            var listingTagsField = typeof(Listing)
                .GetField("_tags", BindingFlags.Instance | BindingFlags.NonPublic);

            if (listingTagsField != null)
            {
                var currentTagList = listingTagsField.GetValue(listing) as IList<Tag>;
                if (currentTagList == null)
                    return Task.CompletedTask;

                // Clear old tags
                currentTagList.Clear();

                // Add new ones
                foreach (var tag in newTags)
                {
                    currentTagList.Add(tag);
                }
            }

            // Mark listing as modified
            _context.Entry(listing).State = EntityState.Modified;

            return Task.CompletedTask;
        }

        private void AttachTags(List<Tag> tags)
        {
            foreach (var tag in tags)
            {
                if (_context.Entry(tag).State == EntityState.Detached)
                {
                    _context.Attach(tag);
                }
            }
        }


    }
}
