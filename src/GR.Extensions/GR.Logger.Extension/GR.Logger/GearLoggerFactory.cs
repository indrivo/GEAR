using GR.Logger.Abstractions;
using Microsoft.Extensions.Logging;

namespace GR.Logger
{
    public class GearLoggerFactory : IGearLoggerFactory
    {
        #region Injectable

        /// <summary>
        /// Inject logger factory
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        #endregion

        public GearLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public virtual ILogger CreateLogger(string categoryName)
        {
            return _loggerFactory.CreateLogger(categoryName);
        }

        /// <summary>
        /// Add new provider
        /// </summary>
        /// <param name="provider"></param>
        public virtual void AddProvider(ILoggerProvider provider)
        {
            _loggerFactory.AddProvider(provider);
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            _loggerFactory.Dispose();
        }
    }
}
