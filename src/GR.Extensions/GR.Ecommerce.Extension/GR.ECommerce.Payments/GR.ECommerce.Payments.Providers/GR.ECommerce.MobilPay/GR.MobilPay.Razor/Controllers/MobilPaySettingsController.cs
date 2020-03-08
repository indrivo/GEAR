using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Abstractions;
using GR.Core.Extensions;
using GR.MobilPay.Abstractions.Models;
using GR.MobilPay.Razor.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.MobilPay.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    [AutoValidateAntiforgeryToken]
    public class MobilPaySettingsController : Controller
    {
        #region Injectable
        /// <summary>
        /// Inject mobil pay options
        /// </summary>

        private readonly IWritableOptions<MobilPayConfiguration> _writableOptions;

        #endregion

        public MobilPaySettingsController(IWritableOptions<MobilPayConfiguration> writableOptions)
        {
            _writableOptions = writableOptions;
        }

        /// <summary>
        /// Mobil pay configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var model = _writableOptions.Value.Adapt<MobilPayConfigurationViewModel>();
            return View(model);
        }

        /// <summary>
        /// Save mobilPay settings
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index([Required]MobilPayConfigurationViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (!model.PrivateCertificate.FileName.Contains(model.Signature))
            {
                ModelState.AddModelError(string.Empty, "Invalid private key");
                return View(model);
            }

            if (!model.PublicCertificate.FileName.Contains(model.Signature))
            {
                ModelState.AddModelError(string.Empty, "Invalid private key");
                return View(model);
            }

            var rootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var privateKeyPath = Path.Combine(rootPath, model.PrivateCertificate.FileName);
            if (!_writableOptions.Value.PathToPrivateKey.IsNullOrEmpty())
            {
                if (System.IO.File.Exists(privateKeyPath))
                    System.IO.File.Delete(privateKeyPath);
            }

            using (var stream = System.IO.File.Create(privateKeyPath))
            {
                await model.PrivateCertificate.CopyToAsync(stream);
            }

            var publicKeyPath = Path.Combine(rootPath, model.PublicCertificate.FileName);

            if (!_writableOptions.Value.PathToCertificate.IsNullOrEmpty())
            {
                if (System.IO.File.Exists(publicKeyPath))
                    System.IO.File.Delete(publicKeyPath);
            }
            using (var stream = System.IO.File.Create(publicKeyPath))
            {
                await model.PublicCertificate.CopyToAsync(stream);
            }

            _writableOptions.Update(options =>
            {
                options.Signature = model.Signature;
                options.IsSandbox = model.IsSandbox;
                options.PathToCertificate = model.PublicCertificate.FileName;
                options.PathToPrivateKey = model.PrivateCertificate.FileName;
            });
            ViewData["isSuccess"] = true;
            return View(model);
        }
    }
}