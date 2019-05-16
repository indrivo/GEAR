using System.Collections.Generic;
using ST.Core;

namespace ST.Report.Abstractions.Models
{
    public class DynamicReportFolder : BaseModel
    {
        public string Name { get; set; }
        public IEnumerable<DynamicReportDbModel> Reports { get; set; }
    }
}