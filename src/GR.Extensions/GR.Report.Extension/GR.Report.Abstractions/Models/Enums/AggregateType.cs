using System.ComponentModel;
using GR.Report.Abstractions.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace GR.Report.Abstractions.Models.Enums
{
    public enum AggregateType
    {
        [Display(Name = AggregateTypeConstants.None)]
        [Description("")]
        None = 0,

        [Display(Name = AggregateTypeConstants.Count)]
        [Description("count")]
        Count = 1,

        [Display(Name = AggregateTypeConstants.Sum)]
        [Description("sum")]
        Sum = 2,

        [Display(Name = AggregateTypeConstants.Min)]
        [Description("min")]
        Min = 3,

        [Display(Name = AggregateTypeConstants.Max)]
        [Description("max")]
        Max = 4,

        [Display(Name = AggregateTypeConstants.Avg)]
        [Description("avg")]
        Avg = 5,

        [Display(Name = AggregateTypeConstants.Agg)]
        [Description("array_agg")]
        ArrayAgg = 6,
    }
}
