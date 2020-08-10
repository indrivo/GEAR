using System.Collections.Generic;

namespace GR.IdentityDocuments.Abstractions.Models
{
    public class IdentityDocumentType : IDocumentType
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Requirements
        /// </summary>
        public virtual IEnumerable<string> Requirements { get; set; } = new List<string>();
    }
}
