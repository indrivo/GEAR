using System;
using System.ComponentModel.DataAnnotations;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;
using GR.Forms.Abstractions.Enums;

namespace GR.Forms.Abstractions.Models.FormModels
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class FormFieldEvent : BaseModel
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
