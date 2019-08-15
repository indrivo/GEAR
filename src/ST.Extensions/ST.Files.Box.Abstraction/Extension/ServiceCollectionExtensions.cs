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
            services.AddTransient<IFileManager, TFileService>();
            IoC.RegisterService<IFileBoxManager, TFileService>();
            return services;
        }


        public static IServiceCollection AddFileBoxModuleStorage<TFileContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TFileContext : DbContext, IFileContext
        {
            services.AddScopedContextFactory<IFileContext, TFileContext>();
            services.AddDbContext<TFileContext>(options);
            return services;
        }
    }


}

