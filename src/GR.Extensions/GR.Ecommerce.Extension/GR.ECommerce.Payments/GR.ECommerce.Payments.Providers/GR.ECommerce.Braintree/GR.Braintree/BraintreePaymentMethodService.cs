using System;
using System.Linq;
using System.Threading.Tasks;
using Braintree;
using GR.Braintree.Abstractions;
using GR.Braintree.Abstractions.Models;
using GR.Braintree.Extensions;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.Identity.Abstractions;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using B = Braintree;

namespace GR.Braintree
{
    public class BraintreePaymentMethodService : IBraintreePaymentMethod
    {
        #region Inject

        /// <summary>
        /// Inject Braintree options
        /// </summary>
        private readonly IOptionsSnapshot<BraintreeConfiguration> _options;

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

        #endregion

        public BraintreePaymentMethodService(IOptionsSnapshot<BraintreeConfiguration> options, IOrderProductService<Order> orderProductService, IPaymentService paymentService, IUserManager<GearUser> userManager)
        {
            _options = options;
            _orderProductService = orderProductService;
            _paymentService = paymentService;
            _userManager = userManager;
        }

        /// <summary>
        /// Create gateway
        /// </summary>
        /// <returns></returns>
        public virtual IBraintreeGateway CreateGateway()
        {
            if (_options.Value == null) throw new Exception("No Braintree configuration");
            var conf = _options.Value;
            return new BraintreeGateway(conf.IsProduction ? B.Environment.PRODUCTION : B.Environment.SANDBOX, conf.MerchantId, conf.PublicKey, conf.PrivateKey);
        }

        /// <summary>
        /// Pay the order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        public async Task<ResultModel<BraintreeSuccessPaymentResult>> ChargeAsync(Guid? orderId, string nonce)
        {
            var result = new ResultModel<BraintreeSuccessPaymentResult>();
            if (orderId == null) return new InvalidParametersResultModel<BraintreeSuccessPaymentResult>("Order not found!");
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return result;

            var isPayedRequest = await _paymentService.IsOrderPayedAsync(orderId);
            if (isPayedRequest.IsSuccess) return result;
            var order = orderRequest.Result;
            var user = await _userManager.UserManager.FindByIdAsync(order.UserId.ToString());
            var orderState = order.OrderState;

            var gateway = CreateGateway();

            var payment = new Payment
            {
                PaymentMethodId = "Braintree",
                PaymentStatus = PaymentStatus.Failed,
                Total = order.Total,
                UserId = user.Id.ToGuid()
            };

            var request = new TransactionRequest
            {
                Amount = order.Total,
                PaymentMethodNonce = nonce,
                OrderId = order.Id.ToString(),
                //LineItems = lineItemsRequest.ToArray(),
                //CustomerId = order.CustomerId.ToString(),
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true,
                    SkipAdvancedFraudChecking = false,
                    SkipCvv = false,
                    SkipAvs = false,
                }
            };
            var braintreeResult = gateway.Transaction.Sale(request);
            var transactionId = string.Empty;
            if (braintreeResult.IsSuccess())
            {
                var transaction = braintreeResult.Target;
                payment.GatewayTransactionId = transaction.Id;
                transactionId = transaction.Id;
                payment.PaymentStatus = PaymentStatus.Succeeded;
                payment.FailureMessage = braintreeResult.Message;
                orderState = OrderState.PaymentReceived;
            }
            else
            {
                orderState = OrderState.PaymentFailed;
                payment.FailureMessage = braintreeResult.Message + "\n" + braintreeResult.Errors.ToGearErrors().Join();
            }


            var addPaymentRequest = await _paymentService.AddPaymentAsync(orderId, payment);
            if (addPaymentRequest.IsSuccess)
            {
                await _orderProductService.ChangeOrderStateAsync(orderId, orderState);
            }

            return braintreeResult.IsSuccess()
                ? new SuccessResultModel<BraintreeSuccessPaymentResult>(new BraintreeSuccessPaymentResult
                {
                    OrderId = orderId.Value,
                    TransactionId = transactionId
                })
                : new ResultModel<BraintreeSuccessPaymentResult> { Errors = braintreeResult.Errors.ToGearErrors().ToList() };
        }

        /// <summary>
        /// Get client token
        /// </summary>
        /// <returns></returns>
        public virtual async Task<string> GetClientToken()
        {
            var gateway = CreateGateway();
            return await gateway.ClientToken.GenerateAsync();
        }
    }
}
