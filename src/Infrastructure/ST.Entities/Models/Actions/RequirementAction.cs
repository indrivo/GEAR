using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ST.Entities.Models.Actions
{
    public class RequirementAction : ExtendedModel
    {
        [Display(Name = "Requirement")]
        public Guid RequirementId { get; set; }
        public string Name { get; set; }
        public int Treegrid { get; set; }
        public string Description { get; set; }
        public DateTime DeadLine { get; set; }
    }
}
