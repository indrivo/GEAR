using System.Linq;
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
    }
}