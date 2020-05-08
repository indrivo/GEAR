using System;
using System.Threading.Tasks;
using GR.AccountActivity.Abstractions;
using GR.AccountActivity.Abstractions.Models;
using GR.Audit.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GR.AccountActivity.Impl.Data
{
    public class UserActivityDbContext : TrackerDbContext, IActivityContext
    {
        public const string Schema = "AccountActivity";

        public UserActivityDbContext(DbContextOptions<UserActivityDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Devices
        /// </summary>
        public virtual DbSet<UserDevice> Devices { get; set; }

        /// <summary>
        /// Activities
        /// </summary>
        public virtual DbSet<UserActivity> Activities { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}