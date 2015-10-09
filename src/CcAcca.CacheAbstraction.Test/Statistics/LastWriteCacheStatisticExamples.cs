// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Threading;
using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test.Statistics
{
    [TestFixture]
    public class LastWriteCacheStatisticExamples : CacheStatisticExamplesBase
    {
        private IStatisticsCache _cache;


        #region Setup/Teardown

        [SetUp]
        public void TestInitialise()
        {
            _cache = CreateCacheWith(new LastWrtieCacheStatistic());
        }

        #endregion


        [Test]
        public void BeforeFirstReadShouldRecordNoTime()
        {
            AssertNoAccessTime(_cache.Statistics, CacheStatisticsKeys.LastWrite);
        }

        [Test]
        public void AddOrUpdateShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.AddOrUpdate("key1", new object());

            Thread.Sleep(60);

            //then
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastWrite, expectedTime);
        }

        
        [Test]
        public void AddOrUpdateFactoryShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.AddOrUpdate("key1", new object(), (k, v) => new object());

            Thread.Sleep(60);

            //then
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastWrite, expectedTime);
        }


        [Test]
        public void GetOrAdd_WhenItemNotAlreadyCached_ShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.GetOrAdd("key1", _ => new object());

            Thread.Sleep(60);

            //then
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastWrite, expectedTime);
        }


        [Test]
        public void GetOrAdd_WhenItemAlreadyCached_ShouldNotChangeTime()
        {
            //given
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.GetOrAdd("key1", _ => new object());
            Thread.Sleep(60);

            //when
            _cache.GetOrAdd("key1", _ => new object());

            //then
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastWrite, expectedTime);
        }

        [Test]
        public void ContainsShouldNotRecordTime()
        {
            //when
            _cache.Contains("key1");

            //then
            AssertNoAccessTime(_cache.Statistics, CacheStatisticsKeys.LastWrite);
        }


        [Test]
        public void GetCacheItemShouldNotRecordTime()
        {
            //when
            _cache.GetCacheItem<object>("key1");

            //then
            AssertNoAccessTime(_cache.Statistics, CacheStatisticsKeys.LastWrite);
        }

        
        [Test]
        public void GetDataShouldNotRecordTime()
        {
            //when
            _cache.GetData<object>("key1");

            //then
            AssertNoAccessTime(_cache.Statistics, CacheStatisticsKeys.LastWrite);
        }


        [Test]
        public void RemoveShouldRecordTime()
        {
            //given
            _cache.AddOrUpdate("key1", new object());
            Thread.Sleep(60);

            //when
            _cache.Remove("key1");

            //then
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastWrite, expectedTime);
        }


        [Test]
        public void WhenItemMissing_RemoveShouldStillRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.Remove("key1");

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastWrite, expectedTime);
        }

        [Test]
        public void FlushShouldResetToNoRecordedTime()
        {
            //given
            _cache.AddOrUpdate("key1", "whatever");

            //when
            _cache.Flush();

            //then
            AssertNoAccessTime(_cache.Statistics, CacheStatisticsKeys.LastWrite);
        }
    }
}