using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Savr.Domain.Entities;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Savr.Persistence.Data
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<Listing> Listings { get; set; } = default!;
        public DbSet<Group> Groups { get; set; } = default!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
