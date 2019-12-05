using System;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Models;
using GR.Documents.Abstractions.ViewModels.DocumentViewModels;
using GR.Files.Abstraction;
using GR.Identity.Abstractions;
using GR.WorkFlows.Abstractions;

namespace GR.Documents
{
    [Author(Authors.LUPEI_NICOLAE, 1.1, "Override methods for init start state of entry")]
    public class DocumentWithWorkflowService : DocumentService, IDocumentServiceWithWorkflow
    {
        #region Injectable

        /// <summary>
        /// Inject workflow executor
        /// </summary>
        protected readonly IWorkFlowExecutorService WorkFlowExecutorService;

        #endregion

        public DocumentWithWorkflowService(IDocumentContext documentContext, IUserManager<ApplicationUser> userManager, IFileManager fileManager, IWorkFlowExecutorService workFlowExecutorService) : base(documentContext, userManager, fileManager)
        {
            WorkFlowExecutorService = workFlowExecutorService;
        }

        
        /// <summary>
        /// Add new document version
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override async Task<ResultModel> AddNewDocumentVersionAsync(AddNewVersionDocumentViewModel model)
        {
            var addNewVersionRequest = await base.AddNewDocumentVersionAsync(model);
            if (!addNewVersionRequest.IsSuccess) return addNewVersionRequest;
            var entryId = (Guid)addNewVersionRequest.Result;
            return await WorkFlowExecutorService.SetStartStateForEntryAsync("DocumentVersion", entryId.ToString());
        }
    }
}
