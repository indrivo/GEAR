using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using GR.Core.Extensions;
using GR.Email.Abstractions;
using GR.Email.Abstractions.Models.EmailViewModels;

namespace GR.Email.Razor.Controllers
{
    [Authorize]
    public class EmailController : Controller
    {
        /// <summary>
        /// Inject configurations
        /// </summary>
        private readonly IOptions<EmailSettingsViewModel> _options;

        private readonly IEmailSender _emailSender;

        public EmailController(IOptions<EmailSettingsViewModel> options, IEmailSender emailSender)
        {
            _options = options;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Get view with email settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EmailSettings()
        {
            return View(_options.Value);
        }

        /// <summary>
        /// Update email settings
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EmailSettings(EmailSettingsViewModel model)
        {
            var result = _emailSender.ChangeSettings(model);
            if (result.IsSuccess) return View();
            ModelState.AppendResultModelErrors(result.Errors);
            return View(model);
        }
    }
}