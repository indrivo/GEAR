using System;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.IdentityDocuments.Abstractions.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.IdentityDocuments.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register user preferences module
        /// </summary>
        /// <typeparam name="TDocumentsService"></typeparam>
        /// <typeparam name="TVerificationService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityDocumentsModule<TDocumentsService, TVerificationService>(this IServiceCollection services)
            where TDocumentsService : class, IIdentityDocumentService
            where TVerificationService : class, IIdentityVerificationService
        {
            services.AddGearScoped<IIdentityDocumentService, TDocumentsService>();
            services.AddGearScoped<IIdentityVerificationService, TVerificationService>();

            services.AddScoped<IDocumentType, GovernmentIdFrontDocumentType>();
            services.AddScoped<IDocumentType, GovernmentIdBackDocumentType>();
            services.AddScoped<IDocumentType, ProofOfResidenceDocumentType>();
            services.AddScoped<IDocumentType, SelfieDocumentType>();
            services.AddScoped<IDocumentType, PassportDocumentType>();
            return services;
        }

        /// <summary>
        /// Add user preferences storage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityDocumentsModuleStorage<TContext>(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TContext : DbContext, IIdentityDocumentsContext
        {
            services.AddDbContext<TContext>(options);
            services.AddGearScoped<IIdentityDocumentsContext, TContext>();

            SystemEvents.Database.OnAllMigrate += (sender, args) =>
            {
                GearApplication.GetHost().MigrateDbContext<TContext>();
            };

            return services;
        }
    }
}
