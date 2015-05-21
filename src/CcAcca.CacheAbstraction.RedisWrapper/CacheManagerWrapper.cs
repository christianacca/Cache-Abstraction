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

        public int Count
        {
            get { return 0; }
        }

        public void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            object putValue = ReferenceEquals(null, value) ? (object) _nullInstance : value;
            Impl.Put(key, putValue);
        }

        public bool Contains(string key)
        {
            return Impl.GetCacheItem(key) != null;
        }

        public void Flush()
        {
            Impl.Clear();
        }

        public CacheItem<T> GetCacheItem<T>(string key)
        {
            var implItem = Impl.GetCacheItem(key);
            if (implItem == null) return null;
            if (Equals(_nullInstance, implItem.Value))
            {
                return new CacheItem<T>(default(T));
            }

            return new CacheItem<T>((T)implItem.Value);
        }

        public object LockKey
        {
            get { return Impl; }
        }

        public void Remove(string key)
        {
            Impl.Remove(key);
        }

        private static readonly string _nullInstance = "{1C0017F4-01E3-4FB5-B1F6-4004D28F08B8}";
    }
}