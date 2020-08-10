using System.Collections.Generic;
using GR.IdentityDocuments.Abstractions.Models;
using Microsoft.Extensions.Localization;

namespace GR.IdentityDocuments.Abstractions.Helpers
{
    public class PassportDocumentType : IdentityDocumentType
    {
        #region Injectable

        private readonly IStringLocalizer _localizer;

        #endregion

        public PassportDocumentType(IStringLocalizer localizer)
        {
            _localizer = localizer;
            SetUp();
        }

        public void SetUp()
        {
            Id = "passport";
            Name = "Passport";
            Description = "Upload your identification document front and back in clear view, or upload passport.";
            Requirements = new List<string>
            {
                "Max size 10MB",
                "Allowed file extensions: JPG, JPEG, BMP, PNG"
            };
        }
    }
}   