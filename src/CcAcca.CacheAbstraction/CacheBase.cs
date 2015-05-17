// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Convenience base class that provide some base functionality for implementors of the <see cref="ICache" />
    /// interface
    /// </summary>
    public abstract class CacheBase
    {
        protected CacheBase(CacheIdentity id)
        {
            if (id == null) throw new ArgumentNullException("id");

            Id = id;
        }

        public CacheIdentity Id { get; private set; }

        public virtual string InstanceName { get; set; }

        public virtual T As<T>() where T : class, ICache
        {
            return null;
        }
    }
}