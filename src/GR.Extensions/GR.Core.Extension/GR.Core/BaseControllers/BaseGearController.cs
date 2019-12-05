using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GR.Core.BaseControllers
{
    public abstract class BaseGearController : Controller
    {
        #region Helpers

        protected readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        #endregion

        protected BaseGearController()
        {

        }

        /// <summary>
        /// Return json async
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="serializerSettings"></param>
        /// <returns></returns>
        public virtual async Task<JsonResult> JsonAsync<TResult>(Task<TResult> task, JsonSerializerSettings serializerSettings = null)
        {
            return Json(await task, serializerSettings ?? SerializerSettings);
        }
    }
}
