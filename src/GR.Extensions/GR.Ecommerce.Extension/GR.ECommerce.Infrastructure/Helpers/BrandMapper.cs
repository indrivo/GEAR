using AutoMapper;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.BrandViewModels;

namespace GR.ECommerce.Infrastructure.Helpers
{
    public class BrandMapper : Profile
    {
        public BrandMapper()
        {
            CreateMap<Brand, CreateBrandViewModel>()
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}
