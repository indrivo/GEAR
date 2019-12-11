using System;
using GR.Core.Extensions;
using GR.Report.Abstractions.Models.Dto;

namespace GR.Report.Abstractions.Models.ViewModels
{
    public class DynamicReportViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DynamicReportFolderViewModel DynamicReportFolder { get; set; }
        public DynamicReportDto ReportDataModel { get; set; }
        public string ReportData => ReportDataModel == null ? string.Empty : ReportDataModel.SerializeAsJson();

        public virtual string Author { get; set; }

        /// <summary>Stores the time when object was created</summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Stores the Id of the User that modified the object. Nullable
        /// </summary>
        public virtual string ModifiedBy { get; set; }

        /// <summary>Stores the time when object was modified. Nullable</summary>
        public DateTime Changed { get; set; }

        /// <summary>
        /// Stores state of the Object. True if object is deleted and false otherwise
        /// </summary>
        public virtual bool IsDeleted { get; set; }
    }

}
