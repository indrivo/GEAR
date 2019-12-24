using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Abstractions.Models.AddressModels
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class District : BaseModel
    {
        public long StateOrProvinceId { get; set; }

        public StateOrProvince StateOrProvince { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        public string Name { get; set; }

        [StringLength(450)]
        public string Type { get; set; }

        public string Location { get; set; }
    }
}