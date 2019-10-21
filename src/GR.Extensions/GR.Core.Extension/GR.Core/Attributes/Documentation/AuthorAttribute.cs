using System;

namespace GR.Core.Attributes.Documentation
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
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
        /// <param name="changes"></param>
        public AuthorAttribute(string name, double version = 1.0, string changes = null)
        {
            _name = name;
            _version = version;
        }
    }
}
