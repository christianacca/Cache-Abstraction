// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Adds composite functions to a <see cref="ICache"/> instance
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Filters the <paramref name="caches"/> to those that implement the requested derived <see cref="ICache"/> 
        /// interface of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The derived <see cref="ICache"/> interface to filter on</typeparam>
        public static IEnumerable<T> CacheOfType<T>(this IEnumerable<ICache> caches) where T : class, ICache
        {
            return (caches ?? Enumerable.Empty<ICache>()).Select(c => c.As<T>()).Where(c => c != null).Distinct();
        }


        /// <summary>
        /// Extends the <paramref name="cache"/> with additional behaviours
        /// </summary>
        /// <returns>
        /// The original <paramref name="cache"/> wrapped in one or more <see cref="CacheDecorator"/> instances 
        /// </returns>
        public static ICache DecorateWith(this ICache cache, CacheDecoratorOptions options)
        {
            return new CacheDecoratorChainBuilder().AddDecorators(cache, options);
        }

        /// <summary>
        /// Returns an enumerable that iterates the decorators that are applied to <paramref name="cache"/>
        /// </summary>
        public static IEnumerable<ICache> GetDecoratorChain(this ICache cache)
        {
            return GetCacheChainIterator(cache, false);
        }


        /// <summary>
        /// Returns an enumerable that iterates the chain of cache decorators and optionally the decorated cache
        /// </summary>
        private static IEnumerable<ICache> GetCacheChainIterator(this ICache cache, bool includeDecorated)
        {
            var cacheDecorator = cache as CacheDecorator;
            if (cacheDecorator == null)
            {
                if (includeDecorated)
                {
                    yield return cache;
                }
                yield break;
            }

            yield return cacheDecorator;
            while (cacheDecorator.DecoratedCache is CacheDecorator)
            {
                cacheDecorator = (CacheDecorator) cacheDecorator.DecoratedCache;
                yield return cacheDecorator;
            }
            if (includeDecorated)
            {
                yield return cacheDecorator.DecoratedCache;
            }
        }

        /// <summary>
        /// Returns an enumerable that iterates the decorators that are applied to <paramref name="cache"/> along with
        /// the <paramref name="cache"/> itself
        /// </summary>
        public static IEnumerable<ICache> GetDecoratorChainAndDecorated(this ICache cache)
        {
            return GetCacheChainIterator(cache, true);
        }

        /// <summary>
        /// Returns the existing object in the cache associated with <paramref name="key"/>. If the 
        /// <paramref name="key"/> does not exist, the <paramref name="constructor"/> delegate will be executed 
        /// to create an instance of the object. This newly created object will be added to the cache and returned.
        /// </summary>
        /// <typeparam name="T">The type of the object that is cached</typeparam>
        /// <param name="cache">The cache store</param>
        /// <param name="key">The unique key that is associated with the object stored in cache</param>
        /// <param name="constructor">A delegate reponsible for creating the missing object</param>
        /// <param name="cachePolicy">
        ///     Optional cache policy that controls behaviour such as when the value returned <paramref name="constructor"/>
        ///     will be aged out of the cache
        /// </param>
        /// <remarks>
        /// <para>
        /// If <paramref name="cache"/> implements a method named GetOrAdd this will be executed in preference.
        /// </para>
        /// <para>
        /// The <paramref name="cache"/> may already be associated with a cache policy. In this case, any 
        /// <paramref name="cachePolicy"/> parameter supplied will override the cache policy defined for the
        /// <paramref name="cache"/>.
        /// </para>
        /// </remarks>
        public static T GetOrAdd<T>(this ICache cache, string key, Func<string, T> constructor,
                                    object cachePolicy = null)
        {
            if (cache == null) throw new ArgumentNullException("cache");

            // try method implemented by a protected cache decorator first as it can avoid constructor being called twice
            // when two threads experience a cache miss at the same time,
            // otherwise fallback to a sensible default implementation
            var protectedCache = cache.As<IMultiThreadProtectedCache>();
            if (protectedCache != null)
            {
                return protectedCache.GetOrAdd(key, constructor, cachePolicy);
            }
            else
            {
                return GetOrAddImpl(cache, key, constructor, cachePolicy);
            }
        }

        internal static T GetOrAddImpl<T>(ICache cache, string key, Func<string, T> constructor,
                                          object cachePolicy = null)
        {
            lock (cache.LockKey)
            {
                if (cache.Contains(key))
                {
                    return cache.GetData<T>(key);
                }
            }

            // Note: it's possible that another thread will have already added an item into the cache with our key
            // if this has happened we're going to discard what constructor returns and return what's in the cache 
            // - this mirrors the behaviour of ConcurrentDictionary
            // IMPORTANT: what we do NOT want to do is extend the lock above to include the call to constructor as we
            // have no way of knowing how long we would have to wait blocking access to the cache
            // If this is a problem for your use case, then use IMultiThreadProtectedCache which will guarantee that 
            // constructor function will only run once
            T newValue = constructor(key);
            lock (cache.LockKey)
            {
                if (cache.Contains(key))
                {
                    return cache.GetData<T>(key);
                }
                cache.AddOrUpdate(key, newValue, cachePolicy);
                return newValue;
            }
        }

        /// <summary>
        /// Determines whether the <paramref name="cache"/> instance is decorated with a cache decorator of type
        /// <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of decorator that is to be searched for</typeparam>
        /// <example>
        /// <code>
        /// cache.IsDecoratedWith&lt;IStatisticsCache&gt;()
        /// </code>
        /// </example>
        public static bool IsDecoratedWith<T>(this ICache cache) where T : class, ICache
        {
            return cache.GetDecoratorChain().CacheOfType<T>().Any();
        }

        /// <summary>
        /// Remove the cache items associated with the <paramref name="keys"/> supplied
        /// </summary>
        /// <remarks>
        /// By default the operation will lock the <paramref name="cache"/> instance so that the cache cannot be
        /// modified whilst in the processes of removing items
        /// </remarks>
        public static void RemoveRange(this ICache cache, IEnumerable<string> keys, bool lockCache = true)
        {
            var keysToRemove = new List<string>(keys);
            if (lockCache)
            {
                lock (cache.LockKey)
                {
                    foreach (string key in keysToRemove)
                    {
                        cache.Remove(key);
                    }
                }
            }
            else
            {
                foreach (string key in keysToRemove)
                {
                    cache.Remove(key);
                }
            }
        }

        /// <summary>
        /// Wraps the existing <paramref name="store"/> in a <see cref="SimpleInmemoryCache"/> so that it can be consumed as
        /// and extended as an <see cref="ICache"/> instance
        /// </summary>
        /// <remarks>
        /// Use the <paramref name="options"/> to define such things as the <see cref="ICache.InstanceName"/> and
        /// the extended behaviours that the cache instance should receive
        /// </remarks>
        public static ICache WrapCache(this ConcurrentDictionary<string, object> store,
                                       CacheCreationOptions options = null)
        {
            if (options != null)
            {
                var wrapped = new SimpleInmemoryCache(store, options.Id);
                return new CacheDecoratorChainBuilder().AddDecorators(wrapped, options.DecoratorOptions);
            }
            else
            {
                return new SimpleInmemoryCache(store);
            }
        }

        /// <summary>
        /// Wraps the <paramref name="store"/> in a <see cref="ObjectCacheWrapper"/> so that it can be consumed and
        /// extended as an <see cref="ICache"/> instance
        /// </summary>
        /// <remarks>
        /// <para>
        /// Use the <paramref name="options"/> to define such things as the <see cref="ICache.InstanceName"/> and
        /// the extended behaviours that the cache instance should receive
        /// </para>
        /// <para>
        /// Supply a <paramref name="cacheItemPolicySelector"/> that will return the <see cref="CacheItemPolicy"/>
        /// that should be associated with items added to the cache
        /// </para>
        /// </remarks>
        public static ICache WrapCache(this ObjectCache store,
                                       CacheCreationOptions options = null,
                                       Func<string, object, CacheItemPolicy> cacheItemPolicySelector = null)
        {
            if (options != null)
            {
                var wrapped = new ObjectCacheWrapper(store, options.InstanceName, cacheItemPolicySelector);
                return new CacheDecoratorChainBuilder().AddDecorators(wrapped, options.DecoratorOptions);
            }
            else
            {
                return new ObjectCacheWrapper(store, cacheItemPolicySelector: cacheItemPolicySelector);
            }
        }
    }
}