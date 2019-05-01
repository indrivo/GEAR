using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace ST.Core.Helpers
{
    public sealed class IoC
    {
        private static readonly object LockObj = new object();

        private static IWindsorContainer _container;

        private static IoC _instance = new IoC();

        private IoC()
        {

        }

        public static IWindsorContainer Container
        {
            get => _container;

            set
            {
                lock (LockObj)
                {
                    _container = value;
                }
            }
        }


        public static IoC Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new IoC();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Register new service
        /// </summary>
        /// <typeparam name="TAbstraction"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void RegisterService<TAbstraction, TImplementation>() where TImplementation : class, TAbstraction where TAbstraction : class
        {
            if (_container == null) _container = new WindsorContainer();
            _container.Register(Component.For<TAbstraction>()
                .ImplementedBy<TImplementation>());
        }

        /// <summary>
        /// Resolve generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        /// <summary>
        /// Resolve generic type by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Resolve<T>(string key)
        {
            return _container.Resolve<T>(key);
        }

        /// <summary>
        /// Resolve service by runtime type param
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Resolve(Type type)
        {
            return _container.Resolve(type);
        }
    }
}
