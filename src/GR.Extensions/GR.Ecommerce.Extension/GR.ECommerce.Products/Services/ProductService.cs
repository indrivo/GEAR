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

        public ProductService(ICommerceContext commerceContext)
        {
            _commerceContext = commerceContext;
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