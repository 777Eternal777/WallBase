using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EfContext;
using SharpRepository.Caching.Redis;
using SharpRepository.EfRepository;
using SharpRepository.Repository.Caching;
using SharpRepository.Repository.Configuration;
using WallBase.Logic;

namespace Wallbase.IoCSetup
{
    public static class RepositoryConfiguration
    {
        public static SharpRepositoryConfiguration GetConfig()
        {
            var config = new SharpRepositoryConfiguration();
            config.AddRepository(new EfRepositoryConfiguration("ef5", "DefaultConnection", typeof(WallbaseDB)));
            config.AddCachingStrategy(new TimeoutCachingStrategyConfiguration("timeout", 30)); 
            config.AddCachingStrategy(new StandardCachingStrategyConfiguration("standard")); 
            config.AddCachingStrategy(new NoCachingStrategyConfiguration("none")); 
            config.AddCachingProvider(new RedisCachingProviderConfiguration("inmemory", "127.0.0.1", 6379,"",false)); 

            config.DefaultRepository = "ef5";
            config.DefaultCachingStrategy = "timeout";
            config.DefaultCachingProvider = "inmemory";
            return config;
        }

    }
}
