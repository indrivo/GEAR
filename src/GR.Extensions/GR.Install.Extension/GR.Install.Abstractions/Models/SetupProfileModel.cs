using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Install.Abstractions.Models
{
	public class SetupProfileModel
	{
		/// <summary>
		/// User Name of sys admin
		/// </summary>
		[Required]
		public string UserName { get; set; }

		/// <summary>
		/// Email
		/// </summary>
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		/// <summary>
		/// Password
		/// </summary>
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		/// <summary>
		/// Confirm password
		/// </summary>
		[Required]
		[DataType(DataType.Password)]
		[Compare("Password")]
		public string ConfirmPassword { get; set; }

		/// <summary>
		/// First Name of system admin
		/// </summary>
		[Required]
		public string FirstName { get; set; }

		/// <summary>
		/// Last Name of system admin
		/// </summary>
		[Required]
		public string LastName { get; set; }

		/// <summary>
		/// Gender of system admin
		/// </summary>
		[Required]
		public Gender Gender { get; set; }

		/// <summary>
		/// Birth date of system admin
		/// </summary>
		public DateTime BirthDate { get; set; }

		/// <summary>
		/// Street address
		/// </summary>
		public string Street { get; set; }

		/// <summary>
		/// State 
		/// </summary>
		public string State { get; set; }

		/// <summary>
		/// Postal code
		/// </summary>
		public string PostalCode { get; set; }

		/// <summary>
		/// City
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// Country
		/// </summary>
		public string Country { get; set; }
	}

	public enum Gender
	{
		Male, Female
	}
}
