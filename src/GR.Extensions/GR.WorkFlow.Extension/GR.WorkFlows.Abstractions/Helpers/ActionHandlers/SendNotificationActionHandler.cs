using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using GR.WorkFlows.Abstractions.Models;

namespace GR.WorkFlows.Abstractions.Helpers.ActionHandlers
{
    public class SendNotificationAction : BaseWorkFlowAction
    {
        #region Injectable

        /// <summary>
        /// Inject notifier
        /// </summary>
        private readonly INotify<GearRole> _notify;

        #endregion

        public SendNotificationAction(EntryState entry, Transition transition, IEnumerable<Transition> nextTransitions) : base(entry, transition, nextTransitions)
        {
            _notify = IoC.Resolve<INotify<GearRole>>();
        }

        /// <summary>
        /// Execute data
        /// </summary> 
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task InvokeExecuteAsync(Dictionary<string, string> data)
        {
            var rolesForPrevTransition = await Executor.GetAllowedRolesToTransitionAsync(CurrentTransition);
            var subject = "Entry x";
            if (data.ContainsKey("Name")) subject = data["Name"];
            await _notify.SendNotificationAsync(rolesForPrevTransition, new Notification
            {
                Subject = $"{subject} state changed",
                Content = $"{subject} has changed its status from {CurrentTransition?.FromState?.Name} to {CurrentTransition?.ToState?.Name}",
                SendLocal = true,
                SendEmail = true,
                NotificationTypeId = NotificationType.Info
            }, EntryState.TenantId);

            foreach (var nextTransition in NextTransitions)
            {
                var rolesForNextTransition = await Executor.GetAllowedRolesToTransitionAsync(nextTransition);

                await _notify.SendNotificationAsync(rolesForNextTransition, new Notification
                {
                    Subject = "You have new actions",
                    Content = $"{subject} can be switched to {nextTransition?.ToState.Name} state",
                    SendLocal = true,
                    SendEmail = true,
                    NotificationTypeId = NotificationType.Info
                }, EntryState.TenantId);
            }
        }
    }
}
