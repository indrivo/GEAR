using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Entities.Models;
using ST.Entities.Models.KPI;
using ST.Entities.Models.Nomenclator;
using ST.Entities.Models.Request;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;

namespace ST.CORE.Models.RoleViewModels
{
	public class UpdateRequestViewModel : Request
	{
		public IEnumerable<NomenclatorItem> InterestedParts { get; set; }
		public IEnumerable<NomenclatorItem> Requests { get; set; }
	

		[Display(Name = "Interested Part")]
		public string SelectedInterestedPart { get; set; }
		[Display(Name = "Request")]
		public string SelectedRequest { get; set; }
		
		
	}
}