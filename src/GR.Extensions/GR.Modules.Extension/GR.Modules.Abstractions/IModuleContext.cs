using GR.Core.Abstractions;
using GR.Modules.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Modules.Abstractions
{
    public interface IModuleContext : IDbContext
    {
        /// <summary>
        /// Modules
        /// </summary>
        DbSet<Module> Modules { get; set; }
    }
}