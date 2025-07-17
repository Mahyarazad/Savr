using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;
using Savr.Domain.Entities;
using System.Reflection.Emit;


namespace Savr.Persistence.Configurtions
{

    public class ListingConfiguration : IEntityTypeConfiguration<Listing>
    {
        public void Configure(EntityTypeBuilder<Listing> builder)
        {
            builder.ToTable("Listings");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(p => p.Location)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(p => p.CreationDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.UpdateDate)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(p => p.IsAvailable)
                .HasDefaultValue(true);

            builder.Property(p => p.AverageRating)
                .HasPrecision(3, 2) // Example: 4.75
                .HasDefaultValue(0m);

            builder.Property(p => p.PreviousPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.CurrentPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.PriceWithPromotion)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.PriceDropPercentage)
                .HasPrecision(5, 2); // e.g. 25.32

            builder.HasIndex(p => new { p.Description, p.Name }, "UniqueIndex_PDes_PName")
                .IsUnique(true);

            builder.HasQueryFilter(p => p.IsAvailable);

            builder.HasMany(p => p.Tags)
                .WithOne(x=>x.Listing)
                .HasForeignKey(x=>x.ListingId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(nameof(Listing.Tags))
                .UsePropertyAccessMode(PropertyAccessMode.Field); // Use the backing field "_reviews"

            builder.HasMany(x=> x.Reviews)
                .WithOne(x => x.Listing)
                .HasForeignKey(x => x.ListingId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(nameof(Listing.Reviews))
                .UsePropertyAccessMode(PropertyAccessMode.Field); // Use the backing field "_reviews"   


            builder.HasQueryFilter(p => p.IsAvailable);

            // ✅ Vector field for PostgreSQL (pgvector)
            //builder.Property(e => e.Embedding)
            //  .HasColumnType("vector(1536)")
            //  .HasConversion(
            //      v => new Vector(v),        // float[] → Vector
            //      v => v.ToArray()           // Vector → float[]
            //  );
        }
    }

}
