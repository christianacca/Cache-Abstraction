// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using CacheManager.Core;
using CcAcca.CacheAbstraction.Distributed;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test.Redis
{
    [TestFixture]
    public class RedisWrapperExamples : CacheExamplesBase
    {
        #region Test helpers

        protected override ICache CreateCache()
        {
            ICacheManager<object> impl = CacheFactory.Build("redis_cache",
                settings => {
                    settings
                        .WithMaxRetries(100)
                        .WithRetryTimeout(1000)
                        .WithRedisConfiguration("redisCache",
                            config => {
                                config
                                    .WithAllowAdmin()
                                    .WithDatabase(0)
                                    .WithConnectionTimeout(5000)
                                    .WithEndpoint("localhost", 6379);
                            })
                        .WithRedisBackPlate("redisCache")
                        .WithRedisCacheHandle("redisCache", true);
                });
            // for good measure we should clear any peristent items
            impl.Clear();

            return new CacheManagerWrapper("redis_cache.instance1", impl);
        }

        #endregion
    }
}