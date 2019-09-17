using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ST.Core.Helpers;
using ST.ECommerce.Abstractions.Models;

namespace ST.ECommerce.Abstractions
{
    public interface IProductRepository<TProduct> where TProduct : Product
    {
        /// <summary>
        /// Get all products
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<TProduct>>> GetAllProducts(Func<TProduct, bool> predicate = null);
    }
}