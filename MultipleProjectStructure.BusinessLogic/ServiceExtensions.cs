using Microsoft.Extensions.DependencyInjection;
using MultipleProjectStructure.BusinessLogic.Services;
using MultipleProjectStructure.BusinessLogic.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleProjectStructure.BusinessLogic
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<IImageService, ImageService>();
            return services;
        }
    }
}
