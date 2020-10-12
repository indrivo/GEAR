using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.IdentityDocuments.Abstractions.Enums;

namespace GR.IdentityDocuments.Abstractions
{
    public interface IIdentityVerificationService
    {
        /// <summary>
        /// Update user kyc decision
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="decision"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateUserKycStateAsync(Guid userId, DocumentValidationState decision, string reason);

        /// <summary>
        ///  Update document kyc
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="decision"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateDocumentKycStateAsync(Guid documentId, DocumentValidationState decision, string reason);
    }
}