using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Configuration.Models;
using ST.Entities.Models.Forms;

namespace ST.CORE.ViewModels.FormsViewModels
{
	public class FieldValidationViewModel
	{
		/// <summary>
		/// Field data
		/// </summary>
		[Required]
		public Field Field { get; set; }

		/// <summary>
		/// Form field validations
		/// </summary>
		[Required]
		public IList<FormValidation> FormValidations { get; set; }
	}
}
