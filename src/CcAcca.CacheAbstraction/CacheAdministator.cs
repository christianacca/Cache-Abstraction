// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CcAcca.CacheAbstraction.Statistics;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Registry and management of long lived caches within an application
    /// </summary>
    public class CacheAdministator
    {
        #region Member Variables

        private readonly ConcurrentDictionary<ICache, bool> _allCaches = new ConcurrentDictionary<ICache, bool>();
        private readonly Func<CacheStatistics> _cacheStatisticsFactory;

        #endregion


        public CacheAdministator() : this(() => CacheStatistics.All) {}

        public CacheAdministator(Func<CacheStatistics> cacheStatisticsFactory)
        {
            _cacheStatisticsFactory = cacheStatisticsFactory;
        }


        #region Properties

        public virtual IEnumerable<CacheInfo> AllDefaultCacheInfos
        {
            get
            {
                return AllCaches.Select(ToCacheInfo)
                                .OrderBy(info => info.CacheId)
                                .ThenByDescending(info => info.LastUse);
            }
        }

        private CacheInfo ToCacheInfo(ICache cache)
        {
            var statisticsCache = cache.As<IStatisticsCache>();
            CacheStatistics statistics = statisticsCache != null ? statisticsCache.Statistics : CacheStatistics.Empty;
            var pausableCache = cache.As<IPausableCache>();
            IDictionary<string, CacheItemAccessInfo> itemStatsDictionary =
                statistics.SafeGetValue<IDictionary<string, CacheItemAccessInfo>>(CacheStatisticsKeys.ItemAccess) ??
                new Dictionary<string, CacheItemAccessInfo>();
            ICollection<CacheItemAccessInfo> itemStats = itemStatsDictionary.Values;

            return new CacheInfo
                {
                    CacheId = cache.Id.ToString(),
                    CacheName = cache.Id.Name,
                    InstanceName = cache.Id.InstanceName,
                    IsPaused = pausableCache == null || pausableCache.IsPaused,
                    IsPausable = pausableCache != null,
                    ItemCount = cache.Count,
                    LastRead = statistics.SafeGetValue<DateTimeOffset?>(CacheStatisticsKeys.LastRead),
                    LastFlush = statistics.SafeGetValue<DateTimeOffset?>(CacheStatisticsKeys.LastFlush),
                    LastUse = statistics.SafeGetValue<DateTimeOffset?>(CacheStatisticsKeys.LastUse),
                    LastWrite = statistics.SafeGetValue<DateTimeOffset?>(CacheStatisticsKeys.LastWrite),
                    CacheHitRatio = statistics.SafeGetValue<decimal?>(CacheStatisticsKeys.CacheHitRatio),
                    ItemAccessStatistics = itemStats
                };
        }

        public virtual IEnumerable<ICache> AllCaches
        {
            get { return _allCaches.Keys; }
        }


        private IEnumerable<IPausableCache> PausableCaches
        {
            get { return _allCaches.Keys.CacheOfType<IPausableCache>(); }
        }

        #endregion


        public virtual bool Contains(ICache cache)
        {
            return _allCaches.ContainsKey(cache);
        }

        private ICache DecorateWithStatistics(ICache cache)
        {
            var builder = new CacheDecoratorChainBuilder();
            cache = builder.AddDecorators(cache,
                                          new CacheDecoratorOptions
                                              {
                                                  IsStatisticsOn = true,
                                                  Statistics = _cacheStatisticsFactory()
                                              });
            return cache;
        }

        public virtual void PauseAllSPausableCaches()
        {
            foreach (IPausableCache cache in PausableCaches)
            {
                cache.IsPaused = true;
            }
        }


        public virtual void StartAllPauableCaches()
        {
            foreach (IPausableCache cache in PausableCaches)
            {
                cache.IsPaused = false;
            }
        }


        public virtual void FlushAll()
        {
            foreach (ICache cache in _allCaches.Keys)
            {
                cache.Flush();
            }
        }

        private bool SetIsPaused(CacheIdentity cacheId, bool isPaused)
        {
            IPausableCache cache = PausableCaches.SingleOrDefault(c => c.Id == cacheId);
            if (cache == null) return false;

            cache.IsPaused = isPaused;
            return true;
        }

        /// <summary>
        /// Starts the cache
        /// </summary>
        /// <param name="cacheId">The unique id of the instance to start</param>
        /// <returns>
        /// True if a <em>pausable</em> cache was found matching the name supplied
        /// </returns>
        /// <remarks>
        /// <c>true</c> will be returned even if the cache is already started
        /// </remarks>
        public virtual bool StartCache(CacheIdentity cacheId)
        {
            return SetIsPaused(cacheId, false);
        }

        /// <summary>
        /// Pauses the cache
        /// </summary>
        /// <param name="cacheId">The unique id of the instance to pause</param>
        /// <returns>
        /// True if a <em>pausable</em> cache was found matching the name supplied
        /// </returns>
        /// <remarks>
        /// <c>true</c> will be returned even if the cache is already paused
        /// </remarks>
        public virtual bool PauseCache(CacheIdentity cacheId)
        {
            return SetIsPaused(cacheId, true);
        }


        /// <summary>
        /// Register the <paramref name="cache"/>
        /// </summary>
        /// <exception cref="ArgumentException">
        /// <returns>The <paramref name="cache"/> extended with <see cref="IStatisticsCache"/> behaviour</returns>
        /// Where a cache with the same <see cref="ICache.Id"/> has already been registered</exception>
        /// <remarks>
        /// Any cache registered will be extended with the <see cref="IStatisticsCache"/> behaviour if not already
        /// </remarks>
        public virtual ICache Register(ICache cache)
        {
            if (cache == null) throw new ArgumentNullException("cache");

            //don't add special case null instances
            if (cache.As<INullCache>() != null) return cache;

            if (AllCaches.Any(c => c.Id == cache.Id))
            {
                throw new ArgumentException(string.Format("A cache has already been registered with the Id '{0}'",
                                                          cache.Id));
            }
            bool hasExistingStatisticsDecorator = cache.IsDecoratedWith<IStatisticsCache>();
            cache = DecorateWithStatistics(cache);
            _allCaches[cache] = !hasExistingStatisticsDecorator;
            return cache;
        }


        /// <summary>
        /// Unregisters the <paramref name="cache"/> from this administator
        /// </summary>
        /// <returns>
        /// The <paramref name="cache"/> minus <see cref="IStatisticsCache"/> behaviour previously added by this administrator
        /// </returns>
        /// <remarks>
        /// Any <see cref="IStatisticsCache"/> behaviour added to the <paramref name="cache"/> when it was registered
        /// with this administrator will be removed
        /// </remarks>
        public virtual ICache Unregister(ICache cache)
        {
            bool removeStatistics;
            bool found = _allCaches.TryRemove(cache, out removeStatistics);
            if (found && removeStatistics)
            {
                var chainBuilder = new CacheDecoratorChainBuilder();
                return chainBuilder.RemoveDecorators(cache, new CacheDecoratorOptions {IsStatisticsOn = false});
            }
            else
            {
                return cache;
            }
        }

        /// <summary>
        /// Deletes the item from the cache identified by it's <see cref="ICache.Id" />
        /// </summary>
        /// <param name="cacheId">The unique id of the cache to remove item from </param>
        /// <param name="key">The key associated with the cached item to be removed</param>
        /// <returns>True if a cache was found matching the id supplied</returns>
        /// <remarks>
        /// <c>true</c> will be returned even if the item was not in the cache
        /// </remarks>
        /// <exception cref="ArgumentException">Cache is paused</exception>
        public bool RemoveItem(CacheIdentity cacheId, string key)
        {
            ICache cache = AllCaches.SingleOrDefault(c => c.Id == cacheId);
            if (cache == null) return false;

            if (cache.As<IPausableCache>() != null && cache.As<IPausableCache>().IsPaused)
            {
                throw new ArgumentException(string.Format("Cannot remove item from paused cache ('{0}')", cacheId));
            }

            cache.Remove(key);
            return true;
        }
    }
}