using System;

namespace ST.Cms.ViewModels.InstallerModels
{
	public class CommerceSyncResultViewModel
	{
		/// <summary>
		/// Tenant id
		/// </summary>
		public Guid TenantId { get; set; }

		/// <summary>
		/// Tenant machine name
		/// </summary>
		public string TenantMachineName { get; set; }
	}
}
