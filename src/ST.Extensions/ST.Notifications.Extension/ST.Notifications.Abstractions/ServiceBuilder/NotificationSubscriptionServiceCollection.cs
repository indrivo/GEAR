using Microsoft.Extensions.DependencyInjection;

namespace ST.Notifications.Abstractions.ServiceBuilder
{
    public interface INotificationSubscriptionServiceCollection
    {
        IServiceCollection Services { get; set; }
    }

    public class NotificationSubscriptionServiceCollection : INotificationSubscriptionServiceCollection
    {
        public NotificationSubscriptionServiceCollection(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; set; }
    }
}
