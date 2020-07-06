using System;
using GR.Core.Razor.BaseControllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace GR.Core.Razor.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JsonProducesAttribute : ProducesAttribute
    {
        public JsonProducesAttribute(Type type) : base(type)
        {
            ContentTypes = new MediaTypeCollection
            {
                ContentType.ApplicationJson
            };
        }

        public JsonProducesAttribute(params string[] additionalContentTypes) : base(ContentType.ApplicationJson, additionalContentTypes)
        {
        }
    }
}