using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Card.Abstractions.Models;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions;
using GR.Identity.Abstractions;

namespace GR.Card.Abstractions
{
    public interface ICardPayPaymentMethodService : IPaymentMethodService
    {
        /// <summary>
        /// Pay order with card
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> PayOrderAsync([Required] OrderCreditCardPayViewModel model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        Task<ResultModel> SaveCardAsync(GearUser user, CreditCardPayViewModel card);

        /// <summary>
        /// Get saved card
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<CreditCardPayViewModel>> GetSavedCreditCardAsync();
    }
}