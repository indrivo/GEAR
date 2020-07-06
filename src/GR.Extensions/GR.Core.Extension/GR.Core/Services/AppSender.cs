using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using Microsoft.AspNetCore.Identity;

namespace GR.Core.Services
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class AppSender : IAppSender
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SendAsync(string provider, string subject, string message, string to)
        {
            var service = IoC.ResolveNonRequired<ISender>($"sender_{provider}");
            if (service == null) return new NotFoundResultModel();
            return await service.SendAsync(subject, message, to);
        }

        /// <summary>
        /// Send to user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SendAsync<TUser>(TUser user, string subject, string message, params string[] providers)
            where TUser : IdentityUser<Guid>
        {
            var results = new List<ResultModel>();
            foreach (var provider in providers)
            {
                var service = IoC.ResolveNonRequired<ISender>($"sender_{provider}");
                if (service == null) return new NotFoundResultModel();
                var reqResponse = await service.SendAsync(user, subject, message);
                results.Add(reqResponse);
            }

            return new ResultModel().JoinResults(results).ToBase();
        }

        /// <summary>
        /// Register new provider
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="name"></param>
        public virtual void RegisterProvider<TProvider>(string name)
            where TProvider : class, ISender
        {
            var providerName = $"sender_{name}";
            if (!IoC.IsServiceRegistered(providerName))
            {
                IoC.RegisterTransientService<ISender, TProvider>(providerName);
            }
        }
    }
}