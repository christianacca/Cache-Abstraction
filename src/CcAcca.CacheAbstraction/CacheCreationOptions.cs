// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// Defines the properties and behaviours that an instance of a <see cref="ICache"/> should be created with
    /// </summary>
    public class CacheCreationOptions
    {
        private CacheDecoratorOptions _decoratorOptions;

        /// <summary>
        /// Sets the <see cref="CacheIdentity.Name"/> of the cache instance to be created
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Id of the cache derived from the <see cref="Name"/> and <see cref="InstanceName"/>
        /// </summary>
        public CacheIdentity Id
        {
            get { return Name == null ? null : new CacheIdentity(Name, InstanceName); }
        }

        /// <summary>
        /// Sets the <see cref="CacheIdentity.InstanceName"/> of the cache instance to be created
        /// </summary>
        public virtual string InstanceName { get; set; }

        /// <summary>
        /// The extended behaviours that the cache instance should receive
        /// </summary>
        public virtual CacheDecoratorOptions DecoratorOptions
        {
            get { return _decoratorOptions ?? CacheDecoratorOptions.None; }
            set { _decoratorOptions = value; }
        }

        /// <summary>
        /// Convenience method for defining the creation options for a cache
        /// </summary>
        /// <remarks>
        /// Equivalent of defining the options to create a cache with a <paramref name="cacheId"/> and extended with the 
        /// <see cref="CacheDecoratorOptions.Default"/> set of cache decorators
        /// </remarks>
        public static CacheCreationOptions DefaultsWith(CacheIdentity cacheId)
        {
            return new CacheCreationOptions
                {
                    Name = cacheId.Name,
                    InstanceName = cacheId.InstanceName,
                    DecoratorOptions = CacheDecoratorOptions.Default
                };
        }
    }
}