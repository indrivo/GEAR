using System.Collections.Generic;
using GR.IdentityDocuments.Abstractions.Models;
using Microsoft.Extensions.Localization;

namespace GR.IdentityDocuments.Abstractions.Helpers
{
    public class SelfieDocumentType : IdentityDocumentType
    {
        #region Injectable

        private readonly IStringLocalizer _localizer;

        #endregion

        public SelfieDocumentType(IStringLocalizer localizer)
        {
            _localizer = localizer;
            SetUp();
        }

        public void SetUp()
        {
            Id = "selfieWithId";
            Name = "Selfie Verification";
            Description = "Upload a selfie of your face while holding up the ID document.";
            Requirements = new List<string>
            {
                "Max size 10MB",
                "Allowed file extensions: JPG, JPEG, BMP, PNG"
            };
        }
    }
}