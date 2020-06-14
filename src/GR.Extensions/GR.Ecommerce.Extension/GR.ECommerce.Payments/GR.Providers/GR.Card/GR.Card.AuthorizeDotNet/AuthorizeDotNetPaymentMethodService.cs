using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using GR.Card.Abstractions;
using GR.Card.Abstractions.Helpers;
using GR.Card.Abstractions.Models;
using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Encryption;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Validators;
using GR.ECommerce.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Abstractions.Helpers.Responses;
using GR.Identity.Profile.Abstractions;
using GR.Identity.Profile.Abstractions.Models.AddressModels;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;

namespace GR.Card.AuthorizeDotNet
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class AuthorizeDotNetPaymentMethodService : ICardPayPaymentMethodService
    {
        #region Injectable

        /// <summary>
        /// Inject options
        /// </summary>
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly IWritableOptions<CardSettingsViewModel> _options;

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

        #endregion

        public AuthorizeDotNetPaymentMethodService(IWritableOptions<CardSettingsViewModel> options, IOrderProductService<Order> orderProductService, IPaymentService paymentService, IUserManager<GearUser> userManager, IUserAddressService userAddressService)
        {
            _options = options;
            _orderProductService = orderProductService;
            _paymentService = paymentService;
            _userManager = userManager;
            _userAddressService = userAddressService;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = _options.Value.IsSandbox
                ? AuthorizeNet.Environment.SANDBOX
                : AuthorizeNet.Environment.PRODUCTION;

            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType
            {
                name = _options.Value.ApiKey,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _options.Value.TransactionKey,
            };
        }

        /// <summary>
        /// Pay with authorize.net
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> PayOrderAsync([Required] OrderCreditCardPayViewModel model)
        {
            var state = ModelValidator.IsValid<CreditCardPayViewModel, Guid>(model);
            if (!state.IsSuccess) return state;
            var result = new ResultModel<Guid>();
            var isValidCardInfo = CreditCardValidator.IsCreditCardInfoValid(model);
            if (!isValidCardInfo)
            {
                result.AddError("Invalid card info, please provide valid card info");
                return result;
            }

            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess)
            {
                result.AddError("User not Found");
                return result;
            }
            var user = userRequest.Result;

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

            var addressRequest = await _userAddressService.GetAddressByIdAsync(order.BillingAddress);
            var address = addressRequest.IsSuccess ? addressRequest.Result : new Address();

            if (!user.Id.Equals(order.UserId))
            {
                result.AddError("This order was created by another person");
                return result;
            }

            await _orderProductService.ChangeOrderStateAsync(model.OrderId, OrderState.PendingPayment);

            var creditCard = new creditCardType
            {
                cardNumber = model.CardNumber,
                expirationDate = model.ExpirationDate,
                cardCode = model.CardCode
            };

            var billingAddress = new customerAddressType
            {
                firstName = user.FirstName,
                lastName = user.LastName,
                address = address.AddressLine1,
                city = address.StateOrProvince.Name,
                zip = address.ZipCode,
                country = address.Country.Name,
                email = user.Email.ToLowerInvariant(),
                phoneNumber = user.PhoneNumber
            };

            //standard api call to retrieve response
            var paymentType = new paymentType
            {
                Item = creditCard
            };

            var itemId = 0;
            // Add line Items
            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),
                currencyCode = order.Currency?.Code,
                amount = order.Total,
                payment = paymentType,
                billTo = billingAddress,
                lineItems = order.ProductOrders.Select(productOrder => new lineItemType
                {
                    itemId = (++itemId).ToString(),
                    name = productOrder.Product.DisplayName,
                    quantity = productOrder.Amount,
                    unitPrice = productOrder.FinalPrice
                }).ToArray()
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            var payment = new Payment
            {
                PaymentMethodId = CreditCardResources.CreditCardProvider,
                PaymentStatus = PaymentStatus.Failed,
                Total = order.Total,
                UserId = userRequest.Result.Id
            };

            // validate response
            if (response != null)
            {
                var transactionDescription = new StringBuilder();
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {
                        transactionDescription.AppendLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        transactionDescription.AppendLine("Response Code: " + response.transactionResponse.responseCode);
                        transactionDescription.AppendLine("Message Code: " + response.transactionResponse.messages[0].code);
                        transactionDescription.AppendLine("Description: " + response.transactionResponse.messages[0].description);
                        transactionDescription.AppendLine("Success, Auth Code : " + response.transactionResponse.authCode);
                        payment.GatewayTransactionId = response.transactionResponse.transId;
                        payment.PaymentStatus = PaymentStatus.Succeeded;
                        result.IsSuccess = true;
                    }
                    else
                    {
                        transactionDescription.AppendLine("Failed Transaction.");
                        if (response.transactionResponse.errors != null)
                        {
                            transactionDescription.AppendLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            transactionDescription.AppendLine("Error message: " + response.transactionResponse.errors[0].errorText);
                            result.AddError(response.transactionResponse.errors[0].errorCode, response.transactionResponse.errors[0].errorText);
                        }
                    }
                }
                else
                {
                    transactionDescription.AppendLine("Failed Transaction.");
                    if (response.transactionResponse?.errors != null)
                    {
                        transactionDescription.AppendLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        transactionDescription.AppendLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        result.AddError(response.transactionResponse.errors[0].errorCode, response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        transactionDescription.AppendLine("Error Code: " + response.messages.message[0].code);
                        transactionDescription.AppendLine("Error message: " + response.messages.message[0].text);
                        result.AddError(response.messages.message[0].code, response.messages.message[0].text);
                    }
                }

                payment.FailureMessage = transactionDescription.ToString();
                var nextOrderState = payment.PaymentStatus == PaymentStatus.Succeeded
                    ? OrderState.PaymentReceived
                    : OrderState.PaymentFailed;

                var orderStateChanged = await _orderProductService.ChangeOrderStateAsync(model.OrderId, nextOrderState);
                var paymentMake = await _paymentService.AddPaymentAsync(model.OrderId, payment);
                result = result.JoinResults(new List<ResultModel>
                {
                    orderStateChanged, paymentMake
                });
                result.Result = model.OrderId;
            }
            else
            {
                result.AddError("Something went wrong, try again");
            }

            if (model.SaveCreditCard) await SaveCardAsync(user, model);

            return result;
        }

        /// <summary>
        /// Save credit card
        /// </summary>
        /// <param name="user"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SaveCardAsync(GearUser user, CreditCardPayViewModel card)
        {
            var serializedCard = card.Is<CreditCardPayViewModel>()?.SerializeAsJson();
            var key = GenerateKey(user);
            var encrypted = EncryptHelper.Encrypt(serializedCard, key);
            var saveRequest = await _userManager.UserManager.SetAuthenticationTokenAsync(user, CreditCardResources.CreditCardProvider, "card", encrypted);
            return saveRequest.ToResultModel();
        }

        /// <summary>
        /// Get saved card
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<CreditCardPayViewModel>> GetSavedCreditCardAsync()
        {
            var result = new ResultModel<CreditCardPayViewModel>();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new UserNotFoundResult<CreditCardPayViewModel>();
            var user = userRequest.Result;

            var key = GenerateKey(user);
            var cardHash = await _userManager.UserManager.GetAuthenticationTokenAsync(user, CreditCardResources.CreditCardProvider, "card");
            if (cardHash.IsNullOrEmpty())
            {
                result.AddError("No credit card saved");
                return result;
            }

            var serializedString = EncryptHelper.Decrypt(cardHash, key);
            var card = serializedString.Deserialize<CreditCardPayViewModel>();

            result.IsSuccess = card != null;
            result.Result = card;
            return result;
        }

        #region Helpers

        private static string GenerateKey(GearUser user)
        {
            return $"key_{user.Id}_{user.UserName}";
        }

        #endregion
    }
}