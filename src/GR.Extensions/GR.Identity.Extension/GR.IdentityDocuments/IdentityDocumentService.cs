using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.IdentityDocuments.Abstractions;
using GR.IdentityDocuments.Abstractions.Enums;
using GR.IdentityDocuments.Abstractions.Models;
using GR.IdentityDocuments.Abstractions.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GR.IdentityDocuments
{
    public class IdentityDocumentService : IIdentityDocumentService
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IIdentityDocumentsContext _context;

        /// <summary>
        /// Inject document types
        /// </summary>
        private readonly IEnumerable<IDocumentType> _documentTypes;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Include mapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILogger<IdentityDocumentService> Logger;

        #endregion

        public IdentityDocumentService(IIdentityDocumentsContext context, IEnumerable<IDocumentType> documentTypes, IUserManager<GearUser> userManager, IMapper mapper, ILogger<IdentityDocumentService> logger)
        {
            _context = context;
            _documentTypes = documentTypes;
            _userManager = userManager;
            _mapper = mapper;
            Logger = logger;
        }

        /// <summary>
        /// Get user kyc for current user
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<UserKyc>> GetUserKycAsync()
        {
            var userId = _userManager.FindUserIdInClaims().Result;
            return await GetUserKycAsync(userId);
        }

        /// <summary>
        /// Get user kyc
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<UserKyc>> GetUserKycAsync(Guid userId)
        {
            var kyc = await _context.UserKyc.FirstOrDefaultAsync(x => x.UserId.Equals(userId));
            if (kyc == null)
            {
                var userResponse = await _userManager.FindUserByIdAsync(userId);
                if (!userResponse.IsSuccess) return userResponse.Map<UserKyc>();
                var newKyc = new UserKyc
                {
                    UserId = userId
                };
                await _context.UserKyc.AddAsync(newKyc);
                var dbResult = await _context.PushAsync();
                if (!dbResult.IsSuccess) return dbResult.Map<UserKyc>();
                return new SuccessResultModel<UserKyc>(newKyc);
            }
            return new SuccessResultModel<UserKyc>(kyc);
        }

        /// <summary>
        /// Get user kyc info
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<UserKycInfo>> GetUserKycInfoAsync(Guid userId)
        {
            var kyc = await _context.UserKyc
                .Include(x => x.Documents)
                .FirstOrDefaultAsync(x => x.UserId.Equals(userId));
            if (kyc == null) return new NotFoundResultModel<UserKycInfo>();
            var info = new UserKycInfo
            {
                ValidationState = kyc.ValidationState,
                Reason = kyc.Reason,
                PendingDocuments = kyc.Documents.Count(x => x.ValidationState == DocumentValidationState.Pending),
                SubmitDate = kyc.Changed,
                KycId = kyc.Id,
                Documents = new List<UserKycDocumentInfo>()
            };

            foreach (var document in kyc.Documents)
            {
                info.Documents.Add(new UserKycDocumentInfo
                {
                    ValidationState = document.ValidationState,
                    Reason = document.Reason,
                    DocumentId = document.Id,
                    DocumentType = _documentTypes.FirstOrDefault(x => x.Id.Equals(document.DocumentType))
                });
            }

            return new SuccessResultModel<UserKycInfo>(info);
        }

        /// <summary>
        /// Get available document types
        /// </summary>
        /// <returns></returns>
        public virtual Task<ResultModel<IEnumerable<IdentityDocumentType>>> GetAvailableDocumentTypesAsync()
        {
            var data = new SuccessResultModel<IEnumerable<IdentityDocumentType>>(_mapper.Map<IEnumerable<IdentityDocumentType>>(_documentTypes));
            return Task.FromResult(data.Is<ResultModel<IEnumerable<IdentityDocumentType>>>());
        }

        /// <summary>
        /// Upload document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UploadDocumentAsync(UploadIdentityDocumentViewModel model)
        {
            var userId = _userManager.FindUserIdInClaims().Result;
            var kycResult = await GetUserKycAsync(userId);
            if (!kycResult.IsSuccess) return kycResult.ToBase();
            var kyc = kycResult.Result;
            Logger.LogInformation("User: {User} start to upload document {DocType}", userId, model.Type);
            var existentDocument = await _context.IdentityDocuments.FirstOrDefaultAsync(x => x.UserKycId.Equals(kycResult.Result.Id) && x.DocumentType.Equals(model.Type));
            if (existentDocument == null)
            {
                var document = new IdentityDocument
                {
                    UserKycId = kyc.Id,
                    Reason = null,
                    ValidationState = DocumentValidationState.Pending,
                    DocumentType = model.Type,
                    ContentType = model.File.ContentType,
                    FileName = model.File.FileName
                };
                using (var memoryStream = new MemoryStream())
                {
                    await model.File.CopyToAsync(memoryStream);
                    document.Blob = memoryStream.ToArray();
                }

                await _context.IdentityDocuments.AddAsync(document);
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.File.CopyToAsync(memoryStream);
                    existentDocument.Blob = memoryStream.ToArray();
                }

                existentDocument.ContentType = model.File.ContentType;
                existentDocument.FileName = model.File.FileName;
                existentDocument.Reason = null;
                existentDocument.ValidationState = DocumentValidationState.Pending;

                _context.IdentityDocuments.Update(existentDocument);
            }

            kyc.ValidationState = DocumentValidationState.Pending;
            _context.UserKyc.Update(kyc);

            return await _context.PushAsync();
        }

        /// <summary>
        /// Upload documents
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UploadDocumentsAsync(IEnumerable<UploadIdentityDocumentViewModel> documents)
        {
            var tasks = documents.Select(document => UploadDocumentAsync(document)).ToList();

            var results = (await Task.WhenAll(tasks)).ToList();
            return results.JoinResults();
        }

        /// <summary>
        /// Get verification documents requests
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<UserKycItem>> GetVerificationDocumentsRequestsWithPaginationAsync(DTParameters parameters)
        {
            var data = await _context.UserKyc
                .Include(x => x.User)
                .Include(x => x.Documents)
                .AsNoTracking()
                .Where(x => x.User != null && x.ValidationState == DocumentValidationState.Pending)
                .GetPagedAsDtResultAsync(parameters);
            return _mapper.Map<DTResult<UserKycItem>>(data);
        }
    }
}