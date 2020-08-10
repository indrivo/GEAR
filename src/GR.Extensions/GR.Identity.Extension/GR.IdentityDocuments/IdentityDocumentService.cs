using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.IdentityDocuments.Abstractions;
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
            Logger.LogInformation("User: {User} start to upload document {DocType}", userId, model.Type);
            var existentDocument = await _context.IdentityDocuments.FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.DocumentType.Equals(model.Type));
            if (existentDocument == null)
            {
                var document = new IdentityDocument
                {
                    UserId = userId,
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

                _context.IdentityDocuments.Update(existentDocument);
            }

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
    }
}
