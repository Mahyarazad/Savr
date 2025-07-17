using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Savr.Identity.Models;
using Savr.Application.Abstractions.Identity;
using Savr.Application.Features.Identity.Commands.Login;
using Savr.Application.Features.Identity.Commands.Register;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Savr.Application.DTOs;
using Savr.Application.Features.Identity.Queries;
using Microsoft.EntityFrameworkCore;

namespace Savr.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
            , IHttpContextAccessor httpContextAccessor
            , IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<LoginCommandResult>> Login(LoginCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(command.Email);
            
            if(user == null)
            {
                return Result.Fail("There is no registered username or Email.");
            }



            if (user != null && user!.LockoutEnd != null)
            {
                return Result.Fail("Your account has been locked due to multiple invalid requets.");
            }


            var singInResult = await _signInManager.CheckPasswordSignInAsync(user!, command.Password, true);
            var roles = await _userManager.GetRolesAsync(user); // move this earlier

            if (singInResult.IsLockedOut)
            {
                return Result.Fail("Account Locked, too many invalid login attempts.");
            }

            if(singInResult.Succeeded)
            {
                var token = await GenerateJWTToken(user, roles);
                await _signInManager.SignInAsync(user, false);
                //_httpContextAccessor.HttpContext.Session.SetString("UserName", user.Email.Split("@")[0]);
                //_httpContextAccessor.HttpContext.Session.SetString("USer", System.Text.Json.JsonSerializer.Serialize(user));

                return Result.Ok(new LoginCommandResult(user.Id, user.UserName, user.Email, new JwtSecurityTokenHandler().WriteToken(token)));
            }

            return Result.Fail("Invalid password.");
        }

        public async Task<Result<RegisterCommandResult>> Register(RegisterCommand command, CancellationToken cancellationToken = default)
        {

            var existingUser = await _userManager.FindByEmailAsync(command.Email);
            if (existingUser != null)
            {
                return Result.Fail(new Error($"'{existingUser.UserName}' already exists."));
            }

            var email = command.Email?.Trim().ToLower();
            var username = email?.Split('@')[0];

            var user = new ApplicationUser()
            {
                Email = email,
                UserName = username,
                Firstname = command.FirstName?.Trim(),
                Lastname = command.LastName?.Trim(),
                EmailConfirmed = true,
            };

            var identityResult = await _userManager.CreateAsync(user, command.Password);

            if(identityResult.Succeeded)
            {
                var role = string.IsNullOrWhiteSpace(command.Role) ? "User" : command.Role;

                var roleResult = await _userManager.AddToRoleAsync(user, role);

                if (!roleResult.Succeeded)
                {
                    return Result.Fail(roleResult.Errors.Select(e => e.Description).ToArray());
                }

                return Result.Ok(new RegisterCommandResult(user.Id));
                

            }

            return Result.Fail(identityResult.Errors.Select(x=> x.Description).ToArray());

        }

        public async Task<Result<IEnumerable<ApplicationUserDto>>> GetAllUsers(CancellationToken cancellationToken = default)
        {
            var users = await _userManager.Users.ToListAsync(cancellationToken);

            var userDtos = new List<ApplicationUserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userDtos.Add(new ApplicationUserDto(
                    Id: user.Id,
                    UserName: user.UserName,
                    Email: user.Email,
                    EmailConfirmed: user.EmailConfirmed,
                    PhoneNumber: user.PhoneNumber,
                    PhoneNumberConfirmed: user.PhoneNumberConfirmed,
                    TwoFactorEnabled: user.TwoFactorEnabled,
                    Firstname: user.Firstname,
                    Lastname: user.Lastname,
                    Roles: roles.ToList()
                ));
            }

            return userDtos;
        }

        private async Task<JwtSecurityToken> GenerateJWTToken(ApplicationUser user, IList<string> roles)
        {
           // var userClaims = await _userManager.GetClaimsAsync(user);
           // var role = await _userManager.GetRolesAsync(user);

           // var claims = new[]
           // {
           //     new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
           //     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
           //     new Claim(JwtRegisteredClaimNames.Email, user.Email),
           //     // I dont like the log ClaimType.GivenName in the token like this => http://schemas.microsoft.com/ws/2008/06/identity/claims/userdata
           //     new Claim("Uid", user.Id),
           //     new Claim("Name", user.Firstname),
           //     new Claim("Role", role[0]),
           //}.Union(userClaims);

            

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
