using GR.Identity.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Core.Helpers.Templates;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Email.Abstractions;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Abstractions.Helpers;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.MultiTenant.Abstractions;
using GR.MultiTenant.Abstractions.Helpers;
using GR.MultiTenant.Abstractions.ViewModels;
using GR.Notifications.Abstractions;

namespace GR.MultiTenant.Services
{
    public class OrganizationService : IOrganizationService<Tenant>
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject context accessor
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Inject localizer
        /// </summary>
        private readonly IStringLocalizer _localizer;

        /// <summary>
        /// Inject email sender
        /// </summary>
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Inject Microsoft url helper
        /// </summary>
        private readonly IUrlHelper _urlHelper;

        /// <summary>
        /// Inject hub
        /// </summary>
        private readonly INotificationHub _hub;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="emailSender"></param>
        /// <param name="urlHelper"></param>
        /// <param name="localizer"></param>
        /// <param name="hub"></param>
        public OrganizationService(ApplicationDbContext context, IUserManager<GearUser> userManager, IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender, IUrlHelper urlHelper, IStringLocalizer localizer, INotificationHub hub)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _urlHelper = urlHelper;
            _localizer = localizer;
            _hub = hub;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get all users for organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public virtual IEnumerable<GearUser> GetAllowedUsersByOrganizationId(Guid organizationId)
        {
            if (organizationId == Guid.Empty) return new List<GearUser>();
            return _context.Users.Where(x => x.TenantId == organizationId && !x.IsDeleted);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get all tenants
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Tenant> GetAllTenants()
            => _context.Tenants.ToList();

        /// <summary>
        /// Check if user is a part of organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public virtual async Task<bool> IsUserPartOfOrganizationAsync(Guid? userId, Guid? tenantId)
        {
            if (!userId.HasValue || !tenantId.HasValue) return false;
            var user = await _userManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.ToGuid().Equals(userId));
            return user != null && user.TenantId.Equals(tenantId);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get disabled users
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public virtual IEnumerable<GearUser> GetDisabledUsersByOrganizationId(Guid organizationId)
        {
            if (organizationId == Guid.Empty) return new List<GearUser>();
            return _context.Users.Where(x => x.TenantId == organizationId && x.IsDeleted);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get tenant id
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public virtual Tenant GetTenantById(Guid tenantId) =>
            tenantId == Guid.Empty
                ? default
                : _context.Tenants
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id.Equals(tenantId));

        /// <inheritdoc />
        /// <summary>
        /// Get all users for organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        public virtual IEnumerable<GearUser> GetUsersByOrganization(Tenant organization) => organization == null
            ? new List<GearUser>()
            : _context.Users.Where(x => x.TenantId.Equals(organization.Id)).ToList();

        /// <inheritdoc />
        /// <summary>
        /// Get users by organization and role
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public virtual IEnumerable<GearUser> GetUsersByOrganization(Guid organizationId, Guid roleId)
        {
            if (organizationId == Guid.Empty || roleId == Guid.Empty) return new List<GearUser>();
            var role = _context.Roles.FirstOrDefault(x =>
                string.Equals(x.Id, roleId.ToString(), StringComparison.CurrentCultureIgnoreCase));
            if (role == null) return default;
            var usersId = _context.UserRoles.Where(x =>
                    string.Equals(x.RoleId, roleId.ToString(), StringComparison.CurrentCultureIgnoreCase))
                .ToList()
                .Select(x => x.UserId).ToList();
            return _context.Users.Where(x => x.TenantId == organizationId && usersId.Contains(x.Id));
        }

        /// <inheritdoc />
        /// <summary>
        /// Get all users for organization
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<GearUser>>> GetUsersByOrganizationIdAsync(Guid tenantId)
        {
            if (tenantId == Guid.Empty) return new InvalidParametersResultModel<IEnumerable<GearUser>>();
            var members = await _context.Users.Where(x => x.TenantId == tenantId).ToListAsync();
            return new SuccessResultModel<IEnumerable<GearUser>>(members);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get user organization
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Tenant GetUserOrganization(GearUser user)
        {
            Arg.NotNull(user, nameof(GetUserOrganization));
            return _context.Tenants
                .Include(x => x.Country)
                .FirstOrDefault(x => x.Id.Equals(user.TenantId));
        }

        /// <inheritdoc />
        /// <summary>
        /// Get Tenant by current user 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<Tenant>> GetTenantByCurrentUserAsync()
        {
            var resultModel = new ResultModel<Tenant>();
            var currentUser = await _userManager.UserManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (currentUser != null)
            {
                resultModel.IsSuccess = true;
                resultModel.Result = await _context.Tenants.FirstOrDefaultAsync(x => x.Id.Equals(currentUser.TenantId));
                return resultModel;
            }

            resultModel.IsSuccess = false;
            return resultModel;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create new Organization User
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> CreateNewOrganizationUserAsync(GearUser user, IEnumerable<string> roles)
        {
            Arg.NotNull(user, nameof(CreateNewOrganizationUserAsync));
            var mResult = new ResultModel();
            var result = await _userManager.UserManager.CreateAsync(user, PasswordGenerator.GenerateRandomPassword());
            if (result.Succeeded)
            {
                var userRoles = await _userManager.RoleManager.Roles
                    .Where(x => roles.Contains(x.Id))
                    .Select(x => x.Name)
                    .ToListAsync();
                userRoles.Add(GlobalResources.Roles.ANONIMOUS_USER);
                userRoles.Add(GlobalResources.Roles.USER);
                var userResult = await _userManager.UserManager.AddToRolesAsync(user, userRoles);
                if (userResult.Succeeded)
                {
                    mResult.IsSuccess = true;
                }
                else mResult.AppendIdentityErrors(userResult.Errors);
            }
            else mResult.AppendIdentityErrors(result.Errors);

            return mResult;
        }

        /// <inheritdoc />
        /// <summary>
        /// Send email for confirmation
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task SendInviteToEmailAsync(GearUser user)
        {
            Arg.NotNull(user, nameof(SendInviteToEmailAsync));
            var code = await _userManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = _urlHelper.Action("ConfirmInvitedUserByEmail", "Company", new { userId = user.Id, confirmToken = code },
                _httpContextAccessor.HttpContext.Request.Scheme);
            var mail = $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirm email</a>";
            var templateRequest = TemplateManager.GetTemplateBody("invite-new-user");
            if (templateRequest.IsSuccess)
            {
                var tenant = GetTenantById(user.TenantId.GetValueOrDefault());
                mail = templateRequest.Result.Inject(new Dictionary<string, string>
                {
                    { "Link", HtmlEncoder.Default.Encode(callbackUrl) },
                    { "CompanyName", tenant?.Name }
                });
            }
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", mail);
        }

        /// <summary>
        /// Send confirm email
        /// </summary>
        /// <returns></returns>
        public virtual async Task SendConfirmEmailRequest(GearUser user)
        {
            Arg.NotNull(user, nameof(SendConfirmEmailRequest));
            var code = await _userManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = _urlHelper.Action("ConfirmEmail", "Company", new { userId = user.Id, confirmToken = code },
                _httpContextAccessor.HttpContext.Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirm email</a>");
        }

        /// <inheritdoc />
        /// <summary>
        /// Get default user image
        /// </summary>
        /// <returns></returns>
        public virtual byte[] GetDefaultImage()
        {
            var path = Path.Combine(AppContext.BaseDirectory, MultiTenantResources.EmbeddedResources.COMPANY_IMAGE);
            if (!File.Exists(path))
                throw new Exception(MultiTenantResources.Exceptions.E_MULTI_TENANT_COMPANY_IMAGE_NULL);

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var binary = new BinaryReader(stream))
                {
                    var data = binary.ReadBytes((int)stream.Length);
                    return data;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return default;
        }

        /// <summary>
        /// Return list of available roles
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<GearRole>> GetRoles()
        {
            var rolesToExclude = new HashSet<string>
            {
                GlobalResources.Roles.ADMINISTRATOR,
                GlobalResources.Roles.ANONIMOUS_USER
            };

            var roles = await _userManager.RoleManager.Roles
                .AsNoTracking()
                .Where(x => !x.IsDeleted && !rolesToExclude.Any(z => z.Equals(x.Name)))
                .ToListAsync();

            return roles;
        }

        /// <summary>
        /// Invite new user by email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> InviteNewUserByEmailAsync(InviteNewUserViewModel model)
        {
            Arg.NotNull(model, nameof(InviteNewUserByEmailAsync));
            var resultModel = new ResultModel();
            if (await CheckIfUserExistAsync(model.Email))
            {
                resultModel.Errors.Add(new ErrorModel
                {
                    Key = string.Empty,
                    Message = "Email is in use"
                });
                return resultModel;
            }

            var newUser = new GearUser
            {
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                UserName = model.Email.Split('@')[0],
                NormalizedUserName = model.Email.Split('@')[0].ToUpper(),
                EmailConfirmed = false,
                Created = DateTime.Now,
                Author = _httpContextAccessor.HttpContext.User.Identity.Name,
                LastPasswordChanged = DateTime.Now,
                IsEditable = true
            };

            var tenant = await GetTenantByCurrentUserAsync();
            if (!tenant.IsSuccess)
            {
                resultModel.IsSuccess = false;
                resultModel.Errors.Add(new ErrorModel
                {
                    Key = string.Empty,
                    Message = "Tenant not found"
                });
                return resultModel;
            }

            newUser.TenantId = tenant.Result.Id;

            var result = await CreateNewOrganizationUserAsync(newUser, model.Roles);
            if (!result.IsSuccess) return result;
            await SendInviteToEmailAsync(newUser);
            resultModel.IsSuccess = true;
            return resultModel;
        }


        /// <summary>
        /// Get filtered list of organization
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual DTResult<OrganizationListViewModel> GetFilteredList(DTParameters param)
        {
            if (param == null) return new DTResult<OrganizationListViewModel>();
            var filtered = _context.Filter<Tenant>(param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount);

            var list = filtered.Select(x => new OrganizationListViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Created = x.Created,
                Changed = x.Changed,
                ModifiedBy = x.ModifiedBy,
                Author = x.Author,
                Users = GetUsersByOrganization(x).Count()
            });

            return new DTResult<OrganizationListViewModel>
            {
                Draw = param.Draw,
                Data = list.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
        }

        /// <summary>
        /// Get countries
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IEnumerable<SelectListItem>> GetCountrySelectList()
        {
            var countrySelectList = await _context.Countries
                .AsNoTracking()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id
                })
                .OrderBy(x => x.Text)
                .ToListAsync();

            countrySelectList.Insert(0, new SelectListItem(_localizer["system_select_country"], string.Empty));

            return countrySelectList;
        }

        /// <summary>
        /// Create new organization
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<CreateTenantViewModel>> CreateOrganizationAsync(CreateTenantViewModel data)
        {
            var response = new ResultModel<CreateTenantViewModel>();
            var tenantMachineName = TenantUtils.GetTenantMachineName(data.Name);
            if (string.IsNullOrEmpty(tenantMachineName))
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid name for tenant"));
                data.CountrySelectListItems = await GetCountrySelectList();
                response.Result = data;
                return response;
            }

            var model = data.GetBase();
            model.MachineName = tenantMachineName;
            var check = _context.Tenants.FirstOrDefault(x => x.MachineName == tenantMachineName);
            if (check != null)
            {
                data.CountrySelectListItems = await GetCountrySelectList();
                response.Errors.Add(new ErrorModel(string.Empty, "Tenant exists"));
                response.Result = data;
                return response;
            }

            if (data.OrganizationLogoFormFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await data.OrganizationLogoFormFile.CopyToAsync(memoryStream);
                    model.OrganizationLogo = memoryStream.ToArray();
                }
            }

            _context.Tenants.Add(model);

            var dbResult = await _context.SaveAsync();
            if (dbResult.IsSuccess) response.IsSuccess = true;
            else dbResult.Errors = dbResult.Errors;

            data.Id = model.Id;
            data.MachineName = model.MachineName;
            response.Result = data;
            data.CountrySelectListItems = await GetCountrySelectList();
            return response;
        }

        /// <summary>
        /// Get filtered list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<CompanyUsersViewModel>> LoadFilteredListCompanyUsersAsync(DTParameters param)
        {
            var def = new DTResult<CompanyUsersViewModel>
            {
                Draw = param.Draw,
                Data = new List<CompanyUsersViewModel>(),
                RecordsFiltered = 0,
                RecordsTotal = 0
            };

            var reqCurrentUser = await _userManager.GetCurrentUserAsync();
            if (!reqCurrentUser.IsSuccess) return def;
            var currentUser = reqCurrentUser.Result;
            if (currentUser.TenantId != null)
            {
                var tenant = GetTenantById(currentUser.TenantId.Value);
                if (tenant == null) return def;
            }

            var filtered = _context.Filter<GearUser>(param.Search.Value, param.SortOrder,
                param.Start,
                param.Length,
                out var totalCount,
                x => !x.IsDeleted && x.TenantId == currentUser.TenantId && x.Id != currentUser.Id).ToList();

            var rs = filtered.Select(async x =>
            {
                var u = x.Adapt<CompanyUsersViewModel>();
                u.Roles = await _userManager.UserManager.GetRolesAsync(x);
                u.IsOnline = _hub.GetUserOnlineStatus(x.Id.ToGuid());
                return u;
            }).Select(x => x.Result);

            var finalResult = new DTResult<CompanyUsersViewModel>
            {
                Draw = param.Draw,
                Data = rs.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            return finalResult;
        }

        /// <summary>
        /// Check if tenant name use by another company
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public async Task<bool> IsTenantNameUsedAsync(string tenantName)
        {
            var tenantMachineName = TenantUtils.GetTenantMachineName(tenantName).ToLowerInvariant();
            return await _context.Tenants.AnyAsync(x => x.MachineName.ToLowerInvariant().Equals(tenantMachineName));
        }

        /// <summary>
        /// Get members in role
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<GearUser>>> GetUsersInRoleAsync(Guid? tenantId, string roleName)
        {
            if (roleName.IsNullOrEmpty() || tenantId == null) return new InvalidParametersResultModel<IEnumerable<GearUser>>();
            var membersRequest = await GetUsersByOrganizationIdAsync(tenantId.Value);
            if (!membersRequest.IsSuccess) return new NotFoundResultModel<IEnumerable<GearUser>>();
            var members = membersRequest.Result.ToList();
            var data = new Collection<GearUser>();
            foreach (var member in members)
            {
                var isInRole = await _userManager.UserManager.IsInRoleAsync(member, roleName);
                if (isInRole) data.Add(member);
            }
            return new SuccessResultModel<IEnumerable<GearUser>>(data);
        }

        /// <summary>
        /// Get company administrator
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public async Task<ResultModel<GearUser>> GetCompanyAdministratorByTenantIdAsync(Guid? tenantId)
        {
            if (tenantId == null) return new InvalidParametersResultModel<GearUser>();
            var companyAdminRequest = await GetUsersInRoleAsync(tenantId, MultiTenantResources.Roles.COMPANY_ADMINISTRATOR);
            if (!companyAdminRequest.IsSuccess) return companyAdminRequest.Map<GearUser>();
            var admin = companyAdminRequest.Result.FirstOrDefault();
            if (admin != null) return new SuccessResultModel<GearUser>(admin);
            return new NotFoundResultModel<GearUser>();
        }

        #region Validation

        /// <summary>
        /// Check if exist any user with this email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> CheckIfUserExistAsync(string email)
        {
            return await _userManager.UserManager.Users.AnyAsync(x => x.Email.ToLower().Equals(email.ToLower()));
        }

        #endregion
    }
}