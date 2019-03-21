using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Entities.Models.Nomenclator;
using ST.Entities.Models.Requirement;

namespace ST.CORE.ViewModels.RequirementViewModels
{
	public class UpdateRequirementViewModel : Requirement
	{
		public IEnumerable<NomenclatorItem> InterestedParts { get; set; }
		public IEnumerable<NomenclatorItem> Requirement { get; set; }
	

		[Display(Name = "Interested Part")]
		public string SelectedInterestedPart { get; set; }
		[Display(Name = "Requirement")]
		public string SelectedRequirement { get; set; }
		[Display(Name = "Interested Part Type")]
		public string InterestedPartType { get; set; }


	}
}