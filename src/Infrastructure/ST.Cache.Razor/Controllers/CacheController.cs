using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST.BaseBusinessRepository;
using ST.Cache.Abstractions;

namespace ST.Cache.Razor.Controllers
{
    [Authorize(Roles = Core.Settings.SuperAdmin)]
    public class CacheController : Controller
    {
        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cacheService"></param>
        public CacheController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// List of redis keys
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ViewBag.IsConnected = _cacheService.IsConnected();
            var model = _cacheService.GetAllKeys();
            return View(model);
        }

        /// <summary>
        /// Remove key from redis cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RemoveItem([Required]string key)
        {
            await _cacheService.RemoveAsync(key);
            return StatusCode(200);
        }

        /// <summary>
        /// Flush all
        /// </summary>
        /// <returns></returns>
        [Route("[controller]/[action]")]
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel))]
        public IActionResult FlushAll()
        {
            _cacheService.FlushAll();
            return Json(new ResultModel
            {
                IsSuccess = true
            });
        }
    }
}
