using GR.AccountActivity.Abstractions.Models;
using GR.Core.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GR.AccountActivity.Abstractions
{
    public interface IActivityContext : IDbContext
    {
        /// <summary>
        /// Devices
        /// </summary>
        DbSet<UserDevice> Devices { get; set; }

        /// <summary>
        /// Activities
        /// </summary>
        DbSet<UserActivity> Activities { get; set; }
    }
}