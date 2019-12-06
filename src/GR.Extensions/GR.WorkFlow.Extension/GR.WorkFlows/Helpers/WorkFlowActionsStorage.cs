using System;
using System.Collections.Generic;
using System.Linq;
using GR.Core.Extensions;

namespace GR.WorkFlows.Helpers
{
    public static class WorkFlowActionsStorage
    {
        /// <summary>
        /// Storage
        /// </summary>
        private static readonly Dictionary<string, Type> Storage = new Dictionary<string, Type>();

        /// <summary>
        /// Get action type
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static Type GetActionType(string className)
        {
            var search = Storage.FirstOrDefault(x => x.Key.Equals(className));
            return search.IsNull() ? null : search.Value;
        }

        /// <summary>
        /// Add new action type
        /// </summary>
        /// <param name="className"></param>
        /// <param name="actionType"></param>
        public static void AppendActionType(string className, Type actionType)
        {
            if (Storage.ContainsKey(className)) return;
            Storage.Add(className, actionType);
        }
    }
}
