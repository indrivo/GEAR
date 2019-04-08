using ST.Identity.Services.Abstractions;

namespace ST.Configuration.Models
{
    public class FormValidation : ICacheModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Default { get; set; }
    }
}
