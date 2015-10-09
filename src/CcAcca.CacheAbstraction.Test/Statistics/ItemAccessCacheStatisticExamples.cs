// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Generic;
using System.Threading;
using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test.Statistics
{
    [TestFixture]
    public class ItemAccessCacheStatisticExamples : CacheStatisticExamplesBase
    {
        private IStatisticsCache _cache;


        #region Setup/Teardown

        [SetUp]
        public void TestInitialise()
        {
            _cache = CreateCacheWith(new ItemAccessCacheStatistic());
        }

        #endregion


        [Test]
        public void AddShouldRecordWriteTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.AddOrUpdate("key1", new object());

            Thread.Sleep(60);

            //then
            CacheItemAccessInfo itemStats =
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess)
                    ["key1"];
            AssertAccessTime(itemStats.LastWrite, expectedTime);
        }


        [Test]
        public void AddShouldNotRecordReadTime()
        {
            //when
            _cache.AddOrUpdate("key1", new object());

            //then
            CacheItemAccessInfo itemStats =
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess)
                    ["key1"];
            Assert.That(itemStats.LastRead, Is.Null);
        }


        [Test]
        public void GetCacheItemShouldRecordReadTime()
        {
            // given 
            _cache.AddOrUpdate("key1", new object());
            Thread.Sleep(60);

            // when
            _cache.GetCacheItem<object>("key1");
            DateTimeOffset expectedTime = DateTimeOffset.Now;

            // then
            CacheItemAccessInfo itemStats =
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess)
                    ["key1"];
            AssertAccessTime(itemStats.LastRead, expectedTime);
        }


        [Test]
        public void GetCacheItemShouldIncrementAccessCount()
        {
            // given 
            _cache.AddOrUpdate("key1", new object());

            // when
            _cache.GetCacheItem<object>("key1");
            _cache.GetCacheItem<object>("key1");

            // then
            CacheItemAccessInfo itemStats =
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess)
                    ["key1"];
            Assert.That(itemStats.ReadCount, Is.EqualTo(2));
        }


        [Test]
        public void AddOrUpdateShouldResetAccessCount()
        {
            // given 
            _cache.AddOrUpdate("key1", new object());
            _cache.GetCacheItem<object>("key1");
            _cache.GetCacheItem<object>("key1");

            // when
            _cache.AddOrUpdate("key1", new object());

            // then
            CacheItemAccessInfo itemStats =
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess)
                    ["key1"];
            Assert.That(itemStats.ReadCount, Is.EqualTo(0));
        }

        
        [Test]
        public void AddOrUpdateFactoryShouldResetAccessCount()
        {
            // given 
            _cache.AddOrUpdate("key1", new object());
            _cache.GetCacheItem<object>("key1");
            _cache.GetCacheItem<object>("key1");

            // when
            _cache.AddOrUpdate("key1", new object(), (k, v) => new object());

            // then
            CacheItemAccessInfo itemStats =
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess)
                    ["key1"];
            Assert.That(itemStats.ReadCount, Is.EqualTo(0));
        }


        [Test]
        public void AddOrUpdateShouldResetLastRead()
        {
            // given 
            _cache.AddOrUpdate("key1", new object());
            _cache.GetCacheItem<object>("key1");

            // when
            _cache.AddOrUpdate("key1", new object());

            // then
            CacheItemAccessInfo itemStats =
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess)
                    ["key1"];
            Assert.That(itemStats.LastRead, Is.Null);
        }

        
        [Test]
        public void AddOrUpdateFactoryShouldResetLastRead()
        {
            // given 
            _cache.AddOrUpdate("key1", new object());
            _cache.GetCacheItem<object>("key1");

            // when
            _cache.AddOrUpdate("key1", new object(), (k, v) => new object());

            // then
            CacheItemAccessInfo itemStats =
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess)
                    ["key1"];
            Assert.That(itemStats.LastRead, Is.Null);
        }


        [Test]
        public void GetCacheItemShouldSkipRecordingStatsForMissingItems()
        {
            // when
            _cache.GetCacheItem<object>("key1");

            // then
            var stats =
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess);
            Assert.That(stats, Is.Empty);
        }


        [Test]
        public void RemoveShouldRemoveItemStats()
        {
            // given
            _cache.AddOrUpdate("key1", new object());
            Assert.IsNotEmpty(
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess),
                "checking assumptions");

            // when
            _cache.Remove("key1");

            // then
            var stats =
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess);
            Assert.That(stats, Is.Empty);
        }

        [Test]
        public void FlushShouldRemoveAllItemStats()
        {
            // given
            _cache.AddOrUpdate("key1", new object());
            _cache.AddOrUpdate("key2", new object());
            Assert.IsNotEmpty(
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess),
                "checking assumptions");

            // when
            _cache.Flush();

            // then
            var stats =
                _cache.Statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess);
            Assert.That(stats, Is.Empty);
        }
    }
}