using System;
using System.Threading.Tasks;
using GR.Subscriptions.Abstractions;
using GR.Subscriptions.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Subscriptions.Razor.Controllers
{
    [Authorize]
    public sealed class SubscriptionController : Controller
    {
        #region Injectable

        /// <summary>
        /// Subscription service
        /// </summary>
        private readonly ISubscriptionService<Subscription> _subscriptionService;

        #endregion


        public SubscriptionController(ISubscriptionService<Subscription> subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<JsonResult> GetSubscription(Guid? id)
        {
            return Json(await _subscriptionService.GetSubscriptionByIdAsync(id));
        }

        /// <summary>
        /// Get subscription by user
        /// </summary>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<JsonResult> GetSubscriptions()
        {
            return Json(await _subscriptionService.GetSubscriptionsByUserAsync());
        }

        /// <summary>
        /// Has valid subscriptions
        /// </summary>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public async Task<JsonResult> HasValidSubscriptions()
        {
            return Json(await _subscriptionService.HasValidSubscription());
        }
    }
}
