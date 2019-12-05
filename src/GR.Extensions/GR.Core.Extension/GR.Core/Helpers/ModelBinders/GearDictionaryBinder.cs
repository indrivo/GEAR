﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GR.Core.Helpers.ModelBinders
{
    public class GearDictionaryBinder<TValue>  : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            // Try to fetch the value of the argument by name
            var valueProviderResult =
                bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName,
                valueProviderResult);

            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value)) return Task.CompletedTask;
            var model = value.Deserialize<Dictionary<string, TValue>>();

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}
