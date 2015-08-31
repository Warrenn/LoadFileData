using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension;
using Microsoft.Practices.Unity.Mvc;

namespace LoadFileData.Web
{
    public static partial class UnityConfig
    {
        private static readonly Lazy<IUnityContainer> LazyContainer
            = new Lazy<IUnityContainer>(BuildUnityContainer);

        public static IUnityContainer Container
        {
            get { return LazyContainer.Value; }
        }

        public static void Initialize(HttpConfiguration config)
        {
            var container = Container;
            var mvc4Resolver = new UnityDependencyResolver(container);
            DependencyResolver.SetResolver(mvc4Resolver);
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var section = (UnityConfigurationSection)
                ConfigurationManager.GetSection(UnityConfigurationSection.SectionName);
            var returnContainer = new UnityContainer();
            var container = section.Configure(returnContainer);
            container.RegisterInstance(container);
            container.AddNewExtension<Interception>();

            var config = ConfigurationSourceFactory.Create();
            var factory = new ExceptionPolicyFactory(config);

            var exManager = factory.CreateManager();
            ExceptionPolicy.SetExceptionManager(exManager);

            RegisterTypes(container);

            var locator = new UnityServiceLocator(returnContainer);
            ServiceLocator.SetLocatorProvider(() => locator);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
        }
    }
}