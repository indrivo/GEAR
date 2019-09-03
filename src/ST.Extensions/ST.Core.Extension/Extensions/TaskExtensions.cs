using System.Threading.Tasks;

namespace ST.Core.Extensions
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
    }
}
