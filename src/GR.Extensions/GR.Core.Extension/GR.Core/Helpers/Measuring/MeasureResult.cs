using System;

namespace GR.Core.Helpers.Measuring
{
    public class MeasureResult<T>
    {
        /// <summary>
        /// Result
        /// </summary>
        public virtual T Result { get; set; }

        /// <summary>
        /// Elapsed
        /// </summary>
        public virtual TimeSpan Elapsed { get; set; }

        /// <summary>
        /// Elapsed
        /// </summary>
        public virtual long ElapsedMilliseconds { get; set; }
    }
}