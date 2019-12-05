using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GR.Cache.Abstractions;
using GR.Core;
using GR.Core.Abstractions;
using GR.Core.Helpers;

namespace GR.Cache.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    public class CacheController : Controller
    {
        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Cache options
        /// </summary>
        private readonly IWritableOptions<RedisConnectionConfig> _writableOptions;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cacheService"></param>
        /// <param name="writableOptions"></param>
        public CacheController(ICacheService cacheService, IWritableOptions<RedisConnectionConfig> writableOptions)
        {
            _cacheService = cacheService;
            _writableOptions = writableOptions;
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

        /// <summary>
        /// Settings
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Settings()
        {
            return View(_writableOptions.Value);
        }

        /// <summary>
        /// Settings
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Settings(RedisConnectionConfig model)
        {
            _writableOptions.Update(opt =>
            {
                opt.Host = model.Host;
                opt.Port = model.Port;
            });
            return View();
        }
    }
}
