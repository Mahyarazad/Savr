using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Savr.Identity.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Savr.Presentation.Controllers
{
    [ApiController]
    [Route("auth")]
    public class GoogleAuthController : ControllerBase
    {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GoogleAuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
            , IOptions<JwtSettings> jwtSettings, IHttpContextAccessor httpContextAccessor)
        {
            
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("google-login")]
        public IActionResult LoginWithGoogle()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback", "GoogleAuth")
            };

            return Challenge(props, "Google");
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return Unauthorized("Google authentication failed.");

            var googleClaims = result.Principal.Claims;

            var email = googleClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = googleClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var googleId = googleClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(googleId))
                return BadRequest("Email or Google ID missing.");

            var firstName = name?.Split(' ').FirstOrDefault() ?? "";
            var lastName = name?.Split(' ').Skip(1).FirstOrDefault() ?? "";

            // Try to find the user by external login
            var user = await _userManager.FindByLoginAsync("Google", googleId);

            if (user == null)
            {
                // Fallback: find by email
                user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    // Check if already linked
                    var logins = await _userManager.GetLoginsAsync(user);
                    if (!logins.Any(l => l.LoginProvider == "Google"))
                    {
                        var addLoginResult = await _userManager.AddLoginAsync(
                            user,
                            new UserLoginInfo("Google", googleId, "Google")
                        );

                        if (!addLoginResult.Succeeded)
                            return BadRequest("Failed to link Google login.");
                    }
                }
                else
                {
                    // Create new user
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        Firstname = firstName,
                        Lastname = lastName,
                        EmailConfirmed = true
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                        return BadRequest("Failed to create user.");

                    // Add external login after creating user
                    var addLoginResult = await _userManager.AddLoginAsync(
                        user,
                        new UserLoginInfo("Google", googleId, "Google")
                    );

                    if (!addLoginResult.Succeeded)
                        return BadRequest("Failed to link Google login.");
                }

                // Optional: Add default role
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("User"))
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }

            // Generate JWT token
            var userRoles = await _userManager.GetRolesAsync(user);
            var token = GenerateJWTToken(user, userRoles);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Set secure cookie
            HttpContext.Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            // Redirect to frontend
            return Redirect("https://localhost:3000/login-success");

        }




        [HttpGet("login-failed")]
        public IActionResult LoginFailed([FromQuery] string? error = "Unknown error")
        {
            Log.Warning("Google login failed: {Error}", error);

            return BadRequest(new
            {
                message = "Google sign-in failed",
                error = error
            });
        }


        private JwtSecurityToken GenerateJWTToken(ApplicationUser user, IList<string> roles)
        {

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                    new Claim("Uid", user.Id),
                    new Claim("Name", user.Firstname ?? "")
                };

            if (roles.Count > 0)
            {
                claims.Add(new Claim(ClaimTypes.Role, roles[0]));
            }

            // Uses the same secret key for both encryption and decryption.
            // This key can be a password, code, or a random string of letters or numbers.
            // Symmetric encryption is faster and easier to use than asymmetric encryption,
            // but it's less secure because if the key is compromised, the data can be easily decrypted.

            //Asymmetric encryption
            //Uses two different keys, a public key for encryption and a private key for decryption.
            //Asymmetric encryption may be more suitable for situations where data is exchanged between
            //two independent parties

            // found the implementation 
            //https://stefanescueduard.github.io/2020/04/25/jwt-authentication-with-asymmetric-encryption-in-asp-dotnet-core/

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings!.Secret));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken =
                new JwtSecurityToken(
                    issuer: _jwtSettings!.Issuer,
                    audience: _jwtSettings!.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: signingCredentials
                );
            return jwtSecurityToken;
        }

    }
}
