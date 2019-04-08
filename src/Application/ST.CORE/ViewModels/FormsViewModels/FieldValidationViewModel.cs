using System.Collections.Generic;
using ST.Configuration.Models;
using ST.Entities.Models.Forms;

namespace ST.CORE.ViewModels.FormsViewModels
{
	public class FieldValidationViewModel
	{
		public Field Field { get; set; }
		public Dictionary<bool, FormValidation> FormValidations { get; set; }
	}
}
