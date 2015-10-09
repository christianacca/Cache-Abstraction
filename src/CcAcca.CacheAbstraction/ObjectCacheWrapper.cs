// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Linq;
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

        private readonly ObjectCache _impl;
        private readonly Func<string, object, CacheItemPolicy> _cacheItemPolicySelector;
        private readonly string _partitionKey;

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

            _impl = impl;
            _cacheItemPolicySelector = cacheItemPolicySelector ?? DefaultCachePolicySelector;
            _partitionKey = String.Format("CachePartition:{0}", Id);
        }

        #endregion


        private void AddPartitionToken()
        {
            // add our partition as an item so that we can associate the partition as a dependency to items that get added.
            // That way when the partition item gets changed this will automatically clean its associated items
            var policy = new CacheItemPolicy
            {
                Priority = CacheItemPriority.Default,
                AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration,
                SlidingExpiration = ObjectCache.NoSlidingExpiration
            };
            Impl.Add(_partitionKey, Guid.NewGuid(), policy);
        }

        private CacheItemPolicy GetItemPolicy(string itemKey, object itemValue, CacheItemPolicy selectedPolicy)
        {
            // this should be the only place to create the partition token if it doesn't exist it
            // might have been removed by Flush but next time AddOrUpdate gets called, the partition token
            // should be re added...
            if (!Impl.Contains(_partitionKey))
            {
                AddPartitionToken();
            }

            selectedPolicy = selectedPolicy ?? CacheItemPolicySelector(itemKey, itemValue);

            return new CacheItemPolicy
            {
                Priority = selectedPolicy.Priority,
                ChangeMonitors = { Impl.CreateCacheEntryChangeMonitor(new[] { _partitionKey }) },
                AbsoluteExpiration = selectedPolicy.AbsoluteExpiration,
                SlidingExpiration = selectedPolicy.SlidingExpiration,
                RemovedCallback = selectedPolicy.RemovedCallback,
                UpdateCallback = selectedPolicy.UpdateCallback
            };
        }



        #region Properties

        public virtual Func<string, object, CacheItemPolicy> CacheItemPolicySelector
        {
            get { return _cacheItemPolicySelector; }
        }


        private ObjectCache Impl
        {
            get { return _impl; }
        }


        #endregion



        #region ICache Members

        public virtual void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            CacheItemPolicy itemPolicy = GetItemPolicy(key, value, (CacheItemPolicy)cachePolicy);
            lock (LockKey)
            {
                Impl.Set(GetFullKey(key), value, itemPolicy);
            }
        }


        /// <remarks>
        /// <para>
        /// WARNING: this implementation of <see cref="ICache.AddOrUpdate{T}(string,T,System.Func{string,T,T},object)"/> 
        /// will lock the cache whilst <paramref name="updateValueFactory"/> runs.
        /// This will be terrible for scalability if <paramref name="updateValueFactory"/> is not fast as requests for items 
        /// to this cache will have to wait.
        /// </para>
        /// <para>
        /// Instead consider using <see cref="SimpleInmemoryCache"/> or other implementations of <see cref="ICache"/>
        /// that does not suffer from the same locking problem
        /// </para>
        /// </remarks>
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

            string fullItemKey = GetFullKey(key);
            lock (this.LockKey)
            {
                object existingValue = Impl.AddOrGetExisting(fullItemKey, addValue, GetItemPolicy(key, addValue, (CacheItemPolicy)cachePolicy));
                if (existingValue != null)
                {
                    T updateValue = updateValueFactory(key, (T)existingValue);
                    Impl.Set(fullItemKey, updateValue, GetItemPolicy(key, updateValue, (CacheItemPolicy)cachePolicy));
                }
            }
        }

        public virtual bool Contains(string key)
        {
            return Impl.Contains(GetFullKey(key));
        }


        public virtual int? Count
        {
            get { return Impl.Count(x => x.Key.StartsWith(PartionKeyPrefix)); }
        }


        public virtual void Flush()
        {
            lock (LockKey)
            {
                if (Impl.Contains(_partitionKey))
                {
                    // triggers the removal of all items associated with the PartitionKey
                    Impl[_partitionKey] = Guid.NewGuid();
                }
            }
        }


        public virtual CacheItem<T> GetCacheItem<T>(string key)
        {
            object existingValue = Impl.Get(GetFullKey(key));
            if (ReferenceEquals(existingValue, null))
            {
                return null;
            }

            return new CacheItem<T>((T)existingValue);
        }


        public virtual void Remove(string key)
        {
            lock (LockKey)
            {
                Impl.Remove(GetFullKey(key));
            }
        }

        #endregion


        private static Func<string, object, CacheItemPolicy> _defaultCachePolicySelector;

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