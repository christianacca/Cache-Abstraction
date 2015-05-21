// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    [TestFixture]
    public class ObjectCacheWrapperExamples : CacheExamplesBase
    {
        #region Test helpers

        protected override ICache CreateCache()
        {
            return new ObjectCacheWrapper();
        }

        #endregion


        [Test]
        public void CanCreateWithCacheNameAndInstanceName()
        {
            var cache = new ObjectCacheWrapper("SomeCache.Instance1");
            Assert.That(cache.Id.Name, Is.EqualTo("SomeCache"), "CacheName");
            Assert.That(cache.Id.InstanceName, Is.EqualTo("Instance1"), "InstanceName");
        }

        [Test]
        public void ItemsAddedToCacheShouldUseCachePolicySupplied()
        {
            // given
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(1)
            };
            ICache cache = new ObjectCacheWrapper(cacheItemPolicySelector: (key, value) => policy);

            // when, then
            cache.GetOrAdd("key1", _ => new object());
            cache.AddOrUpdate("key2", new object());

            Assert.That(cache.Contains("key1"), Is.True, "checking assumptions for GetOrAdd");
            Assert.That(cache.Contains("key2"), Is.True, "checking assumptions for Add");

            Thread.Sleep(2000); // wait until item has expired and should be removed

            Assert.That(cache.Contains("key1"), Is.False, "GetOrAdd did not use expiry policy");
            Assert.That(cache.Contains("key2"), Is.False, "Add did not use expiry policy");
        }
    }


    [TestFixture]
    public class ObjectCacheWrapperPartionedCacheExamples : PartitionedCacheExamplesBase
    {
        protected override ICollection<ICache> CreateCachesWithSharedStorage()
        {
            var memoryCache = new MemoryCache("MemoryCache");
            var results = new[]
            {
                new ObjectCacheWrapper(memoryCache, "1"),
                new ObjectCacheWrapper(memoryCache, "2"),
                new ObjectCacheWrapper(memoryCache, "3")
            };
            return results;
        }

        [Test]
        public void Count_ShouldOnlyCountItemsInPartition()
        {
            // given
            var cache1 = Caches.ElementAt(0);
            var cache2 = Caches.ElementAt(1);
            var cache3 = Caches.ElementAt(2);

            // when
            cache1.AddOrUpdate("key1", 1);

            cache2.AddOrUpdate("key1", 3);
            cache2.AddOrUpdate("key2", 4);
            cache2.AddOrUpdate("key3", 5);

            // then
            Assert.That(cache1.Count, Is.EqualTo(1));
            Assert.That(cache2.Count, Is.EqualTo(3));
            Assert.That(cache3.Count, Is.EqualTo(0));
            Assert.That(cache3.Contains("key3"), Is.False);
        }
    }
}