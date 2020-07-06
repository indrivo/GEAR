using System;

namespace GR.Hooks.Gitlab.Models
{
    public class Commit
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public DateTime Timestamp { get; set; }
        public string Url { get; set; }
        public Author Author { get; set; }
        public string[] Added { get; set; }
        public string[] Modified { get; set; }
        public object[] Removed { get; set; }
    }
}