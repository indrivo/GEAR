using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Entities.Abstractions.Models;
using GR.Forms.Abstractions.Models.FormModels;

namespace GR.Forms.Razor.ViewModels.FormsViewModels
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
