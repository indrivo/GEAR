using System.ComponentModel.DataAnnotations;

namespace GR.Cms.ViewModels.InstallerModels
{
	public class SetupOrganizationViewModel
	{
		/// <summary>
		/// Name of organization
		/// </summary>
		[Required]
		public  string Name { get; set; }

		/// <summary>
		/// The Url address of website
		/// </summary>
		public string SiteWeb { get; set; }
	}
}
