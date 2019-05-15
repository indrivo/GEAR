using System.Collections.Generic;

namespace ST.Report.Abstractions.Models
{
    /// <summary>
    /// Result Model for sql query
    /// </summary>
    public class DynamicReportQueryResultViewModel
    {
        public DynamicReportQueryResultViewModel()
        {
            Columns = new List<ResultValues>();
        }
        public List<ResultValues> Columns { get; set; }
    }
    public class ResultValues
    {
        public string Column { get; set; }
        public string Value { get; set; }
    }
}