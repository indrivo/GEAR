using System;
using GR.Core.Helpers;
using Microsoft.Extensions.Options;

namespace GR.Core.Abstractions
{
    public interface IWritableOptions<out T> : IOptionsSnapshot<T> where T : class, new()
    {
        /// <summary>
        /// Update .json settings file.
        /// </summary>
        /// <param name="applyChanges"></param>
        /// <param name="filePath"></param>
        ResultModel Update(Action<T> applyChanges, string filePath = null);
    }
}
