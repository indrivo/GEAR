using System;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;

namespace GR.WebHooks.Abstractions.Helpers
{
    public static class ProviderHookHelper
    {
        public static void RegisterOutgoingProvider<TProvider>(Guid providerId)
            where TProvider : class, IHookSender
        {
            var providerName = $"outgoing_hook_provider_{providerId}";
            if (!IoC.IsServiceRegistered(providerName))
            {
                IoC.RegisterTransientService<IHookSender, TProvider>(providerName);
            }
        }

        public static ResultModel<IHookSender> ResolveOutgoingProvider(Guid? providerId)
        {
            var service = IoC.ResolveNonRequired<IHookSender>($"outgoing_hook_provider_{providerId}");
            if (service == null) return new NotFoundResultModel<IHookSender>();
            return new SuccessResultModel<IHookSender>(service);
        }


        public static void RegisterIncomingProvider<TProvider>(Guid providerId)
            where TProvider : class, IHookReceiver
        {
            var providerName = $"incoming_hook_provider_{providerId}";
            if (!IoC.IsServiceRegistered(providerName))
            {
                IoC.RegisterTransientService<IHookReceiver, TProvider>(providerName);
            }
        }

        public static ResultModel<IHookReceiver> ResolveIncomingProvider(Guid? providerId)
        {
            var service = IoC.ResolveNonRequired<IHookReceiver>($"incoming_hook_provider_{providerId}");
            if (service == null) return new NotFoundResultModel<IHookReceiver>();
            return new SuccessResultModel<IHookReceiver>(service);
        }
    }
}