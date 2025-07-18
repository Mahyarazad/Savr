
using Microsoft.AspNetCore.Identity;
using Savr.Domain.Entities;

namespace Savr.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; } = default!;
        public string Lastname { get; set; } = default!;

        private readonly List<RefreshToken> _refreshTokens = new();
        public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    }

}
