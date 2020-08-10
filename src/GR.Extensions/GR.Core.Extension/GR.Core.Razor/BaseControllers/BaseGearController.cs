using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Patterns;
using GR.Core.Razor.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GR.Core.Razor.BaseControllers
{
    [Author(Authors.LUPEI_NICOLAE)]
    public abstract class BaseGearController : Controller
    {
        #region Constants

        /// <summary>
        /// Default cache response seconds
        /// </summary>
        public const int DefaultCacheResponseSeconds = 60;

        #endregion

        #region Helpers

        /// <summary>
        /// Default template
        /// </summary>
        public const string DefaultApiRouteTemplate = "api/[controller]/[action]";

        /// <summary>
        /// Mapper
        /// </summary>
        protected IMapper Mapper => Singleton<IMapper, IMapper>.GetOrSetInstance(
            () => Task.FromResult(Request.HttpContext.RequestServices.GetRequiredService<IMapper>()));

        /// <summary>
        /// Serialization settings
        /// </summary>
        protected virtual JsonSerializerSettings SerializerSettings => new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = GearSettings.Date.DateFormatWithTime
        };

        #endregion

        /// <summary>
        /// Return json async
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        [NonAction]
        public virtual async Task<JsonResult> JsonAsync<TResult>(Task<TResult> task)
        {
            return Json(await task);
        }

        /// <summary>
        /// Return json response with elapsed time
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        [NonAction]
        public virtual async Task<JsonResult> JsonWithTimeAsync<TResult>(Task<ResultModel<TResult>> task)
        {
            var watch = new Stopwatch();
            var start = DateTime.Now;
            watch.Start();
            var response = await task;
            var vm = Mapper.Map<ElapsedTimeJsonResultModel<TResult>>(response);
            watch.Stop();
            vm.Started = start;
            vm.Completed = DateTime.Now;
            vm.ElapsedMilliseconds = watch.ElapsedMilliseconds;
            return Json(vm);
        }

        /// <summary>
        /// Return Json with invalid validations as ResultModel errors
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected virtual JsonResult JsonModelStateErrors() => Json(new ResultModel().AttachModelState(ModelState));
    }
}
