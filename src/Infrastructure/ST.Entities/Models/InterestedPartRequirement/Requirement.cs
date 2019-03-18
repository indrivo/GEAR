using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ST.Entities.Models.Requirement
{
    public class Requirement : ExtendedModel
    {
        //[Display(Name = "Interested Party")]
        //public Guid InterestedPartId { get; set; }

        //[Display(Name = "Requirement")]
        //public Guid RequirementId { get; set; }

        public string Name { get; set; }
        public string Comments { get; set; }        
        public Guid? ParentId { get; set; }
        public int Treegrid { get; set; }
        public int? ParentTreegrid { get; set; }


    }
}
