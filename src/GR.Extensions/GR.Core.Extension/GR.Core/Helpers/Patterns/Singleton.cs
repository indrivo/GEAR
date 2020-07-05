using System;
using System.Threading.Tasks;

namespace GR.Core.Helpers.Patterns
{
    public class Singleton<T, TResolver>
        where TResolver : T
    {
        // We now have a lock object that will be used to synchronize threads
        // during first access to the Singleton.
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object _lock = new object();

        /// <summary>
        /// Stored instance
        /// </summary>
        private static T _instance;

        /// <summary>
        /// Resolver
        /// </summary>
        private static Func<Task<TResolver>> _resolver = () => Task.FromResult(Activator.CreateInstance<TResolver>());

        /// <summary>
        /// Get or set instance
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static T GetOrSetInstance(Func<Task<TResolver>> resolver)
        {
            _resolver = resolver;
            return Instance;
        }

        /// <summary>
        /// Retrieve instance
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = _resolver().GetAwaiter().GetResult();
                        }
                    }
                }

                return _instance;
            }
        }
    }
}