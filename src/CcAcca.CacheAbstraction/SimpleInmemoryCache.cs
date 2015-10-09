// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CcAcca.CacheAbstraction
{
    using System.CodeDom;

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


        #region Properties

        private IEnumerable<string> PartionedKeys
        {
            get { return _inmemoryCache.Keys.Where(k => k.StartsWith(PartionKeyPrefix)); }
        }

        #endregion


        #region ICache Members

        public virtual void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (cachePolicy != null)
            {
                throw new NotSupportedException("CachePolicy paramater not support by this ICache implementation");
            }

            lock (LockKey)
            {
                _inmemoryCache[GetFullKey(key)] = value;
            }
        }

        public virtual void AddOrUpdate<T>(string key, T addValue, Func<string, T, T> updateValueFactory, object cachePolicy = null)
        {
            if (addValue == null)
            {
                throw new ArgumentNullException("addValue");
            }
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException("updateValueFactory");
            }
            if (cachePolicy != null)
            {
                throw new NotSupportedException("CachePolicy paramater not support by this ICache implementation");
            }

            lock (LockKey)
            {
                _inmemoryCache.AddOrUpdate(GetFullKey(key), addValue, (k, existingValue) => AssertIsNotNull(updateValueFactory(key, (T)existingValue)));
            }
        }

        public virtual bool Contains(string key)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return _inmemoryCache.ContainsKey(GetFullKey(key));
        }


        public virtual int? Count
        {
            get { return PartionedKeys.Count(); }
        }


        public virtual void Flush()
        {
            lock (LockKey)
            {
                foreach (string key in PartionedKeys)
                {
                    object ignored;
                    _inmemoryCache.TryRemove(key, out ignored);
                }
            }
        }


        public virtual CacheItem<T> GetCacheItem<T>(string key)
        {
            object item;
            // ReSharper disable once InconsistentlySynchronizedField
            bool found = _inmemoryCache.TryGetValue(GetFullKey(key), out item);
            return found ? new CacheItem<T>((T) item) : null;
        }


        public virtual void Remove(string key)
        {
            string fullKey = GetFullKey(key);
            lock (LockKey)
            {
                object whatever;
                _inmemoryCache.TryRemove(fullKey, out whatever);
            }
        }

        #endregion

        private static T AssertIsNotNull<T>(T value)
        {
            if (!ReferenceEquals(value, null))
            {
                return value;
            }
            throw new ArgumentNullException("value");
        }


        private static string CreateUniqueCacheName()
        {
            return string.Format("SimpleInmemoryCache-{0}", Guid.NewGuid());
        }
    }
}