using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.BaseRepository;
using ST.Entities.Models.Pages;

namespace ST.CORE.ViewModels.Pages
{
	public class PageViewModel : Page
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Title { get; set; }
		[Required]
		public new Guid PageTypeId { get; set; }
		[Required]
		public new string Path { get; set; }
		public IEnumerable<PageType> PageTypes { get; set; }
		public IEnumerable<Page> Layouts { get; set; }
		public string Description { get; set; }
	}
}
