namespace Savr.Application
{
    public class RefereshToken
    {
        public string Token { get; set; } = Guid.NewGuid().ToString();
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsRevoked { get; set; } = false;
    }
}
