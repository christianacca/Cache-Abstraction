// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test.Statistics
{
    [TestFixture]
    public class CachHitRatioCacheStatisticExamples : CacheStatisticExamplesBase
    {
        private IStatisticsCache _cache;


        [SetUp]
        public void TestInitialise()
        {
            _cache = CreateCacheWith(new CacheHitRatioCacheStatistic());
        }


        [Test]
        public void ShouldBeNullBeforeCacheUsed()
        {
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.Null);
        }


        [Test]
        public void GetData_Miss()
        {
            // when
            _cache.GetData<object>("key1");

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.EqualTo(0m));
        }


        [Test]
        public void GetData_Hit()
        {
            // given
            _cache.AddOrUpdate("key1", new object());

            // when
            _cache.GetData<object>("key1");

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.EqualTo(1m));
        }


        [Test]
        public void Contains_Miss()
        {
            // when
            _cache.Contains("key1");

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.EqualTo(0m));
        }


        [Test]
        public void Contains_Hit()
        {
            // given
            _cache.AddOrUpdate("key1", new object());

            // when
            _cache.Contains("key1");

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.EqualTo(1m));
        }


        [Test]
        public void GetData_MixtureOfHitsAndMisses()
        {
            // given, when
            _cache.AddOrUpdate("key1", new object());
            _cache.GetData<object>("key1"); // hit
            _cache.GetData<object>("key1"); // hit
            _cache.GetData<object>("key2"); // miss

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.EqualTo(0.6667m));
        }


        [Test]
        public void Contains_MixtureOfHitsAndMisses()
        {
            // given, when
            _cache.AddOrUpdate("key1", new object());
            _cache.Contains("key1"); // hit
            _cache.Contains("key1"); // hit
            _cache.Contains("key2"); // miss

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.EqualTo(0.6667m));
        }


        [Test]
        public void GetData_Contains_MixtureOfHitsAndMisses()
        {
            // given, when
            _cache.AddOrUpdate("key1", new object());
            _cache.GetData<object>("key1"); // hit
            _cache.GetData<object>("key1"); // hit
            _cache.GetData<object>("key2"); // miss
            _cache.Contains("key1"); // hit
            _cache.Contains("key1"); // hit
            _cache.Contains("key2"); // miss

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.EqualTo(0.6667m));
        }

        [Test]
        public void Remove_ShouldNotInfluenceResult()
        {
            // given
            _cache.AddOrUpdate("key1", new object());
            _cache.GetData<object>("key1");
            _cache.GetData<object>("key2");

            // when
            _cache.Remove("key1");

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.EqualTo(0.5m));
        }

        [Test]
        public void GetOrAdd_Miss()
        {
            // when
            _cache.GetOrAdd("key1", _ => new object());

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.EqualTo(0m));
        }


        [Test]
        public void GetOrAdd_Hit()
        {
            // given
            _cache.AddOrUpdate("key1", new object());

            // when
            _cache.GetOrAdd("key1", _ => new object());

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.EqualTo(1m));
        }

        [Test]
        public void GetOrAdd_MixOfHitsAndMisses()
        {
            // given
            _cache.AddOrUpdate("key1", new object());

            // when
            _cache.GetOrAdd("key1", _ => new object()); // hit
            _cache.GetOrAdd("key1", _ => new object()); // hit
            _cache.GetOrAdd("key2", _ => new object()); // miss

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.EqualTo(0.6667m));
        }

        [Test]
        public void FlushShouldResetToZero()
        {
            // given
            _cache.AddOrUpdate("key1", new object());
            _cache.GetOrAdd("key1", _ => new object());

            // when
            _cache.Flush();

            // then
            var ratio = _cache.Statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio);
            Assert.That(ratio, Is.Null);
        }
    }
}