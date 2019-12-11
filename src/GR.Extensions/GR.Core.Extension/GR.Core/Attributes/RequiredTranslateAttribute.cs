using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredTranslateAttribute : RequiredAttribute
    {
        /// <summary>
        /// Translate key
        /// </summary>
        public string Key { get; set; }
    }
}
