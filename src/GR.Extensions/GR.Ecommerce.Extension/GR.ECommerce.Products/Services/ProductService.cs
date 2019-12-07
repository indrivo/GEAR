using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.Models.Currencies;
using GR.ECommerce.Abstractions.Models.Settings;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.Identity.Abstractions;
using GR.MultiTenant.Abstractions.Helpers;
using GR.Orders.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Products.Services
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [Documentation("Basic Implementation of product service")]
    public class ProductService : IProductService<Product>
    {
        /// <summary>
        /// Inject commerce context
        /// </summary>
        private readonly ICommerceContext _commerceContext;

        /// <summary>
        /// Inject payment context
        /// </summary>
        private readonly IPaymentContext _paymentContext;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject order db context
        /// </summary>
        private readonly IOrderDbContext _orderDbContext;

        public ProductService(ICommerceContext commerceContext, IUserManager<GearUser> userManager, IPaymentContext paymentContext, IOrderDbContext orderDbContext)
        {
            _commerceContext = commerceContext;
            _userManager = userManager;
            _paymentContext = paymentContext;
            _orderDbContext = orderDbContext;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Product>>> GetAllProducts(Func<Product, bool> predicate = null)
        {
            var result = new ResultModel<IEnumerable<Product>>();
            var data = await _commerceContext.Products.ToListAsync();
            if (predicate != null)
            {
                data = data.Where(predicate).ToList();
            }

            result.IsSuccess = true;
            result.Result = data;
            return result;
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Product>> GetProductByIdAsync(Guid? productId)
        {
            if (productId == null) return new NotFoundResultModel<Product>();
            var product = await _commerceContext.Products
                .Include(x => x.Brand)
                .Include(x => x.ProductAttributes)
                .ThenInclude(x => x.ProductAttribute)
                .Include(x => x.ProductCategories)
                .ThenInclude(x => x.Category)
                .Include(x => x.ProductPrices)
                .Include(x => x.ProductVariations)
                .ThenInclude(x => x.ProductVariationDetails)
                .ThenInclude(x => x.ProductOption)
                .FirstOrDefaultAsync(x => x.Id.Equals(productId));
            if (product == null) return new NotFoundResultModel<Product>();
            var response = new ResultModel<Product>
            {
                IsSuccess = true,
                Result = product
            };

            return response;
        }

        /// <summary>
        /// Get subscription plans
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<SubscriptionPlanViewModel>>> GetSubscriptionPlansAsync()
        {
            var products = await _commerceContext.Products
                .Include(x => x.ProductAttributes)
                .ThenInclude(x => x.ProductAttribute)
                .Include(x => x.ProductPrices)
                .Include(x => x.ProductVariations)
                .ThenInclude(x => x.ProductVariationDetails)
                .ThenInclude(x => x.ProductOption)
                .Where(x => x.ProductTypeId.Equals(ProductTypes.SubscriptionPlan)
                            && x.IsPublished)
                .ToListAsync();

            var currency = (await GetGlobalCurrencyAsync()).Result;
            var response = new ResultModel<IEnumerable<SubscriptionPlanViewModel>>
            {
                IsSuccess = true,
                Result = products.Select(product => ProductMapper.Map(product, currency))
            };

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<Currency>> GetGlobalCurrencyAsync()
        {
            var settings = await _commerceContext.CommerceSettings
                .FirstOrDefaultAsync(x => x.Key.Equals(CommerceResources.SettingsParameters.CURRENCY));

            if (settings == null) return await SetGlobalCurrencyAsync(CommerceResources.SystemCurrencies.USD);
            {
                var currency =
                    await _commerceContext.Currencies.FirstOrDefaultAsync(x => x.Code.Equals(settings.Value));
                return new SuccessResultModel<Currency>(currency);
            }

        }

        /// <summary>
        /// Set global currency for products
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<ResultModel<Currency>> SetGlobalCurrencyAsync(string code)
        {
            var currency = await _commerceContext.Currencies
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                x.Code.Equals(code));

            if (currency == null) return new NotFoundResultModel<Currency>();
            var dbResponse = await AddOrUpdateSettingAsync(CommerceResources.SettingsParameters.CURRENCY, currency.Code);
            return dbResponse.Map(currency);
        }

        /// <summary>
        /// Get all currencies
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Currency>>> GetAllCurrenciesAsync() =>
            new SuccessResultModel<IEnumerable<Currency>>(await _commerceContext.Currencies.ToListAsync());

        /// <summary>
        /// Get setting
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<ResultModel<TOutput>> GetSettingAsync<TOutput>(string key)
            where TOutput : class
        {
            var setting = await _commerceContext.CommerceSettings.FirstOrDefaultAsync(x => x.Key.Equals(key));
            if (setting == null) return new InvalidParametersResultModel<TOutput>();
            return new SuccessResultModel<TOutput>(setting.Value.Deserialize<TOutput>());
        }

        /// <summary>
        /// Add or update setting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<ResultModel> AddOrUpdateSettingAsync(string key, object value, CommerceSettingType type = CommerceSettingType.Text)
        {
            var setting = await _commerceContext.CommerceSettings.FirstOrDefaultAsync(x => x.Key.Equals(key));
            if (setting == null)
            {
                await _commerceContext.CommerceSettings.AddAsync(new CommerceSetting
                {
                    Key = key,
                    Value = ParseSettingValue(value, type)
                });
            }
            else
            {
                setting.Value = ParseSettingValue(value, type);
                _commerceContext.CommerceSettings.Update(setting);
            }

            return await _commerceContext.PushAsync();
        }

        /// <summary>
        /// Get price by variation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel> GetPriceByVariationAsync(ProductPriceVariationViewModel model)
        {
            var resultModel = new ResultModel();

            var prod = await _commerceContext.Products.Include(i => i.ProductPrices).FirstOrDefaultAsync(x => x.Id == model.ProductId);

            if (prod != null)
            {
                if (model.VariationId is null)
                {
                    resultModel.IsSuccess = true;
                    resultModel.Result = new { Price = prod.PriceWithDiscount * model.Quantity };
                    return resultModel;
                }

                var productVariation = _commerceContext.ProductVariations.FirstOrDefault(x => x.Id == model.VariationId);

                if (productVariation is null)
                {
                    resultModel.IsSuccess = false;
                    resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                    return resultModel;
                }

                resultModel.IsSuccess = true;
                resultModel.Result = new { Price = productVariation.Price * model.Quantity };
                return resultModel;
            }

            resultModel.IsSuccess = false;
            resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));


            return resultModel;
        }

        /// <summary>
        /// Remove product attribute
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public async Task<ResultModel> RemoveAttributeAsync(Guid? productId, Guid? attributeId)
        {
            if (productId == null || attributeId == null) return new InvalidParametersResultModel();
            var result = await _commerceContext.ProductAttributes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProductAttributeId == attributeId && x.ProductId == productId);

            if (result == null) return new NotFoundResultModel();
            _commerceContext.ProductAttributes.Remove(result);
            var dbResult = await _commerceContext.PushAsync();
            return dbResult;
        }

        /// <summary>
        /// Get commerce general statistic
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<ResultModel<SalesStatisticViewModel>> GetCommerceGeneralStatisticAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null) return new InvalidParametersResultModel<SalesStatisticViewModel>();
            var model = new SalesStatisticViewModel
            {
                NewCustomers = (await GetNewCustomersGeneralInfoAsync(startDate, endDate)).Result,
                TotalEarnings = (await GetTotalEarningsAsync(startDate, endDate)).Result,
                OrderReceived = (await GetOrderReceivedStatisticAsync(startDate, endDate)).Result
            };
            return new SuccessResultModel<SalesStatisticViewModel>(model);
        }

        public async Task<ResultModel<Dictionary<int, object>>> GetYearReportAsync()
        {
            var year = DateTime.Today.Year;
            var data = new Dictionary<int, object>();
            for (var i = 1; i <= 12; i++)
            {
                var users = await _userManager.UserManager.Users.CountAsync(x => x.Created.Year.Equals(year) && x.Created.Month.Equals(i));
                var orders = await _orderDbContext.Orders.Where(x => x.OrderState == OrderState.PaymentReceived)
                    .CountAsync(x => x.Created.Year.Equals(year) && x.Created.Month.Equals(i));
                data.Add(i, new
                {
                    Users = users,
                    Sales = orders
                });
            }
            return new SuccessResultModel<Dictionary<int, object>>(data);
        }

        /// <summary>
        /// Get customers info
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task<ResultModel<NewCustomersStatisticViewModel>> GetNewCustomersGeneralInfoAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null) return new InvalidParametersResultModel<NewCustomersStatisticViewModel>();
            var companyRole = await _userManager.RoleManager.FindByNameAsync(MultiTenantResources.Roles.COMPANY_ADMINISTRATOR);
            if (companyRole == null) return new NotFoundResultModel<NewCustomersStatisticViewModel>();
            var newCustomersRequest = await _userManager.GetUsersInRolesAsync(new List<GearRole> { companyRole });
            if (!newCustomersRequest.IsSuccess) return newCustomersRequest.Map<NewCustomersStatisticViewModel>();
            var newCustomers = newCustomersRequest.Result.ToList();
            var inPeriod = newCustomers.Where(x => x.Created >= startDate && x.Created <= endDate).ToList();
            var model = new NewCustomersStatisticViewModel
            {
                NewCustomers = inPeriod.Count,
                Percentage = inPeriod.Count.PercentOf(newCustomers.Count)
            };
            return new SuccessResultModel<NewCustomersStatisticViewModel>(model);
        }

        /// <summary>
        /// Get total earnings info
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task<ResultModel<TotalEarningsStatisticViewModel>> GetTotalEarningsAsync(
            DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null) return new InvalidParametersResultModel<TotalEarningsStatisticViewModel>();
            var total = await _paymentContext.Payments
                .Where(x => x.PaymentStatus.Equals(PaymentStatus.Succeeded))
                .ToListAsync();
            var inPeriod = await _paymentContext.Payments
                .Where(x => x.PaymentStatus.Equals(PaymentStatus.Succeeded) && x.Created >= startDate &&
                            x.Created <= endDate)
                .ToListAsync();
            var model = new TotalEarningsStatisticViewModel
            {
                TotalEarnings = inPeriod?.Sum(x => x.Total) ?? 0,
                Percentage = inPeriod?.Count.PercentOf(total.Count) ?? 0
            };
            return new SuccessResultModel<TotalEarningsStatisticViewModel>(model);
        }

        /// <summary>
        /// Get order statistic
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private async Task<ResultModel<OrderReceivedStatisticViewModel>> GetOrderReceivedStatisticAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null) return new InvalidParametersResultModel<OrderReceivedStatisticViewModel>();
            var total = await _orderDbContext.Orders.CountAsync();
            var inPeriod = await _orderDbContext.Orders
                .AsNoTracking()
                .CountAsync(x => x.Created >= startDate && x.Created <= endDate);
            var model = new OrderReceivedStatisticViewModel
            {
                TotalOrderReceived = inPeriod,
                Percentage = inPeriod.PercentOf(total)
            };
            return new SuccessResultModel<OrderReceivedStatisticViewModel>(model);
        }

        #region Helpers

        /// <summary>
        /// Parse setting
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string ParseSettingValue(object value, CommerceSettingType type)
        {
            var response = string.Empty;

            switch (type)
            {
                case CommerceSettingType.Text:
                case CommerceSettingType.Number:
                    response = value.ToString();
                    break;
                case CommerceSettingType.Object:
                case CommerceSettingType.Array:
                    response = value.SerializeAsJson();
                    break;
            }

            return response;
        }

        #endregion
    }
}