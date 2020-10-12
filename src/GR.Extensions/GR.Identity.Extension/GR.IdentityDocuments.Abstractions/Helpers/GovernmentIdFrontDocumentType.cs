using System.Collections.Generic;
using GR.IdentityDocuments.Abstractions.Models;
using Microsoft.Extensions.Localization;

namespace GR.IdentityDocuments.Abstractions.Helpers
{
    public class GovernmentIdFrontDocumentType : IdentityDocumentType
    {
        #region Injectable

        private readonly IStringLocalizer _localizer;

        #endregion

        public GovernmentIdFrontDocumentType(IStringLocalizer localizer)
        {
            _localizer = localizer;
            SetUp();
        }

        public void SetUp()
        {
            Id = "governmentIdFront";
            Name = _localizer["personal_id_front"];
            Description = _localizer["upload_identity_document_info"];
            Requirements = new List<string>
            {
                _localizer.GetString("max_size_with_format", 10),
                _localizer["document_allowed_file_extensions"]
            };
        }
    }
}