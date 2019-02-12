using System.Collections.Generic;
using System.Linq;
using ST.Identity.Data.Permissions;

namespace ST.Identity
{
	public class PermissionBuilder
	{
		public static PermissionForServiceBuilder WithService(string serviceName)
		{
			return new PermissionForServiceBuilder(serviceName);
		}
	}

	public class PermissionForServiceBuilder
	{
		private readonly string _serviceName;

		public PermissionForServiceBuilder(string serviceName)
		{
			_serviceName = serviceName;
		}

		public PermissionWithServiceAndModuleBuilder WithModule(string moduleName)
		{
			return new PermissionWithServiceAndModuleBuilder(_serviceName, moduleName);
		}
	}

	public class PermissionWithServiceAndModuleBuilder
	{
		private readonly string _moduleName;
		private readonly string _serviceName;
		public PermissionWithServiceAndModuleBuilder(string serviceName, string moduleName)
		{
			_serviceName = serviceName;
			_moduleName = moduleName;
		}

		public IEnumerable<PermissionUserToExecute> BuildPermissions(params string[] actionNames)
		{
			return actionNames.Select(action => $"{_serviceName}:{_moduleName}:{action}")
				.Select(PermissionUserToExecute.ToPermissionObject);
		}

		public IEnumerable<PermissionUserToExecute> BuildPermissionsWithDescriptions(
			params KeyValuePair<string, string>[] actionsAndDescriptions)
		{
			return actionsAndDescriptions.Select(f => new PermissionUserToExecute
            {
				Service = _serviceName,
				Module = _moduleName,
				Action = f.Key,
				Description = f.Value
			});
		}
	}

	public class ServiceDefinition
	{
		private const string ServiceName = "Identity";


				public ServiceDefinition()
				{
					var builder = new PermissionBuilder();
					var dict = new Dictionary<string, string>
					{
						["Create"] = "Create new user in the system",
						["Read"] = "View a user's personal info",
						["Update"] = "Update user's personal info",
						["Delete"] = "Delete a user from the system"
					};
					var permis = PermissionBuilder.WithService(ServiceName)
						.WithModule("Users")
						.BuildPermissionsWithDescriptions(dict.ToArray());
				}

				public static IEnumerable<PermissionUserToExecute> SupportedPermissions =>
							new List<PermissionUserToExecute>
			{
				new PermissionUserToExecute
                {
					Service = ServiceName,
					Module = "Users",
					Action = "Create",
					Description = "Create a new User in the system"
				},
				new PermissionUserToExecute
                {
					Service = ServiceName,
					Module = "Users",
					Action = "Delete",
					Description = "Deletes a user from the system"
				},
				new PermissionUserToExecute
                {
					Service = ServiceName,
					Module = "Users",
					Action = "Edit",
					Description = "Edit a selected user's personal information"
				}
			};
	}
}