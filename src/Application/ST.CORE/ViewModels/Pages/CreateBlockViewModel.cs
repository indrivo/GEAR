using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ST.Entities.Models.Pages;

namespace ST.CORE.ViewModels.Pages
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
