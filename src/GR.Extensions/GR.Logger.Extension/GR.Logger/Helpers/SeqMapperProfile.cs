using AutoMapper;
using GR.Logger.Abstractions.ViewModels;
using Seq.Api.Model.Events;

namespace GR.Logger.Helpers
{
    public class SeqMapperProfile : Profile
    {
        public SeqMapperProfile()
        {
            CreateMap<LogEventViewModel, EventEntity>()
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}
