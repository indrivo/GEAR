using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.EmailTwoFactorAuth.Models;

namespace GR.EmailTwoFactorAuth.Helpers
{
    public static class CodeProcessor
    {
        /// <summary>
        /// Interval
        /// </summary>
        public static int Interval = 1000;

        /// <summary>
        /// Max counter
        /// </summary>
        public static int MaxCounter = 120;

        /// <summary>
        /// Storage
        /// </summary>
        private static readonly ConcurrentDictionary<string, Guid> Codes = new ConcurrentDictionary<string, Guid>();

        /// <summary>
        /// Push new code
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        public static void Push(Guid userId, string code)
        {
            Codes.TryAdd(code, userId);
            new Task(() =>
            {
                var autoEvent = new AutoResetEvent(false);
                TimerCallback tmCallback = TmCallback;
                var state = new CheckStateModel
                {
                    UserId = userId,
                    Code = code,
                    Event = autoEvent
                };
                var timer = new Timer(tmCallback, state, Interval, Interval);
                autoEvent.WaitOne();
                timer.Dispose();
                Codes.TryRemove(code, out _);
            }).Start();
        }

        /// <summary>
        /// Timer callback
        /// </summary>
        /// <param name="state"></param>
        private static void TmCallback(object state)
        {
            var stateInfo = state.Is<CheckStateModel>();
            if (!Codes.Any(x => x.Key.Equals(stateInfo.Code)))
                stateInfo.Event.Set();
            if (++stateInfo.Counter >= MaxCounter) stateInfo.Event.Set();
        }

        /// <summary>
        /// Check if match
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool Match(Guid userId, string code)
        {
            var match = Codes.FirstOrDefault(x => x.Value.Equals(userId) && x.Key.Equals(code));
            if (!match.IsNull())
            {
                Codes.TryRemove(code, out _);
            }
            return !match.IsNull();
        }
    }
}
