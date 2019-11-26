using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.WorkFlows.Abstractions;
using GR.WorkFlows.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.WorkFlows
{
    public class WorkFlowService : IWorkFlowService
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IWorkFlowContext _workFlowContext;

        #endregion

        public WorkFlowService(IWorkFlowContext workFlowContext)
        {
            _workFlowContext = workFlowContext;
        }

        /// <summary>
        /// Get flow by id
        /// </summary>
        /// <param name="workFlowId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<WorkFlow>> GetWorkFlowByIdAsync(Guid? workFlowId)
        {
            if (workFlowId == null) return new InvalidParametersResultModel<WorkFlow>();
            var workFlow = await _workFlowContext.WorkFlows
                .Include(x => x.States)
                .Include(x => x.Transitions)
                .ThenInclude(x => x.Actions)
                .Include(x => x.Transitions)
                .ThenInclude(x => x.FromState)
                .Include(x => x.Transitions)
                .ThenInclude(x => x.ToState)
                .FirstOrDefaultAsync(x => x.Id.Equals(workFlowId));
            if (workFlow == null) return new NotFoundResultModel<WorkFlow>();
            return new SuccessResultModel<WorkFlow>(workFlow);
        }
    }
}
