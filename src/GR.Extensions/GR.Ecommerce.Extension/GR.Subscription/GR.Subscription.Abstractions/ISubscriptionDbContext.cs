using GR.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using GR.Subscription.Abstractions.Models;


namespace GR.Subscription.Abstractions
{
    public interface ISubscriptionDbContext : IDbContext
    {
        /// <summary>
        /// Subscription
        /// </summary>
        DbSet<Models.Subscription> Subscription { get; set; }
    }
}
