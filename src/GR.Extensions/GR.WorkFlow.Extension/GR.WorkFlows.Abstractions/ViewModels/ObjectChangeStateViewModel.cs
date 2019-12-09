using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core.Helpers.ModelBinders;
using Microsoft.AspNetCore.Mvc;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class ObjectChangeStateViewModel
    {
        /// <summary>
        /// Entry id
        /// </summary>
        [Required]
        public virtual string EntryId { get; set; }

        /// <summary>
        /// Workflow id
        /// </summary>
        [Required]
        public virtual Guid? WorkFlowId { get; set; }

        /// <summary>
        /// New state id
        /// </summary>
        [Required]
        public virtual Guid? NewStateId { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public virtual string Message { get; set; }

        /// <summary>
        /// Object data config
        /// </summary>
        [ModelBinder(typeof(GearDictionaryBinder<string>))]
        public virtual Dictionary<string, string> EntryObjectConfiguration { get; set; } = new Dictionary<string, string>();
    }
}
