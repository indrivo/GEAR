using System.Collections.Generic;
using EventBus.Core.Events;
using Permission = ST.Identity.Data.Permissions.PermissionUserToExecute;

namespace ST.Identity.Data.EventBus
{
	public class PermissionEchoRecievedIntegrationEvent : IntegrationEvent
	{
		public IEnumerable<Permission> RequiredPermissions { get; set; }
	}
}