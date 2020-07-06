using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.ECommerce.Payments.Abstractions.ViewModels;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Infrastructure.Services
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    [Documentation("Basic implementation of payment service")]
    public class PaymentService : IPaymentService
    {
        #region Injectable

        /// <summary>
        /// Inject payment service
        /// </summary>
        private readonly IPaymentContext _paymentContext;

        /// <summary>
        /// Inject order service
        /// </summary>
        private readonly IOrderProductService<Order> _orderProductService;

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        public PaymentService(IPaymentContext paymentContext, IOrderProductService<Order> orderProductService, IMapper mapper)
        {
            _paymentContext = paymentContext;
            _orderProductService = orderProductService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get payments methods
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<PaymentMethodViewModel>>> GetActivePaymentMethodsAsync()
        {
            var response = new ResultModel<IEnumerable<PaymentMethodViewModel>>();
            var methods = await _paymentContext.PaymentMethods
                .AsNoTracking()
                .Where(x => x.IsEnabled).ToListAsync();
            var mapped = _mapper.Map<IEnumerable<PaymentMethodViewModel>>(methods);
            response.IsSuccess = true;
            response.Result = mapped;
            return response;
        }

        /// <summary>
        /// Get payments methods
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<PaymentMethodViewModel>>> GetAllPaymentMethodsAsync()
        {
            var response = new ResultModel<IEnumerable<PaymentMethodViewModel>>();
            var methods = await _paymentContext
                .PaymentMethods
                .AsNoTracking()
                .ToListAsync();

            var mapped = _mapper.Map<IEnumerable<PaymentMethodViewModel>>(methods);
            response.IsSuccess = true;
            response.Result = mapped;
            return response;
        }

        /// <summary>
        /// Get payment for order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<Payment>>> GetPaymentsForOrderAsync(Guid? orderId)
        {
            var response = new ResultModel<IEnumerable<Payment>>();
            if (orderId == null) return new NotFoundResultModel<IEnumerable<Payment>>();
            var payment = await _paymentContext.Payments
                .AsNoTracking()
                .Where(x => x.OrderId.Equals(orderId)).ToListAsync();
            if (payment == null) return new NotFoundResultModel<IEnumerable<Payment>>();
            response.IsSuccess = true;
            response.Result = payment;
            return response;
        }

        /// <summary>
        /// Is order payed
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<ResultModel> IsOrderPayedAsync(Guid? orderId)
        {
            var response = new ResultModel();
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return orderRequest.ToBase();
            var paymentRequest = await GetPaymentsForOrderAsync(orderId);
            if (!paymentRequest.IsSuccess) return paymentRequest.ToBase();
            if (paymentRequest.Result.Any(x => x.PaymentStatus == PaymentStatus.Succeeded))
                response.IsSuccess = true;
            return response;
        }

        /// <summary>
        /// Is payment method supported
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> IsPaymentMethodSupportedAsync(string method)
        {
            var match = await _paymentContext.PaymentMethods.AnyAsync(x => x.Name.Equals(method) && x.IsEnabled);
            return new ResultModel { IsSuccess = match };
        }

        /// <summary>
        /// Add payment
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="payment"></param>
        /// <returns></returns>
        public async Task<ResultModel> AddPaymentAsync(Guid? orderId, Payment payment)
        {
            var orderRequest = await _orderProductService.GetOrderByIdAsync(orderId);
            if (!orderRequest.IsSuccess) return orderRequest.ToBase();
            payment.OrderId = orderId.GetValueOrDefault();
            await _paymentContext.Payments.AddAsync(payment);
            return await _paymentContext.PushAsync();
        }

        /// <summary>
        /// Enable payment method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ActivatePaymentMethodAsync(string id)
        {
            if (id.IsNullOrEmpty()) return new InvalidParametersResultModel();
            var paymentMethod = await _paymentContext.PaymentMethods
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name.Equals(id));
            if (paymentMethod == null) return new NotFoundResultModel();
            if (paymentMethod.IsEnabled) return new SuccessResultModel<object>().ToBase();
            paymentMethod.IsEnabled = true;
            _paymentContext.PaymentMethods.Update(paymentMethod);
            return await _paymentContext.PushAsync();
        }

        /// <summary>
        /// Disable payment method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DisablePaymentMethodAsync(string id)
        {
            if (id.IsNullOrEmpty()) return new InvalidParametersResultModel();
            var paymentMethod = await _paymentContext.PaymentMethods
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name.Equals(id));
            if (paymentMethod == null) return new NotFoundResultModel();
            if (!paymentMethod.IsEnabled) return new SuccessResultModel<object>().ToBase();
            paymentMethod.IsEnabled = false;
            _paymentContext.PaymentMethods.Update(paymentMethod);
            return await _paymentContext.PushAsync();
        }
    }
}