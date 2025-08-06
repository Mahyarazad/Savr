using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Savr.Identity.Data;
using Savr.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Savr.Application.Abstractions.Identity;
using Savr.Identity.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.Extensions.FileProviders;

namespace Savr.Identity
{
    public static class IdentityExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("IdentityServer"));
            });

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            IdentityBuilder builder = services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<UserDbContext>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddDefaultTokenProviders();


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                // for external login
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
           .AddCookie(IdentityConstants.ApplicationScheme)
           .AddCookie()

           .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
           {
               options.SaveToken = true;
               options.RequireHttpsMetadata = false;
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   RoleClaimType = ClaimTypes.Role,
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidAudience = configuration["JwtSettings:Audience"],
                   ValidIssuer = configuration["JwtSettings:Issuer"],
                   IssuerSigningKey = new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]))
               };
           })
           .AddGoogle("Google", options =>
           {
               options.ClientId = configuration["Authentication:Google:ClientId"];
               options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
               options.CallbackPath = "/signin-google";
               options.SaveTokens = true;

               options.Events.OnRemoteFailure = context =>
               {
                   var error = Uri.EscapeDataString(context.Failure?.Message ?? "Unknown");
                   context.Response.Redirect($"/auth/login-failed?error={error}");
                   context.HandleResponse();
                   return Task.CompletedTask;
               };
           })
           .AddFacebook("Facebook", options =>
           {
               options.AppId = configuration["Authentication:Facebook:AppId"];
               options.AppSecret = configuration["Authentication:Facebook:AppSecret"];
               options.CallbackPath = "/signin-facebook";
               options.SaveTokens = true;

               options.Events.OnRemoteFailure = context =>
               {
                   var error = Uri.EscapeDataString(context.Failure?.Message ?? "Unknown");
                   context.Response.Redirect($"/auth/login-failed?error={error}");
                   context.HandleResponse();
                   return Task.CompletedTask;
               };
           });
           //.AddApple("Apple", options =>
           //{
           //    options.ClientId = configuration["Authentication:Apple:ClientId"];
           //    options.KeyId = configuration["Authentication:Apple:KeyId"];
           //    options.TeamId = configuration["Authentication:Apple:TeamId"];
           //    options.CallbackPath = "/signin-apple";
           //    options.SaveTokens = true;

           //    options.Scope.Add("name");
           //    options.Scope.Add("email");

           //    // ✅ Load private key from file
           //    var keyFilePath = configuration["Authentication:Apple:PrivateKeyPath"]; // e.g., "Keys/AuthKey_ABC123.p8"
           //    var fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
           //    options.UsePrivateKey(keyId => fileProvider.GetFileInfo(keyFilePath));

           //    options.Events.OnRemoteFailure = context =>
           //    {
           //        var error = Uri.EscapeDataString(context.Failure?.Message ?? "Unknown");
           //        context.Response.Redirect($"/auth/apple-login-failed?error={error}");
           //        context.HandleResponse();
           //        return Task.CompletedTask;
           //    };
           //});

            services.AddAuthorization();

            services.AddScoped<IAuthService, AuthService>();




            return services;
        }

        public static IApplicationBuilder UseIdentity(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
