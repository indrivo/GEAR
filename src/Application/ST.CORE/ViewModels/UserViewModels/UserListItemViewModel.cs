using System.Collections.Generic;

namespace ST.CORE.ViewModels.UserViewModels
{
	public class UserListItemViewModel
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string CreatedBy { get; set; }
		public string CreatedDate { get; set; }
		public string ModifiedBy { get; set; }
		public string Changed { get; set; }
		public IList<string> Roles { get; set; }
		public int Sessions { get; set; }
		public string AuthenticationType { get; set; }
		public string Organization { get; set; }
	}
}