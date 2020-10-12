using System.Linq;
using AutoMapper;
using GR.IdentityDocuments.Abstractions.Enums;
using GR.IdentityDocuments.Abstractions.Models;
using GR.IdentityDocuments.Abstractions.ViewModels;

namespace GR.IdentityDocuments.Abstractions.Helpers
{
    public class IdentityDocumentsMapperProfile : Profile
    {
        public IdentityDocumentsMapperProfile()
        {
            CreateMap<IDocumentType, IdentityDocumentType>()
                .IncludeAllDerived()
                .ReverseMap();

            CreateMap<UserKyc, UserKycItem>()
                .ForMember(x => x.FullName, o
                    => o.MapFrom(m => m.User.GetFullName()))
                .ForMember(x => x.FirstName, o
                    => o.MapFrom(m => m.User.FirstName))
                .ForMember(x => x.LastName, o
                    => o.MapFrom(m => m.User.LastName))
                .ForMember(x => x.PhoneNumber, o
                    => o.MapFrom(m => m.User.PhoneNumber))
                .ForMember(x => x.Email, o
                    => o.MapFrom(m => m.User.Email))
                .ForMember(x => x.UserId, o
                    => o.MapFrom(m => m.User.Id))
                .ForMember(x => x.SubmitDate, o
                    => o.MapFrom(m => m.Changed))
                .ForMember(x => x.PendingDocuments, o
                    => o.MapFrom(m => m.Documents.Count(x => x.ValidationState == DocumentValidationState.Pending)))
                .IncludeAllDerived()
                .ReverseMap();
        }
    }
}