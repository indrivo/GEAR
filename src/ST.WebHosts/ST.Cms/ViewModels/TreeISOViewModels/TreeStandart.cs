using System;
using System.Collections.Generic;

namespace ST.Cms.ViewModels.TreeISOViewModels
{
	public sealed class TreeStandard
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IList<TreeCategory> Categories { get; set; }
	}

	public sealed class TreeCategory
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IList<TreeRequirement> Requirements { get; set; }
		public IList<TreeCategory> SubCategories { get; set; }
		public TreeCategoryAction Actions { get; set; }
	}

	public sealed class TreeCategoryAction
	{
		public int TotalActions { get; set; }
		public int OpenActions { get; set; }
		public int Percentage { get; set; } = 100;
	}

	public sealed class TreeRequirement
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Hint { get; set; }
		public string DueMode { get; set; }
		public IEnumerable<TreeRequirementDocument> Documents { get; set; }
		public IEnumerable<TreeRequirement> Requirements { get; set; }
	}

	public sealed class TreeRequirementDocument
	{
		public Guid Id { get; set; }
		public string FileName { get; set; }
		public string FileLink { get; set; }
		public string DocumentCategory { get; set; }
	}
}
