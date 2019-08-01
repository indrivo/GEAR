using System;

namespace ST.Install.Abstractions.Models
{
	public class CommerceSyncResultModel
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
