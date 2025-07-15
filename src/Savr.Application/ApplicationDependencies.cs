using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Savr.Application.Abstractions.Data;
using Savr.Application.Feature.Data;
using Savr.Application.Features.Identity.Commands.Login;
using Savr.Application.Features.Identity.Commands.Register;
using Savr.Application.Features.Products.Commands.CreateProduct;
using Savr.Application.Features.Products.Commands.DeleteProduct;
using Savr.Application.Features.Products.Commands.UpdateProduct;
using System.Reflection;

namespace Savr.Application
{
    public static class ApplicationDependencies
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            services.AddScoped<IValidator<CreateProductCommand>,CreateProductCommandValidator>();

            services.AddScoped<IValidator<UpdateProductCommand>,UpdateProductCommandValidator>();

            services.AddScoped<IValidator<LoginCommand>,LoginCommandValidator>();

            services.AddScoped<IValidator<RegisterCommand>, RegsiterCommandValidator>();

            services.AddScoped<IValidator<DeleteProductCommand>, DeleteProductCommandValidator>();

            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
