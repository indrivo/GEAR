using System;
using System.Linq;
using GR.Core;
using GR.Core.Helpers;
using GR.Identity.Abstractions.Helpers;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Permissions.Abstractions.Configurators;
using GR.MultiTenant.Abstractions;
using GR.Subscriptions.Abstractions.Models;

namespace GR.Subscriptions.Abstractions.Helpers
{
    public static class SubscriptionRules
    {
        /// <summary>
        /// Register rule for max users on company
        /// </summary>
        public static void RegisterLimitNumberOfUsersRule()
        {
            PermissionCustomRules.RegisterCustomRule(async (permissions, roles, tenant, userId) =>
            {
                if (roles.Contains(GlobalResources.Roles.ADMINISTRATOR)) return true;
                if (!permissions.Contains(UserPermissions.UserCreate)) return true;
                if (tenant == null) return false;

                bool grant;
                var organizationService = IoC.Resolve<IOrganizationService<Tenant>>();
                var subscriptionService = IoC.Resolve<ISubscriptionService<Subscription>>();
                var companyAdminRequest = await organizationService.GetCompanyAdministratorByTenantIdAsync(tenant);
                if (!companyAdminRequest.IsSuccess) grant = false;
                else
                {
                    var subscriptionsRequest = await subscriptionService.GetValidSubscriptionsForUserAsync(companyAdminRequest.Result.Id);
                    if (!subscriptionsRequest.IsSuccess) grant = false;
                    else
                    {
                        var subscriptions = subscriptionsRequest.Result.ToList();
                        grant = false;
                        var maxUsers = 0;
                        foreach (var subscription in subscriptions)
                        {
                            var allowedUsers = subscription.SubscriptionPermissions.FirstOrDefault(x => x.Name.Equals("Number of users"))?.Value;
                            if (allowedUsers == null) continue;
                            try
                            {
                                var currentMaxUsers = Convert.ToInt32(allowedUsers);
                                if (currentMaxUsers > maxUsers) maxUsers = currentMaxUsers;
                            }
                            catch
                            {
                                //Ignore
                            }
                        }

                        if (maxUsers > 0)
                        {
                            var totalExistentUsers = organizationService.GetAllowedUsersByOrganizationId(tenant.Value).Count();
                            grant = totalExistentUsers < maxUsers;
                        }
                    }
                }

                return grant;
            });
        }
    }
}
