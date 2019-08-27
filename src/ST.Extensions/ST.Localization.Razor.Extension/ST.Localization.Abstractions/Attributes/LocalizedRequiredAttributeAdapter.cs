using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.Extensions.Localization;

namespace ST.Localization.Abstractions.Attributes
{
    public class LocalizedRequiredAttributeAdapter : RequiredAttributeAdapter
    {
        public LocalizedRequiredAttributeAdapter(RequiredAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
        {

        }
    }

    public class LocalizedValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        public virtual IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            IAttributeAdapter adapter = null;

            var type = attribute.GetType();

            if (type == typeof(RequiredAttribute))
            {
                adapter = new LocalizedRequiredAttributeAdapter((RequiredAttribute)attribute, stringLocalizer);
            }

            return adapter;
        }
    }
}
