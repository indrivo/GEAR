using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ST.Audit.Abstractions.Extensions;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Files.Box.Abstraction.Models.ViewModels;

namespace ST.Files.Box.Abstraction.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileBoxModule<TFileService>(this IServiceCollection services)
            where TFileService : class, IFileBoxManager
        {
            services.AddTransient<IFileBoxManager, TFileService>();
            IoC.RegisterService<IFileBoxManager, TFileService>();
            return services;
        }

        public static IServiceCollection AddFileBoxModuleStorage<TFileBoxContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options, IConfiguration configuration)
            where TFileBoxContext : DbContext, IFileBoxContext
        {
            services.AddScopedContextFactory<IFileBoxContext, TFileBoxContext>();
            services.AddDbContext<TFileBoxContext>(options, ServiceLifetime.Transient);
            services.RegisterAuditFor<IFileBoxContext>("File box module");
            services.ConfigureWritable<List<FileBoxSettingsViewModel>>(configuration.GetSection("FileBoxSettings"));
            return services;
        }
    }
}