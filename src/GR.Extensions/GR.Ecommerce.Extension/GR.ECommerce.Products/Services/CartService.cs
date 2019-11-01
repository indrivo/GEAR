using GR.ECommerce.Abstractions;
using System;
using System.Collections;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using GR.Identity.Abstractions;
using System.Linq;
using GR.Core.Extensions;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using GR.ECommerce.Razor.ViewModels;

namespace GR.ECommerce.BaseImplementations.Data.Services
{
    public class CartService : ICartService
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly ICommerceContext _context;

        /// <summary>
        /// Inject user
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        #endregion

        public CartService(ICommerceContext context, IUserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        /// <summary>
        /// Get cart by login user
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<Cart>> GetCartByUserAsync()
        {
            var response = new ResultModel<Cart>();

            var user = _userManager.GetCurrentUserAsync();
            var cart = await _context.Carts
                .Include(i => i.CartItems)
                .ThenInclude(i => i.Product)
                .ThenInclude(i=> i.ProductPrices)
                .FirstOrDefaultAsync(x => x.UserId == user.Result.Result.Id.ToGuid());

            if (cart == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Cart not found!"));
                return response;
            }

            response.IsSuccess = true;
            response.Result = cart;
            return response;
        }


        /// <summary>
        /// save new item to cart
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel> AddToCardAsync(AddToCartViewModel model)
        {
            var resultModel = new ResultModel();
            var product = _context.Products.FirstOrDefault(x => x.Id == model.ProductId);
            var user = _userManager.GetCurrentUserAsync();

            if (product != null)
            {
                var cart = _context.Carts.FirstOrDefault(x => x.UserId == user.Result.Result.Id.ToGuid());

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = user.Result.Result.Id.ToGuid(),
                    };

                    await _context.Carts.AddAsync(cart);
                }

                var cartItem = _context.CartItems.FirstOrDefault(x => x.ProductId == model.ProductId && x.CartId == cart.Id && x.ProductVariationId == model.VariationId);

                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        ProductId = model.ProductId,
                        Amount = model.Quantity,
                        CartId = cart.Id,
                    };

                    await _context.CartItems.AddAsync(cartItem);
                }
                else
                {
                    cartItem.Amount += model.Quantity;
                    _context.CartItems.Update(cartItem);
                }

                var result =  await _context.SaveChangesAsync();
                resultModel.IsSuccess = true;
                resultModel.Result = result;
                return resultModel;
            }
            return resultModel;

        }


        /// <summary>
        /// Delete cart by id
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <returns></returns>
        public async Task<ResultModel> DeleteCartItemAsync([Required] Guid? cartItemId)
        {

            var resultModel = new ResultModel();
            if (cartItemId is null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                return resultModel;
            }

            var cartItem = _context.CartItems.FirstOrDefault(x => x.Id == cartItemId);

            if (cartItem == null) return resultModel;

            _context.CartItems.Remove(cartItem);
            var dbResult = await _context.PushAsync();
            return dbResult;
        }

        /// <summary>
        /// set new quantity for cart item
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public async Task<ResultModel> SetQuantityAsync([Required] Guid? cartItemId, [Required] int? quantity)
        {
            var resultModel = new ResultModel();
            if (cartItemId is null || quantity is null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                return resultModel;
            }

            var cartItem = _context.CartItems.FirstOrDefault(x => x.Id == cartItemId);
            if (cartItem == null) return resultModel;

            cartItem.Amount = (int)quantity;

            _context.CartItems.Update(cartItem);
            var dbResult = await _context.PushAsync();
            dbResult.Result = new { totalPrice = GetTotalPrice(cartItem.CartId) };
            return dbResult;
        }

        /// <summary>
        /// get total price 
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        private double GetTotalPrice(Guid cartId)
        {
            var cart = _context.Carts.Include(i => i.CartItems).ThenInclude(i => i.Product).FirstOrDefault(x => x.Id == cartId);

            if (cart != null)
            {
                return cart.CartItems.Sum(s => s.Product?.PriceWithDiscount * s.Amount) ?? 0;
            }

            return 0;
        }
    }
}
