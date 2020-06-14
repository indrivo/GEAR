using System;
using System.ComponentModel.DataAnnotations;
using GR.Core.Extensions;

namespace GR.Core.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MinAgeAttribute : ValidationAttribute
    {
        private readonly int _limit;

        private readonly string _messageFormat;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="messageFormat"></param>
        public MinAgeAttribute(int limit, string messageFormat = null)
        {
            _limit = limit;
            _messageFormat = messageFormat;
        }

        /// <summary>
        /// Check if valid
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var bDay = DateTime.Parse(value.ToString());
            var today = DateTime.Today;
            var age = today.Year - bDay.Year;
            if (bDay > today.AddYears(-age))
            {
                age--;
            }

            if (age >= _limit) return null;
            var message = _messageFormat.IsNullOrEmpty() ? "Sorry you are not old enough" : string.Format(_messageFormat, _limit);

            var result = new ValidationResult(message);
            return result;
        }
    }
}
