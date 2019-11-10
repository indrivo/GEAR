using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Enums;
using GR.ECommerce.Payments.Abstractions.Models;
using GR.Orders.Abstractions.Models;
using Microsoft.EntityFrameworkCore;
using GR.Orders.Abstractions;

namespace GR.ECommerce.Products.Services
{
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

        #endregion

        public PaymentService(IPaymentContext paymentContext, IOrderProductService<Order> orderProductService)
        {
            _paymentContext = paymentContext;
            _orderProductService = orderProductService;
        }

        /// <summary>
        /// Get payments methods
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<PaymentMethod>>> GetActivePaymentMethodsAsync()
        {
            var response = new ResultModel<IEnumerable<PaymentMethod>>();
            var methods = await _paymentContext.PaymentMethods.Where(x => x.IsEnabled).ToListAsync();
            response.IsSuccess = true;
            response.Result = methods;
            return response;
        }

        /// <summary>
        /// Get payments methods
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<PaymentMethod>>> GetAllPaymentMethodsAsync()
        {
            var response = new ResultModel<IEnumerable<PaymentMethod>>();
            var methods = await _paymentContext.PaymentMethods.ToListAsync();
            response.IsSuccess = true;
            response.Result = methods;
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
            var payment = await _paymentContext.Payments.Where(x => x.OrderId.Equals(orderId)).ToListAsync();
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
    }
}