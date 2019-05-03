using System;
using ST.Core;

namespace ST.Identity.Abstractions
{
	public class UserGroup : BaseModel
	{
	    public AuthGroup AuthGroup { get; set; }
		public Guid AuthGroupId { get; set; }
		public ApplicationUser User { get; set; }
		public string UserId { get; set; }
	}
}