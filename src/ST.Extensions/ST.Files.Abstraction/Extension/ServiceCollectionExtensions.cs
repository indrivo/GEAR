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

        public static IServiceCollection AddFileModuleStorage<TFileContext>(this IServiceCollection services) where TFileContext : DbContext, IFileContext
        {
            IFileContext ContextFactory(IServiceProvider x)
            {
                var context = x.GetService<TFileContext>();
                IoC.RegisterScopedService<IFileContext, TFileContext>(context);
                return context;
            }

            services.AddScoped(ContextFactory);
            return services;
        }


    }
}
