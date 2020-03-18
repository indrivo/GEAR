using GR.Core.Abstractions;
using GR.Processes.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Processes.Abstractions
{
    public interface IProcessesDbContext : IDbContext
    {
        /// <summary>
        /// Schemas
        /// </summary>
        DbSet<STProcessSchema> ProcessSchemas { get; set; }
        /// <summary>
        /// Extracted processes from schema(diagram) 
        /// </summary>
        DbSet<STProcess> Processes { get; set; }
        /// <summary>
        /// Instances of a process
        /// </summary>
        DbSet<STProcessInstance> ProcessInstances { get; set; }
        /// <summary>
        /// History of process transitions
        /// </summary>
        DbSet<STProcessInstanceHistory> ProcessInstanceHistories { get; set; }
        /// <summary>
        /// Process transitions
        /// </summary>
        DbSet<STProcessTransition> ProcessTransitions { get; set; }

        /// <summary>
        /// Outgoing transitions
        /// </summary>
        DbSet<STOutgoingTransition> OutGoingTransitions { get; set; }

        /// <summary>
        /// Incoming transitions
        /// </summary>
        DbSet<STIncomingTransition> IncomingTransitions { get; set; }

        /// <summary>
        /// Transition actors
        /// </summary>
        DbSet<STTransitionActor> TransitionActors { get; set; }
        /// <summary>
        /// Process tasks
        /// </summary>
        DbSet<STProcessTask> ProcessTasks { get; set; }
        /// <summary>
        /// User history of process tasks
        /// </summary>
        DbSet<UserProcessTasks> UserProcessTasks { get; set; }
    }
}
