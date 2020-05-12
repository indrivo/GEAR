using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.UserPreferences.Abstractions;
using GR.UserPreferences.Abstractions.Helpers.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace GR.UserPreferences.Api.Controllers
{
    [Author(Authors.LUPEI_NICOLAE)]
    [JsonApiExceptionFilter]
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [Route("api/user-preferences/[action]")]
    public sealed class UserPreferencesApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject user preferences service
        /// </summary>
        private readonly IUserPreferencesService _service;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="service"></param>
        public UserPreferencesApiController(IUserPreferencesService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get preference value by key
        /// </summary>
        /// <param name="key">Key of preferences, value is extracted in text format,
        /// it is need to be serialized</param>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<string>))]
        public async Task<JsonResult> GetValueByKey([Required]string key)
            => await JsonAsync(_service.GetValueByKeyAsync(key));

        /// <summary>
        /// Add or update user preference
        /// </summary>
        /// <param name="key">Represent the key of user preference,
        /// key must be registered in system, for usage of key, check what
        /// type of keys are registered</param>
        /// <param name="value">Represent the serialized value of setting,
        /// if value is not saved previously, it will be saved</param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        public async Task<JsonResult> AddOrUpdatePreferenceSetting([Required]string key, string value)
            => await JsonAsync(_service.AddOrUpdatePreferenceSettingAsync(key, value));

        /// <summary>
        /// Get configuration for keys
        /// </summary>
        /// <param name="key">Represent the key of user preference,
        /// key must be registered in system, for usage of key, check what
        /// type of keys are registered</param>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(BaseBuildResponse<object>))]
        public async Task<JsonResult> GetKeyConfiguration([Required]string key)
            => await JsonAsync(_service.GetPreferenceConfigurationAsync(key));

        /// <summary>
        /// Get registered keys
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel<IEnumerable<string>>))]
        public JsonResult GetRegisteredKeys()
            => Json(_service.GetAvailableKeys());
    }
}