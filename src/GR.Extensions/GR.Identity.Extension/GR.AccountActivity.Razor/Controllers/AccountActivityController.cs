using System;
using System.Threading.Tasks;
using GR.AccountActivity.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers.Global;
using GR.Core.Razor.BaseControllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.AccountActivity.Razor.Controllers
{
    [Authorize]
    [Author(Authors.LUPEI_NICOLAE)]
    public class AccountActivityController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject service
        /// </summary>
        private readonly IUserActivityService _userActivityService;

        #endregion


        public AccountActivityController(IUserActivityService userActivityService)
        {
            _userActivityService = userActivityService;
        }

        /// <summary>
        /// Account activity
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Non confirmed device view
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> NotConfirmedDevice(Guid? deviceId)
        {
            if (deviceId == null) return NotFound();
            var deviceReq = await _userActivityService.FindDeviceByIdAsync(deviceId);
            if (!deviceReq.IsSuccess) return NotFound();

            if (deviceReq.Result.IsConfirmed) return RedirectToAction("Index", "Home");
            return View(deviceReq.Result);
        }

        /// <summary>
        /// Fail to confirm device
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public IActionResult ErrorToConfirm()
        {
            return View();
        }

        /// <summary>
        /// Device confirmed
        /// </summary>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public IActionResult DeviceConfirmed()
        {
            return View();
        }

        /// <summary>
        /// Confirm device handler
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> ConfirmDevice(Guid deviceId, string code)
        {
            if (string.IsNullOrEmpty(code) || deviceId != Guid.Empty) NotFound();

            var req = await _userActivityService.ConfirmDeviceAsync(deviceId, code);
            return RedirectToAction(req.IsSuccess ? nameof(DeviceConfirmed) : nameof(ErrorToConfirm));
        }
    }
}
