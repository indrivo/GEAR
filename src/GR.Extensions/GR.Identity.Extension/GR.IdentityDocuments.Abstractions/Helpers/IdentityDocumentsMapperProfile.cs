using AutoMapper;
using GR.IdentityDocuments.Abstractions.Models;

namespace GR.IdentityDocuments.Abstractions.Helpers
{
    public class IdentityDocumentsMapperProfile : Profile
    {
        public IdentityDocumentsMapperProfile()
        {
            CreateMap<IDocumentType, IdentityDocumentType>()
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}
