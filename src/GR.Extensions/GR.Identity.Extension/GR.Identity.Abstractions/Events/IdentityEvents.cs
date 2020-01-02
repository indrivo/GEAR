using GR.Core.Events;
using GR.Identity.Abstractions.Events.EventArgs.Authorization;
using GR.Identity.Abstractions.Events.EventArgs.Users;
using System;

namespace GR.Identity.Abstractions.Events
{
    public static class IdentityEvents
    {
        public struct Authorization
        {
            #region Events

            /// <summary>
            /// On user log in
            /// </summary>
            public static event EventHandler<UserLogInEventArgs> OnUserLogIn;

            /// <summary>
            /// On user log out
            /// </summary>
            public static event EventHandler<UserLogOutEventArgs> OnUserLogout;

            #endregion Events

            #region Triggers

            /// <summary>
            /// User log in
            /// </summary>
            /// <param name="e"></param>
            public static void UserLogin(UserLogInEventArgs e) => SystemEvents.InvokeEvent(null, OnUserLogIn, e, nameof(OnUserLogIn));

            /// <summary>
            /// User log out
            /// </summary>
            /// <param name="e"></param>
            public static void UserLogout(UserLogOutEventArgs e) => SystemEvents.InvokeEvent(null, OnUserLogout, e, nameof(OnUserLogout));

            #endregion Triggers
        }

        public struct Users
        {
            #region Events

            public static event EventHandler<UserCreatedEventArgs> OnUserCreated;

            public static event EventHandler<UserDeleteEventArgs> OnUserDeleted;

            public static event EventHandler<UserUpdatedEventArgs> OnUserUpdated;

            public static event EventHandler<UserChangePasswordEventArgs> OnUserPasswordChange;

            public static event EventHandler<UserForgotPasswordEventArgs> OnUserForgotPassword;

            public static event EventHandler<UserEnablingEventArgs> OnUserEnabling;

            public static event EventHandler<UserEmailConfirmEventArgs> OnUserEmailConfirm;

            #endregion Events

            #region Triggers

            public static void UserCreated(UserCreatedEventArgs e) => SystemEvents.InvokeEvent(null, OnUserCreated, e, nameof(OnUserCreated));

            public static void UserUpdated(UserUpdatedEventArgs e) => SystemEvents.InvokeEvent(null, OnUserUpdated, e, nameof(OnUserUpdated));

            public static void UserDelete(UserDeleteEventArgs e) => SystemEvents.InvokeEvent(null, OnUserDeleted, e, nameof(OnUserDeleted));

            public static void UserPasswordChange(UserChangePasswordEventArgs e) => SystemEvents.InvokeEvent(null, OnUserPasswordChange, e, nameof(OnUserPasswordChange));

            public static void UserForgotPassword(UserForgotPasswordEventArgs e) => SystemEvents.InvokeEvent(null, OnUserForgotPassword, e, nameof(OnUserForgotPassword));

            public static void UserEnabling(UserEnablingEventArgs e) => SystemEvents.InvokeEvent(null, OnUserEnabling, e, nameof(OnUserEnabling));

            public static void UserEmailConfirm(UserEmailConfirmEventArgs e) => SystemEvents.InvokeEvent(null, OnUserEmailConfirm, e, nameof(OnUserEmailConfirm));

            #endregion Triggers
        }

        /// <summary>
        /// Register app events
        /// </summary>
        public static void RegisterEvents()
        {
            Authorization.OnUserLogIn += delegate { };
            Authorization.OnUserLogout += delegate { };
            Users.OnUserCreated += delegate { };
            //register events on global storage
            SystemEvents.Common.RegisterEventGroup(nameof(Authorization), SystemEvents.GetEvents(typeof(Authorization)));
            SystemEvents.Common.RegisterEventGroup(nameof(Users), SystemEvents.GetEvents(typeof(Users)));
        }
    }
}