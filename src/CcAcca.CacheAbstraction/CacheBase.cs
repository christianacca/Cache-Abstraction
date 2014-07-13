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
        private readonly object _locker = new object();

        protected CacheBase(CacheIdentity id)
        {
            if (id == null) throw new ArgumentNullException("id");

            Id = id;
        }

        public virtual object LockKey
        {
            get { return _locker; }
        }

        public CacheIdentity Id { get; private set; }

        public virtual string InstanceName { get; set; }

        public virtual T As<T>() where T : class, ICache
        {
            return null;
        }
    }
}