using AutoMapper;
using GR.Localization.Abstractions.Models;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;
using Microsoft.Extensions.Localization;

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

            CreateMap<LocalizedString, LocalizedStringViewModel>()
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}
