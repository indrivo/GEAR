using System;
using System.ComponentModel.DataAnnotations;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Core;
using ST.Entities.Models.Enums;

namespace ST.Entities.Models.Forms
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class FormFieldEvent : ExtendedModel
    {
        /// <summary>
        /// Name what describe event handler
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Description of handler
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Handler what describe event action
        /// </summary>
        [Required]
        public string Handler { get; set; }

        /// <summary>
        /// Type of action event
        /// </summary>
        [Required]
        public FormEvent Event { get; set; }

        /// <summary>
        /// Reference to form field
        /// </summary>
        public Field Field { get; set; }

        public Guid FieldId { get; set; }
    }
}
