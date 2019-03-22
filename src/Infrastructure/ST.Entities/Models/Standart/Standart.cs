using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ST.Audit.Models;

namespace ST.Entities.Models.Standart
{
  
  
    public class Standards : ExtendedModel
    {
        [Required]
        public string Name { get; set; }
       
    }
   

}
