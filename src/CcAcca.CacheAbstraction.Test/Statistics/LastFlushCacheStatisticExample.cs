// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Threading;
using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test.Statistics
{
    [TestFixture]
    public class LastFlushCacheStatisticExample : CacheStatisticExamplesBase
    {
        private IStatisticsCache _cache;


        [SetUp]
        public void TestInitialise()
        {
            _cache = CreateCacheWith(new LastFlushCacheStatistic());
        }


        [Test]
        public void BeforeFirstFlushShouldRecordNoTime()
        {
            AssertNoAccessTime(_cache.Statistics, CacheStatisticsKeys.LastFlush);
        }


        [Test]
        public void FlushShouldRecordTime()
        {
            //when
            DateTimeOffset expectedTime = DateTimeOffset.Now;
            _cache.Flush();

            //then
            Thread.Sleep(60);
            AssertAccessTime(_cache.Statistics, CacheStatisticsKeys.LastFlush, expectedTime);
        }
    }
}