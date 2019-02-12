using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ST.Identity.Data.Permissions;

namespace ST.Identity.Data.UserProfiles
{

	public class RoleProfile 
	{

		public ApplicationRole ApplicationRole { get; set; } 
		public Profile Profile { get; set; }

		[Key]
		[Column(Order = 1)]
		public Guid ProfileId { get; set; }

		[Key]
		[Column(Order = 2)]
		public string ApplicationRoleId { get; set; }

	}
}
