using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace GR.Documents.Abstractions.ViewModels.DocumentViewModels
{
    public class AddNewVersionDocumentViewModel
    {

        public Guid DocumentId { get; set; }
        public string Comments { get; set; }
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public bool IsMajorVersion { get; set; }

    }
}
