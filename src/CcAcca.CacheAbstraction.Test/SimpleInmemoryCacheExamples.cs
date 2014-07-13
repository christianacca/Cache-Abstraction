// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

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
}