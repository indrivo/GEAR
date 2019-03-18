using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Entities.Models;
using ST.Entities.Models.KPI;
using ST.Entities.Models.Nomenclator;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;

namespace ST.CORE.Models.RoleViewModels
{
	public class UpdateKPIViewModel: KPI
	{
		public IEnumerable<NomMeasurement> MeasurementUnits { get; set; }
		public IEnumerable<NomPeriod> NomPeriod { get; set; }
		public IEnumerable<NomKPICategory> NomKPICategory { get; set; }
		public IEnumerable<NomFulfillment> NomFulfillment { get; set; }

		[Display(Name = "Measurement Unit")]
		public string SelectedMeasurementUnit { get; set; }
		[Display(Name = "Fulfillment Criterion")]
		public string SelectedFulfillmentCriterion { get; set; }
		[Display(Name = "Period")]
		public string SelectedPeriod { get; set; }
		[Display(Name = "Category")]
		public string SelectedCategory { get; set; }
		
	}
}


