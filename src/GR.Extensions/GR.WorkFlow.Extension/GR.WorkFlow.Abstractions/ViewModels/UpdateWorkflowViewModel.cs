using System;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class UpdateWorkflowViewModel : AddNewWorkflowViewModel
    {
        public virtual Guid Id { get; set; }
    }
}
