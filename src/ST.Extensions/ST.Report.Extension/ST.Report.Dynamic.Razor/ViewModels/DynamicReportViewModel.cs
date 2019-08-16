using ST.Report.Abstractions.Models;
using System;
using System.Collections.Generic;

namespace ST.Report.Dynamic.Razor.ViewModels
{
    public class DynamicReportCreateRunViewModel
    {
        public DynamicReportCreateRunViewModel()
        {
            ChartDto = new DynamicReportChartDto()
            {
                ChartType = ChartType.Count,
                GraphType = GraphType.List,
                TimeFrameEnum = TimeFrameEnum.Month
            };
            FiltersList = new List<DynamicReportFilter>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid DynamicReportFolderId { get; set; }
        public string TableName { get; set; }
        public List<DynamicReportColumnDataModel> ColumnList { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public List<DynamicReportFilter> FiltersList { get; set; }
        public DynamicReportChartDto ChartDto { get; set; }
    }

    public class DynamicReportFolderViewModel
    {
        public string Name { get; set; }
    }


    public class SelectOption
    {
        public int id { get; set; }

        public string text { get; set; }
    }

}
