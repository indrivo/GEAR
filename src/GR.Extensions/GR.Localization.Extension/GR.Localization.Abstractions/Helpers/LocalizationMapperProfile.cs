using AutoMapper;
using GR.Localization.Abstractions.Models;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;

namespace GR.Localization.Abstractions.Helpers
{
    public class LocalizationMapperProfile : Profile
    {
        public LocalizationMapperProfile()
        {
            CreateMap<Language, LanguageCreateViewModel>()
                .IncludeAllDerived()
                .ForMember(m => m.IsDisabled, o
                    => o.MapFrom(x => x.IsDeleted))
                .ReverseMap();
        }
    }
}
