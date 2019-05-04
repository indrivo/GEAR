using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Helpers;
using ST.Entities.Security.Abstractions;
using ST.Entities.Security.Data;
using ST.Entities.Security.Services;
using ST.Identity.Abstractions;

namespace ST.Entities.Security.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register services
        /// </summary>
        /// <typeparam name="TAclContext"></typeparam>
        /// <typeparam name="TIdentityContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEntityAcl<TAclContext, TIdentityContext>(this IServiceCollection services)
            where TAclContext : EntitySecurityDbContext
            where TIdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
        {
            services.AddTransient<IEntityAclService, EntityAclService<TAclContext, TIdentityContext>>();
            IoC.RegisterService<IEntityAclService, EntityAclService<TAclContext, TIdentityContext>>();
            return services;
        }
    }
}
