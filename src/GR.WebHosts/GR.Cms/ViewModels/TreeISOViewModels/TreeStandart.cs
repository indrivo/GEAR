using System;
using System.Collections.Generic;
using System.Linq;

namespace GR.Cms.ViewModels.TreeISOViewModels
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
		public string Number { get; set; }
		public string Name { get; set; }
		public IList<TreeRequirement> Requirements { get; set; }
		public IList<TreeCategory> SubCategories { get; set; }

		/// <summary>
		/// Actions for current category
		/// </summary>
		public TreeCategoryAction CategoryActions { get; set; }

		/// <summary>
		/// Actions for current and childs
		/// </summary>
		public TreeCategoryAction Actions
		{
			get
			{
				var response = new TreeCategoryAction
				{
					OpenActions = CategoryActions.OpenActions +
								  SubCategories.Sum(x => x.CategoryActions.OpenActions),
					ClosedActions = CategoryActions.ClosedActions +
									SubCategories.Sum(x => x.CategoryActions.ClosedActions)
				};
				return response;
			}
		}
	}

	public sealed class TreeCategoryAction
	{
		public int TotalActions => ClosedActions + OpenActions;
		public int OpenActions { get; set; } = 0;
		public int ClosedActions { get; set; } = 0;

		/// <summary>
		/// Get percentage of completion
		/// </summary>
		public int Percentage
		{
			get
			{
				//For exclude exception divide to 0
				if (TotalActions == 0) return 100;
				return OpenActions * 100 / TotalActions;
			}
		}
	}

	public sealed class TreeRequirement
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Hint { get; set; }
		public RequirementDueMode RequirementDueMode { get; set; }
		public IEnumerable<TreeRequirementDocument> Documents { get; set; }
		public IEnumerable<TreeRequirement> Requirements { get; set; }
	}

	public sealed class RequirementDueMode
	{
		public Guid? DueModeId { get; set; }
		public string DueModeValue { get; set; }
	}

	//TODO: Integrate with document library after develop document lib
	public sealed class TreeRequirementDocument
	{
		public Guid Id { get; set; }
		public string FileName { get; set; }
		public string FileLink { get; set; }
		public string DocumentCategory { get; set; }
	}
}