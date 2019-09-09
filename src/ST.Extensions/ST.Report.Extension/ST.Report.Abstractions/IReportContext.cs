using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.Report.Abstractions.Models;

namespace ST.Report.Abstractions
{
    public interface IReportContext: IDbContext
    {
        /// <summary>
        /// DynamicReports
        /// </summary>
        DbSet<DynamicReport> DynamicReports { get; set; }

        /// <summary>
        /// DynamicReports Folders
        /// </summary>
        DbSet<DynamicReportFolder> DynamicReportsFolders { get; set; }
    }
}
