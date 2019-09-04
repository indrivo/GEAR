using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST.Audit.Abstractions.Extensions;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Files.Abstraction.Models.ViewModels;

namespace ST.Files.Abstraction.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileModule<TFileService>(this IServiceCollection services)
            where TFileService : class , IFileManager
        {
            services.AddTransient<IFileManager, TFileService>();
            IoC.RegisterService<IFileManager, TFileService>();
            return services;
        }

        public static IServiceCollection AddFileModuleStorage<TFileContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options, IConfiguration configuration)
            where TFileContext : DbContext, IFileContext
        {
            services.AddScopedContextFactory<IFileContext, TFileContext>();
            services.AddDbContext<TFileContext>(options, ServiceLifetime.Transient);
            services.RegisterAuditFor<TFileContext>("Physic File module");
            services.ConfigureWritable<FileSettingsViewModel>(configuration.GetSection("FileSettings"));
            return services;
        }
    }
}