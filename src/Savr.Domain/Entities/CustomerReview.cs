using Savr.Domain.Primitives;

namespace Savr.Domain.Entities
{
    public class CustomerReview : BaseEntity<long>
    {
        protected CustomerReview() { }

        public Guid UserId { get; private set; }
        public string UserName { get; private set; } = default!;
        public string Comment { get; private set; } = default!;
        public int Rating { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Helpful vote count
        public int HelpfulCount => _helpfulVoterIds.Count;

        // Foreign key to Listing
        public long ListingId { get; private set; }
        public Listing Listing { get; private set; } = default!;

        // Tracks users who marked this review as helpful
        private readonly HashSet<Guid> _helpfulVoterIds = new();
        public IReadOnlyCollection<Guid> HelpfulVoterIds => _helpfulVoterIds;

        public CustomerReview(Guid userId, string userName, string comment, int rating)
        {
            UserId = userId;
            UserName = userName;
            Comment = comment;
            Rating = rating;
            CreatedAt = DateTime.UtcNow;
        }

        public bool MarkHelpful(Guid voterUserId)
        {
            // Returns true if added (i.e., user hasn't voted before)
            return _helpfulVoterIds.Add(voterUserId);
        }

        public bool UnmarkHelpful(Guid voterUserId)
        {
            return _helpfulVoterIds.Remove(voterUserId);
        }
    }


}
