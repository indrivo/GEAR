using GR.Report.Abstractions.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace GR.Report.Abstractions.Models.Enums
{
    public enum FilterType
    {
        [Display(Name = FilterTypeConstants.Equal)]
        Equal = 1,

        [Display(Name = FilterTypeConstants.NotEqual)]
        NotEqual = 2,

        [Display(Name = FilterTypeConstants.GreaterThan)]
        GreaterThan = 3,

        [Display(Name = FilterTypeConstants.LessThan)]
        LessThan = 4,

        [Display(Name = FilterTypeConstants.GreaterThanOrEqual)]
        GreaterThanOrEqual = 5,

        [Display(Name = FilterTypeConstants.LessThanOrEqual)]
        LessThanOrEqual = 6,

        [Display(Name = FilterTypeConstants.Contains)]
        Contains = 7,

        [Display(Name = FilterTypeConstants.GroupBy)]
        GroupBy = 8,
    }
}
