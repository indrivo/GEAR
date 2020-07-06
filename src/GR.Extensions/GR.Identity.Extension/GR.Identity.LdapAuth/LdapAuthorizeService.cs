using System;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.ViewModels.AccountViewModels;
using GR.Identity.LdapAuth.Abstractions;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions.Events;
using GR.Identity.Abstractions.Events.EventArgs.Authorization;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.LdapAuth.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GR.Identity.LdapAuth
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class LdapAuthorizeService : ILdapAuthorizeService
    {
        #region Injectable

        /// <summary>
        /// Inject SignIn Manager
        /// </summary>
        private readonly SignInManager<GearUser> _signInManager;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Inject context accessor
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Inject ldap user manager
        /// </summary>
        private readonly ILdapUserManager<LdapUser> _ldapUserManager;

        /// <summary>
        /// Inject ldap service
        /// </summary>
        private readonly ILdapService<LdapUser> _ldapService;

        /// <summary>
        /// Inject config
        /// </summary>
        private readonly LdapConfiguration _ldapConfiguration;

        #endregion

        public LdapAuthorizeService(SignInManager<GearUser> signInManager, IUserManager<GearUser> userManager, ILogger<LdapAuthorizeService> logger, IHttpContextAccessor contextAccessor, ILdapUserManager<LdapUser> ldapUserManager, ILdapService<LdapUser> ldapService, LdapConfiguration ldapConfiguration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _ldapUserManager = ldapUserManager;
            _ldapService = ldapService;
            _ldapConfiguration = ldapConfiguration;
        }

        /// <summary>
        /// Log in user with password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> LoginAsync(GearUser user, string password, bool rememberMe = false)
        => await LoginAsync(new LoginViewModel
        {
            UserName = user.UserName,
            Password = password,
            RememberMe = rememberMe
        });

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> LoginAsync(GearUser user)
        {
            var response = new ResultModel();
            if (user == null)
            {
                response.AddError("User not found");
                return response;
            }
            if (user.IsDeleted)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "The user is deleted and cannot log in"));
                return response;
            }

            if (user.IsPasswordExpired() && !await _userManager.UserManager.IsInRoleAsync(user, GlobalResources.Roles.ADMINISTRATOR))
            {
                response.Errors.Add(new ErrorModel(string.Empty,
                    "Password has been expired, you need to change the password"));
                return response;
            }

            var userAd = _ldapService.GetUserByUserName(user.UserName);
            if (userAd == null)
            {
                response.AddError("User not fount in AD");
                return response;
            }

            await _signInManager.SignInAsync(user, true);
            IdentityEvents.Authorization.UserLogin(new UserLogInEventArgs
            {
                IpAdress = _userManager.GetRequestIpAddress(),
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                HttpContext = _contextAccessor.HttpContext
            });
            response.IsSuccess = true;
            return response;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> LoginAsync(LoginViewModel model)
        {
            var response = new ResultModel();

            var userAd = _ldapService.GetUserByUserName(model.UserName);
            if (userAd == null)
            {
                response.AddError("User not fount in AD");
                return response;
            }

            var user = await _userManager.UserManager.FindByNameAsync(model.UserName);

            if (user == null && _ldapConfiguration.AutoImportOnLogin)
            {
                var importResult = await _ldapUserManager.ImportAdUserAsync(model.UserName, model.Password);
                if (importResult.IsSuccess)
                {
                    user = await _userManager.UserManager.FindByNameAsync(model.UserName);
                }
            }

            if (user != null)
            {
                if (user.IsDeleted)
                {
                    response.Errors.Add(new ErrorModel(string.Empty, "The user is deleted and cannot log in"));
                    return response;
                }

                if (user.IsPasswordExpired() && !await _userManager.UserManager.IsInRoleAsync(user, GlobalResources.Roles.ADMINISTRATOR))
                {
                    response.AddError("Password has been expired, you need to change the password");
                    return response;
                }

                var ldapAuth = _ldapService.Authenticate(userAd.DistinguishedName, model.Password);
                if (!ldapAuth.IsSuccess)
                {
                    response.AddError("Invalid credentials");
                    return response;
                }
                await _userManager.ChangeUserPasswordAsync(user, model.Password);
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe,
                        false);
                if (result.Succeeded)
                {
                    user.LastLogin = DateTime.Now;
                    user.DisableAuditTracking = true;
                    await _userManager.UserManager.UpdateAsync(user);
                    _logger.LogInformation("User logged in.");
                    var claims = await _userManager.UserManager.GetClaimsAsync(user);
                    IdentityEvents.Authorization.UserLogin(new UserLogInEventArgs
                    {
                        IpAdress = _userManager.GetRequestIpAddress(),
                        UserId = user.Id,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        HttpContext = _contextAccessor.HttpContext
                    });

                    var exist = claims.Any(x => x.Type.Equals(nameof(Tenant).ToLowerInvariant()));
                    if (!exist)
                    {
                        var claim = new Claim(nameof(Tenant).ToLowerInvariant(), user.TenantId?.ToString());
                        await _userManager.UserManager.AddClaimAsync(user, claim);
                    }

                    response.IsSuccess = true;
                    return response;
                }

                response.AddError("Invalid login attempt.");
                return response;
            }

            response.AddError("Invalid credentials!");
            return response;
        }

        /// <summary>
        /// Check password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> CheckPasswordAsync(LoginViewModel model)
        {
            var response = new ResultModel();
            var user = await _userManager.UserManager.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);
            if (user == null)
            {
                response.AddError("User not found");
                return response;
            }

            var userAd = _ldapService.GetUserByUserName(model.UserName);
            if (userAd == null)
            {
                response.AddError("User not fount in AD");
                return response;
            }

            var ldapAuth = _ldapService.Authenticate(userAd.DistinguishedName, model.Password);
            response.IsSuccess = ldapAuth.IsSuccess;
            return response;
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel> LogoutAsync()
        {
            var result = new ResultModel();
            var userReq = await _userManager.GetCurrentUserAsync();
            if (!userReq.IsSuccess)
            {
                result.AddError(nameof(AuthorizationFailure), "Error on logout!!");
                return result;
            }

            var user = userReq.Result;

            try
            {
                await _signInManager.SignOutAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                result.Errors.Add(new ErrorModel(nameof(Exception), e.Message));
                return result;
            }

            IdentityEvents.Authorization.UserLogout(new UserLogOutEventArgs
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IpAdress = _userManager.GetRequestIpAddress()
            });

            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<AuthenticationScheme>>> GetAuthenticationSchemes()
        {
            var schemes = await _signInManager.GetExternalAuthenticationSchemesAsync();
            return new SuccessResultModel<IEnumerable<AuthenticationScheme>>(schemes);
        }

        #region Helpers

        /// <summary>
        /// Remove claims
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<ResultModel> ClearUserClaimsAsync(GearUser user)
        {
            var response = new ResultModel();
            if (user == null) return response;
            var claims = await _userManager.UserManager.GetClaimsAsync(user);
            var identityResult = await _userManager.UserManager.RemoveClaimsAsync(user, claims);
            if (identityResult.Succeeded)
            {
                response.IsSuccess = identityResult.Succeeded;
                return response;
            }

            response.AppendIdentityErrors(identityResult.Errors);
            return response;
        }

        #endregion
    }
}
