using System;
using ST.Core.Events;
using ST.MultiTenant.Abstractions.Events.EventArgs;

namespace ST.MultiTenant.Abstractions.Events
{
    public static class TenantEvents
    {
        public struct Company
        {
            #region Events

            /// <summary>
            /// On user log in
            /// </summary>
            public static event EventHandler<CompanyRegisterEventArgs> OnCompanyRegistered;

            #endregion

            #region Triggers

            /// <summary>
            /// User log in
            /// </summary>
            /// <param name="e"></param>
            public static void CompanyRegistered(CompanyRegisterEventArgs e) => SystemEvents.InvokeEvent(null, OnCompanyRegistered, e, nameof(OnCompanyRegistered));

            #endregion
        }

        /// <summary>
        /// Register app events
        /// </summary>
        public static void RegisterEvents()
        {
            //register events on global storage
            SystemEvents.Common.RegisterEventGroup(nameof(Company), SystemEvents.GetEvents(typeof(Company)));
        }
    }
}
