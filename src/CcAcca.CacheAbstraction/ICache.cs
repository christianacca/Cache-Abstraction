// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using CcAcca.CacheAbstraction.Statistics;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Defines the main beahiour of a cache
    /// </summary>
    /// <remarks>
    /// For additional caching functionality that can be added to a cache, see concreate implementations of <see
    /// cref="CacheDecorator"/> such as
    /// <see cref="StatisticsDecorator"/>
    /// </remarks>
    /// <seealso cref="CacheExtensions"/>
    public interface ICache
    {
        /// <summary>
        /// Returns a reference to a derived <see cref="ICache"/> interface that this instance may implement, otherwise
        /// returns null
        /// </summary>
        /// <typeparam name="T">The derived <see cref="ICache"/> interface reference requested</typeparam>
        T As<T>() where T : class, ICache;

        int Count { get; }

        /// <summary>
        /// Checks the cache for an existing item associated with the value of the <paramref name="key"/> supplied, if no
        /// item found adds it otherwise overwrites the entry with the <paramref name="value"/>
        /// </summary>
        /// <param name="key">The key to associate with the <paramref name="value"/></param>
        /// <param name="value">The value to add to the cache</param>
        /// <param name="cachePolicy">
        /// Optional cache policy that controls behaviour such as when the value returned <paramref name="value"/>
        /// will be aged out of the cache
        /// </param>
        /// <remarks>
        /// The cache may already be associated with a cache policy. In this case, any <paramref name="cachePolicy"/>
        /// parameter supplied will override the cache policy defined for the cache
        /// </remarks>
        void AddOrUpdate<T>(string key, T value, object cachePolicy = null);

        bool Contains(string key);

        /// <summary>
        /// Clears the cache
        /// </summary>
        void Flush();

        /// <summary>
        /// Return the item associated with the <paramref name="key"/> supplied or null if not found
        /// </summary>
        CacheItem<T> GetCacheItem<T>(string key);

        /// <summary>
        /// Returns an object that this instance will use to lock the cache items during updates
        /// </summary>
        object LockKey { get; }

        /// <summary>
        /// The unique identify for this instance
        /// </summary>
        CacheIdentity Id { get; }

        /// <summary>
        /// Remove the cache item associated with the key
        /// </summary>
        /// <remarks>
        /// This is no-op when an item is not found associated with the key
        /// </remarks>
        void Remove(string key);
    }
}