using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.Notifications.Abstractions;
using GR.Notifications.Razor.ViewModels.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Notifications.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    public class NotificationSubscriptionsController : Controller
    {
        /// <summary>
        /// Inject repository
        /// </summary>
        private readonly INotificationSubscriptionRepository _subscriptionRepository;

        public NotificationSubscriptionsController(INotificationSubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        /// <summary>
        /// Default view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var events = _subscriptionRepository.Events
                .Select(async x =>
                {
                    var template = await _subscriptionRepository.GetEventTemplateAsync(x.EventId);
                    return new NotificationSubscribeGetViewModel
                    {
                        Subject = template?.Result?.Subject,
                        EventId = x.EventId,
                        Template = template?.Result?.Value,
                        EventGroupName = x.EventGroupName,
                        SubscribedRoles = (await _subscriptionRepository.GetRolesSubscribedToEventAsync(x.EventId))
                            .Result
                    };
                }).Select(x => x.Result).OrderBy(x => x.EventGroupName).GroupBy(x => x.EventGroupName);

            var roles = _subscriptionRepository.Roles.OrderBy(x => x.Name);
            ViewBag.Events = events;
            ViewBag.Roles = roles;
            return View();
        }

        /// <summary>
        /// Subscribe to event
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SaveNotificationSubscription([Required]NotificationSubscribeViewModel model)
        {
            if (model == null) return Json(new ResultModel());
            if (!ModelState.IsValid) return Json(new ResultModel());
            var response = await _subscriptionRepository
                .SubscribeRolesToEventAsync(model.Roles, model.Event, model.Template, model.Subject);
            return Json(response);
        }
    }
}