using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ST.Entities.Data;
using ST.Identity.Abstractions;

namespace ST.Cms.ViewModels.FilesViewModels
{
	public class DocumentCreateUpdateViewModel : Document
	{
		public IEnumerable<ApplicationUser> AvailableUsers { get; set; }
		public IFormFile FileBlob { get; set; }
	}
}
