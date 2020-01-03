using System;
using System.Threading.Tasks;
using GR.Audit.Contexts;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using GR.WorkFlows.Abstractions;
using GR.WorkFlows.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.WorkFlows.Data
{
    [Author(Authors.LUPEI_NICOLAE, 1.1, "Add entities for WF")]
    public class WorkFlowsDbContext : TrackerDbContext, IWorkFlowContext
    {
        /// <summary>
        /// Context schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "WorkFlows";

        public WorkFlowsDbContext(DbContextOptions<WorkFlowsDbContext> options) : base(options)
        {
        }

        #region Entities

        public virtual DbSet<WorkFlow> WorkFlows { get; set; }
        public virtual DbSet<Transition> Transitions { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<TransitionRole> TransitionRoles { get; set; }
        public virtual DbSet<WorkflowAction> WorkflowActions { get; set; }
        public virtual DbSet<TransitionAction> TransitionActions { get; set; }
        public virtual DbSet<WorkFlowEntityContract> Contracts { get; set; }
        public virtual DbSet<EntryState> EntryStates { get; set; }
        public virtual DbSet<EntryStateHistory> EntryStateHistories { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            builder.Entity<TransitionRole>().HasKey(x => new { x.RoleId, x.TransitionId });
            builder.Entity<TransitionAction>().HasKey(x => new { x.TransitionId, x.ActionId });
            builder.Entity<EntryState>().HasIndex(x => x.EntryId);
            builder.Entity<WorkFlowEntityContract>().HasIndex(x => x.EntityName);
            builder.Entity<EntryState>()
                .HasMany(x => x.EntryStateHistories)
                .WithOne(x => x.EntryState)
                .OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}
