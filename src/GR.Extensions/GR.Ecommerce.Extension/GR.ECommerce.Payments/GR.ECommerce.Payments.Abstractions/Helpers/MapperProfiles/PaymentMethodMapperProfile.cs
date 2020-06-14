using AutoMapper;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.ECommerce.Payments.Abstractions.ViewModels;

namespace GR.ECommerce.Payments.Abstractions.Helpers.MapperProfiles
{
    public class PaymentMethodMapperProfile : Profile
    {
        public PaymentMethodMapperProfile()
        {
            //Payment method map 
            CreateMap<PaymentMethod, PaymentMethodViewModel>()
                .IncludeAllDerived()
                .ForMember(x => x.Description,
                    o => o.MapFrom(x => PaymentProviders.GetProviderInfo(x.Name).Description))
                .ForMember(x => x.DisplayName,
                    o => o.MapFrom(x => PaymentProviders.GetProviderInfo(x.Name).DisplayName))
                .ReverseMap();
        }
    }
}