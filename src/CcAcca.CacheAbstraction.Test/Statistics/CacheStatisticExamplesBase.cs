// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using CcAcca.CacheAbstraction.Statistics;
using NUnit.Framework;

namespace CcAcca.CacheAbstraction.Test.Statistics
{
    public class CacheStatisticExamplesBase
    {
        protected ICache _backingStore;


        protected IStatisticsCache CreateCacheWith(params CacheStatistic[] statistics)
        {
            _backingStore = new ObjectCacheWrapper();
            return new StatisticsDecorator(statistics, _backingStore);
        }


        protected void AssertAccessTime(CacheStatistics statistics, string statisticName, DateTimeOffset expected)
        {
            var actual = statistics.SafeGetValue<DateTimeOffset?>(statisticName);
            AssertAccessTime(actual, expected);
        }

        protected void AssertNoAccessTime(CacheStatistics statistics, string statisticName)
        {
            var actual = statistics.SafeGetValue<DateTimeOffset?>(statisticName);
            Assert.That(actual, Is.Null);
        }

        protected void AssertAccessTime(DateTimeOffset? actual, DateTimeOffset expected)
        {
            Assert.That(actual, Is.Not.Null, "no time recorded");
            Assert.That(actual, Is.Not.EqualTo(DateTime.MinValue), "no time recorded");

            Assert.That(actual,
                        Is.GreaterThanOrEqualTo(expected.AddMilliseconds(-30)).And
                          .LessThanOrEqualTo(expected.AddMilliseconds(+30)),
                        "incorrect recording");
        }
    }
}