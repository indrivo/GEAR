using System;
using System.Threading.Tasks;
using Polly;

namespace GR.Core.Helpers
{
    public static class GearPolicy
    {
        /// <summary>
        /// Default time spans
        /// </summary>
        private static readonly TimeSpan[] Waiters = {
            TimeSpan.FromSeconds(3),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(8),
        };

        /// <summary>
        /// Execute and retry on fail case
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task<ResultModel<T>> ExecuteAndRetry<T>(Func<Task<ResultModel<T>>> func)
        {
            var retry = Policy
                .HandleResult<ResultModel<T>>(x => !x.IsSuccess)
                .WaitAndRetryAsync(Waiters);
            var result = await retry.ExecuteAsync(func);
            return result;
        }

        /// <summary>
        /// Execute and retry on fail case
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task<ResultModel> ExecuteAndRetry(Func<Task<ResultModel>> func)
        {
            var retry = Policy
                .HandleResult<ResultModel>(x => !x.IsSuccess)
                .WaitAndRetryAsync(Waiters);
            var result = await retry.ExecuteAsync(func);
            return result;
        }
    }
}
