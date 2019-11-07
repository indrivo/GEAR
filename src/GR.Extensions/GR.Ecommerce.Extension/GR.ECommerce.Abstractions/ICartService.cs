using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.ViewModels.CartViewModels;

namespace GR.ECommerce.Abstractions
{
    public interface ICartService
    {
        /// <summary>
        /// Get cart by id
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        Task<ResultModel<Cart>> GetCartByIdAsync(Guid? cartId);
        /// <summary>
        /// get cart by user login
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<AddToCartViewModel>> GetCartByUserAsync();

        /// <summary>
        /// get cartItem by id
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <returns></returns>
        Task<ResultModel<CartItem>> GetCartItemByIdAsync(Guid? cartItemId);

        /// <summary>
        /// add new cart
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<CartItem>> AddToCardAsync(AddToCartViewModel model);
        /// <summary>
        /// delete cart items
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteCartItemAsync([Required] Guid? cartItemId);
        /// <summary>
        /// set qauntity from cart items
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<ResultModel> SetQuantityAsync([Required] Guid? cartItemId, [Required] int? quantity);
    }
}