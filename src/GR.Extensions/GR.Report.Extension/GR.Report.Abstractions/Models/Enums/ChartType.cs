using GR.Report.Abstractions.Models.Constants;
using System.ComponentModel.DataAnnotations;

namespace GR.Report.Abstractions.Models.Enums
{

    public enum ChartType
    {
        [Display(Name = ChartTypeConstants.Grid)]
        Grid = 1,

        [Display(Name = ChartTypeConstants.PivotGrid)]
        PivotGrid = 2,

        [Display(Name = ChartTypeConstants.Pie)]
        Pie = 3,

        [Display(Name = ChartTypeConstants.Doughnut)]
        Doughnut = 4,

        [Display(Name = ChartTypeConstants.BarVertical)]
        BarVertical = 5,

        [Display(Name = ChartTypeConstants.BarHorizontal)]
        BarHorizontal = 6,

        [Display(Name = ChartTypeConstants.Line)]
        Line = 7
    }
}
