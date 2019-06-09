using System.ComponentModel.DataAnnotations;
using ST.Core;

namespace ST.PageRender.Abstractions.Models.RenderTemplates
{
    public class Template : BaseModel
    {
        /// <summary>
        /// Template name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Identifier what is used for JSView
        /// </summary>
        [Required]
        public string IdentifierName { get; set; }

        /// <summary>
        /// Template content
        /// </summary>
        public string Value { get; set; }

        public string Description { get; set; }
    }
}
