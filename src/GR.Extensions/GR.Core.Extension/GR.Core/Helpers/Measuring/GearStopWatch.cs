using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GR.Core.Helpers.Measuring
{
    /// <summary>
    /// Custom measure helper
    /// </summary>
    public static class GearStopWatch
    {
        public static async Task<MeasureResult<T>> MeasureAsync<T>(Func<Task<T>> action)
        {
            var watch = new Stopwatch();
            watch.Start();
            var result = await action();
            watch.Stop();
            return new MeasureResult<T>
            {
                Result = result,
                ElapsedMilliseconds = watch.ElapsedMilliseconds,
                Elapsed = watch.Elapsed
            };
        }

        public static async Task<MeasureResult> MeasureAsync(Func<Task> action)
        {
            var watch = new Stopwatch();
            watch.Start();
            await action();
            watch.Stop();
            return new MeasureResult
            {
                ElapsedMilliseconds = watch.ElapsedMilliseconds,
                Elapsed = watch.Elapsed
            };
        }
    }
}
