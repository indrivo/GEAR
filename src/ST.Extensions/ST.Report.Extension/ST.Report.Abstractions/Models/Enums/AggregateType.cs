using ST.Report.Abstractions.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace ST.Report.Abstractions.Models.Enums
{
    public enum AggregateType
    {
        [Display(Name = AggregateTypeConstants.None)]
        None = 0,

        [Display(Name = AggregateTypeConstants.Count)]
        count = 1,

        [Display(Name = AggregateTypeConstants.Sum)]
        sum = 2,

        [Display(Name = AggregateTypeConstants.Min)]
        min = 3,

        [Display(Name = AggregateTypeConstants.Max)]
        max = 4,

        [Display(Name = AggregateTypeConstants.Avg)]
        avg = 5,

        [Display(Name = AggregateTypeConstants.Agg)]
        array_agg = 6,
    }
}
