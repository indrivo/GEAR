using Newtonsoft.Json;
using ST.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST.Report.Abstractions.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Model to save in database
    /// </summary>
    public class DynamicReport : BaseModel
    {
        public string Name { get; set; }
        public Guid DynamicReportFolderId { get; set; }
        public DynamicReportFolder DynamicReportFolder { get; set; }
        public string ReportData { get; set; }

        [NotMapped]
        public DynamicReportDataModel ReportDataModel
        {
            get
            {
                DynamicReportDataModel result = new DynamicReportDataModel();
                if (!string.IsNullOrEmpty(ReportData))
                {
                    try
                    {
                        result = JsonConvert.DeserializeObject<DynamicReportDataModel>(ReportData);
                    }
                    catch { }
                }
                return result;
            }
            set
            {
                if (value != null)
                {
                    ReportData = JsonConvert.SerializeObject(value);
                }
            }
        }
    }


    public class DynamicReportDataModel
    {
        public IEnumerable<string> Tables { get; set; } = new List<string>();
        public IEnumerable<DynamicReportRelation> Relations { get; set; } = new List<DynamicReportRelation>();
        public IEnumerable<DynamicReportField> FieldsList { get; set; } = new List<DynamicReportField>();
        public IEnumerable<DynamicReportFilter> FiltersList { get; set; } = new List<DynamicReportFilter>();
        public IEnumerable<DynamicReportChart> DynamicReportCharts { get; set; } = new List<DynamicReportChart>();
    }

}