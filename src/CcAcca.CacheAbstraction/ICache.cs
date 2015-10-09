// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;
using CcAcca.CacheAbstraction.Statistics;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Defines the main behaviour of a cache
    /// </summary>
    /// <remarks>
    /// <para>
    /// For additional caching functionality that can be added to a cache, see concreate implementations of <see
    /// cref="CacheDecorator"/> such as
    /// <see cref="StatisticsDecorator"/>
    /// </para>
    /// <para>
    /// This interface has been kept lean. For additionaly API see the extension methods
    /// that target this interface.
    /// </para>
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


        /// <summary>
        /// When supported by the cache implementation, returns the number of items currently in the cache
        /// </summary>
        /// <remarks>
        /// <para>
        /// An implemention that does not want to support this property should return null
        /// </para>
        /// <para>
        /// Where one or more <see cref="ICache"/> instances share a backing store this property should
        /// return only those items added to this instance
        /// </para>
        /// </remarks>
        int? Count { get; }

        /// <summary>
        /// Checks the cache for an existing item associated with the value of the <paramref name="key"/> supplied, if no
        /// item found adds it otherwise overwrites the entry with the <paramref name="value"/>
        /// </summary>
        /// <param name="key">The key to associate with the <paramref name="value"/></param>
        /// <param name="value">The value to add to the cache</param>
        /// <param name="cachePolicy">
        /// Optional cache policy that controls behaviour such as when the item will be aged out of the cache
        /// </param>
        /// <remarks>
        /// The cache may already be associated with a cache policy. In this case, any <paramref name="cachePolicy"/>
        /// parameter supplied will override the cache policy defined for the cache
        /// </remarks>
        /// <exception cref="ArgumentNullException">When <paramref name="key"/> or <paramref name="value"/> is null</exception>
        void AddOrUpdate<T>(string key, T value, object cachePolicy = null);

        /// <summary>
        /// Adds a key/value pair to the <see cref="ICache"/> if the key does not already exist, 
        /// or to update a key/value pair in the <see cref="ICache"/> by using the specified function if the key already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value is to be updated</param>
        /// <param name="addValue">The value to be added for an absent key</param>
        /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value</param>
        /// <param name="cachePolicy">
        /// Optional cache policy that controls behaviour such as when the item will be aged out of the cache
        /// </param>
        /// <remarks>
        /// The cache may already be associated with a cache policy. In this case, any <paramref name="cachePolicy"/>
        /// parameter supplied will override the cache policy defined for the cache
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// When <paramref name="key"/> or <paramref name="updateValueFactory"/> is null or <paramref name="updateValueFactory"/> 
        /// returns a null</exception>
        void AddOrUpdate<T>(string key, T addValue, Func<string, T, T> updateValueFactory, object cachePolicy = null);

        /// <summary>
        /// Returns true when an item is found matching the <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
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