// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System.Collections.Generic;
using CacheManager.Core;
using CcAcca.CacheAbstraction.Distributed;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test.Redis
{
    [TestFixture]
    public class RedisWithMemoryCacheWrapperExamples : CacheExamplesBase
    {
        #region Test helpers

        protected override ICache CreateCache()
        {
            var cache = CacheFactory.Build("cache",
                settings => {
                    settings
                        .WithUpdateMode(CacheUpdateMode.Up)
                        .WithSystemRuntimeCacheHandle("cache1");
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
            cache.Clear();

            return new CacheManagerWrapper("redis_cache.instance2", cache);
        }

        #endregion
    }

    [TestFixture]
    public class RedisWithMemoryCachePartionedCacheExamples : PartitionedCacheExamplesBase
    {
        protected override ICollection<ICache> CreateCachesWithSharedStorage()
        {
            var cache = CacheFactory.Build("cache_part",
                settings => {
                    settings
                        .WithUpdateMode(CacheUpdateMode.Up)
                        .WithSystemRuntimeCacheHandle("cache1");
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
            cache.Clear();
            var results = new[]
            {
                new CacheManagerWrapper("redis_memory_part.1", cache),
                new CacheManagerWrapper("redis_memory_part.2", cache),
                new CacheManagerWrapper("redis_memory_part.3", cache)
            };
            return results;
        }
    }
}