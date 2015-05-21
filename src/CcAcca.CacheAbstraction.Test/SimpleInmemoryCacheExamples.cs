// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System.Collections.Concurrent;
using System.Collections.Generic;
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
    }
}