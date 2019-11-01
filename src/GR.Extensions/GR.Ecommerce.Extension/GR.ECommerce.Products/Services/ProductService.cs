using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Products.Services
{
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
                .FirstOrDefaultAsync(x => x.Id.Equals(productId));
            if (product == null) return new NotFoundResultModel<Product>();
            var response = new ResultModel<Product>
            {
                IsSuccess = true,
                Result = product
            };

            return response;
        }
    }
}
