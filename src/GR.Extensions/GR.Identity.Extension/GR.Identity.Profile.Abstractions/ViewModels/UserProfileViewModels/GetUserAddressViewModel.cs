using GR.Identity.Abstractions;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using Newtonsoft.Json;

namespace GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels
{
    public class GetUserAddressViewModel : Address
    {
        /// <summary>
        /// Ignore user
        /// </summary>
        [JsonIgnore]
        public override GearUser User { get; set; }
    }
}