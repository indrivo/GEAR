using System.Threading.Tasks;

namespace ST.TaskManager.Abstractions
{
    public interface ITaskManagerNotificationService
    {
        /// <summary>
        /// Sent notifications to tasks which deadline is tomorrow. 
        /// </summary>
        /// <returns></returns>
        Task TaskExpirationNotificationAsync();
    }
}