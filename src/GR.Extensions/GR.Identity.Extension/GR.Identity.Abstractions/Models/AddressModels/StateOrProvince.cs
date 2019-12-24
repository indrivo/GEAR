using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Abstractions.Models.AddressModels
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class StateOrProvince : IBase<long>
    {
        public long Id { get; set; }

        public StateOrProvince()
        {
        }

        public StateOrProvince(long id)
        {
            Id = id;
        }

        [StringLength(450)]
        public string CountryId { get; set; }

        public Country Country { get; set; }

        [StringLength(450)]
        public string Code { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        public string Name { get; set; }

        [StringLength(450)]
        public string Type { get; set; }
    }
}