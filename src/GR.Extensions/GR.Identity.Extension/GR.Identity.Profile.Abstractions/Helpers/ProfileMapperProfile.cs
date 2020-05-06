using GR.Identity.Profile.Abstractions.Models.AddressModels;
using GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels;

namespace GR.Identity.Profile.Abstractions.Helpers
{
    public class ProfileMapperProfile : AutoMapper.Profile
    {
        public ProfileMapperProfile()
        {
            //Get user address
            CreateMap<Address, GetUserAddressViewModel>()
                .IncludeAllDerived()
                .ReverseMap();

            //Add new user address
            CreateMap<Address, AddNewAddressViewModel>()
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}
