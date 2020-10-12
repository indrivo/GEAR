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
            Name = _localizer["selfie_verification"];
            Description = _localizer["upload_selfie_info"];
            Requirements = new List<string>
            {
                _localizer.GetString("max_size_with_format", 10),
                _localizer["document_allowed_file_extensions"]
            };
        }
    }
}