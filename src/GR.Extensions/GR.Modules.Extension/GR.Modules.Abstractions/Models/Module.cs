using GR.Core;

namespace GR.Modules.Abstractions.Models
{
    public class Module : BaseModel
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
    }
}