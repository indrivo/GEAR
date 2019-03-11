using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ST.Entities.Models.Request
{
    public class Request : ExtendedModel
    {
        [Display(Name = "Interested Party")]
        public Guid InterestedPartId { get; set; }

        [Display(Name = "Request")]
        public Guid RequestId { get; set; }

        public string Comments { get; set; }

    }
}
