using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Calendar.Abstractions.Enums;

namespace GR.Calendar.Abstractions.Models.ViewModels
{
    public class BaseEventViewModel : IValidatableObject
    {
        /// <summary>
        /// Event name
        /// </summary>
        [Required]
        [MaxLength(50)]
        public virtual string Title { get; set; }

        /// <summary>
        /// Event description
        /// </summary>
        public virtual string Details { get; set; }

        /// <summary>
        /// Event location
        /// </summary>
        public virtual string Location { get; set; }

        /// <summary>
        /// Start date
        /// </summary>
        [Required]
        public virtual DateTime StartDate { get; set; }

        /// <summary>
        /// End date
        /// </summary>
        [Required]
        public virtual DateTime EndDate { get; set; }

        /// <summary>
        /// Event priority
        /// </summary>
        public virtual EventPriority Priority { get; set; } = EventPriority.Low;

        /// <summary>
        /// Event member
        /// </summary>
        public virtual ICollection<Guid> Members { get; set; } = new List<Guid>();

        /// <summary>
        /// Minutes to remind
        /// </summary>
        public virtual int MinutesToRemind { get; set; } = 15;

        /// <inheritdoc />
        /// <summary>
        /// Custom validations
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
            {
                yield return new ValidationResult("End Date must be greater than Start Date");
            }
            else if (StartDate < DateTime.Now)
            {
                yield return new ValidationResult("The event can only be created after this current time");
            }
            else if (MinutesToRemind > 60)
            {
                yield return new ValidationResult("MinutesToRemind must be lower than 60 minutes");
            }
        }
    }
}
