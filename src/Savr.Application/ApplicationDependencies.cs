using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Savr.Application.Features.Identity.Commands.Login;
using Savr.Application.Features.Identity.Commands.Register;
using System.Reflection;

namespace Savr.Application
{
    public static class ApplicationDependencies
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            //services.AddScoped<IValidator<CreateProductCommand>,CreateProductCommandValidator>();

            //services.AddScoped<IValidator<UpdateProductCommand>,UpdateProductCommandValidator>();

            services.AddScoped<IValidator<LoginCommand>,LoginCommandValidator>();

            services.AddScoped<IValidator<RegisterCommand>, RegsiterCommandValidator>();

            //services.AddScoped<IValidator<DeleteProductCommand>, DeleteProductCommandValidator>();

            //services.AddScoped<IValidator<DeleteProductCommand>, DeleteProductCommandValidator>();

            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
