// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Threading;
using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test.Statistics
{
    [TestFixture]
    public class LastUseCacheStatisticExamples : CacheStatisticExamplesBase
    {
        private IStatisticsCache _cache;


        #region Setup/Teardown

        [SetUp]
        public void TestInitialise()
        {
            _cache = CreateCacheWith(new LastUseCacheStatistic());
        }

        #endregion


        [Test]
        public void BeforeFirstReadShouldRecordNoTime()
        {
            AssertNoAccessTime(_cache.Statistics, CacheStatisticsKeys.LastUse);
        }


        [Test]
        public void AddOrUpdateShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.AddOrUpdate("key1", new object());

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastUse, expectedTime);
        }

        
        [Test]
        public void AddOrUpdateFactoryShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.AddOrUpdate("key1", new object(), (k, v) => new object());

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastUse, expectedTime);
        }


        [Test]
        public void GetCacheItemShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.GetCacheItem<object>("key1");

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastUse, expectedTime);
        }

        
        [Test]
        public void GetDataShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.GetData<object>("key1");

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastUse, expectedTime);
        }


        [Test]
        public void GetOrAddShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.AddOrUpdate("key1", new object());

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastUse, expectedTime);
        }

        [Test]
        public void FlushShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.Flush();

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastUse, expectedTime);
        }


        [Test]
        public void ContainsShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.Contains("key1");

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastUse, expectedTime);
        }


        [Test]
        public void RemoveShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.Remove("key1");

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastUse, expectedTime);
        }


        [Test]
        public void FlushShouldRecordedTime()
        {
            //given
            _cache.AddOrUpdate("key1", "whatever");
            _cache.GetCacheItem<object>("key1");
            Thread.Sleep(60);

            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.Flush();

            //then
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastUse, expectedTime);
        }
    }
}