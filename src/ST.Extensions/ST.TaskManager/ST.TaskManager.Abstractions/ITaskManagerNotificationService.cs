using System.Threading.Tasks;

namespace ST.TaskManager.Abstractions
{
    public interface ITaskManagerNotificationService
    {
        Task TaskExpirationNotificationAsync();
    }
}