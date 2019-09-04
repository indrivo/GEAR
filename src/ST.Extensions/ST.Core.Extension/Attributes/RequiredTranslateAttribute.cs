using System;
using System.ComponentModel.DataAnnotations;

namespace ST.Core.Attributes
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
