// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System.Collections.Generic;
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


    [TestFixture]
    public class RedisPartionedCacheExamples : PartitionedCacheExamplesBase
    {
        protected override ICollection<ICache> CreateCachesWithSharedStorage()
        {
            ICacheManager<object> impl = CacheFactory.Build("redis_cache_part",
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
            var results = new[]
            {
                new CacheManagerWrapper("redis_part.1", impl),
                new CacheManagerWrapper("redis_part.2", impl),
                new CacheManagerWrapper("redis_part.3", impl)
            };
            return results;
        }
    }
}