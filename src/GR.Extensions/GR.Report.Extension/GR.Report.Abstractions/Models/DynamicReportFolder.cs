using System.Collections.Generic;
using GR.Core;

namespace GR.Report.Abstractions.Models
{
    public class DynamicReportFolder : BaseModel
    {
        public string Name { get; set; }
        public IEnumerable<DynamicReport> Reports { get; set; }
    }
}