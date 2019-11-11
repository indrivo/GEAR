using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Subscriptions.Abstractions;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions.Abstractions.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GR.Subscriptions.Razor.Controllers
{

    //[Route("api/[controller]/[action]")]
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

        // GET: /<controller>/
        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        //[HttpGet("/Subscription")]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Get subscription by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetSubscription(Guid id)
        {
            return Json((await _subscriptionService.GetSubscriptionByIdAsync(id)).Result);
        }

        /// <summary>
        /// Get subscription by user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetSubscription()
        {
            return Json((await _subscriptionService.GetSubscriptionByUserAsync()).Result);
        }

        /// <summary>
        /// Has valid subscription
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> HasValidsSubscription()
        {
            return Json((await _subscriptionService.HasValidsSubscription()).Result);
        }


        /// <summary>
        /// Has valid subscription
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddSubscription(SubscriptionViewModel model)
        {
            return Json((await _subscriptionService.CreateSubscriptionAsync(model)).Result);
        }

    }
}
