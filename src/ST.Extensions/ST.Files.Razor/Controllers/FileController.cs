using Microsoft.AspNetCore.Mvc;
using ST.Core.Helpers;
using ST.Files.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using ST.Core;
using ST.Files.Abstraction.Helpers;
using ST.Files.Abstraction.Models.ViewModels;
using ST.Files.Box.Abstraction.Models.ViewModels;
using ST.Identity.Abstractions;


namespace ST.Files.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    [Route("api/[controller]/[action]")]
    public sealed class FileController : Controller
    {
        /// <summary>
        /// Inject file service
        /// </summary>
        private readonly IFileManager _fileManager;


        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        public FileController(IFileManager fileManager, IUserManager<ApplicationUser> userManager)
        {
            _fileManager = fileManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet("/File")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetFile(Guid id)
        {
            var response = _fileManager.GetFileById(id);
            return response.Result != null
                ? File(response.Result.EncryptedFile, "application/octet-stream", response.Result.FileName)
                : null;
        }

        /// <summary>
        /// Upload/Update file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Upload(Guid id)
        {
            var tenantId = _userManager.CurrentUserTenantId;
            var file = new UploadFileViewModel
            {
                File = Request.Form.Files.FirstOrDefault(),
                Id = id,
            };
            var response = _fileManager.AddFile(file, tenantId ?? Guid.Empty);
            return Json(response);
        }

        /// <summary>
        /// Multiple file upload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadMultiple()
        {
            var response = new List<ResultModel<Guid>>();

            var items = Request.Form.Files;

            response.AddRange(items.Select(item => _fileManager.AddFile(new UploadFileViewModel{File = item, Id = Guid.Empty}, _userManager.CurrentUserTenantId ?? Guid.Empty)));
            return Json(response);
        }

        /// <summary>
        /// Delete file logical
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult Delete(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionMessagesEnum.NullParameter.ToErrorModel());

            var response = _fileManager.DeleteFile(id);
            return Json(response);
        }

        /// <summary>
        /// Restore File
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult Restore(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionMessagesEnum.NullParameter.ToErrorModel());

            var response = _fileManager.RestoreFile(id);
            return Json(response);
        }

        /// <summary>
        /// Delete file permanently
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult DeletePermanent(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionMessagesEnum.NullParameter.ToErrorModel());

            var response = _fileManager.DeleteFilePermanent(id);
            return Json(response);
        }


        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<FileSettingsViewModel>))]
        public JsonResult ChangeSettings(FileSettingsViewModel model)
        {
            if (!ModelState.IsValid) return Json(ModelState.ToErrorModel<FileSettingsViewModel>());

            if (model.TenantId == Guid.Empty)
                model.TenantId = _userManager.CurrentUserTenantId ?? Guid.Empty;

            var response = _fileManager.ChangeSettings(model);
            return Json(response);
        }
    }
}
