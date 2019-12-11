using GR.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using GR.Subscriptions.Abstractions.Models;


namespace GR.Subscriptions.Abstractions
{
    public interface ISubscriptionDbContext : IDbContext
    {
        /// <summary>
        /// Subscription
        /// </summary>
        DbSet<Subscription> Subscription { get; set; }

        /// <summary>
        /// Subscription
        /// </summary>
        DbSet<SubscriptionPermission> SubscriptionPermissions { get; set; }
    }
}
