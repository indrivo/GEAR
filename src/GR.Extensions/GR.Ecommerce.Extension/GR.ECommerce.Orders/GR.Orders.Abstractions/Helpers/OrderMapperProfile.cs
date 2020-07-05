using AutoMapper;
using GR.Orders.Abstractions.Helpers.MapperResolvers;
using GR.Orders.Abstractions.Models;
using GR.Orders.Abstractions.ViewModels.OrderViewModels;

namespace GR.Orders.Abstractions.Helpers
{
    public class OrderMapperProfile : Profile
    {
        public OrderMapperProfile()
        {
            //Order info
            CreateMap<Order, GetOrdersViewModel>()
                .ForMember(x => x.User, o => o.MapFrom<UserOrderMapperResolver>())
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}