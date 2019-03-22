using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ST.Audit.Models;

namespace ST.Entities.Models.Standart
{
  
  
    public class StandardCategories : ExtendedModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public  Guid StandardId { get; set; }       
        public Guid? ParentCategoryId { get; set; }
    }
   

}
