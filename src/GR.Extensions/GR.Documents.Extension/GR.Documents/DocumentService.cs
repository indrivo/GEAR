using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Documents.Abstractions;
using GR.Documents.Abstractions.Models;
using GR.Identity.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GR.Documents
{
    public class DocumentService
    {
        #region Injectable
            /// <summary>
            /// Inject db context 
            /// </summary>
            private IDocumentContext _context;

            /// <summary>
            /// Inject user manager
            /// </summary>
            private readonly IUserManager<ApplicationUser> _userManager;

        #endregion

        public DocumentService(IDocumentContext context, IUserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        

        /// <summary>
        /// Get document by id
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Document>> GetDocumentsByIdAsync(Guid? documentId)
        {

            var response = new ResultModel<Document>();
            if (documentId is null)
                return new InvalidParametersResultModel<Document>();

            var document = await _context.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .ThenInclude(i => i.FileStorage)
                .FirstOrDefaultAsync(x => x.Id == documentId);

            if (document is null) new NotFoundResultModel<Document>();

            response.IsSuccess = true;
            response.Result = document;
            return response;

        }


        /// <summary>
        /// Get document by current user 
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Document>>> GetDocumentsByUserAsync()
        {
            var response = new ResultModel<IEnumerable<Document>>();
            var user = await _userManager.GetCurrentUserAsync();

            if(user is null)
                return new InvalidParametersResultModel<IEnumerable<Document>>();


            var listDocuments = await _context.Documents
                .Include(i => i.DocumentType)
                .Include(i => i.DocumentVersions)
                .ThenInclude(i => i.FileStorage)
                .Where(x => x.UserId == user.Result.Id.ToGuid()).ToListAsync();

            if (listDocuments is null || !listDocuments.Any()) new NotFoundResultModel<IEnumerable<Document>>();

            response.IsSuccess = true;
            response.Result = listDocuments;

            return response;
        }

    }
}
