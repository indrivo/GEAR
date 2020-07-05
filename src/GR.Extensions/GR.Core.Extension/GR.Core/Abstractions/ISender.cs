using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using Microsoft.AspNetCore.Identity;

namespace GR.Core.Abstractions
{
    public interface ISender
    {
        /// <summary>
        /// Send async
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        Task<ResultModel> SendAsync(string subject, string message, string to);

        /// <summary>
        /// Send messages to user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<ResultModel> SendAsync<TUser>(TUser user, string subject, string message) where TUser : IdentityUser<Guid>;

        /// <summary>
        /// Get provider
        /// </summary>
        /// <returns></returns>
        object GetProvider();
    }
}