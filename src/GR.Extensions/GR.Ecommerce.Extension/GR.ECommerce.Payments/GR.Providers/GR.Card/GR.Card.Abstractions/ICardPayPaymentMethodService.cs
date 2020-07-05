using System;
using System.Collections.Generic;
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
        Task<ResultModel<IEnumerable<CreditCardPayViewModel>>> GetSavedCreditCardsAsync();

        /// <summary>
        /// Get saved cards for specific users
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<CreditCardPayViewModel>>> GetSavedCreditCardsAsync(Guid userId);

        /// <summary>
        /// Get hidden cards
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<HiddenCreditCardPayViewModel>>> GetHiddenCardsAsync();

        /// <summary>
        /// Get credit card by id
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        Task<ResultModel<CreditCardPayViewModel>> GetCardByIdAsync(Guid cardId);

        /// <summary>
        /// Get card by id for specific user
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel<CreditCardPayViewModel>> GetCardByIdAsync(Guid cardId, Guid userId);

        /// <summary>
        /// Pay order with saved card 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel<Guid>> PayOrderAsyncWithExistentCardAsync([Required] OrderWithSavedCreditCardPayViewModel model);

        /// <summary>
        /// Remove credit card
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveCreditCardAsync(Guid cardId);

        /// <summary>
        /// Get default credit card
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<CreditCardPayViewModel>> GetDefaultCardAsync();

        /// <summary>
        /// Get default card for specific user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ResultModel<CreditCardPayViewModel>> GetDefaultCardAsync(Guid userId);

        /// <summary>
        /// Set default credit card
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        Task<ResultModel> SetDefaultCardAsync(Guid cardId);
    }
}