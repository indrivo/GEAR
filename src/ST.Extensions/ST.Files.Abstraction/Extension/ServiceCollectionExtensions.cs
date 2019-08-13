using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST.Core.Helpers;

namespace ST.Files.Abstraction.Extension
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddFileService<TFileService>(this IServiceCollection services)
            where TFileService : class , IFileService
        {
            services.AddTransient<IFileService,TFileService>();
            IoC.RegisterService<IFileService,TFileService>();
            return services;
        }


        public static IServiceCollection AddFileModuleStorage<TFileContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
            where TFileContext : DbContext, IFileContext
        {
            services.AddDbContext<TFileContext>(options);
            return services;
        }


    }


}

