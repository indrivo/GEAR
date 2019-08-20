using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.Report.Abstractions.Models;

namespace ST.Report.Abstractions
{
    public interface IReportContext: IDbContext
    {
        DbSet<DynamicReport> DynamicReports { get; set; }
        DbSet<DynamicReportFolder> DynamicReportsFolders { get; set; }
    }
}
