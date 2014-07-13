// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System.Collections.Generic;
using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test
{
    [TestFixture]
    public class StatisticsCacheExamles : CacheExamplesBase
    {
        protected override ICache CreateCache()
        {
            var stats = new List<CacheStatistic>
                {
                    new LastWrtieCacheStatistic()
                };
            return new StatisticsDecorator(CacheStatistics.All, new SimpleInmemoryCache());
        }

        [Test]
        public void As_ShouldReturnDecoratorInstance()
        {
            var instance = Cache.As<IStatisticsCache>();
            Assert.That(instance, Is.Not.Null, "instance");
            Assert.IsInstanceOf<StatisticsDecorator>(instance, "wrong implementation");
        }

        [Test]
        public void As_ShouldReturnDecoratorInstance_DecoratorsChained()
        {
            var cache = new MultiThreadProtectedDecorator(Cache);
            var instance = cache.As<IStatisticsCache>();
            Assert.That(instance, Is.Not.Null, "instance");
            Assert.IsInstanceOf<StatisticsDecorator>(instance, "wrong implementation");
        }

        [Test]
        public void ShouldCollectStatistic()
        {
            // when
            Cache.AddOrUpdate("key1", new object());

            // then
            CacheStatistics statistics = Cache.As<IStatisticsCache>().Statistics;
            Assert.That(statistics.HasStatictic(CacheStatisticsKeys.LastWrite), Is.True);
        }

        [Test]
        public void ShouldCollectStatistic_DecoratorsChained()
        {
            // given
            var chainedDecorators = new PausableDecorator(Cache);

            // when
            chainedDecorators.AddOrUpdate("key1", new object());

            // then
            CacheStatistics statistics = chainedDecorators.As<IStatisticsCache>().Statistics;
            Assert.That(statistics.HasStatictic(CacheStatisticsKeys.LastWrite), Is.True);
        }
    }
}