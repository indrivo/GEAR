using System;
using System.Threading.Tasks;
using GR.Audit.Contexts;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using GR.Modules.Abstractions;
using GR.Modules.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Modules.Infrastructure.Data
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class ModuleDbContext : TrackerDbContext, IModuleContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Modules";

        public ModuleDbContext(DbContextOptions<ModuleDbContext> options) : base(options)
        {
        }

        #region Entities

        public DbSet<Module> Modules { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
        }

        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}