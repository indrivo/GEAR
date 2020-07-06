using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Core.Razor.Attributes;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using GR.Identity.LdapAuth.Abstractions;
using GR.Identity.LdapAuth.Abstractions.Models;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.LdapAuth.Razor.Api
{
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route(DefaultApiRouteTemplate)]
    [Admin]
    public class LdapUsersApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject Ldap User Manager
        /// </summary>
        private readonly ILdapUserManager<LdapUser> _ldapUserManager;

        #endregion

        public LdapUsersApiController(ILdapUserManager<LdapUser> ldapUserManager)
        {
            _ldapUserManager = ldapUserManager;
        }

        /// <summary>
        /// Get all ldap users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [JsonProduces(typeof(ResultModel<IEnumerable<LdapUser>>))]
        public JsonResult GetAllLdapUsers()
            => Json(new SuccessResultModel<IEnumerable<LdapUser>>(_ldapUserManager.Users.ToList()));

        /// <summary>
        /// Get Ad users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [JsonProduces(typeof(ResultModel<IEnumerable<LdapUser>>))]
        public async Task<JsonResult> GetNotAddedLdapUsers()
            => await JsonAsync(_ldapUserManager.GetNotAddedLdapUsersAsync());

        /// <summary>
        /// Import Ad user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [JsonProduces(typeof(ResultModel<Guid>))]
        public virtual async Task<JsonResult> ImportAdUser([Required] string userName)
            => await JsonAsync(_ldapUserManager.ImportAdUserAsync(userName));

        /// <summary>
        /// Load user with ajax
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost, Admin]
        [JsonProduces(typeof(DTResult<UserListItemViewModel>))]
        public async Task<JsonResult> GetAdUsersWithPagination(DTParameters param)
        {
            var data = await _ldapUserManager.GetAllLdapUsersWithPaginationAsync(param);
            return Json(data);
        }
    }
}