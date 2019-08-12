using Microsoft.AspNetCore.Mvc;
using ST.Core.BaseControllers;
using ST.Core.Helpers;
using ST.Files.Abstraction;
using ST.Files.Abstraction.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace ST.Files.Razor.Controllers
{
   public class FileController : Controller
   {
        /// <summary>
        /// Inject file service
        /// </summary>
        private readonly IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet, Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult GetFile(Guid fileId)
        {
            var response = _fileService.GetFileById(fileId);
            return Json(response);   
        }

        /// <summary>
        /// Get files
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet, Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult GetFiles(List<Guid> filesIds)
        {
            var response = _fileService.GetFilesByIds(filesIds);
            return Json(response);
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet, Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult Upload(FileViewModel fileViewModel)
        {
            var response = _fileService.AddFile(fileViewModel);
            return Json(response);
        }


        /// <summary>
        /// Delete file logical
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete form!", success = false });
            var res = _fileService.DeleteFile(Guid.Parse(id));
            return Json(new { message = "Form was delete with success!", success = res.IsSuccess });
        }

        /// <summary>
        /// Delete file permanently
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [ValidateAntiForgeryToken]
        [HttpPost, Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult DeletePermatent(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete form!", success = false });
            var res = _fileService.DeleteFilePermanent(Guid.Parse(id));
            return Json(new { message = "Form was delete with success!", success = res.IsSuccess });
        }
    }
}
