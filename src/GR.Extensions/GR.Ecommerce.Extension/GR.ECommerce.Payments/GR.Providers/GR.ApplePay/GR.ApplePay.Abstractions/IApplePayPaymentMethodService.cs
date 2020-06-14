using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using GR.ApplePay.Abstractions.Models;
using GR.ApplePay.Abstractions.ViewModels;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace GR.ApplePay.Abstractions
{
    public interface IApplePayPaymentMethodService : IPaymentMethodService
    {
        /// <summary>
        /// Get merchant session
        /// </summary>
        /// <param name="model"></param>
        /// <param name="httpContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResultModel<JObject>> GetMerchantSessionAsync(
            ValidateMerchantSessionModel model,
            HttpContext httpContext,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create payment
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        Task<ResultModel<ApplePayTransactionInfo>> CreatePaymentAsync(Guid? orderId);

        /// <summary>
        /// Get merchant identifier
        /// </summary>
        /// <returns></returns>
        string GetMerchantIdentifier();

        /// <summary>
        /// Get certificate
        /// </summary>
        /// <returns></returns>
        X509Certificate2 GetCertificate();

        /// <summary>
        /// Approve complete transaction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> ApproveCompleteTransactionAsync(ApproveTransactionRequestViewModel model);

        /// <summary>
        /// Get domain association file
        /// </summary>
        /// <returns></returns>
        ResultModel<byte[]> GetAssociationDomainFile();
    }
}