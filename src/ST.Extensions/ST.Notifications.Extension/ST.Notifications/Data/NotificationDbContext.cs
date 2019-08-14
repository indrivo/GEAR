using Microsoft.EntityFrameworkCore;
using ST.Audit.Contexts;
using ST.Core.Abstractions;
using ST.Notifications.Abstractions;
using ST.Notifications.Abstractions.Models.Data;
using ST.Notifications.Extensions;

namespace ST.Notifications.Data
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
        /// Set operational entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DbSet<T> SetEntity<T>() where T : class, IBaseModel
        {
            return Set<T>();
        }
    }
}
