using System;

namespace GR.Entities.Abstractions.Events.EventArgs
{
    public class ExecutedQueryEventArgs : System.EventArgs
    {
        public ExecutedQueryEventArgs()
        {
            Date = DateTime.Now;
        }
        public DateTime Date { get; set; }
        public string Query { get; set; }
        public object QueryResult { get; set; }
        public bool Completed { get; set; } = true;
        public long Elapsed;
        public Exception Exception { get; set; }
    }
}
