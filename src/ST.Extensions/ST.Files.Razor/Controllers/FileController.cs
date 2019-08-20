using Microsoft.AspNetCore.Mvc;
using ST.Core.Helpers;
using ST.Files.Abstraction;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using ST.Files.Abstraction.Models.ViewModels;


namespace ST.Files.Razor.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    public sealed class FileController : Controller
    {
        /// <summary>
        /// Inject file service
        /// </summary>
        private readonly IFileManager _fileManager;

        public FileController(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

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
            return response.Result != null ? File(response.Result.EncryptedFile, "application/octet-stream", response.Result.FileName) : null;
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
                Id = id
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
        [Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult Delete(Guid id)
        {
            if (id != Guid.Empty) return Json(new {message = "Fail to delete file!", success = false});

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
            if (id != Guid.Empty) return Json(new {message = "Fail to restore file!", success = false});

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
            if (id != Guid.Empty) return Json(new {message = "Fail to delete form!", success = false});

            var response = _fileManager.DeleteFilePermanent(id);
            return Json(response);
        }
    }
}
