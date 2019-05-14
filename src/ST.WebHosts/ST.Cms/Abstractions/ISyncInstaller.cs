using System.Threading.Tasks;
using ST.Cms.ViewModels.InstallerModels;
using ST.Identity.Abstractions;
using ST.Identity.Data.MultiTenants;

namespace ST.Cms.Abstractions
{
	public interface ISyncInstaller
	{
		Task RegisterCommerceUser(ApplicationUser user, Tenant tenant, SyncEcommerceAccountViewModel data);
	}
}
