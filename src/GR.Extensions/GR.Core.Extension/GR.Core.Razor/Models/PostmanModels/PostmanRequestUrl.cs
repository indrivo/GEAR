using System.Collections.Generic;

namespace GR.Core.Razor.Models.PostmanModels
{
    public class PostmanRequestUrl
    {
        public virtual string Protocol { get; set; }
        public virtual string Host { get; set; }
        public virtual string Path { get; set; }
        public virtual IEnumerable<object> Query { get; set; }
    }
}
