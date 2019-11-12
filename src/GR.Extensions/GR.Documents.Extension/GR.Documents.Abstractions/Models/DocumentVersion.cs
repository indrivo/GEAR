using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text;
using System.Transactions;
using GR.Core;
using GR.Files.Abstraction.Models;


namespace GR.Subscriptions.Abstractions.Models
{
    public class DocumentVersion: BaseModel
    {

        /// <summary>
        /// Document Id
        /// </summary>
        [Required]
        public Guid DocumentId { get; set; }

        /// <summary>
        /// Document
        /// </summary>
        public Document Document { get; set; }


        /// <summary>
        /// File Id
        /// </summary>
        public Guid FileStorageId { get; set; }


        /// <summary>
        /// File
        /// </summary>
        public FileStorage FileStorage { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }


        /// <summary>
        /// Version number
        /// </summary>
        [Required]
        public double VersionNumber { get; set; }

        /// <summary>
        /// Is arhiv
        /// </summary>
        [Required]
        public bool IsArhiv { get; set; }


        /// <summary>
        /// Coments
        /// </summary>
        public string Coments { get; set; }

        /// <summary>
        /// Previous version
        /// </summary>
        public DocumentVersion PreviousVersion { get; set; }


        /// <summary>
        /// Major version
        /// </summary>
        [Required]
        public bool IsMajorVersion { get; set; }

    }
}
