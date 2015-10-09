using System;
using System.Collections.Generic;

namespace CcAcca.CacheAbstraction.Statistics
{
    public class StatisticsDecorator : CacheDecorator, IStatisticsCache
    {
        #region Member Variables

        private readonly ICacheActivityRecorder _statistics;

        #endregion


        #region Constructors

        public StatisticsDecorator(IEnumerable<CacheStatistic> statistics, ICache cache) 
            : this(new CacheStatistics(statistics), cache)
        {
        }

        public StatisticsDecorator(CacheStatistics statistics, ICache cache)
            : base(cache)
        {
            if (statistics == null) throw new ArgumentNullException("statistics");

            _statistics = statistics;
        }

        #endregion


        #region Properties

        public virtual CacheStatistics Statistics
        {
            get { return (CacheStatistics) _statistics; }
        }

        #endregion


        public override void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            _statistics.ItemAddOrUpdated(key);
            base.AddOrUpdate(key, value, cachePolicy);
        }

        public override void AddOrUpdate<T>(string key, T addValue, Func<string, T, T> updateFactory, object cachePolicy = null)
        {
            _statistics.ItemAddOrUpdated(key);
            base.AddOrUpdate(key, addValue, updateFactory, cachePolicy);
        }


        public override bool Contains(string key)
        {
            _statistics.ContainsCalled();
            bool found = base.Contains(key);
            if (found)
            {
                _statistics.ItemHit(key);
            }
            else
            {
                _statistics.ItemMiss(key);
            }
            return found;
        }


        public override void Flush()
        {
            _statistics.FlushCalled();
            base.Flush();
        }


        public override CacheItem<T> GetCacheItem<T>(string key)
        {
            _statistics.ItemRetrieved(key);
            CacheItem<T> item = base.GetCacheItem<T>(key);
            if (item == null)
            {
                _statistics.ItemMiss(key);
            }
            else
            {
                _statistics.ItemHit(key);
            }
            return item;
        }

        public override void Remove(string key)
        {
            _statistics.ItemRemoved(key);
            base.Remove(key);
        }
    }
}