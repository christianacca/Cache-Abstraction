using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CcAcca.CacheAbstraction.Statistics
{
    public class CacheStatistics : ICacheActivityRecorder
    {
        #region Member Variables

        private readonly IDictionary<string, CacheStatistic> _statistics = new Dictionary<string, CacheStatistic>();

        #endregion


        #region Constructors

        public CacheStatistics(IEnumerable<CacheStatistic> statistics)
        {
            foreach (CacheStatistic statistic in statistics)
            {
                _statistics.Add(statistic.Name, statistic);
            }
        }

        #endregion


        #region ICacheActivityRecorder Members

        void ICacheActivityRecorder.ContainsCalled()
        {
            Record(s => s.ContainsCalled());
        }


        void ICacheActivityRecorder.FlushCalled()
        {
            Record(s => s.FlushCalled());
        }


        void ICacheActivityRecorder.ItemAddOrUpdated(string key)
        {
            Record(s => s.ItemAddOrUpdated(key));
        }

        void ICacheActivityRecorder.ItemRemoved(string key)
        {
            Record(s => s.ItemRemoved(key));
        }


        void ICacheActivityRecorder.ItemRetrieved(string key)
        {
            Record(s => s.ItemRetrieved(key));
        }


        void ICacheActivityRecorder.ItemMiss(string key)
        {
            Record(s => s.ItemMiss(key));
        }

        
        void ICacheActivityRecorder.ItemHit(string key)
        {
            Record(s => s.ItemHit(key));
        }

        #endregion


        public bool IsEmpty
        {
            get { return _statistics.Count == 0; }
        }

        public T GetValue<T>(string statiticsName)
        {
            return (T) _statistics[statiticsName].CurrentValue;
        }


        public bool HasStatictic(string statisticName)
        {
            return _statistics.ContainsKey(statisticName);
        }


        private void Record(Action<CacheStatistic> methodCalled)
        {
            foreach (CacheStatistic statistic in _statistics.Values)
            {
                methodCalled(statistic);
            }
        }


        public T SafeGetValue<T>(string statiticsName)
        {
            if (!_statistics.ContainsKey(statiticsName)) return default(T);

            return (T) _statistics[statiticsName].CurrentValue;
        }


        #region Class Members

        private static readonly ConcurrentDictionary<Func<CacheStatistic>, object> AllStatisticsFactory = new ConcurrentDictionary<Func<CacheStatistic>, object>();
        private static readonly object IgnoredValue = null;


        static CacheStatistics()
        {
            Empty = new CacheStatistics(new List<CacheStatistic>());
            RegisterStatisticFactory(() => new LastWrtieCacheStatistic(),
                              () => new LastReadCacheStatistic(),
                              () => new LastUseCacheStatistic(),
                              () => new LastFlushCacheStatistic(),
                              () => new CacheHitRatioCacheStatistic(),
                              () => new ItemAccessCacheStatistic());

        }

        public static void RegisterStatisticFactory(params Func<CacheStatistic>[] statistics)
        {
            foreach (var statistic in statistics)
            {
                AllStatisticsFactory.TryAdd(statistic, IgnoredValue);
            }
        }


        public static CacheStatistics Empty { get; private set; }
        
        
        public static CacheStatistics All
        {
            get
            {
                var stats = AllStatisticsFactory.Keys.Select(f => f());
                return new CacheStatistics(stats);
            }
        }



        #endregion
    }



    public class CacheStatisticsKeys
    {
        #region Member Variables

        public const string CacheHitRatio = "CacheHitRatioStatistic";
        public const string ItemAccess = "ItemAccessStatistic";
        public const string LastFlush = "LastFlushStatistic";
        public const string LastRead = "LastReadStatistic";
        public const string LastUse = "LastUseStatistic";
        public const string LastWrite = "LastWrtieStatistic";

        #endregion
    }
}