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
        private readonly INotify<ApplicationRole> _notify;

        #endregion

        public SendNotificationAction(Transition transition, IEnumerable<Transition> nextTransitions) : base(transition, nextTransitions)
        {
            _notify = IoC.Resolve<INotify<ApplicationRole>>();
        }

        /// <summary>
        /// Execute data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task InvokeExecuteAsync(Dictionary<string, object> data)
        {
            var rolesForPrevTransition = await Executor.GetAllowedRolesToTransitionAsync(CurrentTransition);
            await _notify.SendNotificationAsync(rolesForPrevTransition, new Notification
            {
                Subject = "Workflow state changed",
                Content = $"Entry x has changed its status to {CurrentTransition?.FromState.Name}",
                SendLocal = true,
                SendEmail = true,
                NotificationTypeId = NotificationType.Info
            }, null);

            foreach (var nextTransition in NextTransitions)
            {
                var rolesForNextTransition = await Executor.GetAllowedRolesToTransitionAsync(nextTransition);
                await _notify.SendNotificationAsync(rolesForNextTransition, new Notification
                {
                    Subject = "Workflow state changed",
                    Content = "Entry x was ",
                    SendLocal = true,
                    SendEmail = true,
                    NotificationTypeId = NotificationType.Info
                }, null);
            }
        }
    }
}
