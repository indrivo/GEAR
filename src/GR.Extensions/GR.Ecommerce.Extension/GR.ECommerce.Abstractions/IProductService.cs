using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.Models.Currencies;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;

namespace GR.ECommerce.Abstractions
{
    public interface IProductService<TProduct> where TProduct : Product
    {
        /// <summary>
        /// Get all products
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<TProduct>>> GetAllProducts(Func<TProduct, bool> predicate = null);

        /// <summary>
        /// Get product bu id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<ResultModel<Product>> GetProductByIdAsync(Guid? productId);

        /// <summary>
        /// Get subscription plans
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<SubscriptionPlanViewModel>>> GetSubscriptionPlansAsync();

        /// <summary>
        /// Get global currency
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<Currency>> GetGlobalCurrencyAsync();

        /// <summary>
        /// Set global currency for products
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<ResultModel<Currency>> SetGlobalCurrencyAsync(string code);

        /// <summary>
        /// Get all currencies
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Currency>>> GetAllCurrenciesAsync();

        /// <summary>
        /// Get setting by key
        /// </summary>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<ResultModel<TOutput>> GetSettingAsync<TOutput>(string key) where TOutput : class;

        /// <summary>
        /// Add or update setting
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<ResultModel> AddOrUpdateSettingAsync(string key, object value, CommerceSettingType type = CommerceSettingType.Text);

        /// <summary>
        /// Get price by variation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> GetPriceByVariationAsync(ProductPriceVariationViewModel model);

        /// <summary>
        /// Remove attribute
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveAttributeAsync(Guid? productId, Guid? attributeId);

        /// <summary>
        /// Get commerce general statistic
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<ResultModel<SalesStatisticViewModel>> GetCommerceGeneralStatisticAsync(DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// Get report for year
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<Dictionary<int, object>>> GetYearReportAsync();
    }
}