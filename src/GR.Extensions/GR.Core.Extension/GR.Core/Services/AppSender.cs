using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;

namespace GR.Core.Services
{
    public class AppSender
    {
        /// <summary>
        /// 
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