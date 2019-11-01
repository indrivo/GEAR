using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Razor.ViewModels;

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
        Task<ResultModel<Cart>> GetCartByUserAsync();
        /// <summary>
        /// add new cart
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> AddToCardAsync(AddToCartViewModel model);
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