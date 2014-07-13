// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    [TestFixture]
    public class GlobalCacheProviderExamples
    {
        private GlobalCacheProvider _cacheProvider;
        private CacheDecoratorOptions _originalDecoratorDefaults;

        [SetUp]
        public void SetUp()
        {
            _cacheProvider = new GlobalCacheProvider();
            _originalDecoratorDefaults = CacheDecoratorOptions.Default;
        }

        [TearDown]
        public void TearDown()
        {
            // reset any defaults thay may have been tampered with during a test
            CacheDecoratorOptions.Default = _originalDecoratorDefaults;
        }


        [Test]
        public void Get_ShouldDefaultToReturningSimpleInmemoryCacheExtendedWithDefaultDecorators()
        {
            // given
            CacheDecoratorOptions.Default = new CacheDecoratorOptions {IsPausableOn = true};

            // when
            ICache cache = _cacheProvider.Get<ICache>("SomeCacheName");

            // then
            Assert.That(cache, Is.Not.Null);
            List<ICache> chain = cache.GetDecoratorChainAndDecorated().ToList();
            Assert.That(chain.Count, Is.EqualTo(2), "Chain Count");
            Assert.That(chain[0], Is.InstanceOf<IPausableCache>(), "First");
            Assert.That(chain[1], Is.InstanceOf<SimpleInmemoryCache>(), "Second");
        }


        [Test]
        public void Get_ShouldAssignCacheId()
        {
            ICache cache = _cacheProvider.Get<ICache>("SomeCacheName");
            Assert.That(cache.Id.Name, Is.EqualTo("SomeCacheName"), "CacheName");
            Assert.That(cache.Id.InstanceName, Is.Empty, "InstanceName");
        }


        [Test]
        public void Get_ShouldAssignCacheId_InstanceNameSupplied()
        {
            ICache cache = _cacheProvider.Get<ICache>("SomeCacheName.SomeInstanceName");
            Assert.That(cache.Id.Name, Is.EqualTo("SomeCacheName"), "CacheName");
            Assert.That(cache.Id.InstanceName, Is.EqualTo("SomeInstanceName"), "InstanceName");
        }


        [Test]
        public void Get_ShouldReturnExistingCacheWithSameId()
        {
            ICache first = _cacheProvider.Get<ICache>("SomeCacheName.SomeInstanceName");
            ICache second = _cacheProvider.Get<ICache>("SomeCacheName.SomeInstanceName");
            Assert.That(first, Is.SameAs(second));
        }


        [Test]
        public void WhenConfiguredWithCacheAdministrator_Get_ShouldRegisterCache()
        {
            // given
            var cacheProvider = new GlobalCacheProvider {CacheAdministator = new CacheAdministator()};

            // when
            ICache cache = cacheProvider.Get<ICache>("SomeCacheName");

            // then
            Assert.That(cacheProvider.CacheAdministator.Contains(cache), Is.True);
        }


        [Test]
        public void WhenConfiguredWithCacheAdministrator_Get_ShouldNotTryAndRegisterCacheTwice()
        {
            // given
            var cacheProvider = new GlobalCacheProvider {CacheAdministator = new CacheAdministator()};
            cacheProvider.Get<ICache>("SomeCacheName");

            // when
            cacheProvider.Get<ICache>("SomeCacheName");

            // then
            Assert.That(cacheProvider.CacheAdministator.AllCaches.Count(), Is.EqualTo(1));
        }
    }
}