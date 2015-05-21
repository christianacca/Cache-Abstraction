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
        private readonly object _lock = new object();

        protected CacheBase(CacheIdentity id)
        {
            if (id == null) throw new ArgumentNullException("id");

            Id = id;
            PartionKeyPrefix = String.Format("{0}:", Id);
        }

        public CacheIdentity Id { get; private set; }

        protected string PartionKeyPrefix { get; set; }

        public virtual object LockKey
        {
            get { return _lock; }
        }

        public virtual T As<T>() where T : class, ICache
        {
            return null;
        }

        /// <summary>
        /// Gets the key qualified by the name of the partition
        /// </summary>
        /// <param name="key">The non-qualified key.</param>
        protected string GetFullKey(string key)
        {
            return string.Concat(PartionKeyPrefix, key);
        }
    }
}