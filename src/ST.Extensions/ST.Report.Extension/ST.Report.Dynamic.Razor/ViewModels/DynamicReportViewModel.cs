using ST.Core.Extensions;
using ST.Report.Abstractions.Models;
using System;

namespace ST.Report.Dynamic.Razor.ViewModels
{
    public class DynamicReportViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid DynamicReportFolderId { get; set; }
        public DynamicReportDataModel ReportDataModel { get; set; }
        public string ReportData => ReportDataModel == null ? string.Empty : ReportDataModel.Serialize();
    }

    public class DynamicReportFolderViewModel
    {
        public string Name { get; set; }
    }


    public class SelectOption
    {
        public int Id { get; set; }

        public string Text { get; set; }
    }

    public class ResponseClass
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

}
