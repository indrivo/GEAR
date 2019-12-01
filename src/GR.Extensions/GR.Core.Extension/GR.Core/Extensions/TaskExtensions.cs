using System.Threading.Tasks;

namespace GR.Core.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Execute async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static T ExecuteAsync<T>(this Task<T> task) => task.GetAwaiter().GetResult();

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static void ExecuteAsync(this Task task) => task.GetAwaiter().GetResult();
    }
}
