using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core.Helpers.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class AddNewStateViewModel
    {
        /// <summary>
        /// Transition name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Reference to workflow
        /// </summary>
        public virtual Guid WorkFlowId { get; set; }

        /// <summary>
        /// Settings
        /// </summary>
        [ModelBinder(typeof(GearDictionaryBinder<string>))]
        public virtual Dictionary<string, string> AdditionalSettings { get; set; }
    }
}
