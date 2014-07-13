using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CcAcca.CacheAbstraction.Statistics
{
    public class ItemAccessCacheStatistic : CacheStatistic
    {
        private readonly ConcurrentDictionary<string, CacheItemAccessInfo> _itemStats = new ConcurrentDictionary<string, CacheItemAccessInfo>(); 
        public ItemAccessCacheStatistic() : base(CacheStatisticsKeys.ItemAccess)
        {
        }

        public override void ItemAddOrUpdated(string key)
        {
            CacheItemAccessInfo itemStats = _itemStats.GetOrAdd(key, k => new CacheItemAccessInfo {Key = k, LastWrite = DateTimeOffset.Now});
            itemStats.ReadCount = 0;
            itemStats.LastRead = null;
        }

        public override void FlushCalled()
        {
            _itemStats.Clear();
        }

        public override void ItemRetrieved(string key)
        {
            CacheItemAccessInfo info;
            if (_itemStats.TryGetValue(key, out info))
            {
                info.LastRead = DateTimeOffset.Now;
                info.IncrementReadCount();
            }
        }

        public override void ItemRemoved(string key)
        {
            CacheItemAccessInfo ignore;
            _itemStats.TryRemove(key, out ignore);
        }

        public override object CurrentValue
        {
            get { return new Dictionary<string, CacheItemAccessInfo>(_itemStats); }
        }
    }
}