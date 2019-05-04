using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Entities.Models.Pages;

namespace ST.PageRender.Razor.ViewModels.PageViewModels
{
	public class CreateBlockViewModel : Block
	{
		public IEnumerable<BlockCategory> BlockCategories { get; set; }
		[Required]
		public override string Name { get; set; }
		[Required]
		public override Guid BlockCategoryId { get; set; }
	}
}
