using GR.Core.Razor.BaseControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GR.WebHooks.Abstractions;
using GR.WebHooks.Abstractions.ViewModels;

namespace GR.WebHooks.Razor.Controllers
{
    [AllowAnonymous]
    public class IncomingWebHookController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject hook service
        /// </summary>
        private readonly IIncomingHookService _hookService;

        #endregion

        public IncomingWebHookController(IIncomingHookService hookService)
        {
            _hookService = hookService;
        }

        /// <summary>
        /// Receive 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Receive(IncomingHookRequestViewModel model)
        {
            var request = await _hookService.ReceiveEventAsync(model, HttpContext);
            return Json(request);
        }
    }
}