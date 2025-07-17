using Savr.Domain.Primitives;

namespace Savr.Domain.Entities
{
    public class Tag : BaseEntity<long>
    {
        protected Tag() { }
        public Tag(string name)
        {
            Name = name;
        }

        public string Name { get; private set; } = default!;

        public long ListingId { get; private set; }
        // Navigation Property
        public Listing? Listing { get; private set; }
    }
}