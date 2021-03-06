﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Identity.Abstractions.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authentication;

namespace GR.Identity.Abstractions
{
    public interface IAuthorizeService
    {
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> LoginAsync(LoginViewModel model);

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        Task<ResultModel> LogoutAsync();

        /// <summary>
        /// Clear user claims
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ResultModel> ClearUserClaimsAsync(GearUser user);

        /// <summary>
        /// Get auth schemes
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<AuthenticationScheme>>> GetAuthenticationSchemes();

        /// <summary>
        /// Check password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> CheckPasswordAsync(LoginViewModel model);
    }
}