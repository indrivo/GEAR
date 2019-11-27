using GR.Documents.Abstractions.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GR.Documents.Abstractions.ViewModels.DocumentViewModels
{
    public class AddDocumentViewModel
    {
        
        public string DocumentCode { get; set; }
        [Required]
        public string Tile { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        [Required]
        public Guid DocumentTypeId { get; set; }
        public IFormFile File { get; set; }
        public string Url { get; set; }
        public string Comments { get; set; }
        public IEnumerable<SelectListItem> ListDocumentTypes { get; set; } = new List<SelectListItem>();
    }
}
