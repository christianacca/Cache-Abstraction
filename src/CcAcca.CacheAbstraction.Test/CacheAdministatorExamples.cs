// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    [TestFixture]
    public class CacheAdministatorExamples
    {
        private CacheAdministator _administator;

        [SetUp]
        public void Setup()
        {
            _administator = new CacheAdministator();
        }

        [Test]
        public void Register_ShouldExtendCacheWithStatisticsDecorator()
        {
            // given
            ICache cache = NewCache();
            Assert.That(cache.IsDecoratedWith<IStatisticsCache>(), Is.False, "checking assumptions");

            // when
            cache = _administator.Register(cache);

            // then
            Assert.That(cache.IsDecoratedWith<IStatisticsCache>(), Is.True);
        }


        [Test]
        public void Unregister_ShouldRemoveStatisticsDecorator()
        {
            // given
            ICache cache = NewCache();
            cache = _administator.Register(cache);

            // when
            cache = _administator.Unregister(cache);

            // then
            Assert.That(cache.IsDecoratedWith<IStatisticsCache>(), Is.False);
        }


        [Test]
        public void Unregister_ShouldNotRemoveStatisticsDecoratorAddedBeforeRegistration()
        {
            // given
            ICache cache = NewCache();
            cache = cache.DecorateWith(new CacheDecoratorOptions {IsStatisticsOn = true});
            cache = _administator.Register(cache);

            // when
            cache = _administator.Unregister(cache);

            // then
            Assert.That(cache.IsDecoratedWith<IStatisticsCache>(), Is.True);
        }


        private static ICache NewCache()
        {
            return new SimpleInmemoryCache();
        }
    }
}