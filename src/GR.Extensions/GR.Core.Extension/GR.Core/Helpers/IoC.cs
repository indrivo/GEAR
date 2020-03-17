using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using GR.Core.Exceptions;
using GR.Core.Extensions;
using GR.Core.Helpers.Patterns;

namespace GR.Core.Helpers
{
    public static class IoC
    {
        /// <summary>
        /// IoC container
        /// </summary>
        public static IWindsorContainer Container => Singleton<IWindsorContainer, WindsorContainer>.Instance;

        ///// <summary>
        ///// IoC container
        ///// </summary>
        //private static IWindsorContainer _container;

        ///// <summary>
        ///// Container resolver
        ///// </summary>
        //public static IWindsorContainer Container => _container ?? (_container = new WindsorContainer());


        /// <summary>
        /// Register new service
        /// </summary>
        /// <typeparam name="TAbstraction"></typeparam>
        public static void RegisterService<TAbstraction>(string providerName, Type provider) where TAbstraction : class
        {
            if (providerName.IsNullOrEmpty()) throw new Exception("Provider name must be not null");
            Container.Register(Component.For<TAbstraction>()
                    .ImplementedBy(provider)
                    .Named(providerName));
        }

        /// <summary>
        /// Register new service
        /// </summary>
        /// <typeparam name="TAbstraction"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void RegisterTransientService<TAbstraction, TImplementation>() where TImplementation : class, TAbstraction where TAbstraction : class
        {
            if (!IsServiceRegistered<TAbstraction>())
                Container.Register(Component.For<TAbstraction>()
                .ImplementedBy<TImplementation>().LifestyleTransient());
        }


        /// <summary>
        /// Register new service
        /// </summary>
        /// <typeparam name="TAbstraction"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void RegisterTransientService<TAbstraction, TImplementation>(TImplementation instance) where TImplementation : class, TAbstraction where TAbstraction : class
        {
            if (!IsServiceRegistered<TAbstraction>())
                Container.Register(Component.For<TAbstraction>()
                    .Instance(instance)
                    .LifestyleTransient());
        }

        /// <summary>
        /// Register named transient service
        /// </summary>
        /// <typeparam name="TAbstraction"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="providerName"></param>
        public static void RegisterTransientService<TAbstraction, TImplementation>(string providerName) where TImplementation : class, TAbstraction where TAbstraction : class
        {
            if (!IsServiceRegistered(providerName))
                Container.Register(Component.For<TAbstraction>()
                    .ImplementedBy<TImplementation>()
                    .LifestyleTransient()
                    .Named(providerName));
        }

        /// <summary>
        /// Register singleton service
        /// </summary>
        /// <typeparam name="TAbstraction"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void RegisterSingletonService<TAbstraction, TImplementation>() where TImplementation : class, TAbstraction where TAbstraction : class
        {
            if (!IsServiceRegistered<TAbstraction>())
                Container.Register(Component.For<TAbstraction>()
                    .ImplementedBy<TImplementation>()
                    .LifestyleSingleton());
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
        /// Register new service
        /// </summary>
        /// <typeparam name="TAbstraction"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public static void RegisterScopedService<TAbstraction, TImplementation>() where TImplementation : class, TAbstraction where TAbstraction : class
        {
            if (!IsServiceRegistered<TAbstraction>())
                Container.Register(Component.For<TAbstraction>()
                    .LifestyleCustom<MsScopedLifestyleManager>().ImplementedBy(typeof(TImplementation)));
        }

        /// <summary>
        /// Register service
        /// </summary>
        /// <typeparam name="TAbstraction"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="configuration"></param>
        public static void RegisterService<TAbstraction, TImplementation>(Func<ComponentRegistration<TAbstraction>, ComponentRegistration<TAbstraction>> configuration)
            where TImplementation : class, TAbstraction where TAbstraction : class
        {
            var component = Component.For<TAbstraction>()
                .ImplementedBy<TImplementation>();
            component = configuration(component);
            Container.Register(component);
        }

        /// <summary>
        /// Check if service is registered
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static bool IsServiceRegistered<TService>()
            => Container.Kernel.HasComponent(typeof(TService));

        /// <summary>
        /// Check if service is registered
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static bool IsServiceRegistered(string provider)
            => Container.Kernel.HasComponent(provider);

        /// <summary>
        /// Check if service is registered
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private static bool IsServiceRegistered(Type service)
            => Container.Kernel.HasComponent(service);

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
        /// Resolve generic type
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static TService ResolveNonRequired<TService>()
            => !IsServiceRegistered<TService>() ? default : Container.Resolve<TService>();

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
