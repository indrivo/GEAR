using System.ComponentModel.DataAnnotations;
using GR.Core.Helpers.ConnectionStrings;

namespace GR.Install.Abstractions.Models
{
	public class SetupModel
	{
		/// <summary>
		/// Site name
		/// </summary>
		[Required]
		public string SiteName { get; set; }

		/// <summary>
		/// Database provider
		/// </summary>
		[Required]
		public DbProviderType DataBaseType { get; set; }

		/// <summary>
		/// Database connection
		/// </summary>
		[Required]
		public string DatabaseConnectionString { get; set; }

		/// <summary>
		/// Organization settings
		/// </summary>
		public SetupOrganizationViewModel Organization { get; set; }

		/// <summary>
		/// Super admin profile info
		/// </summary>
		public SetupProfileModel SysAdminProfile { get; set; }
	}
}
