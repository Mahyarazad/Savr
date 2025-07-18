using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Savr.Domain.Entities;

namespace Savr.Persistence.Configurtions
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {

            builder.ToTable(nameof(Group));

            builder.HasKey(x => x.Id);

            var adminUserGuid = Guid.Parse("3d32337a-7372-4261-98b9-8352c83d8751");

            builder.HasData(
                new Group(1, "Food & Drinks", "Restaurants, cafes, and food delivery services.", adminUserGuid),
                new Group(2, "Beauty & Fitness", "Gyms, spas, salons, and wellness services.", adminUserGuid),
                new Group(3, "Attraction & Leisure", "Entertainment venues, attractions, and recreational activities.", adminUserGuid),
                new Group(4, "Fashion & Retail", "Clothing stores, boutiques, and shopping centers.", adminUserGuid),
                new Group(5, "Everyday Services", "Essential services for daily needs like laundry or repairs.", adminUserGuid),
                new Group(6, "Hotel & Travel", "Hotels, travel agencies, and transportation services.", adminUserGuid),
                new Group(7, "Education", "Schools, tutors, and educational institutions.", adminUserGuid),
                new Group(8, "Healthcare", "Hospitals, clinics, and healthcare providers.", adminUserGuid),
                new Group(9, "Financial Services", "Banks, insurance, and financial consulting.", adminUserGuid),
                new Group(10, "Real Estate", "Property listings, realtors, and housing services.", adminUserGuid),
                new Group(11, "Consulting", "Professional consulting for businesses or individuals.", adminUserGuid)
            );
        }
    }
}
