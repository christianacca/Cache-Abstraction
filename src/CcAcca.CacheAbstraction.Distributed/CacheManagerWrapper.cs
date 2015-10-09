// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using CacheManager.Core;

namespace CcAcca.CacheAbstraction.Distributed
{
    /// <summary>
    /// Adapts the ICacheManager from the https://github.com/MichaCo/CacheManager library to match
    /// <see cref="ICache"/>
    /// </summary>
    /// <remarks>
    /// ICacheManager implementations add support for various distributed caches such as Redis an Memcached
    /// </remarks>
    public class CacheManagerWrapper : CacheBase, ICache
    {
        public CacheManagerWrapper(CacheIdentity id, ICacheManager<object> impl) : base(id)
        {
            Impl = impl;
        }

        private ICacheManager<object> Impl { get; set; }

        public virtual int? Count
        {
            get { return null; }
        }

        public virtual void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            this.Impl.Put(key, value, this.Id.ToString());
        }

        public virtual bool Contains(string key)
        {
            return GetCacheItem<object>(key) != null;
        }

        public virtual void Flush()
        {
            Impl.ClearRegion(Id.ToString());
        }

        public virtual CacheItem<T> GetCacheItem<T>(string key)
        {
            var implItem = Impl.GetCacheItem(key, Id.ToString());
            if (implItem == null) return null;

            return new CacheItem<T>((T)implItem.Value);
        }

        public virtual void Remove(string key)
        {
            Impl.Remove(key, Id.ToString());
        }
    }
}