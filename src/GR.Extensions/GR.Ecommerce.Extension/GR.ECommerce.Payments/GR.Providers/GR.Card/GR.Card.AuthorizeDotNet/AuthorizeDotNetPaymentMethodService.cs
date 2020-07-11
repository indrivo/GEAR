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
using GR.Core.Helpers.Responses;
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

        /// <summary>
        /// Inject configuration
        /// </summary>
        private readonly CreditCardsConfiguration _configuration;

        #endregion

        public AuthorizeDotNetPaymentMethodService(IWritableOptions<CardSettingsViewModel> options, IOrderProductService<Order> orderProductService, IPaymentService paymentService, IUserManager<GearUser> userManager, IUserAddressService userAddressService, CreditCardsConfiguration configuration)
        {
            _options = options;
            _orderProductService = orderProductService;
            _paymentService = paymentService;
            _userManager = userManager;
            _userAddressService = userAddressService;
            _configuration = configuration;
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
        /// Pay order with saved card
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> PayOrderAsyncWithExistentCardAsync([Required] OrderWithSavedCreditCardPayViewModel model)
        {
            var state = ModelValidator.IsValid<OrderWithSavedCreditCardPayViewModel, Guid>(model);
            if (!state.IsSuccess) return state;
            var choseCardRequest = await GetCardByIdAsync(model.CardId);
            if (!choseCardRequest.IsSuccess) return choseCardRequest.Map<Guid>();
            var card = choseCardRequest.Result;
            return await PayOrderAsync(new OrderCreditCardPayViewModel
            {
                CardNumber = card.CardNumber,
                Month = card.Month,
                Year = card.Year,
                Owner = card.Owner,
                CardCode = card.CardCode,
                OrderId = model.OrderId
            });
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
        /// Verify card
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> VerifyCardAsync(CreditCardPayViewModel card)
        {
            var state = ModelValidator.IsValid(card);
            if (!state.IsSuccess) return state;
            var result = new ResultModel();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess)
            {
                result.AddError("User not Found");
                return result;
            }
            var user = userRequest.Result;

            var sendMoneyRequest = await PayToBankAsync(user, card, _configuration.VerificationCardCurrencyCode, _configuration.VerificationCardValue);
            if (!sendMoneyRequest.IsSuccess) return sendMoneyRequest.ToBase();

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.voidTransaction.ToString(),    // refund type
                refTransId = sendMoneyRequest.Result
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();
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
                        result.IsSuccess = true;
                        result.Result = response.transactionResponse.transId;
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
            }
            else
            {
                result.AddError("Something went wrong, try again");
            }

            return result;
        }

        /// <summary>
        /// Pay to bank
        /// </summary>
        /// <param name="user"></param>
        /// <param name="card"></param>
        /// <param name="currency"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<string>> PayToBankAsync(GearUser user, CreditCardPayViewModel card, string currency, decimal value)
        {
            var result = new ResultModel<string>();
            var creditCard = new creditCardType
            {
                cardNumber = card.CardNumber,
                expirationDate = card.ExpirationDate,
                cardCode = card.CardCode
            };

            var addressRequest = await _userAddressService.GetDefaultAddressAsync(user.Id);
            if (!addressRequest.IsSuccess) return addressRequest.Map<string>();
            var address = addressRequest.Result;
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

            // Add line Items
            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),
                currencyCode = currency,
                amount = value,
                payment = paymentType,
                billTo = billingAddress
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

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
                        result.IsSuccess = true;
                        result.Result = response.transactionResponse.transId;
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
            }
            else
            {
                result.AddError("Something went wrong, try again");
            }

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
            var result = new ResultModel();
            var cards = new List<CreditCardPayViewModel>();
            var savedCardsRequest = await GetSavedCreditCardsAsync(user.Id);
            if (savedCardsRequest.IsSuccess) cards = savedCardsRequest.Result.ToList();
            if (cards.Select(x => x.CardNumber).Contains(card.CardNumber))
            {
                result.AddError("Card already exists");
                return result;
            }

            var newCard = card.Is<CreditCardPayViewModel>();
            newCard.CardId = Guid.NewGuid();
            cards.Add(newCard);

            return await UpdateCardsAsync(cards, user);
        }

        /// <summary>
        /// Get saved card
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<CreditCardPayViewModel>>> GetSavedCreditCardsAsync()
        {
            var result = new ResultModel<IEnumerable<CreditCardPayViewModel>>();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new UserNotFoundResult<IEnumerable<CreditCardPayViewModel>>();
            var user = userRequest.Result;

            var key = GenerateKey(user);
            var cardHash = await _userManager.UserManager.GetAuthenticationTokenAsync(user, CreditCardResources.CreditCardProvider, "cards");
            if (cardHash.IsNullOrEmpty())
            {
                result.AddError("No credit cards saved");
                return result;
            }

            var serializedString = EncryptHelper.Decrypt(cardHash, key);
            var cards = serializedString.Deserialize<IEnumerable<CreditCardPayViewModel>>();

            result.IsSuccess = cards != null;
            result.Result = cards;
            return result;
        }

        /// <summary>
        /// Get saved cards for specific user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<CreditCardPayViewModel>>> GetSavedCreditCardsAsync(Guid userId)
        {
            var result = new ResultModel<IEnumerable<CreditCardPayViewModel>>();
            var userRequest = await _userManager.FindUserByIdAsync(userId);
            if (!userRequest.IsSuccess) return new UserNotFoundResult<IEnumerable<CreditCardPayViewModel>>();
            var user = userRequest.Result;

            var key = GenerateKey(user);
            var cardHash = await _userManager.UserManager.GetAuthenticationTokenAsync(user, CreditCardResources.CreditCardProvider, "cards");
            if (cardHash.IsNullOrEmpty())
            {
                result.AddError("No credit cards saved");
                return result;
            }

            var serializedString = EncryptHelper.Decrypt(cardHash, key);
            var cards = serializedString.Deserialize<IEnumerable<CreditCardPayViewModel>>();

            result.IsSuccess = cards != null;
            result.Result = cards;
            return result;
        }

        /// <summary>
        /// Get card by id
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<CreditCardPayViewModel>> GetCardByIdAsync(Guid cardId)
        {
            var result = new ResultModel<CreditCardPayViewModel>();
            var cardsRequest = await GetSavedCreditCardsAsync();
            if (!cardsRequest.IsSuccess)
            {
                result.AddError("No credit card found");
                return result;
            }

            var cards = cardsRequest.Result.ToList();
            var card = cards.FirstOrDefault(x => x.CardId.Equals(cardId));
            if (card != null) return new SuccessResultModel<CreditCardPayViewModel>(card);

            result.AddError("No credit card found");
            return result;
        }

        /// <summary>
        /// Get card by id
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<CreditCardPayViewModel>> GetCardByIdAsync(Guid cardId, Guid userId)
        {
            var result = new ResultModel<CreditCardPayViewModel>();
            var cardsRequest = await GetSavedCreditCardsAsync(userId);
            if (!cardsRequest.IsSuccess)
            {
                result.AddError("No credit card found");
                return result;
            }

            var cards = cardsRequest.Result.ToList();
            var card = cards.FirstOrDefault(x => x.CardId.Equals(cardId));
            if (card != null) return new SuccessResultModel<CreditCardPayViewModel>(card);

            result.AddError("No credit card found");
            return result;
        }

        /// <summary>
        /// Get default card for current user
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<CreditCardPayViewModel>> GetDefaultCardAsync()
        {
            var result = new ResultModel<CreditCardPayViewModel>();
            var cardsRequest = await GetSavedCreditCardsAsync();
            if (!cardsRequest.IsSuccess)
            {
                result.AddError("No credit card founds");
                return result;
            }

            var cards = cardsRequest.Result.ToList();
            var card = cards.FirstOrDefault(x => x.IsDefault) ?? cards.FirstOrDefault();
            if (card != null) return new SuccessResultModel<CreditCardPayViewModel>(card);

            result.AddError("No credit card found");
            return result;
        }

        /// <summary>
        /// Get default card for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<CreditCardPayViewModel>> GetDefaultCardAsync(Guid userId)
        {
            var result = new ResultModel<CreditCardPayViewModel>();
            var cardsRequest = await GetSavedCreditCardsAsync(userId);
            if (!cardsRequest.IsSuccess)
            {
                result.AddError("No credit card founds");
                return result;
            }

            var cards = cardsRequest.Result.ToList();
            var card = cards.FirstOrDefault(x => x.IsDefault) ?? cards.FirstOrDefault();
            if (card != null) return new SuccessResultModel<CreditCardPayViewModel>(card);

            result.AddError("No credit card found");
            return result;
        }

        /// <summary>
        /// Remove credit card
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveCreditCardAsync(Guid cardId)
        {
            var result = new ResultModel();
            if (cardId == Guid.Empty) return new InvalidParametersResultModel($"{nameof(cardId)} is required");

            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new UserNotFoundResult<object>().ToBase();
            var user = userRequest.Result;
            var cardsRequest = await GetSavedCreditCardsAsync();
            if (!cardsRequest.IsSuccess)
            {
                result.AddError("No credit cards available");
                return result;
            }

            var cards = cardsRequest.Result.Where(x => x.CardId != cardId).ToList();
            return await UpdateCardsAsync(cards, user);
        }

        /// <summary>
        /// Set default card
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SetDefaultCardAsync(Guid cardId)
        {
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new UserNotFoundResult<object>().ToBase();
            var user = userRequest.Result;
            var result = new ResultModel();
            if (cardId == Guid.Empty) return new InvalidParametersResultModel($"{nameof(cardId)} is required");
            var cardsRequest = await GetSavedCreditCardsAsync(user.Id);
            if (!cardsRequest.IsSuccess)
            {
                result.AddError("No credit cards available");
                return result;
            }

            var cards = cardsRequest.Result.ToList();
            var existCard = cards.FirstOrDefault(x => x.CardId != null && x.CardId.Value == cardId);
            if (existCard == null)
            {
                result.AddError("The selected card was not found");
                return result;
            }

            foreach (var card in cards)
            {
                card.IsDefault = card.CardId == cardId;
            }
            return await UpdateCardsAsync(cards, user);
        }

        /// <summary>
        /// Get hidden cards 
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<HiddenCreditCardPayViewModel>>> GetHiddenCardsAsync()
        {
            var result = new ResultModel<IEnumerable<HiddenCreditCardPayViewModel>>();
            var cardsRequest = await GetSavedCreditCardsAsync();
            if (!cardsRequest.IsSuccess)
            {
                result.AddError("No credit cards available");
                return result;
            }

            var cards = cardsRequest.Result;

            result.Result = cards.Select(card => new HiddenCreditCardPayViewModel(card.CardNumber)
            {
                Owner = card.Owner,
                Month = card.Month,
                Year = card.Year,
                CardId = card.CardId,
                IsDefault = card.IsDefault
            }).ToList();
            result.IsSuccess = true;
            return result;
        }

        /// <summary>
        /// Add new card
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddNewCardAsync(CreditCardPayViewModel card)
        {
            var state = ModelValidator.IsValid(card);
            if (!state.IsSuccess) return state;
            var result = new ResultModel();
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return new UserNotFoundResult<object>().ToBase();
            var user = userRequest.Result;
            var cards = new List<CreditCardPayViewModel>();
            var savedCardsRequest = await GetSavedCreditCardsAsync();
            if (savedCardsRequest.IsSuccess) cards = savedCardsRequest.Result.ToList();
            if (cards.Select(x => x.CardNumber).Contains(card.CardNumber))
            {
                result.AddError("Card already exists");
                return result;
            }

            var newCard = card.Is<CreditCardPayViewModel>();
            newCard.CardId = Guid.NewGuid();

            var verifyResponse = await VerifyCardAsync(newCard);
            if (!verifyResponse.IsSuccess) return verifyResponse;
            cards.Add(newCard);

            return await UpdateCardsAsync(cards, user);
        }

        #region Helpers

        private static string GenerateKey(GearUser user)
        {
            return $"key_{user.Id}_{user.UserName}";
        }

        private async Task<ResultModel> UpdateCardsAsync(IEnumerable<CreditCardPayViewModel> cards, GearUser user)
        {
            var serializedCard = cards.SerializeAsJson();
            var key = GenerateKey(user);
            var encrypted = EncryptHelper.Encrypt(serializedCard, key);
            var saveRequest = await _userManager.UserManager.SetAuthenticationTokenAsync(user, CreditCardResources.CreditCardProvider, "cards", encrypted);
            return saveRequest.ToResultModel();
        }

        #endregion
    }
}