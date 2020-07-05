using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using Microsoft.AspNetCore.Identity;

namespace GR.Core.Abstractions
{
    public interface IAppSender
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<ResultModel> SendAsync(string provider, string subject, string message, string to);

        /// <summary>
        /// Send message
        /// </summary>
        /// <typeparam name="TUser"></typeparam>
        /// <param name="user"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="providers"></param>
        /// <returns></returns>
        Task<ResultModel> SendAsync<TUser>(TUser user, string subject, string message, params string[] providers)
            where TUser : IdentityUser<Guid>;

        /// <summary>
        /// Register new provider
        /// </summary>
        void RegisterProvider<TProvider>(string name)
            where TProvider : class, ISender;
    }
}