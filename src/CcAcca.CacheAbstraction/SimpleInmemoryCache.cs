// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Concurrent;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// A cache for those situations where all you need is an inmemory cache and you are happy to control when
    /// items are expired by explicitly calling <see cref="Flush"/>
    /// </summary>
    public class SimpleInmemoryCache : CacheBase, ICache
    {
        #region Member Variables

        private readonly ConcurrentDictionary<string, object> _inmemoryCache;

        #endregion


        #region Constructors

        public SimpleInmemoryCache(CacheIdentity id = null) : this(new ConcurrentDictionary<string, object>(), id) {}


        /// <summary>
        /// Creates an instance from a pre-existing <see cref="ConcurrentDictionary{TKey,TValue}"/>
        /// </summary>
        public SimpleInmemoryCache(ConcurrentDictionary<string, object> cache, CacheIdentity id = null)
            : base(id ?? CreateUniqueCacheName())
        {
            if (cache == null) throw new ArgumentNullException("cache");

            _inmemoryCache = cache;
        }

        #endregion


        #region ICache Members

        public void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            if (cachePolicy != null)
            {
                throw new NotSupportedException("CachePolicy paramater not support by this ICache implementation");
            }

            lock (LockKey)
            {
                _inmemoryCache[key] = value;
            }
        }


        public bool Contains(string key)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _inmemoryCache.ContainsKey(key);
        }


        public int Count
        {
            get { return _inmemoryCache.Count; }
        }


        public void Flush()
        {
            lock (LockKey)
            {
                _inmemoryCache.Clear();
            }
        }


        public virtual CacheItem<T> GetCacheItem<T>(string key)
        {
            object item;
            // ReSharper disable once InconsistentlySynchronizedField
            bool found = _inmemoryCache.TryGetValue(key, out item);
            return found ? new CacheItem<T>((T) item) : null;
        }


        public object LockKey
        {
            get { return _inmemoryCache; }
        }

        public void Remove(string key)
        {
            lock (LockKey)
            {
                object whatever;
                _inmemoryCache.TryRemove(key, out whatever);
            }
        }

        #endregion


        private static string CreateUniqueCacheName()
        {
            return string.Format("SimpleInmemoryCache-{0}", Guid.NewGuid());
        }
    }
}