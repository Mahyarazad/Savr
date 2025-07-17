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
        public DbSet<Tag> Tags { get; set; } = default!;
        public DbSet<CustomerReview> CustomerReviews { get; set; } = default!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var adminUserId = "3d32337a-7372-4261-98b9-8352c83d8751";
            Guid.TryParse(adminUserId, out var adminUserGuid);

            modelBuilder.Entity<Group>().HasData(
                 new Group("Food & Drinks", "Restaurants, cafes, and food delivery services.", adminUserGuid) { Id = -1 },
                new Group("Beauty & Fitness", "Gyms, spas, salons, and wellness services.", adminUserGuid) { Id = -2 },
                new Group("Attraction & Leisure", "Entertainment venues, attractions, and recreational activities.", adminUserGuid) { Id = -3 },
                new Group("Fashion & Retail", "Clothing stores, boutiques, and shopping centers.", adminUserGuid) { Id = -4 },
                new Group("Everyday Services", "Essential services for daily needs like laundry or repairs.", adminUserGuid) { Id = -5 },
                new Group("Hotel & Travel", "Hotels, travel agencies, and transportation services.", adminUserGuid) { Id = -6 },
                new Group("Education", "Schools, tutors, and educational institutions.", adminUserGuid) { Id = -7 },
                new Group("Healthcare", "Hospitals, clinics, and healthcare providers.", adminUserGuid) { Id = -8 },
                new Group("Financial Services", "Banks, insurance, and financial consulting.", adminUserGuid) { Id = -9 },
                new Group("Real Estate", "Property listings, realtors, and housing services.", adminUserGuid) { Id = -10 },
                new Group("Consulting", "Professional consulting for businesses or individuals.", adminUserGuid) { Id = -11 }
            );

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Listing>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Property("UpdateDate").CurrentValue = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

       
    }
}
