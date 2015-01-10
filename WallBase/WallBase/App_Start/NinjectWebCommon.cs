using System.Collections.Generic;
using System.Linq;
using Ninject.Web.Mvc;
using WallBase.Ioc.Ninject;
using Wallbase.IoCSetup;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WallBase.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(WallBase.App_Start.NinjectWebCommon), "Stop")]

namespace WallBase.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                NinjectResolver.Initialize(kernel);
                KernelInitialization.Initialize(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IValueCalculator>().To<LinqValueCalculator>();
            
        }

    }
    public interface IValueCalculator
    {
        decimal ValueProducts(IEnumerable<string> products);
    }
    public class LinqValueCalculator : IValueCalculator
    {
        public decimal ValueProducts(IEnumerable<string> products)
        {
            return products.Count();
        }
    }


}
