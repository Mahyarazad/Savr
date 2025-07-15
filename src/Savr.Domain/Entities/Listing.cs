using FluentResults;
using Savr.Domain.Primitives;

namespace Savr.Domain.Entities
{
    public class Listing : BaseEntity<long>
    {
        protected Listing() { }

        public string Name { get; private set; } = default!;
        public DateTime CreationDate { get; private set; }
        public DateTime UpdateDate { get; private set; }
        public string ManufactureEmail { get; private set; } = default!;
        public string? ManufacturePhone { get; private set; }
        public bool IsAvailable { get; private set; }
        public Guid UserId { get; private set; }

        //public float[]? Embedding { get; private set; }
        public long GroupId { get; private set; }
        public Group? Group { get; private set; }

        private Listing(string name, string manufactureEmail, string? manufacturePhone, Guid userId, long groupId)
        {
            Name = name;
            ManufactureEmail = manufactureEmail;
            ManufacturePhone = manufacturePhone;
            CreationDate = DateTime.UtcNow;
            
            IsAvailable = true;
            UserId = userId;
            GroupId = groupId;
        }

        public static Result<Listing> Create(string name, string manufactureEmail, string? manufacturePhone, Guid userId, long groupId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Fail("Name is required.");

            if (string.IsNullOrWhiteSpace(manufactureEmail))
                return Result.Fail("Manufacture email is required.");

            var product = new Listing(name, manufactureEmail, manufacturePhone, userId, groupId);
            return Result.Ok(product);
        }

        public Result<Listing> Update(string name, string manufactureEmail, string? manufacturePhone)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Fail("Name is required.");

            if (string.IsNullOrWhiteSpace(manufactureEmail))
                return Result.Fail("Manufacture email is required.");

            Name = name;
            ManufactureEmail = manufactureEmail;
            ManufacturePhone = manufacturePhone;
            UpdateDate = DateTime.UtcNow;
            return Result.Ok(this);
        }

        public Result<Listing> Activate()
        {
            if (IsAvailable)
                return Result.Fail("Listing is already active.");

            IsAvailable = true;
            return Result.Ok(this);
        }

        public Result<Listing> Deactivate()
        {
            if (!IsAvailable)
                return Result.Fail("Listing is already inactive.");

            IsAvailable = false;
            return Result.Ok(this);
        }
    }


}
