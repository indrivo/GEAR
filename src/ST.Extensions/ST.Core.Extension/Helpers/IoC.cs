using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using ST.Core.Exceptions;

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

        public static IWindsorContainer Container => _container ?? (_container = new WindsorContainer());

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
            if (!IsServiceRegistered<TAbstraction>())
                Container.Register(Component.For<TAbstraction>()
                .ImplementedBy<TImplementation>());
        }

        /// <summary>
        /// Register service collection 
        /// </summary>
        /// <param name="toMapCollection"></param>
        public static void RegisterServiceCollection(Dictionary<Type, Type> toMapCollection)
        {
            foreach (var serviceInfo in toMapCollection)
            {
                if (!IsServiceRegistered(serviceInfo.Key))
                    Container.Register(Component.For(serviceInfo.Key)
                    .ImplementedBy(serviceInfo.Value));
            }
        }

        /// <summary>
        /// Register new service
        /// </summary>
        /// <typeparam name="TAbstraction"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void RegisterScopedService<TAbstraction, TImplementation>(TImplementation instance) where TImplementation : class, TAbstraction where TAbstraction : class
        {
            if (!IsServiceRegistered<TAbstraction>())
                Container.Register(Component.For<TAbstraction>().Instance(instance)
                        .LifestyleCustom<MsScopedLifestyleManager>());
        }

        /// <summary>
        /// Check if service is registered
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static bool IsServiceRegistered<TService>()
        {
            return Container.Kernel.HasComponent(typeof(TService));
        }

        /// <summary>
        /// Check if service is registered
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private static bool IsServiceRegistered(Type service)
        {
            return Container.Kernel.HasComponent(service);
        }

        /// <summary>
        /// Resolve generic type
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService Resolve<TService>()
        {
            if (!IsServiceRegistered<TService>())
                throw new IoCNotRegisterServiceException($"{typeof(TService).Name} is not registered in IoC container");
            return Container.Resolve<TService>();
        }

        /// <summary>
        /// Resolve generic type by key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Resolve<T>(string key)
        {
            return Container.Resolve<T>(key);
        }

        /// <summary>
        /// Resolve service by runtime type param
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Resolve(Type type)
        {
            if (!IsServiceRegistered(type))
                throw new IoCNotRegisterServiceException($"{type.Name} is not registered in IoC container");
            return Container.Resolve(type);
        }
    }
}
