// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    [TestFixture]
    public class SimpleInmemoryCacheExamples : CacheExamplesBase
    {
        protected override ICache CreateCache()
        {
            return new SimpleInmemoryCache();
        }


        [Test]
        public void CanCreateWithCacheName()
        {
            var cache = new SimpleInmemoryCache("SomeCache");
            Assert.That(cache.Id.Name, Is.EqualTo("SomeCache"), "CacheName");
            Assert.That(cache.Id.InstanceName, Is.Empty, "InstanceName");
        }


        [Test]
        public void CanCreateWithCacheNameAndInstanceName()
        {
            var cache = new SimpleInmemoryCache("SomeCache.Instance1");
            Assert.That(cache.Id.Name, Is.EqualTo("SomeCache"), "CacheName");
            Assert.That(cache.Id.InstanceName, Is.EqualTo("Instance1"), "InstanceName");
        }
    }

    [TestFixture]
    public class SimpleInmemoryPartionedCacheExamples : PartitionedCacheExamplesBase
    {
        protected override ICollection<ICache> CreateCachesWithSharedStorage()
        {
            var dic = new ConcurrentDictionary<string, object>();
            var results = new[]
            {
                new SimpleInmemoryCache(dic, "Dictionary.1"),
                new SimpleInmemoryCache(dic, "Dictionary.2"),
                new SimpleInmemoryCache(dic, "Dictionary.3")
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