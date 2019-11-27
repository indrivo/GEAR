using Newtonsoft.Json;
using GR.Core;
using GR.Core.Extensions;
using GR.Report.Abstractions.Models.Dto;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace GR.Report.Abstractions.Models
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
        public DynamicReportDto ReportDataModel
        {
            get
            {
                DynamicReportDto result = new DynamicReportDto();
                if (!string.IsNullOrEmpty(ReportData))
                {
                    try
                    {
                        result = JsonConvert.DeserializeObject<DynamicReportDto>(ReportData);
                    }
                    catch (Exception ex)
                    {
                        throw new SerializationException(ex.Message);
                    }
                }
                return result;
            }
            set
            {
                if (value != null)
                {
                    ReportData = value.SerializeAsJson();
                }
            }
        }
    }
}