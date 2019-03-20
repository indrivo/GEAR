using System;
using System.Collections.Generic;

namespace ST.CORE.ViewModels.TreeISO
{
	public class TreeStandard
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IList<TreeCategory> Categories { get; set; }
	}

	public class TreeCategory
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IList<TreeRequirement> Requirements { get; set; }
		public IList<TreeCategory> SubCategories { get; set; }
	}

	public class TreeRequirement
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IEnumerable<TreeRequirement> Requirements { get; set; }
	}
}
