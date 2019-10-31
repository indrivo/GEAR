using GR.ECommerce.Abstractions;
using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Products.Services
{
    public class CartService : ICartService
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly ICommerceContext _context;

        #endregion

        public CartService(ICommerceContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get cart by id
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Cart>> GetCartByIdAsync(Guid? cartId)
        {
            var response = new ResultModel<Cart>();
            if (cartId == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Cart not found!"));
                return response;
            }

            var cart = await _context.Carts
                .Include(x => x.CartItems)
                .ThenInclude(x => x.Product)
                .Include(x => x.CartItems)
                .ThenInclude(x => x.ProductVariation)
                .FirstOrDefaultAsync(x => x.Id.Equals(cartId));

            if (cart == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Cart not found!"));
                return response;
            }

            response.IsSuccess = true;
            response.Result = cart;
            return response;
        }
    }
}
