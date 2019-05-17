using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Entities.Abstractions.Models;
using ST.Forms.Abstractions.Models.FormModels;

namespace ST.Forms.Razor.ViewModels.FormsViewModels
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
