using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Identity.Permissions.Abstractions.Configurators
{
    public static class PermissionCustomRules
    {
        /// <summary>
        /// Custom rules
        /// </summary>
        private static readonly List<Func<IEnumerable<string>, IEnumerable<string>, Guid?, Guid?, Task<bool>>> CustomRules = new List<Func<IEnumerable<string>, IEnumerable<string>, Guid?, Guid?, Task<bool>>>();

        /// <summary>
        /// Rule
        /// </summary>
        /// <param name="rule"></param>
        public static void RegisterCustomRule(Func<IEnumerable<string>, IEnumerable<string>, Guid?, Guid?, Task<bool>> rule)
        {
            if (rule != null)
                CustomRules.Add(rule);
        }

        /// <summary>
        /// Check access
        /// </summary>
        /// <param name="permissions"></param>
        /// <param name="roles"></param>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool ExecuteRulesAndCheckAccess(IEnumerable<string> permissions, IEnumerable<string> roles, Guid? tenantId, Guid? userId)
        {
            if (!CustomRules.Any()) return true;
            var haveAccess = CustomRules.Select(async rule => await rule(permissions, roles, tenantId, userId))
                .Select(x => x.Result)
                .All(x => x);

            return haveAccess;
        }
    }
}