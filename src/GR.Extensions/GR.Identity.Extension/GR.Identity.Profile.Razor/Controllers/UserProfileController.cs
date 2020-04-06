using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.ViewModels.UserProfileAddress;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels;
using GR.Localization.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Profile.Razor.Controllers
{
    [Authorize]
    public class UserProfileController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject identity context
        /// </summary>
        private readonly IIdentityContext _identityContext;

        /// <summary>
        /// Inject profile context
        /// </summary>
        private readonly IProfileContext _profileContext;

        /// <summary>
        /// Inject country service
        /// </summary>
        private readonly ICountryService _countryService;

        #endregion

        public UserProfileController(IUserManager<GearUser> userManager, IIdentityContext identityContext, IProfileContext profileContext, ICountryService countryService)
        {
            _userManager = userManager;
            _identityContext = identityContext;
            _profileContext = profileContext;
            _countryService = countryService;
        }


        /// <summary>
        /// User profile info
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> Index()
        {
            var currentUser = (await _userManager.GetCurrentUserAsync()).Result;
            if (currentUser == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                UserId = currentUser.Id,
                TenantId = currentUser.TenantId ?? Guid.Empty,
                UserName = currentUser.UserName,
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                PhoneNumber = currentUser.PhoneNumber,
                AboutMe = currentUser.AboutMe,
                Birthday = currentUser.Birthday,
                Email = currentUser.Email,
                Roles = await _userManager.UserManager.GetRolesAsync(currentUser)
            };
            return View(model);
        }


        #region Partial Views

        [HttpGet]
        public virtual async Task<IActionResult> UserOrganizationPartial(Guid? tenantId)
        {
            if (!tenantId.HasValue)
            {
                return NotFound();
            }

            var tenant = await _identityContext.Tenants.FindAsync(tenantId);
            if (tenant == null)
            {
                return NotFound();
            }

            var model = new UserProfileTenantViewModel
            {
                Name = tenant.Name,
                TenantId = tenant.TenantId,
                Description = tenant.Description,
                Address = tenant.Address,
                SiteWeb = tenant.SiteWeb
            };
            return PartialView("Partial/_OrganizationPartial", model);
        }


        [HttpGet]
        public virtual IActionResult UserAddressPartial(Guid? userId)
        {
            if (!userId.HasValue)
            {
                return NotFound();
            }

            var addressList = _profileContext.UserAddresses
                .AsNoTracking()
                .Where(x => x.UserId.Equals(userId.Value) && x.IsDeleted == false)
                .Include(x => x.Country)
                .Include(x => x.StateOrProvince)
                .Include(x => x.District)
                .Select(address => new UserProfileAddressViewModel
                {
                    Id = address.Id,
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    Phone = address.Phone,
                    ContactName = address.ContactName,
                    District = address.District.Name,
                    Country = address.Country.Name,
                    City = address.StateOrProvince.Name,
                    IsPrimary = address.IsDefault,
                    ZipCode = address.ZipCode,
                })
                .ToList();

            return PartialView("Partial/_AddressListPartial", addressList);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual PartialViewResult ChangeUserPasswordPartial() => PartialView("Partial/_ChangePasswordPartial");

        [HttpGet]
        public virtual async Task<IActionResult> AddUserProfileAddress()
        {
            var model = new AddUserProfileAddressViewModel
            {
                CountrySelectListItems = await GetCountrySelectList()
            };
            return PartialView("Partial/_AddUserProfileAddress", model);
        }

        [HttpGet]
        public virtual async Task<IActionResult> EditUserProfileAddress(Guid? addressId)
        {
            if (!addressId.HasValue)
            {
                return NotFound();
            }

            var currentAddress = await _profileContext.UserAddresses
                .FirstOrDefaultAsync(x => x.Id.Equals(addressId.Value));
            var cityBySelectedCountry = await _profileContext.StateOrProvinces
                .AsNoTracking()
                .Where(x => x.CountryId.Equals(currentAddress.CountryId))
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();
            if (currentAddress == null)
            {
                return NotFound();
            }

            var model = new EditUserProfileAddressViewModel
            {
                Id = currentAddress.Id,
                CountrySelectListItems = await GetCountrySelectList(),
                AddressLine1 = currentAddress.AddressLine1,
                AddressLine2 = currentAddress.AddressLine2,
                Phone = currentAddress.Phone,
                ContactName = currentAddress.ContactName,
                ZipCode = currentAddress.ZipCode,
                CountryId = currentAddress.CountryId,
                CityId = currentAddress.StateOrProvinceId,
                SelectedStateOrProvinceSelectListItems = cityBySelectedCountry,
                IsDefault = currentAddress.IsDefault
            };
            return PartialView("Partial/_EditUserProfileAddress", model);
        }


        /// <summary>
        /// Get view for edit profile info
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> EditProfilePartial(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return NotFound();
            }

            var currentUser = await _userManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
            if (currentUser == null)
            {
                return NotFound();
            }

            var model = new UserProfileEditViewModel
            {
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                Birthday = currentUser.Birthday,
                AboutMe = currentUser.AboutMe,
                PhoneNumber = currentUser.PhoneNumber,
                Email = currentUser.Email
            };
            return PartialView("Partial/_EditProfilePartial", model);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get country list
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<SelectListItem>> GetCountrySelectList()
        {
            var select = new SelectListItem("Select country", "");
            var countryRequest = await _countryService.GetAllCountriesAsync();
            if (!countryRequest.IsSuccess) return new List<SelectListItem>
            {
                select
            };
            return countryRequest.Result.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            });
        }

        #endregion
    }
}
