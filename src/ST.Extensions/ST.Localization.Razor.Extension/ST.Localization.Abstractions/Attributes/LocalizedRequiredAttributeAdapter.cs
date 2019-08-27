using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.Extensions.Localization;
using ST.Core.Attributes;
using ST.Core.Helpers;

namespace ST.Localization.Abstractions.Attributes
{
    public class LocalizedRequiredAttributeAdapter : RequiredAttributeAdapter
    {
        public LocalizedRequiredAttributeAdapter(RequiredAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer) { }

        public LocalizedRequiredAttributeAdapter(RequiredTranslateAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
        {
            var translated = stringLocalizer[attribute.Key].Value;
            attribute.ErrorMessage = attribute.FormatErrorMessage("{0} " + translated);

        }
    }

    public class LocalizedValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        public virtual IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            var localizer = IoC.Resolve<IStringLocalizer>();
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            IAttributeAdapter adapter = null;

            var type = attribute.GetType();

            if (type == typeof(RequiredAttribute))
            {
                adapter = new LocalizedRequiredAttributeAdapter((RequiredAttribute)attribute, localizer);
            }

            if (type == typeof(RequiredTranslateAttribute))
            {
                adapter = new LocalizedRequiredAttributeAdapter((RequiredTranslateAttribute)attribute, localizer);
            }

            return adapter;
        }
    }
}
