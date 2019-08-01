using System.Threading.Tasks;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.Install.Abstractions.Models;

namespace ST.Install.Abstractions
{
	public interface ISyncInstaller
	{
		Task RegisterCommerceUser(ApplicationUser user, Tenant tenant, SyncEcomerceAccountModel data);
	}
}
