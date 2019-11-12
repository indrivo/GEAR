using GR.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GR.Documents.Abstractions.Models
{
    public class DocumentType: BaseModel
    {
        /// <summary>
        /// Name document type
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type document
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Is system
        /// </summary>
        public bool IsSystem { get; set; }

    }
}
