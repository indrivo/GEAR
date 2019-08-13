using Microsoft.AspNetCore.Mvc;
using ST.Core.BaseControllers;
using ST.Core.Helpers;
using ST.Files.Abstraction;
using ST.Files.Abstraction.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;

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


        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet]
        public FileResult GetFile(Guid id)
        {
            var response = _fileService.GetFileById(id);
            return File(response.Result.EncryptedFile,response.Result.FileExtension,response.Result.FileName);

        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public JsonResult Upload(Guid id)
        {
            var file = new UploadFileViewModel
            {
                File = Request.Form.Files.FirstOrDefault(),
                Id = id
            };
            var response = _fileService.AddFile(file);
            return Json(response);
        }

        /// <summary>
        /// Get file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpPost]
        public JsonResult UploadMultiple()
        {
            var response = new List<ResultModel<Guid>>();
            foreach (var item in Request.Form.Files)
            {
                var file = new UploadFileViewModel
                {
                    File = item,
                    Id = Guid.Empty
                };
                response.Add(_fileService.AddFile(file));
            }
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
        public JsonResult DeletePermanent(string id)
        {
            if (string.IsNullOrEmpty(id)) return Json(new { message = "Fail to delete form!", success = false });
            var res = _fileService.DeleteFilePermanent(Guid.Parse(id));
            return Json(new { message = "Form was delete with success!", success = res.IsSuccess });
        }
   }
}
