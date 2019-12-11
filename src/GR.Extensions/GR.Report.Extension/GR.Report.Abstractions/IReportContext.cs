using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.Report.Abstractions.Models;

namespace GR.Report.Abstractions
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
