// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Extends a <see cref="ICache"/> <see cref="CacheExtensions.GetOrAdd{T}"/> with an implementation
    /// that is guaranteed to not execute the value producing function twice when two threads receive a
    /// cache miss at the same time.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default implementation (<see cref="MultiThreadProtectedDecorator"/>) will NOT use locking 
    /// to synchronize access to the cache and therefore will be highly scalable.
    /// </para>
    /// <para>
    /// However, by not maintaining a lock there is the risk that an exception raised by the value producing function 
    /// will be thrown on another thread. This will happen when a second thread reads the key about to be added precisely
    /// after the first thread has confirmed that the key does not exist, but before it has run the function
    /// that produces the value to be added. The second thread will end up running the value producing function
    /// and will receive any exception raised.
    /// </para>
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