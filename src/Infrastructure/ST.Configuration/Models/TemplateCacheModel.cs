using ST.Identity.Services.Abstractions;

namespace ST.Configuration.Models
{
    public class TemplateCacheModel : ICacheModel
    {
        public bool IsSuccess { get; set; }
        /// <summary>
        /// Identifier of template
        /// </summary>
        public string Identifier { get; set; }
        /// <summary>
        /// Value of template
        /// </summary>
        public string Value { get; set; }
    }
}
