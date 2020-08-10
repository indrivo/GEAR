using System;
using System.Threading.Tasks;
using GR.Audit.Abstractions.Models;
using GR.Audit.Contexts;
using GR.Core;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Notifications.EFCore.Data
{
    public class NotificationDbContext : TrackerDbContext, INotificationsContext
    {
        /// <summary>
        /// Context schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Notifications";

        /// <summary>
        /// Check if is migration mode
        /// </summary>
        public static bool IsMigrationMode { get; set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        #region Entities
        /// <summary>
        /// Notifications
        /// </summary>
        public virtual DbSet<Notification> Notifications { get; set; }

        /// <summary>
        /// Notification types
        /// </summary>
        public virtual DbSet<NotificationTypes> NotificationTypes { get; set; }

        #endregion

        /// <summary>
        /// Configurations
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);

            if (IsMigrationMode)
            {
                builder.Ignore<TrackAudit>();
                builder.Ignore<TrackAuditDetails>();
            }
        }


        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async (serviceProvider, cancellationToken) =>
            {
                var service = serviceProvider.GetService<INotificationSeederService>();
                await service.SeedNotificationTypesAsync();
            });
            return Task.CompletedTask;
        }
    }
}