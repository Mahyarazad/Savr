﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Savr.Application.Abstractions.Data;
using Savr.Application.Feature.Data;
using Savr.Persistence.Data;
using Savr.Persistence.Profiles;
using Savr.Persistence.Repositories;
using System.Reflection;
using Npgsql;
using Savr.Application.Abstractions.Persistence.Data;
using Savr.Application.Abstractions.Persistence.Repositories;

namespace Savr.Persistence 
{
    public static class PersistenceDependencies
    {
        public static IServiceCollection AddPersistenceDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Postgres"));
            });

            services.AddDbContext<LogDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("Postgres"));
            });

            services.AddAutoMapper(Assembly.GetAssembly(typeof(IProfile)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IListingRepository, ListingRepository>();

            services.AddScoped<IGroupRepository, GroupRepository>();

            

            services.AddScoped<ICustomerReviewRepository, CustomerReviewRepository>();

            services.AddSingleton<IDapperService, DapperService>();

            return services;
        }
    }
}
