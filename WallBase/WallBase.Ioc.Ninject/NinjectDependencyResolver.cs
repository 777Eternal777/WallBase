using System;
using Ninject;
using SharpRepository.Repository.Ioc;

namespace WallBase.Ioc.Ninject
{
    public class NinjectResolver : BaseRepositoryDependencyResolver
    {
        public static IKernel kernel; //todo : private
        public NinjectResolver(IKernel kernel)
        {
            SetKernel(kernel);
        }

        private static void SetKernel(IKernel kernel)
        {
            if (NinjectResolver.kernel == null)
            {
                NinjectResolver.kernel = kernel;
            }
        }

        public static void Initialize(IKernel kernel)
        {
            SetKernel(kernel);
        }

        public NinjectResolver()
        {
            if (kernel == null)
                throw new Exception("You must initialize resolver before using");
        }

        protected override T ResolveInstance<T>()
        {
            return kernel.Get<T>();
        }

        protected override object ResolveInstance(Type type)
        {
            return kernel.Get(type);
        }
    }
}
