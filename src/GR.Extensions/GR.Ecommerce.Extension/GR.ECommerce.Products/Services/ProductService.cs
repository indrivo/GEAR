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
                .Include(x => x.Currency)
                .FirstOrDefaultAsync();
            if (settings != null) return new SuccessResultModel<Currency>(settings.Currency);

            return await SetGlobalCurrencyAsync(CommerceResources.SystemCurrencies.USD);
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

            var settings = await _commerceContext.CommerceSettings
                .Include(x => x.Currency)
                .FirstOrDefaultAsync();
            if (settings != null)
            {
                settings.CurrencyId = currency.Code;
                _commerceContext.CommerceSettings.Update(settings);
            }
            else
            {
                await _commerceContext.CommerceSettings.AddAsync(new CommerceSetting
                {
                    CurrencyId = currency.Code
                });
            }

            await _commerceContext.PushAsync();
            return new SuccessResultModel<Currency>(currency);
        }

        /// <summary>
        /// Get all currencies
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Currency>>> GetAllCurrenciesAsync()
        {
            var currencies = await _commerceContext.Currencies.ToListAsync();
            return new SuccessResultModel<IEnumerable<Currency>>(currencies);
        }
    }
}
