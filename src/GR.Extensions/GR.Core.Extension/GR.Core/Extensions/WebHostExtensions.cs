using System;
using System.Data.SqlClient;
using GR.Core.Abstractions;
using GR.Core.Events;
using GR.Core.Events.EventArgs.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace GR.Core.Extensions
{
    public static class WebHostExtensions
    {
        public static bool IsInKubernetes(this IWebHost webHost)
        {
            var cfg = webHost.Services.GetService<IConfiguration>();
            var orchestratorType = cfg.GetValue<string>("OrchestratorType");
            return orchestratorType?.ToUpper() == "K8S";
        }

        /// <summary>
        /// Migrate db context
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="webHost"></param>
        /// <returns></returns>
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost) where TContext : DbContext
        {
            return webHost.MigrateDbContext<TContext>((context, services) => { });
        }

        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            var underK8S = webHost.IsInKubernetes();

            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                    if (underK8S)
                    {
                        InvokeSeeder(seeder, context, services);
                    }
                    else
                    {
                        var retry = Policy.Handle<SqlException>()
                             .WaitAndRetry(new[]
                             {
                                TimeSpan.FromSeconds(3),
                                TimeSpan.FromSeconds(5),
                                TimeSpan.FromSeconds(8),
                             });

                        //if the sql server container is not created on run docker compose this
                        //migration can't fail for network related exception. The retry options for DbContext only 
                        //apply to transient exceptions
                        // Note that this is NOT applied when running some orchestrator (let the orchestrator to recreate the failing service)
                        retry.Execute(() => InvokeSeeder(seeder, context, services));

                        SystemEvents.Database.MigrateComplete(new DatabaseMigrateEventArgs
                        {
                            DbContext = context,
                            ContextName = context.GetType().Name
                        });
                    }

                    logger.LogInformation("Migrated database associated with context {DbContextName}", typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database used on context {DbContextName}", typeof(TContext).Name);
                    if (underK8S) throw;          // Rethrow under k8s because we rely on k8s to re-run the pod
                }
            }

            return webHost;
        }

        /// <summary>
        /// Invoke migrations and seed
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="seeder"></param>
        /// <param name="context"></param>
        /// <param name="services"></param>
        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context, services);
            try
            {
                if (context is IDbContext ctx) ctx.InvokeSeedAsync(services).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
