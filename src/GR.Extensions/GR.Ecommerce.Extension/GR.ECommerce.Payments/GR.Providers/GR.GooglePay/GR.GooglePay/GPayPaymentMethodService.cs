using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Validators;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.GooglePay.Abstractions;
using GR.GooglePay.Abstractions.Enums;
using GR.GooglePay.Abstractions.Helpers;
using GR.GooglePay.Abstractions.Models;
using GR.GooglePay.Abstractions.ViewModels;
using GR.Identity.Abstractions;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using Microsoft.Extensions.Options;

namespace GR.GooglePay
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class GPayPaymentMethodService : IGPayPaymentMethodService
    {
        #region Injectable

        /// <summary>
        /// Inject service options
        /// </summary>
        private readonly IOptionsSnapshot<GPaySettingsViewModel> _gPayOptions;

        /// <summary>
        /// Inject order service
        /// </summary>
        private readonly IOrderProductService<Order> _orderProductService;

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject user address
        /// </summary>
        private readonly IUserAddressService _userAddressService;

        public GPayPaymentMethodService(IOptionsSnapshot<GPaySettingsViewModel> gPayOptions, IOrderProductService<Order> orderProductService, IPaymentService paymentService, IUserManager<GearUser> userManager, IUserAddressService userAddressService)
        {
            _gPayOptions = gPayOptions;
            _orderProductService = orderProductService;
            _paymentService = paymentService;
            _userManager = userManager;
            _userAddressService = userAddressService;
        }

        #endregion

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ResultModel<GPayTransactionInfo>> CreatePaymentAsync(Guid? orderId)
        {
            var result = new ResultModel<GPayTransactionInfo>();
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess)
            {
                result.AddError("Order not found");
                return result;
            }

            var isPayedRequest = await _paymentService.IsOrderPayedAsync(orderId);
            if (isPayedRequest.IsSuccess)
            {
                result.AddError("Order was payed before, Check your orders");
                return result;
            }

            var wasInvoicedRequest = await _orderProductService.ItWasInTheStateAsync(orderId, OrderState.Invoiced);
            if (!wasInvoicedRequest.Result)
            {
                result.AddError("Invoice was not created, before calling this api, you must create invoice");
                return result;
            }

            var order = orderRequest.Result;
            var addressRequest = await _userAddressService.GetAddressByIdAsync(order.BillingAddress);
            var address = addressRequest.IsSuccess ? addressRequest.Result : new Address();
            var transactionResponse = new GPayTransactionInfo();
            var tax = 0.0M;
            transactionResponse.DisplayItems.Add(new GPayPaymentItem
            {
                Label = "Tax",
                Type = GPayItemType.TAX.ToString(),
                Price = tax.ToString("N2")
            });

            var price = order.ProductOrders
                .Sum(item => item.AmountFinalPriceWithOutDiscount);

            transactionResponse.DisplayItems.Add(new GPayPaymentItem
            {
                Label = "Subtotal",
                Type = GPayItemType.SUBTOTAL.ToString(),
                Price = price.ToString("N2")
            });

            transactionResponse.TotalPrice = (price + tax).ToString("N2");
            transactionResponse.TotalPriceLabel = "Total";
            transactionResponse.TotalPriceStatus = "FINAL";
            transactionResponse.CurrencyCode = order.Currency?.Code;
            transactionResponse.CountryCode = address?.CountryId;
            await _orderProductService.ChangeOrderStateAsync(orderId, OrderState.PendingPayment);
            result.Result = transactionResponse;
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Execute payment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> ExecutePaymentAsync(GPayPaymentExecuteViewModel model)
        {
            var state = ModelValidator.IsValid<GPayPaymentExecuteViewModel, Guid>(model);
            if (!state.IsSuccess) return state;
            var result = new ResultModel<Guid>();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess)
            {
                result.AddError("User not Found");
                return result;
            }
            var orderRequest = await _orderProductService.GetOrderByIdAsync(model.OrderId);
            if (!orderRequest.IsSuccess)
            {
                result.AddError("Order not Found");
                return result;
            }

            var isPayedRequest = await _paymentService.IsOrderPayedAsync(model.OrderId);
            if (isPayedRequest.IsSuccess)
            {
                result.AddError("Order was payed before, Check your orders");
                return result;
            }

            var wasInvoicedRequest = await _orderProductService.ItWasInTheStateAsync(model.OrderId, OrderState.Invoiced);
            if (!wasInvoicedRequest.Result)
            {
                result.AddError("Invoice was not created, before calling this api, you must create invoice");
                return result;
            }

            var order = orderRequest.Result;

            var userId = _userManager.FindUserIdInClaims().Result;
            if (!userId.Equals(order.UserId))
            {
                result.AddError("This order was created by another person");
                return result;
            }

            var googleTransactionId = model.PaymentMethodData.TokenizationData.Token;
            if (_gPayOptions.Value.IsSandbox) googleTransactionId = $"google_sandbox-{Guid.NewGuid()}";

            var payment = new Payment
            {
                PaymentMethodId = GPayResources.GPayProvider,
                GatewayTransactionId = googleTransactionId,
                FailureMessage = model.SerializeAsJson(),
                PaymentStatus = PaymentStatus.Succeeded,
                Total = order.Total,
                UserId = userRequest.Result.Id
            };

            var orderStateChanged = await _orderProductService.ChangeOrderStateAsync(model.OrderId, OrderState.PaymentReceived);
            var paymentMake = await _paymentService.AddPaymentAsync(model.OrderId, payment);
            result = result.JoinResults(new List<ResultModel>
            {
                orderStateChanged, paymentMake
            });

            if (result.IsSuccess) result.Result = model.OrderId;
            return result;
        }
    }
}