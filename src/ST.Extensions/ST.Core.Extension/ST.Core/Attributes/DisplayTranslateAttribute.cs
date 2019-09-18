using System;

namespace ST.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayTranslateAttribute : Attribute
    {
        /// <summary>
        /// Translate key
        /// </summary>
        public string Key { get; set; }
    }
}
