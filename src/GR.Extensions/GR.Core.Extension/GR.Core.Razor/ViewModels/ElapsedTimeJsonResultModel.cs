using System;
using GR.Core.Helpers;

namespace GR.Core.Razor.ViewModels
{
    public class ElapsedTimeJsonResultModel<T> : ResultModel<T>
    {
        public virtual DateTime Started { get; set; }
        public virtual DateTime Completed { get; set; }
        public virtual long ElapsedMilliseconds { get; set; }
    }
}