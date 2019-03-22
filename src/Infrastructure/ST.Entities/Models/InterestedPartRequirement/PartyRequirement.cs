using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ST.Audit.Models;

namespace ST.Entities.Models.Requirement
{
    public class PartyRequirement : ExtendedModel
    {
        [Display(Name = "Interested Party")]
        public Guid InterestedPartId { get; set; }

        [Display(Name = "Requirement")]
        public Guid RequirementId { get; set; }   
        
        public string Comments { get; set; }      


    }
}
