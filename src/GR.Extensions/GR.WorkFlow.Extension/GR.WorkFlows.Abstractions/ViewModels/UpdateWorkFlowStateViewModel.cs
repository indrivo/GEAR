using System;
using Newtonsoft.Json;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class UpdateWorkFlowStateViewModel : AddNewStateViewModel
    {
        /// <summary>
        /// State id
        /// </summary>
        public virtual Guid StateId { get; set; }

        /// <summary>
        /// Ignore json propriety
        /// </summary>
        [JsonIgnore]
        public override Guid WorkFlowId { get; set; }
    }
}
