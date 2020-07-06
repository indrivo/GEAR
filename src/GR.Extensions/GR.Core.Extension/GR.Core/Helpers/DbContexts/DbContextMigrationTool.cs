using System;
using System.Linq;
using System.Reflection;
using GR.Core.Abstractions;
using GR.Core.Attributes;
using GR.Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GR.Core.Helpers.DbContexts
{
    public static class DbContextMigrationTool
    {
        /// <summary>
        /// Apply pending migrations
        /// </summary>
        public static void ApplyPendingMigrations()
        {
            if (!GearApplication.Configured) return;
            ConsoleWriter.WriteTextAsTitle($"Database migration tool", ConsoleColor.DarkMagenta);
            var handlers = IoC.Container.Kernel.GetAssignableHandlers(typeof(DbContext));
            var index = 0;
            foreach (var handler in handlers)
            {
                var type = handler.ComponentModel.Implementation;
                ConsoleWriter.ColoredWriteLine($"{++index} Context - {type.Name}", ConsoleColor.Cyan);
                if (type.GetCustomAttribute<IgnoreContextAutoMigrationsAttribute>() != null)
                {
                    ConsoleWriter.ColoredWriteLine($"Context is disabled for auto migrations", ConsoleColor.Red);
                    continue;
                }
                var dbContext = (DbContext)IoC.Resolve(type);
                var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
                if (pendingMigrations.Any())
                {
                    var appliedMigrations = dbContext.Database.GetContextAppliedMigrations(dbContext);
                    var invokeSeed = !appliedMigrations.Any();
                    ConsoleWriter.ColoredWriteLine($"Pending migrations: {pendingMigrations.Count}", ConsoleColor.DarkYellow);
                    dbContext.Database.Migrate();
                    if (!invokeSeed) continue;
                    if (dbContext is IDbContext gearContext)
                    {
                        ConsoleWriter.ColoredWriteLine($"Context is migrated for first time, seed will be invoked", ConsoleColor.DarkYellow);
                        gearContext.InvokeSeedAsync(IoC.Resolve<IServiceProvider>()).Wait();
                    }
                }
                else
                {
                    ConsoleWriter.ColoredWriteLine($"No pending migrations", ConsoleColor.Green);
                }
            }
        }
    }
}
