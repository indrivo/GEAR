using System;

namespace ST.Core.Attributes.Documentation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class AuthorAttribute : Attribute
    {
        /// <summary>
        /// Name
        /// </summary>
        private string _name;

        /// <summary>
        /// Version
        /// </summary>
        private double _version;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        public AuthorAttribute(string name, double version = 1.0)
        {
            _name = name;
            _version = version;
        }
    }
}
