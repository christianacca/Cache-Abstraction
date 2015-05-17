// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Runtime.Caching;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Wraps a <see cref="ObjectCache"/> and has a <see cref="CacheItemPolicySelector"/> associated with it that
    /// means that the clients of ObjectCache do not need to know or supply the caching rules themselves
    /// </summary>
    public class ObjectCacheWrapper : CacheBase, ICache
    {
        #region Member Variables

        private ObjectCache _impl;
        private readonly Func<string, object, CacheItemPolicy> _cacheItemPolicySelector;

        #endregion


        #region Constructors

        public ObjectCacheWrapper(CacheIdentity id = null,
                                  Func<string, object, CacheItemPolicy> cacheItemPolicySelector = null)
            : this(
                new MemoryCache(id != null ? id.Name : CreateUniqueCacheName()),
                instanceName: (id != null ? id.InstanceName : null),
                cacheItemPolicySelector: cacheItemPolicySelector) {}


        public ObjectCacheWrapper(ObjectCache impl, string instanceName = null,
                                  Func<string, object, CacheItemPolicy> cacheItemPolicySelector = null)
            : base(new CacheIdentity(impl != null ? impl.Name : null, instanceName))
        {
            if (impl == null) throw new ArgumentNullException("impl");

            if (impl is MemoryCache)
            {
                FlushImplementation = cache => new MemoryCache(impl.Name);
            }

            _impl = impl;
            _cacheItemPolicySelector = cacheItemPolicySelector ?? DefaultCachePolicySelector;
        }

        #endregion


        #region Properties

        public Func<string, object, CacheItemPolicy> CacheItemPolicySelector
        {
            get { return _cacheItemPolicySelector; }
        }

        public Func<ObjectCache, ObjectCache> FlushImplementation { get; set; }

        public ObjectCache Impl
        {
            get { return _impl; }
        }

        #endregion


        #region ICache Members

        public void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            lock (LockKey)
            {
                if (ReferenceEquals(null, value))
                {
                    Impl.Set(key,
                             _nullInstance,
                             (CacheItemPolicy) cachePolicy ?? CacheItemPolicySelector(key, _nullInstance));
                }
                else
                {
                    Impl.Set(key, value, (CacheItemPolicy) cachePolicy ?? CacheItemPolicySelector(key, value));
                }
            }
        }


        public bool Contains(string key)
        {
            return Impl.Contains(key);
        }


        public int Count
        {
            get { return (int) Impl.GetCount(); }
        }


        public void Flush()
        {
            if (FlushImplementation == null)
            {
                throw new InvalidOperationException(
                    "Implementation for the Flush method has not been supplied. Have you set FlushImplementation?");
            }
            lock (LockKey)
            {
                _impl = FlushImplementation(_impl);
            }
        }


        public virtual CacheItem<T> GetCacheItem<T>(string key)
        {
            object existingValue = Impl.Get(key);
            if (ReferenceEquals(existingValue, null))
            {
                return null;
            }

            return existingValue == _nullInstance ? new CacheItem<T>(default(T)) : new CacheItem<T>((T) existingValue);
        }


        public object LockKey
        {
            get {  return Impl;}
        }

        public void Remove(string key)
        {
            lock (LockKey)
            {
                Impl.Remove(key);
            }
        }

        #endregion


        private static Func<string, object, CacheItemPolicy> _defaultCachePolicySelector;
        private static readonly object _nullInstance = new object();

        /// <summary>
        /// By default never expire items
        /// </summary>
        public static Func<string, object, CacheItemPolicy> DefaultCachePolicySelector
        {
            get
            {
                return _defaultCachePolicySelector ??
                       ((key, value) => new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.MaxValue});
            }
            set { _defaultCachePolicySelector = value; }
        }

        private static string CreateUniqueCacheName()
        {
            return string.Format("ObjectCacheWrapper-{0}", Guid.NewGuid());
        }
    }
}