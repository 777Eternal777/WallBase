using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfContext;
using Ninject;
using Ninject.Web.Mvc;
using SharpRepository.Caching.Redis;
using SharpRepository.EfRepository;
using SharpRepository.Repository.Caching;
using SharpRepository.Repository.Configuration;
using WallBase.Ioc.Ninject;
using WallBase.Logic;

namespace Wallbase.IoCSetup
{
    public static class KernelInitialization
    {
        public static void Initialize(this IKernel kernel)
        {

            NinjectResolver.kernel.Bind<DbContext>().To<WallbaseDB>().InThreadScope();
            NinjectResolver.kernel.Bind<WallbaseDB>().To<WallbaseDB>().InThreadScope();

            NinjectResolver.kernel.Bind<WallpapersService>().To<WallpapersService>().InThreadScope();
            NinjectResolver.kernel.BindSharpRepository(RepositoryConfiguration.GetConfig());
        }
    }
}
