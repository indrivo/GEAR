using AutoMapper;
using GR.Localization.Abstractions.Models;
using GR.Localization.Abstractions.ViewModels.LocalizationViewModels;

namespace GR.Localization.Abstractions.Helpers
{
    public class LocalizationMapperProfile : Profile
    {
        public LocalizationMapperProfile()
        {
            CreateMap<LanguageCreateViewModel, Language>()
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}
