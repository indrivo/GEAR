using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ST.Calendar.Abstractions.Models.ViewModels
{
    public class BaseEventViewModel : CalendarEvent, IValidatableObject
    {
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
        }
    }
}
