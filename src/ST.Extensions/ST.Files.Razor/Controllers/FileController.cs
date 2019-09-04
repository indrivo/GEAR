using Microsoft.AspNetCore.Mvc;
using ST.Core.Helpers;
using ST.Files.Abstraction;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using ST.Core;
using ST.Files.Abstraction.Helpers;
using ST.Files.Abstraction.Models.ViewModels;
using ST.Files.Box.Abstraction.Models.ViewModels;
using ST.Identity.Abstractions;


namespace ST.Files.Razor.Controllers
{
    [Authorize(Roles = Settings.ADMINISTRATOR)]
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
            var file = new UploadFileViewModel
            {
                File = Request.Form.Files.FirstOrDefault(),
                Id = id,
                TenantId = _userManager.CurrentUserTenantId
            };
            var response = _fileManager.AddFile(file);
            return Json(response);
        }

        /// <summary>
        /// Multiple file upload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadMultiple()
        {
            var response = Request.Form.Files.Select(item => new UploadFileViewModel
            {
                File = item,
                Id = Guid.Empty,
                TenantId = _userManager.CurrentUserTenantId
            }).Select(file => _fileManager.AddFile(file)).ToList();

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
            if (id == Guid.Empty) return Json(ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.NullParameter));

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
            if (id == Guid.Empty) return Json(ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.NullParameter));

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
            if (id == Guid.Empty) return Json(ExceptionHandler.ReturnErrorModel(ExceptionMessagesEnum.NullParameter));

            var response = _fileManager.DeleteFilePermanent(id);
            return Json(response);
        }


        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<FileSettingsViewModel>))]
        public JsonResult CreateTask(FileBoxSettingsViewModel model)
        {
            if (!ModelState.IsValid) return Json(ExceptionHandler.ReturnErrorModel<FileSettingsViewModel>(ModelState));

            var response = _fileManager.ChangeSettings(model);
            return Json(response);
        }
    }
}
