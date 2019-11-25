using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
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
    }
}