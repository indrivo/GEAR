using ST.Identity.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.Core.Helpers;
using ST.Email.Abstractions;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.MultiTenant.Abstractions;

namespace ST.MultiTenant.Services
{
    public class OrganizationService : IOrganizationService<Tenant>
    {
        /// <summary>
        /// Inject context
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Inject User Manager
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Inject Role Manager
        /// </summary>
        private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Inject email sender
        /// </summary>
        private readonly IEmailSender _emailSender;

        private readonly IUrlHelper _urlHelper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="emailSender"></param>
        /// <param name="urlHelper"></param>
        public OrganizationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, IHttpContextAccessor httpContextAccessor,
            IEmailSender emailSender, IUrlHelper urlHelper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
            _urlHelper = urlHelper;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get all users for organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> GetAllowedUsersByOrganizationId(Guid organizationId)
        {
            return _context.Users.Where(x => x.TenantId == organizationId && !x.IsDeleted);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get all tenants
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tenant> GetAllTenants()
        {
            return _context.Tenants.ToList();
        }

        public IEnumerable<ApplicationUser> GetDisabledUsersByOrganizationId(Guid organizationId)
        {
            return _context.Users.Where(x => x.TenantId == organizationId && x.IsDeleted);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get tenant id
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public Tenant GetTenantById(Guid tenantId)
        {
            return _context.Tenants.AsNoTracking().FirstOrDefault(x => x.Id == tenantId);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get all users for organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> GetUsersByOrganization(Tenant organization)
        {
            return organization == null
                ? default(IEnumerable<ApplicationUser>)
                : _context.Users.Where(x => x.TenantId == organization.Id);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get users by organization and role
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> GetUsersByOrganization(Guid organizationId, Guid roleId)
        {
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
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> GetUsersByOrganizationId(Guid organizationId)
        {
            return _context.Users.Where(x => x.TenantId == organizationId);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get user organization
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Tenant GetUserOrganization(ApplicationUser user)
        {
            Arg.NotNull(user, nameof(GetUserOrganization));
            return _context.Tenants
                .Include(x => x.Country)
                .FirstOrDefault(x => x.Id.Equals(user.TenantId));
        }

        /// <summary>
        /// Get Tenant by current user 
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<Tenant>> GetTenantByCurrentUserAsync()
        {
            var resultModel = new ResultModel<Tenant>();
            var currentUser = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (currentUser != null)
            {
                resultModel.IsSuccess = true;
                resultModel.Result = await _context.Tenants.FirstOrDefaultAsync(x => x.Id.Equals(currentUser.TenantId));
                return resultModel;
            }

            resultModel.IsSuccess = false;
            return resultModel;
        }

        /// <summary>
        /// Create new Organization User
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async Task<ResultModel> CreateNewOrganizationUserAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            var result = await _userManager.CreateAsync(user, GenerateRandomPassword());
            if (result.Succeeded)
            {
                var userRoles = await _roleManager.Roles
                    .Where(x => roles.Contains(x.Id))
                    .Select(x => x.Name)
                    .ToListAsync();
                userRoles.Add(Core.Settings.ANONIMOUS_USER);
                var userResult = await _userManager.AddToRolesAsync(user, userRoles);
                if (userResult.Succeeded)
                {
                    return new ResultModel
                    {
                        IsSuccess = true
                    };
                }
            }

            return new ResultModel
            {
                IsSuccess = false
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// Send email for confirmation
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task SendInviteToEmailAsync(ApplicationUser user)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = _urlHelper.Action("ConfirmEmail", "Account", new {userId = user.Id, confirmToken = code},
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
            var path = Path.Combine(AppContext.BaseDirectory, "Static/Embedded Resources/company.png");
            if (!File.Exists(path))
                return default;

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var binary = new BinaryReader(stream))
                {
                    var data = binary.ReadBytes((int) stream.Length);
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
        public async Task<IEnumerable<ApplicationRole>> GetRoles()
        {
            var rolesToExclude = new HashSet<string>
            {
                Core.Settings.ADMINISTRATOR,
                Core.Settings.ANONIMOUS_USER
            };

            var roles = await _roleManager.Roles
                .AsNoTracking()
                .Where(x => !x.IsDeleted && !rolesToExclude.Any(z => z.Equals(x.Name)))
                .ToListAsync();

            return roles;
        }

        private static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null)
                opts = new PasswordOptions()
                {
                    RequiredLength = 8,
                    RequiredUniqueChars = 4,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireNonAlphanumeric = true,
                    RequireUppercase = true
                };

            var randomChars = new[]
            {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ", // uppercase 
                "abcdefghijkmnopqrstuvwxyz", // lowercase
                "0123456789", // digits
                "!@$?_-" // non-alphanumeric
            };
            var rand = new Random(Environment.TickCount);
            var chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (var i = chars.Count;
                i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars;
                i++)
            {
                var rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        #region Validation

        /// <summary>
        /// Check if exist any user with this email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> CheckIfUserExistAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email.ToLower().Equals(email.ToLower()));
        }

        #endregion
    }
}