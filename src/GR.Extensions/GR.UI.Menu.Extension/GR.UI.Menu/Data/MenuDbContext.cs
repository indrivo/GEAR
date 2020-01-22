using GR.UI.Menu.Abstractions;
using System;
using System.Threading.Tasks;
using GR.Audit.Contexts;
using GR.UI.Menu.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.UI.Menu.Data
{
    public class MenuDbContext : TrackerDbContext, IMenuDbContext
    {
        public const string Schema = "Menu";

        public MenuDbContext(DbContextOptions<MenuDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Menus
        /// </summary>
        public virtual DbSet<MenuGroup> Menus { get; set; }

        /// <summary>
        /// Menu items
        /// </summary>
        public virtual DbSet<MenuItem> MenuItems { get; set; }

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
        /// Seed
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            MenuDbContextSeeder.SeedAsync(services).Wait();
            return Task.CompletedTask;
        }
    }
}