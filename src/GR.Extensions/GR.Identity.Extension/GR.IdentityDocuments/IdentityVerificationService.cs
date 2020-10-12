using System;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.IdentityDocuments.Abstractions;
using GR.IdentityDocuments.Abstractions.Enums;
using Microsoft.EntityFrameworkCore;

namespace GR.IdentityDocuments
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class IdentityVerificationService : IIdentityVerificationService
    {
        #region Injectable

        /// <summary>
        /// Inject identity document service
        /// </summary>
        private readonly IIdentityDocumentService _identityDocumentService;

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IIdentityDocumentsContext _context;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        #endregion

        public IdentityVerificationService(IIdentityDocumentService identityDocumentService, IIdentityDocumentsContext context, IUserManager<GearUser> userManager)
        {
            _identityDocumentService = identityDocumentService;
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Update user kyc decision
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="decision"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateUserKycStateAsync(Guid userId, DocumentValidationState decision, string reason)
        {
            var kycResponse = await _identityDocumentService.GetUserKycAsync(userId);
            if (!kycResponse.IsSuccess) return kycResponse.ToBase();
            var kyc = kycResponse.Result;
            kyc.ValidationState = decision;
            kyc.Reason = reason;
            _context.UserKyc.Update(kyc);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Update document kyc
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="decision"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateDocumentKycStateAsync(Guid documentId, DocumentValidationState decision, string reason)
        {
            var doc = await _context.IdentityDocuments.FirstOrDefaultAsync(x => x.Id.Equals(documentId));
            if (doc == null) return new NotFoundResultModel();
            doc.ValidationState = decision;
            doc.Reason = reason;
            _context.IdentityDocuments.Update(doc);
            return await _context.PushAsync();
        }
    }
}