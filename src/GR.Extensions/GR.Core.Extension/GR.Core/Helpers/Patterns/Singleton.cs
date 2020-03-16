using System;

namespace GR.Core.Helpers.Patterns
{
    public static class Singleton<T, TResolver>
        where TResolver : T
    {
        // We now have a lock object that will be used to synchronize threads
        // during first access to the Singleton.
        private static readonly object _lock = new object();

        /// <summary>
        /// Stored instance
        /// </summary>
        private static T _instance;

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
                            _instance = Activator.CreateInstance<TResolver>();
                        }
                    }
                }

                return _instance;
            }
        }
    }
}
