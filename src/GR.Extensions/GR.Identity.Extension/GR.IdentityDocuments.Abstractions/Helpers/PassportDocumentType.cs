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
            Name = _localizer["passport"];
            Description = _localizer["upload_documents_info"];
            Requirements = new List<string>
            {
                _localizer.GetString("max_size_with_format", 10),
                _localizer["document_allowed_file_extensions"]
            };
        }
    }
}   