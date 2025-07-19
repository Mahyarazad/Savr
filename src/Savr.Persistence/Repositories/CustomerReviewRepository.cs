using Savr.Application.Abstractions.Data;
using Savr.Application.Abstractions.Persistence.Repositories;
using Savr.Domain.Entities;
using Savr.Persistence.Data;

namespace Savr.Persistence.Repositories
{
    public class CustomerReviewRepository : Repository<CustomerReview>, ICustomerReviewRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IDapperService _dapper;
        public CustomerReviewRepository(ApplicationDbContext context, IDapperService dapper) : base(context)
        {
            _context = context;
            _dapper = dapper;
        }
    }
}
