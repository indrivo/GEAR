using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions.Models;

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
    }
}