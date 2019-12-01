using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace GR.Core.Helpers.ModelBinders.ModelBinderProviders
{
    public class GearDictionaryModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(Dictionary<string, object>))
            {
                return new BinderTypeModelBinder(typeof(GearDictionaryBinder<object>));
            }

            return null;
        }
    }
}
