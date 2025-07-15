using Microsoft.Extensions.DependencyInjection;

namespace Savr.Presentation.Configuration
{
    public static class PresentationExtension
    {
        public static IServiceCollection AddHttpConetxt(this IServiceCollection services)
        {

            services.AddHttpContextAccessor();
            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(30);
            //    options.Cookie.IsEssential = true;
            //    options.Cookie.HttpOnly = true;
            //});
            return services;
        }
    }
}
