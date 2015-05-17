// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Threading;
using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test.Statistics
{
    [TestFixture]
    public class LastReadCacheStatisticExamples : CacheStatisticExamplesBase
    {
        private IStatisticsCache _cache;


        [SetUp]
        public void TestInitialise()
        {
            _cache = CreateCacheWith(new LastReadCacheStatistic());
        }


        [Test]
        public void BeforeFirstReadShouldRecordNoTime()
        {
            AssertNoAccessTime(_cache.Statistics, CacheStatisticsKeys.LastRead);
        }


        [Test]
        public void GetCacheItemShouldRecordTime()
        {
            //given
            _cache.AddOrUpdate("key1", "whatever");

            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.GetCacheItem<object>("key1");

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastRead, expectedTime);
        }


        [Test]
        public void WhenItemMissing_GetCacheItemShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.GetCacheItem<object>("key1");

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastRead, expectedTime);
        }


        [Test]
        public void GetOrAddShouldRecordTime()
        {
            //given
            _cache.GetOrAdd("key1", _ => new object());

            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.GetOrAdd("key1", _ => new object());

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastRead, expectedTime);
        }


        [Test]
        [Description("Even though item is ultimately added, the cache is first checked whether it contains item")]
        public void WhenItemMissing_GetOrAddShouldStillRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.GetOrAdd("key1", _ => new object());

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastRead, expectedTime);
        }


        [Test]
        public void ContainsShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.Contains("key1");

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastRead, expectedTime);
        }


        [Test]
        public void FlushShouldResetToNoRecordedTime()
        {
            //given
            _cache.AddOrUpdate("key1", "whatever");
            _cache.GetCacheItem<object>("key1");

            //when
            _cache.Flush();

            //then
            AssertNoAccessTime(_cache.Statistics, CacheStatisticsKeys.LastRead);
        }
    }
}