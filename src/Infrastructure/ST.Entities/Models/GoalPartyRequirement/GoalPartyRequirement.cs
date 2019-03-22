using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ST.Audit.Models;

namespace ST.Entities.Models.Requirement
{
    public class GoalPartyRequirement : ExtendedModel
    {

        [Display(Name = "Goal")]
        public Guid GoalId { get; set; }

        [Display(Name = "PartyRequirement")]
        public Guid PartyRequirementId { get; set; }





    }
}
