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
