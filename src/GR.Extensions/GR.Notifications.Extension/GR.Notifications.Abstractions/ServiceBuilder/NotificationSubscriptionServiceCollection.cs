using Microsoft.Extensions.DependencyInjection;

namespace GR.Notifications.Abstractions.ServiceBuilder
{
    public interface INotificationServiceCollection
    {
        IServiceCollection Services { get; set; }
    }

    public class NotificationServiceCollection : INotificationServiceCollection
    {
        public NotificationServiceCollection(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; set; }
    }
}
