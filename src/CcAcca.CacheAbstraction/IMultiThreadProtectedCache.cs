// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Extends a <see cref="ICache"/> with a robust multi-thread safe implementation of 
    /// <see cref="CacheExtensions.GetOrAdd{T}"/> functionality
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="CacheExtensions.GetOrAdd{T}"/> extension method, this implementation will not execute the 
    /// constructor function (a delegate of type  <see cref="Func{TResult}"/>) twice when two threads receive a cache miss 
    /// at the same time. This might be critical when the calling the constructor function is costly
    /// </remarks>
    public interface IMultiThreadProtectedCache : ICache
    {
        /// <summary>
        /// Returns the existing object in the cache associated with <paramref name="key"/>. If the 
        /// <paramref name="key"/> does not exist, the <paramref name="constructor"/> delegate will be executed 
        /// to create an instance of the object. This newly created object will be added to the cache and returned.
        /// </summary>
        /// <typeparam name="T">The type of the object that is cached</typeparam>
        /// <param name="key">The unique key that is associated with the object stored in cache</param>
        /// <param name="constructor">A delegate reponsible for creating the missing object</param>
        /// <param name="cachePolicy">
        /// Optional cache policy that controls behaviour such as when the value returned <paramref name="constructor"/>
        /// will be aged out of the cache
        /// </param>
        T GetOrAdd<T>(string key, Func<string, T> constructor, object cachePolicy = null);
    }
}