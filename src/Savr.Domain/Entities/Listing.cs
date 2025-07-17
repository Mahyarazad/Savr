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
        public string Description { get; private set; } = default!;

        public string Location { get; private set; } = default!;
        public decimal AverageRating { get; private set; }

        private readonly List<Tag> _tags = new();
        public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

        public bool IsAvailable { get; private set; }
        public Guid UserId { get; private set; }

        public long GroupId { get; private set; }
        public Group? Group { get; private set; }

        private readonly List<CustomerReview> _reviews = new();
        public IReadOnlyCollection<CustomerReview> Reviews => _reviews.AsReadOnly();


        // New Pricing Fields
        public decimal PreviousPrice { get; private set; }
        public decimal CurrentPrice { get; private set; }
        public decimal PriceWithPromotion { get; private set; }

        // Auto-calculated field
        public double PriceDropPercentage { get; private set; }

        private Listing(string name, DateTime creationDate, DateTime updateDate, string description, string location, decimal averageRating, List<Tag> tags, bool isAvailable, Guid userId, long groupId, Group? group, List<CustomerReview> reviews, decimal previousPrice, decimal currentPrice, decimal priceWithPromotion, double priceDropPercentage)
        {
            Name = name;
            CreationDate = creationDate;
            UpdateDate = updateDate;
            Description = description;
            Location = location;
            AverageRating = averageRating;
            _tags = tags;
            IsAvailable = isAvailable;
            UserId = userId;
            GroupId = groupId;
            Group = group;
            _reviews = reviews;
            PreviousPrice = previousPrice;
            CurrentPrice = currentPrice;
            PriceWithPromotion = priceWithPromotion;
            PriceDropPercentage = priceDropPercentage;
        }


        // Factory
        public static Result<Listing> Create(
                string name,
                string description,
                string location,
                Guid userId,
                long groupId,
                decimal previousPrice,
                decimal currentPrice,
                decimal priceWithPromotion)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Fail("Name is required.");

            if (string.IsNullOrWhiteSpace(description))
                return Result.Fail("Description is required.");

            if (string.IsNullOrWhiteSpace(location))
                return Result.Fail("Location is required.");

            if (previousPrice <= 0 || currentPrice <= 0 || priceWithPromotion < 0)
                return Result.Fail("Price values must be valid and greater than 0.");

            var priceDrop = previousPrice > 0
                ? Math.Round((double)((previousPrice - currentPrice) / previousPrice) * 100, 2)
                : 0;

            var listing = new Listing(
                name: name,
                creationDate: DateTime.UtcNow,
                updateDate: DateTime.UtcNow,
                description: description,
                location: location,
                averageRating: 0m,
                tags: new List<Tag>(),
                isAvailable: true,
                userId: userId,
                groupId: groupId,
                group: null,
                reviews: new List<CustomerReview>(),
                previousPrice: previousPrice,
                currentPrice: currentPrice,
                priceWithPromotion: priceWithPromotion,
                priceDropPercentage: priceDrop
            );

            return Result.Ok(listing);
        }


        public Result<Listing> Update(
                            string name,
                            string description,
                            string location,
                            decimal previousPrice,
                            decimal currentPrice,
                            decimal priceWithPromotion,
                                            long groupId,
                                            Guid userId)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Fail("Name is required.");

            if (string.IsNullOrWhiteSpace(description))
                return Result.Fail("Description is required.");

            if (string.IsNullOrWhiteSpace(location))
                return Result.Fail("Location is required.");

            if (previousPrice <= 0 || currentPrice <= 0 || priceWithPromotion < 0)
                return Result.Fail("Price values must be valid and greater than 0.");


            Name = name;
            Description = description;
            Location = location;
            PreviousPrice = previousPrice;
            CurrentPrice = currentPrice;
            PriceWithPromotion = priceWithPromotion;
            GroupId = groupId;
            UserId = userId;
            CalculatePriceDrop();

            PriceDropPercentage = previousPrice > 0
                ? Math.Round((double)((previousPrice - currentPrice) / previousPrice) * 100, 2)
                : 0;

            UpdateDate = DateTime.UtcNow;

            return Result.Ok(this);
        }


        // Activation Methods
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


        public void AddReview(CustomerReview review)
        {
            _reviews.Add(review);
            RecalculateAverageRating();
        }

        private void RecalculateAverageRating()
        {
            if (_reviews.Count == 0)
                AverageRating = 0;
            else
                AverageRating = (decimal)Math.Round(_reviews.Average(r => r.Rating), 2);
        }

        private void CalculatePriceDrop()
        {
            if (PreviousPrice > 0)
            {
                PriceDropPercentage = Math.Round(
                    (double)((PreviousPrice - CurrentPrice) / PreviousPrice) * 100, 2);
            }
            else
            {
                PriceDropPercentage = 0;
            }
        }
    }



}
