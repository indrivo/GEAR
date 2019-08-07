using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ST.Install.Abstractions.Models
{
	public class SyncEcomerceAccountModel
	{
		/// <summary>
		/// Company name
		/// </summary>
		[Required]
		public string CompanyName { get; set; }

		/// <summary>
		/// User
		/// </summary>
		[Required]
		public CommerceUser User { get; set; }

		/// <summary>
		/// Password
		/// </summary>
		[Required]
		public string Password { get; set; }
	}

	public class CommerceUser : IdentityUser<long>
	{
		public const string SettingsDataKey = "Settings";

		public Guid UserGuid { get; set; }

		[Required(ErrorMessage = "The {0} field is required.")]
		[StringLength(450)]
		public string FullName { get; set; }

		public long? VendorId { get; set; }

		public bool IsDeleted { get; set; }

		public DateTimeOffset CreatedOn { get; set; }

		public DateTimeOffset LatestUpdatedOn { get; set; }



		[StringLength(450)]
		public string Culture { get; set; }

		public string ExtensionData { get; set; }


		/// <summary>
		/// Company id
		/// </summary>
		public long? CompanyId { get; set; }

		/// <summary>
		/// Language identifier
		/// </summary>
		public string LanguageIdentifier { get; set; }
	}
}
