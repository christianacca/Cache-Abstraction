// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Represents a cache that can be paused and resumed
    /// </summary>
    public interface IPausableCache : ICache
    {
        bool IsPaused { get; set; }
    }
}