using System.Threading.Tasks;

namespace GR.Notifications.Abstractions
{
    public interface INotificationSeederService
    {
        /// <summary>
        /// Seed notification types
        /// </summary>
        /// <returns></returns>
        Task SeedNotificationTypesAsync();
    }
}