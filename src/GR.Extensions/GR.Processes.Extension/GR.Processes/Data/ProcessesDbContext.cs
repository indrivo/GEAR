using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using GR.Audit.Contexts;
using GR.Core.Helpers.DbContexts;
using GR.Procesess.Models;
using GR.Processes.Abstractions;

namespace GR.Procesess.Data
{
    public class ProcessesDbContext : TrackerDbContext, IProcessesDbContext
    {
        private const string ProcessSchema = "Processes";

        public ProcessesDbContext(DbContextOptions<ProcessesDbContext> options)
            : base(options) { }

        /// <summary>
        /// Schemas
        /// </summary>
        public DbSet<STProcessSchema> ProcessSchemas { get; set; }
        /// <summary>
        /// Extracted processes from schema(diagram) 
        /// </summary>
        public DbSet<STProcess> Processes { get; set; }
        /// <summary>
        /// Instances of a process
        /// </summary>
        public DbSet<STProcessInstance> ProcessInstances { get; set; }
        /// <summary>
        /// History of process transitions
        /// </summary>
        public DbSet<STProcessInstanceHistory> ProcessInstanceHistories { get; set; }
        /// <summary>
        /// Process transitions
        /// </summary>
        public DbSet<STProcessTransition> ProcessTransitions { get; set; }

        /// <summary>
        /// Outgoing transitions
        /// </summary>
        public DbSet<STOutgoingTransition> OutGoingTransitions { get; set; }

        /// <summary>
        /// Incoming transitions
        /// </summary>
        public DbSet<STIncomingTransition> IncomingTransitions { get; set; }

        /// <summary>
        /// Transition actors
        /// </summary>
        public DbSet<STTransitionActor> TransitionActors { get; set; }
        /// <summary>
        /// Process tasks
        /// </summary>
        public DbSet<STProcessTask> ProcessTasks { get; set; }
        /// <summary>
        /// User history of process tasks
        /// </summary>
        public DbSet<UserProcessTasks> UserProcessTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(ProcessSchema);

            builder.Entity<STIncomingTransition>()
                .HasKey(x => new { x.ProcessTransitionId, x.IncomingTransitionId });

            builder.Entity<STOutgoingTransition>()
                .HasKey(x => new { x.ProcessTransitionId, x.OutgoingTransitionId });

            builder.Entity<STProcessInstanceHistory>()
                .HasKey(x => new { x.ProcessTransitionId, x.ProcessInstanceId });

            builder.Entity<STProcessInstanceHistory>()
                .HasOne(x => x.ProcessTransition)
                .WithMany()
                .HasForeignKey(x => x.ProcessTransitionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<STProcessInstanceHistory>()
                .HasOne(x => x.ProcessInstance)
                .WithMany()
                .HasForeignKey(x => x.ProcessInstanceId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }

    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ProcessesDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ProcessesDbContext CreateDbContext(string[] args)
        {
            return DbContextFactory<ProcessesDbContext, ProcessesDbContext>.CreateFactoryDbContext();
        }
    }
}