using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ST.Entities.Models.KPI
{
  
  
    public class KPI : ExtendedModel
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [Display(Name = "Category")]
        public Guid CategoryId { get; set; }

        [Display(Name = "Calculation Method")]
        public string CalculationMethod { get; set; }

        [Display(Name = "Period")]
        public Guid PeriodId { get; set; }

        [Display(Name = "Measurement Unit")]
        public Guid MeasurementUnitId { get; set; }

        public string ProcentGoal { get; set; }
        public bool BoolGoal { get; set; }
        public int IntGoal { get; set; }

        [Display(Name = "Fulfillment Criterion")]
        public Guid FulfillmentCriterionId { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; }
    }
   

}
