using System;
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
        void Update(Action<T> applyChanges, string filePath = null);
    }
}
