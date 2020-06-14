using System;
using System.IO;
using System.Threading.Tasks;
using GR.ApplePay.Abstractions.Helpers;
using GR.ApplePay.Abstractions.ViewModels;
using GR.ApplePay.Razor.ViewModels;
using GR.Core;
using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers.Global;
using GR.Identity.Abstractions.Helpers.Attributes;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace GR.ApplePay.Razor.Controllers
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [AutoValidateAntiforgeryToken]
    [Admin]
    public sealed class ApplePaySettingsController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject settings
        /// </summary>
        private readonly IWritableOptions<ApplePaySettingsViewModel> _applePaySettings;

        #endregion

        public ApplePaySettingsController(IWritableOptions<ApplePaySettingsViewModel> applePaySettings)
        {
            _applePaySettings = applePaySettings;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction("Config");
        }

        /// <summary>
        /// Get config view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Config()
        {
            ViewData["Domain"] = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            var model = _applePaySettings.Value.Adapt<ApplePaySettingsEditViewModel>();
            return View(model);
        }

        /// <summary>
        /// Save new config
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Config(ApplePaySettingsEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (model.MerchantIdCertificate != null)
            {
                var certificatesPath = Path.Combine(AppContext.BaseDirectory, GlobalResources.Paths.CertificatesPath);
                var oldCertificatePath = Path.Combine(certificatesPath, _applePaySettings.Value.MerchantCertificateFileName);
                if (System.IO.File.Exists(oldCertificatePath)) System.IO.File.Delete(oldCertificatePath);
                var newCertificatePath = Path.Combine(certificatesPath, model.MerchantIdCertificate.FileName);
                var uploadFileResult = await model.MerchantIdCertificate.UploadAsync(newCertificatePath);
                if (!uploadFileResult.IsSuccess)
                {
                    ModelState.AppendResultModelErrors(uploadFileResult.Errors);
                    return View(model);
                }
            }

            if (model.DomainVerificationFile != null)
            {
                var domainPath = Path.Combine(AppContext.BaseDirectory, "DomainAssociation");
                var path = Path.Combine(domainPath, ApplePayResources.DomainVerificationFile);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                var uploadFileResult = await model.DomainVerificationFile.UploadAsync(path);
                if (!uploadFileResult.IsSuccess)
                {
                    ModelState.AppendResultModelErrors(uploadFileResult.Errors);
                    return View(model);
                }
            }

            var updateResult = _applePaySettings.Update(options =>
            {
                options.StoreName = model.StoreName;
                options.ApplePayVersion = model.ApplePayVersion;
                options.UseCertificateStore = model.UseCertificateStore;
                options.UsePolyfill = model.UsePolyfill;
                options.AcceptedCardSchemes = model.AcceptedCardSchemes;
                options.MerchantCapabilities = model.MerchantCapabilities;
                options.RequiredBillingContactFields = model.RequiredBillingContactFields;
                options.RequiredShippingContactFields = model.RequiredShippingContactFields;

                if (model.MerchantIdCertificate == null) return;
                options.MerchantCertificatePassword = model.MerchantCertificatePassword;
                options.MerchantCertificateFileName = model.MerchantIdCertificate.FileName;

            }, ApplePayResources.ApplePaySettingsPath);
            if (updateResult.IsSuccess) ViewData["isSuccess"] = true;
            else
            {
                ModelState.AppendResultModelErrors(updateResult.Errors);
            }
            return View(model);
        }
    }
}