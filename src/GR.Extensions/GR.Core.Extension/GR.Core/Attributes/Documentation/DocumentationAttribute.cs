using System;

namespace GR.Core.Attributes.Documentation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DocumentationAttribute : Attribute
    {
        /// <summary>
        /// Documentation
        /// </summary>
        private string _doc;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="documentation"></param>
        public DocumentationAttribute(string documentation)
        {
            _doc = documentation;
        }
    }
}
