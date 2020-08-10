using System.Collections.Generic;

namespace GR.IdentityDocuments.Abstractions
{
    public interface IDocumentType
    {
        /// <summary>
        /// Identifier
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Requirements
        /// </summary>
        IEnumerable<string> Requirements { get; set; }
    }
}