namespace Savr.Identity.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public string UserId { get; set; } = default!;
        public string Token { get; set; } = default!;
        public DateTime ExpiryDate { get; set; }

        public ApplicationUser User { get; set; } = default!;
    }

}
