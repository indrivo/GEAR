using Microsoft.AspNetCore.Http;

namespace GR.IdentityDocuments.Abstractions.ViewModels
{
    public class UploadIdentityDocumentViewModel
    {
        /// <summary>
        /// File
        /// </summary>
        public virtual IFormFile File { get; set; }

        /// <summary>
        /// Document type
        /// </summary>
        public virtual string Type { get; set; }
    }
}