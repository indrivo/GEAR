using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ST.Audit.Models;

namespace ST.Entities.Models.Standart
{
  
  
    public class CategoryRequirements : ExtendedModel
    {
        [Required]
        public string Name { get; set; }        
        public  Guid? ParentRequirementId { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
    }
   

}
