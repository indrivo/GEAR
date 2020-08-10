using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.Attributes;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.IdentityDocuments.Abstractions;
using GR.IdentityDocuments.Abstractions.Models;
using GR.IdentityDocuments.Abstractions.ViewModels;
using Microsoft.AspNetCore.Mvc;

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

        #endregion

        public IdentityDocumentsApiController(IIdentityDocumentService service)
        {
            _service = service;
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
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(ResultModel))]
        public async Task<JsonResult> UploadDocuments(IEnumerable<UploadIdentityDocumentViewModel> model)
            => await JsonAsync(_service.UploadDocumentsAsync(model));
    }
}