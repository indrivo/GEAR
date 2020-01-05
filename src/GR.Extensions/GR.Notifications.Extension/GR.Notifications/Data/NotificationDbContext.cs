using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Contexts;
using GR.Core;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Data;
using GR.Notifications.Abstractions.Seeders;
using GR.Notifications.Extensions;

namespace GR.Notifications.Data
{
    public class NotificationDbContext : TrackerDbContext, INotificationDbContext
    {
        /// <summary>
        /// Context schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Notifications";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        #region Entities
        /// <summary>
        /// Notifications events
        /// </summary>
        public virtual DbSet<NotificationEvent> NotificationEvents { get; set; }

        /// <summary>
        /// Subscriptions
        /// </summary>
        public virtual DbSet<NotificationSubscription> NotificationSubscriptions { get; set; }

        /// <summary>
        /// Notification templates
        /// </summary>
        public virtual DbSet<NotificationTemplate> NotificationTemplates { get; set; }
        #endregion

        /// <summary>
        /// Configurations
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.RegisterNotificationDbContextBuilder();
        }


        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
                {
                    await NotificationManager.SeedNotificationTypesAsync();
                });
            return Task.CompletedTask;
        }
    }
}