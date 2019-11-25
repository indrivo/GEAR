using GR.Core;

namespace GR.WorkFlows.Abstractions.Models
{
    public class TransitionAction : BaseModel
    {
        /// <summary>
        /// Action name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// System runtime name
        /// </summary>
        public virtual string ClassName { get; set; }

        /// <summary>
        /// Class name full path
        /// </summary>
        public virtual string ClassNameWithNameSpace { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Check if is system
        /// </summary>
        public virtual bool IsSystem { get; set; }
    }
}
