using ST.Report.Abstractions.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace ST.Report.Abstractions.Models.Enums
{
    public enum ChartFieldType
    {
        [Display(Name = ChartFieldTypeConstants.Normal)]
        Normal = 0,

        [Display(Name = ChartFieldTypeConstants.Label)]
        Label = 1,

        [Display(Name = ChartFieldTypeConstants.XAxis)]
        XAxis = 2,

        [Display(Name = ChartFieldTypeConstants.YAxis)]
        YAxis = 3,
    }
}
