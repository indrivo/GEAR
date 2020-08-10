using System;
using System.Threading.Tasks;
using GR.Audit.Contexts;
using GR.Core;
using GR.Notifications.Abstractions;
using GR.Notifications.Subscriptions.Abstractions;
using GR.Notifications.Subscriptions.Abstractions.Models;
using GR.Notifications.Subscriptions.EFCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Notifications.Subscriptions.EFCore.Data
{
    public class NotificationsSubscriptionDbContext : TrackerDbContext, INotificationSubscriptionsDbContext
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
        public NotificationsSubscriptionDbContext(DbContextOptions<NotificationsSubscriptionDbContext> options) : base(options)
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
            GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async (s, p) =>
            {
                var service = s.GetService<INotificationSeederService>();
                await service.SeedNotificationTypesAsync();
            });
            return Task.CompletedTask;
        }
    }
}