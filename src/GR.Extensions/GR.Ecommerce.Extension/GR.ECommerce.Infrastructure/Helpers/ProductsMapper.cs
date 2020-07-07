using AutoMapper;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.ProductCategoriesViewModels;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;

namespace GR.ECommerce.Infrastructure.Helpers
{
    public class ProductsMapper : Profile
    {
        public ProductsMapper()
        {
            CreateMap<Product, ProductsPaginatedViewModel>()
                .IncludeAllDerived()
                .ReverseMap();

            CreateMap<ProductAttributes, ProductAttributeItemViewModel>()
                .ForMember(m => m.Label, o => o.MapFrom(m => m.ProductAttribute.Name))
                .ForMember(m => m.AttributeId, o => o.MapFrom(m => m.ProductAttributeId))
                .IncludeAllDerived()
                .ReverseMap();

            CreateMap<Category, GetProductCategoryViewModel>()
                .IncludeAllDerived()
                .ReverseMap();

            CreateMap<Category, CreateCategoryViewModel>()
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}