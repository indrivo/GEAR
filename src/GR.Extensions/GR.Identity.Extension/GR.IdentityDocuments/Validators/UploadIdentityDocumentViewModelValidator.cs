using System.Collections.Generic;
using System.Linq;
using ByteSizeLib;
using FluentValidation;
using GR.Core.Extensions;
using GR.IdentityDocuments.Abstractions;
using GR.IdentityDocuments.Abstractions.ViewModels;
using Microsoft.Extensions.Localization;

namespace GR.IdentityDocuments.Validators
{
    public class UploadIdentityDocumentViewModelValidator : AbstractValidator<UploadIdentityDocumentViewModel>
    {
        public UploadIdentityDocumentViewModelValidator(IEnumerable<IDocumentType> documentTypes, IStringLocalizer localizer)
        {
            var availableDocTypes = documentTypes.Select(x => x.Id);
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.Type)
                .NotNull()
                .Must(x => availableDocTypes.Contains(x))
                .WithMessage(x => $"{x.Type} is not valid document type, please upload document for: {availableDocTypes.Join(", ")}");

            RuleFor(x => x.File)
                .NotNull();

            When(x => x.File != null, () =>
            {
                RuleFor(x => x.File.Length)
                    .NotNull()
                    .LessThanOrEqualTo((long)ByteSize.FromMegaBytes(10).Bytes)
                    .WithMessage("File size is larger than allowed");

                RuleFor(x => x.File.ContentType)
                    .NotNull()
                    .Must(x => x.Equals("image/jpeg") || x.Equals("image/jpg") || x.Equals("image/png"))
                    .WithMessage("Allowed file extensions: JPG, JPEG, BMP, PNG");
            });


            RuleFor(x => x.Type)
                .NotNull()
                .NotEmpty();
        }
    }
}
