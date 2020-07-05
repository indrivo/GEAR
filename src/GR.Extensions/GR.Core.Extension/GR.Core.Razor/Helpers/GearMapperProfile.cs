using AutoMapper;
using GR.Core.Helpers;
using GR.Core.Razor.ViewModels;

namespace GR.Core.Razor.Helpers
{
    public class GearMapperProfile : Profile
    {
        public GearMapperProfile()
        {
            //Elapsed time result
            CreateMap(typeof(ElapsedTimeJsonResultModel<>), typeof(ResultModel<>))
                .IncludeAllDerived()
                .ReverseMap();

            //Pagination response 
            CreateMap(typeof(DTResult<>), typeof(DTResult<>))
                .IncludeAllDerived()
                .ReverseMap();
            
            //Result model
            CreateMap(typeof(ResultModel<>), typeof(ResultModel<>))
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}