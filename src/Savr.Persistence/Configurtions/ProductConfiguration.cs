using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(p => p.ManufactureEmail)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(p => p.ManufacturePhone)
                .HasMaxLength(128);

            builder.Property(p => p.ProductDate)
                .IsRequired();

            builder.Property(p => p.IsAvailable)
                .HasDefaultValue(true);

            builder.HasIndex(p => new { p.ManufactureEmail, p.ProductDate }, "UniqueIndex_MEmail_PDate")
                .IsUnique(true);

            builder.HasQueryFilter(p => p.IsAvailable);

            // ✅ Vector field for PostgreSQL (pgvector)
            builder.Property<float[]>("Embedding")
                .HasColumnType("vector(1536)") // use correct dimensions (e.g., OpenAI = 1536)
                .HasColumnName("embedding");
        }
    }

}
