using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.Models.Currencies;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;

namespace GR.ECommerce.Abstractions
{
    public interface IProductService<TProduct> where TProduct : Product
    {
        /// <summary>
        /// Context
        /// </summary>
        ICommerceContext Context { get; }

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
        /// Add product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> AddProductAsync([Required] Product product);

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

        /// <summary>
        /// get product by min number value by attribute name 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        Task<ResultModel<Product>> GetProductByAttributeMinNumberValueAsync(string attribute);

        #region ProductTypes

        /// <summary>
        /// Add new product type
        /// </summary>
        /// <param name="productType"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> AddProductTypeAsync(ProductType productType);

        #endregion

        #region Brands

        /// <summary>
        /// Add brand
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> AddBrandAsync(Brand brand);

        /// <summary>
        /// Get all brands
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Brand>>> GetAllBrandsAsync();

        #endregion

        #region Filters

        /// <summary>
        /// Get products by filters
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Product>>> GetProductsWithFiltersAsync([Required] ProductsFilterRequest model);

        /// <summary>
        /// Get attributes for filters
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<FilterAttributeValuesViewModel>>> GetAttributesForFiltersAsync();

        #endregion

        #region Product variations

        /// <summary>
        /// Get price by variation
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> GetPriceByVariationAsync(ProductPriceVariationViewModel model);

        /// <summary>
        /// Remove variation option
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="variationId"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveVariationOptionAsync([Required] Guid? productId, [Required] Guid? variationId);

        #endregion
    }
}