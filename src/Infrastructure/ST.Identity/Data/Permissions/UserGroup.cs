using System;
using ST.Entities.Models;

namespace ST.Identity.Data.Permissions
{
	public class UserGroup : ExtendedModel
	{
	    public AuthGroup AuthGroup { get; set; }
		public Guid AuthGroupId { get; set; }
		public UserProfiles.ApplicationUser User { get; set; }
		public string UserId { get; set; }
	}
}