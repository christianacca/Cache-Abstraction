// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Creates <see cref="ICache"/> instances
    /// </summary>
    /// <remarks>
    /// Consult concreate implementation to see what additional behaviours will be provided when an instance of a cache
    /// is requested. For example <see cref="GlobalCacheProvider"/> will register caches it creates with the
    /// <see cref="CacheAdministator"/> that it has been configured with
    /// </remarks>
    public interface ICacheProvider
    {
        /// <summary>
        /// Returns a cache appropriate for the type parameter <typeparamref name="T"/> and <paramref name="cacheId"/> supplied
        /// </summary>
        /// <remarks>
        /// Multiple requests for the same <paramref name="cacheId"/> will return just one cache instance
        /// </remarks>
        ICache Get<T>(CacheIdentity cacheId) where T : class;
    }
}