using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Files.Abstraction;

namespace ST.Files.Box.Abstraction.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFileBoxModule<TFileService>(this IServiceCollection services)
            where TFileService : class , IFileBoxManager
        {
            services.AddTransient<IFileBoxManager, TFileService>();
            IoC.RegisterService<IFileBoxManager, TFileService>();
            return services;
        }


        public static IServiceCollection AddFileBoxModuleStorage<TFileBoxContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TFileBoxContext : DbContext, IFileBoxContext
        {
            services.AddScopedContextFactory<IFileBoxContext, TFileBoxContext>();
            services.AddDbContext<TFileBoxContext>(options,ServiceLifetime.Transient);
            return services;
        }
    }


}

