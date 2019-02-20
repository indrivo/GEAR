using ST.BaseRepository;
using ST.Entities.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ST.Procesess.Models
{
    public class STProcessSchema : ExtendedModel
    {
        /// <summary>
        /// Store Xml schema
        /// </summary>
        [Required]
        public string Diagram { get; set; }

        /// <summary>
        /// Description of schema
        /// </summary>
        [StringLength(50)]
        public string Description { get; set; }

        /// <summary>
        /// State of diagram if is sync or not
        /// </summary>
        [DefaultValue(false)]
        public bool Synchronized { get; set; }

        /// <summary>
        /// Title of schema
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Version of process schema 
        /// </summary>
        [Required]
        [DefaultValue(1)]
        public new int Version { get; set; }

        /// <summary>
        /// Reference to initial schema with version 1
        /// </summary>
        public STProcessSchema InitialSchema { get; set; }

        public Guid? InitialSchemaId { get; set; }
    }
}
