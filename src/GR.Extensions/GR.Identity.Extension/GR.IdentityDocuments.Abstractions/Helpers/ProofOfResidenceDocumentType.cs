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
            Name = "Proof of Residence";
            Description =
                "To verify your address, please upload a scan or a clear photo of utility bill, bank statement not older than 3 months.";
            Requirements = new List<string>
            {
                "Max size 10MB",
                "Allowed file extensions: JPG, JPEG, BMP, PNG"
            };
        }
    }
}
