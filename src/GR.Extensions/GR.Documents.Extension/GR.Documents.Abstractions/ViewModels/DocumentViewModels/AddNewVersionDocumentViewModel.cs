using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

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
