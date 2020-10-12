using System.ComponentModel.DataAnnotations;

namespace GR.Core.Attributes.Validation
{
    public abstract class GearBaseValidationAttribute : ValidationAttribute
    {
        public bool UseTranslation = false;
        public string TranslationKey = null;
    }
}
