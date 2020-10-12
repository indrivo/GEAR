using System.Collections.Generic;
using GR.IdentityDocuments.Abstractions.Models;
using Microsoft.Extensions.Localization;

namespace GR.IdentityDocuments.Abstractions.Helpers
{
    public class ProofOfResidenceDocumentType : IdentityDocumentType
    {
        #region Injectable

        private readonly IStringLocalizer _localizer;

        #endregion

        public ProofOfResidenceDocumentType(IStringLocalizer localizer)
        {
            _localizer = localizer;
            SetUp();
        }

        public void SetUp()
        {
            Id = "utilityBill";
            Name = _localizer["proof_of_residence"];
            Description = _localizer["proof_of_residence_info"];
            Requirements = new List<string>
            {
                _localizer.GetString("max_size_with_format", 10),
                _localizer["document_allowed_file_extensions"]
            };
        }
    }
}
