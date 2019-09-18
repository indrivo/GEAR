using ST.Core;

namespace ST.Dashboard.Abstractions.Models
{
    public class Row : BaseModel
    {
        public string Name { get; set; }
        /// <summary>
        /// Row order
        /// </summary>
        public virtual int Order { get; set; }
    }
}
