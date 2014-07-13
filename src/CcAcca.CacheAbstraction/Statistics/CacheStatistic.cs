using System;
using System.Threading;

namespace CcAcca.CacheAbstraction.Statistics
{
    public abstract class CacheStatistic : ICacheActivityRecorder
    {
        #region Member Variables

        private readonly string _name;

        #endregion


        #region Constructors

        protected CacheStatistic(string name)
        {
            _name = name;
        }

        #endregion


        #region Properties

        public abstract object CurrentValue { get; }

        public virtual string Name
        {
            get { return _name; }
        }

        #endregion


        #region ICacheActivityRecorder Members

        public virtual void ContainsCalled() {}
        public virtual void FlushCalled() {}
        public virtual void ItemAddOrUpdated(string key) {}
        public virtual void ItemRemoved(string key) {}

        public virtual void ItemRetrieved(string key) {}
        public virtual void ItemHit(string key) {}
        public virtual void ItemMiss(string key) {}

        #endregion
    }



    /// <summary>
    /// Convenience base class 
    /// </summary>
    public abstract class AccessTimeCacheStatisticBase : CacheStatistic
    {
        protected DateTimeOffset? _when;

        protected AccessTimeCacheStatisticBase(string name) : base(name) {}

        public override object CurrentValue
        {
            get { return _when; }
        }
    }


    public class CacheHitRatioCacheStatistic : CacheStatistic
    {
        public CacheHitRatioCacheStatistic()
            : base(CacheStatisticsKeys.CacheHitRatio)
        {
        }

        private int _hits;
        private int _misses;


        public override void ItemHit(string key)
        {
            Interlocked.Increment(ref _hits);
        }

        public override void ItemMiss(string key)
        {
            Interlocked.Increment(ref _misses);
        }

        public override void FlushCalled()
        {
            _hits = 0;
            _misses = 0;
        }

        public override object CurrentValue
        {
            get
            {
                if (Total == 0) return null;
                return Decimal.Round((decimal)_hits / (Total), 4);
            }
        }

        private int Total
        {
            get { return _hits + _misses; }
        }
    }


    public class LastFlushCacheStatistic : AccessTimeCacheStatisticBase
    {
        public LastFlushCacheStatistic()
            : base(CacheStatisticsKeys.LastFlush)
        {
        }

        public override void FlushCalled()
        {
            _when = DateTimeOffset.Now;
        }
    }


    /// <summary>
    /// Records when was the last time the cache was used to add, retrieve or search for an item in the cache
    /// </summary>
    public class LastUseCacheStatistic : AccessTimeCacheStatisticBase
    {
        public LastUseCacheStatistic() : base(CacheStatisticsKeys.LastUse) {}

        public override void ContainsCalled()
        {
            _when = DateTimeOffset.Now;
        }

        public override void FlushCalled()
        {
            _when = DateTimeOffset.Now;
        }

        public override void ItemAddOrUpdated(string key)
        {
            _when = DateTimeOffset.Now;
        }

        public override void ItemRetrieved(string key)
        {
            _when = DateTimeOffset.Now;
        }

        public override void ItemRemoved(string key)
        {
            _when = DateTimeOffset.Now;
        }
    }



    /// <summary>
    /// Records when was the last time the cache was used to add an item to the cache
    /// </summary>
    public class LastWrtieCacheStatistic : AccessTimeCacheStatisticBase
    {
        public LastWrtieCacheStatistic() : base(CacheStatisticsKeys.LastWrite) {}

        public override void ItemAddOrUpdated(string key)
        {
            _when = DateTimeOffset.Now;
        }

        public override void FlushCalled()
        {
            _when = null;
        }

        public override void ItemRemoved(string key)
        {
            _when = DateTimeOffset.Now;
        }
    }



    /// <summary>
    /// Records when was the last time the cache was used to retrieve an item from the cache
    /// </summary>
    public class LastReadCacheStatistic : AccessTimeCacheStatisticBase
    {
        public LastReadCacheStatistic() : base(CacheStatisticsKeys.LastRead) {}

        public override void ContainsCalled()
        {
            _when = DateTimeOffset.Now;
        }

        public override void FlushCalled()
        {
            _when = null;
        }

        public override void ItemRetrieved(string key)
        {
            _when = DateTimeOffset.Now;
        }
    }
}