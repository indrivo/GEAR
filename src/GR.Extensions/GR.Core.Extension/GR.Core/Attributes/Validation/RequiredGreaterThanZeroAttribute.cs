using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace GR.Core.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredGreaterThanZeroAttribute : GearBaseValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var localizationService = (IStringLocalizer)validationContext.GetService(typeof(IStringLocalizer));
            return value != null && double.TryParse(value.ToString(), out var i) && i > 0
                ? ValidationResult.Success
                : new ValidationResult(UseTranslation ? localizationService?[TranslationKey] : "The value must be greater than 0");
        }
    }
}
