using System.Threading.Tasks;
using GR.Audit.Contexts;
using GR.Core.Abstractions;
using GR.WorkFlows.Abstractions;
using GR.WorkFlows.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.WorkFlows.Data
{
    public class WorkFlowsDbContext : TrackerDbContext, IWorkFlowContext
    {
        /// <summary>
        /// Context schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "WorkFlows";

        protected WorkFlowsDbContext(DbContextOptions<WorkFlowsDbContext> options) : base(options)
        {
        }

        #region Entities

        public virtual DbSet<WorkFlow> WorkFlows { get; set; }
        public virtual DbSet<Transition> Transitions { get; set; }
        public virtual DbSet<State> States { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DbSet<T> SetEntity<T>() where T : class, IBaseModel => Set<T>();

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public Task InvokeSeedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
