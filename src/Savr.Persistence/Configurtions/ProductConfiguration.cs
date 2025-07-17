using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pgvector;
using Savr.Domain.Entities;


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

            builder.Property(p => p.ManufacturePhone)
                .HasMaxLength(128);

            builder.Property(p => p.CreationDate)
                .IsRequired();

            builder.Property(p => p.IsAvailable)
                .HasDefaultValue(true);

            builder.HasIndex(p => new { p.Description, p.Name }, "UniqueIndex_PDes_PName")
                .IsUnique(true);

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
