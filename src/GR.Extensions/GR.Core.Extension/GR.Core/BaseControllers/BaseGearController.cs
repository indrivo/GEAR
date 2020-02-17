using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GR.Core.BaseControllers
{
    public abstract class BaseGearController : Controller
    {
        #region Helpers

        /// <summary>
        /// Default template
        /// </summary>
        public const string DefaultApiRouteTemplate = "api/[controller]/[action]";

        /// <summary>
        /// Serialization settings
        /// </summary>
        protected virtual JsonSerializerSettings SerializerSettings => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = GearSettings.Date.DateFormat
        };

        #endregion

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

        /// <summary>
        /// Return Json with invalid validations as ResultModel errors
        /// </summary>
        /// <returns></returns>
        protected virtual JsonResult JsonModelStateErrors()
        {
            return Json(new ResultModel().AttachModelState(ModelState));
        }

        #region Helpers

        public struct ContentType
        {
            public const string ApplicationJson = "application/json";
        }

        #endregion
    }
}
