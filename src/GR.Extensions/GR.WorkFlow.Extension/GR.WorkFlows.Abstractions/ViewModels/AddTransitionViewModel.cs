using System;
using System.ComponentModel.DataAnnotations;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class AddTransitionViewModel
    {
        [Required]
        public virtual string Name { get; set; }

        [Required]
        public virtual Guid FromStateId { get; set; }

        [Required]
        public virtual Guid ToStateId { get; set; }
    }
}
