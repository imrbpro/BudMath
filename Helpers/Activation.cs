using Microsoft.Extensions.DependencyInjection;
using Repository.Implementation;
using Repository.Interface;
using Service.Implementation;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudMath.Helpers
{
    public static class Activation
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            return services;
        }
    }
}
