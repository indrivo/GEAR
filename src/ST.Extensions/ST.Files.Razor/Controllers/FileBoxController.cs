using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST.Core;
using ST.Core.Helpers;
using ST.Files.Abstraction.Helpers;
using ST.Files.Abstraction.Models.ViewModels;
using ST.Files.Box.Abstraction;

namespace ST.Files.Razor.Controllers
{
    [Authorize(Roles = Settings.ADMINISTRATOR)]
    [Route("api/[controller]/[action]")]
    public sealed class FileBoxController : Controller
    {
        /// <summary>
        /// Inject file service
        /// </summary>
        private readonly IFileBoxManager _fileManager;

        public FileBoxController(IFileBoxManager fileManager)
        {
            _fileManager = fileManager;
        }

        /// <summary>
        /// Index page
        /// </summary>
        /// <returns></returns>
        [HttpGet("/FileBox")]
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
        public FileResult GetFile(Guid id)
        {
            var response = _fileManager.GetFileById(id);
            return response.IsSuccess
                ? PhysicalFile(Path.Combine(response.Result.Path, response.Result.FileName), "application/octet-stream", response.Result.FileName)
                : null;
        }

        /// <summary>
        /// Upload/Update file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public JsonResult Upload(Guid id)
        {
            var file = new UploadFileViewModel
            {
                File = Request.Form.Files.FirstOrDefault(),
                Id = id
            };
            var response = _fileManager.AddFile(file);
            return Json(response);
        }

        /// <summary>
        /// Multiple Upload
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public JsonResult UploadMultiple()
        {
            var response = Request.Form.Files.Select(item => new UploadFileViewModel
            {
                File = item,
                Id = Guid.Empty
            }).Select(file => _fileManager.AddFile(file)).ToList();

            return Json(response);
        }

        /// <summary>
        /// Delete file logical
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
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
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
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
        [Produces("application/json", Type = typeof(ResultModel<Guid>))]
        public JsonResult DeletePermanent(Guid id)
        {
            if (id == Guid.Empty) return Json(ExceptionMessagesEnum.NullParameter.ToErrorModel());

            var response = _fileManager.DeleteFilePermanent(id);
            return Json(response);
        }
    }
}
