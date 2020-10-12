using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.Attributes;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.IdentityDocuments.Abstractions;
using GR.IdentityDocuments.Abstractions.Enums;
using GR.IdentityDocuments.Abstractions.Models;
using GR.IdentityDocuments.Abstractions.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GR.IdentityDocuments.Api.Controllers
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route(DefaultApiRouteTemplate)]
    public class IdentityDocumentsApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject service
        /// </summary>
        private readonly IIdentityDocumentService _service;

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IIdentityDocumentsContext _context;

        /// <summary>
        /// Inject verification service
        /// </summary>
        private readonly IIdentityVerificationService _identityVerificationService;

        #endregion

        public IdentityDocumentsApiController(IIdentityDocumentService service, IIdentityDocumentsContext context, IIdentityVerificationService identityVerificationService)
        {
            _service = service;
            _context = context;
            _identityVerificationService = identityVerificationService;
        }

        /// <summary>
        /// Get available document types
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [JsonProduces(typeof(ResultModel<IEnumerable<IdentityDocumentType>>))]
        public async Task<JsonResult> GetAvailableDocumentTypes()
            => await JsonAsync(_service.GetAvailableDocumentTypesAsync());

        /// <summary>
        /// Upload document
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(ResultModel))]
        public async Task<JsonResult> UploadDocument(UploadIdentityDocumentViewModel model)
            => await JsonAsync(_service.UploadDocumentAsync(model));

        /// <summary>
        /// Upload documents
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(ResultModel))]
        public async Task<JsonResult> UploadDocuments(IEnumerable<UploadIdentityDocumentViewModel> documents)
            => await JsonAsync(_service.UploadDocumentsAsync(documents));

        /// <summary>
        /// Get incoming verification requests
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        [Admin]
        [JsonProduces(typeof(DTResult<UserKycItem>))]
        public async Task<JsonResult> GetVerificationDocumentsRequestsWithPagination(DTParameters parameters)
            => await JsonAsync(_service.GetVerificationDocumentsRequestsWithPaginationAsync(parameters));

        /// <summary>
        /// Get document image by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDocumentImage([Required] Guid id)
        {
            var document = await _context.IdentityDocuments.FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (document == null) return NotFound();

            return File(document.Blob, document.ContentType);
        }

        /// <summary>
        /// Update user kyc
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="decision"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(ResultModel))]
        public async Task<JsonResult> UpdateUserKycState(Guid userId, DocumentValidationState decision, string reason)
            => await JsonAsync(_identityVerificationService.UpdateUserKycStateAsync(userId, decision, reason));

        /// <summary>
        /// Update doc kyc
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="decision"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(ResultModel))]
        public async Task<JsonResult> UpdateDocumentKycState(Guid documentId, DocumentValidationState decision, string reason)
            => await JsonAsync(_identityVerificationService.UpdateDocumentKycStateAsync(documentId, decision, reason));
    }
}