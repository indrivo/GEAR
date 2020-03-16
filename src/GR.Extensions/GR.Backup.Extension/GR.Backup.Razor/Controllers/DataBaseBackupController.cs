using System.Linq;
using GR.Backup.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GR.Core;
using GR.Core.Razor.BaseControllers;

namespace GR.Backup.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    public class DataBaseBackupController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject backup 
        /// </summary>
        private readonly IBackupService _backupService;

        #endregion

        public DataBaseBackupController(IBackupService backupService)
        {
            _backupService = backupService;
        }

        /// <summary>
        /// Backup manager
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var backups = _backupService.GetBackups()
                .OrderByDescending(x => x.CreationDate)
                .ToList();
            return View(backups);
        }

        /// <summary>
        /// Download backup
        /// </summary>
        /// <param name="backupName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route(DefaultApiRouteTemplate)]
        public IActionResult DownloadBackup(string backupName)
        {
            var blobRequest = _backupService.DownloadBackup(backupName);
            if (!blobRequest.IsSuccess) return NotFound();
            return File(blobRequest.Result, "application/octet-stream", blobRequest.Name);
        }

        /// <summary>
        /// Clear all
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route(DefaultApiRouteTemplate)]
        public JsonResult ClearAll()
            => Json(_backupService.Clear());
    }
}