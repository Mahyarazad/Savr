using Microsoft.EntityFrameworkCore;
using Savr.Domain.Abstractions.Persistence.Repositories;
using Savr.Domain.Entities;

namespace Savr.Persistence.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(DbContext context) : base(context)
        {

        }
    }
}
