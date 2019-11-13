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
        private static List<Func<IEnumerable<string>, IEnumerable<string>, Guid?, Guid?, Task<bool>>> _customRules = new List<Func<IEnumerable<string>, IEnumerable<string>, Guid?, Guid?, Task<bool>>>();

        /// <summary>
        /// Rule
        /// </summary>
        /// <param name="rule"></param>
        public static void RegisterCustomRule(Func<IEnumerable<string>, IEnumerable<string>, Guid?, Guid?, Task<bool>> rule)
        {
            if (rule != null)
                _customRules.Add(rule);
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
            if (!_customRules.Any()) return true;
            var haveAccess = _customRules.Select(async rule => await rule(permissions, roles, tenantId, userId))
                .Select(x => x.Result)
                .All(x => x);

            return haveAccess;
        }
    }
}
