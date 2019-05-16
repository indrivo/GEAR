using Microsoft.EntityFrameworkCore;
using ST.Report.Abstractions.Models;

namespace ST.Report.Abstractions
{
    public interface IReportContext
    {
        DbSet<DynamicReportDbModel> DynamicReports { get; set; }
        DbSet<DynamicReportFolder> DynamicReportsFolders { get; set; }
    }
}
