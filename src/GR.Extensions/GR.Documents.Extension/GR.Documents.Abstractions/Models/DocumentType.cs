using GR.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GR.Documents.Abstractions.Models
{
    public class DocumentType: BaseModel
    {
        /// <summary>
        /// Name document type
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

    }
}
