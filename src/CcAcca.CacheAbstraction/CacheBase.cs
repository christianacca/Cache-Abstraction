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

        /// <summary>
        /// see <see cref="ICache.Id"/>
        /// </summary>
        public CacheIdentity Id { get; private set; }


        /// <summary>
        /// A qualifying prefix to be added to an item to disambiguate items
        /// with the same key stored in different logical partitions of the 
        /// same backing store
        /// </summary>
        protected string PartionKeyPrefix { get; set; }

        /// <summary>
        /// see <see cref="ICache.LockKey"/>
        /// </summary>
        public virtual object LockKey
        {
            get { return _lock; }
        }

        /// <summary>
        /// see <see cref="ICache.As{T}"/>
        /// </summary>
        public virtual T As<T>() where T : class, ICache
        {
            var self = this as T;
            return self;
        }

        /// <summary>
        /// Gets the key qualified by the name of the partition
        /// </summary>
        /// <param name="key">The non-qualified key.</param>
        protected virtual string GetFullKey(string key)
        {
            return string.Concat(PartionKeyPrefix, key);
        }
    }
}