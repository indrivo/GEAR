using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;
using System.Text;

namespace GR.Documents.Abstractions.ViewModels.DocumentViewModels
{
    public class AddNewVersionDocumentViewModel
    {
        [Required]
        public Guid DocumentId { get; set; }
        public string Comments { get; set; }
        [Required]
        public IFormFile File { get; set; }
       
        public bool IsMajorVersion { get; set; }

    }
}
