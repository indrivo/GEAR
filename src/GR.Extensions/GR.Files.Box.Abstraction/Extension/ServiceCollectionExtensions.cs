using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GR.Audit.Abstractions.Extensions;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Files.Box.Abstraction.Models.ViewModels;

namespace GR.Files.Box.Abstraction.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileBoxModule<TFileService>(this IServiceCollection services)
            where TFileService : class, IFileBoxManager
        {
            services.AddTransient<IFileBoxManager, TFileService>();
            IoC.RegisterTransientService<IFileBoxManager, TFileService>();
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