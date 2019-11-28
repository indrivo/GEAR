using GR.Core.Abstractions;
using GR.WorkFlows.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.WorkFlows.Abstractions
{
    public interface IWorkFlowContext : IDbContext
    {
        /// <summary>
        /// WorkFlows
        /// </summary>
        DbSet<WorkFlow> WorkFlows { get; set; }

        /// <summary>
        /// Transitions
        /// </summary>
        DbSet<Transition> Transitions { get; set; }

        /// <summary>
        /// States
        /// </summary>
        DbSet<State> States { get; set; }

        /// <summary>
        /// Transition roles
        /// </summary>
        DbSet<TransitionRole> TransitionRoles { get; set; }

        /// <summary>
        /// Actions
        /// </summary>
        DbSet<WorkflowAction> WorkflowActions { get; set; }

        /// <summary>
        /// Mapped actions to transitions
        /// </summary>
        DbSet<TransitionAction> TransitionActions { get; set; }
    }
}