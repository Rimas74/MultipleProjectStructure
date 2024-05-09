using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using MultipleProjectStructure.Database;
using MultipleProjectStructure.Database.Repositories;
using MultipleProjectStructure.Database.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleProjectStructure.BusinessLogic
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddDbContext<ImageApiDbContext>(options =>
    options.UseSqlServer(connectionString));
            return services;
        }
    }
}
