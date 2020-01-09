using GR.Core.Abstractions;
using GR.UI.Menu.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.UI.Menu.Abstractions
{
    public interface IMenuDbContext : IDbContext
    {
        /// <summary>
        /// Menus
        /// </summary>
        DbSet<MenuGroup> Menus { get; set; }

        /// <summary>
        /// Menu items
        /// </summary>
        DbSet<MenuItem> MenuItems { get; set; }
    }
}