using ST.Identity.Abstractions;
using ST.Identity.Data.Permissions;

namespace ST.Identity.Razor.ViewModels.RoleViewModels
{
	public class RoleListViewModel : ApplicationRole
	{
		public string ClientName { get; set; }
	}
}
