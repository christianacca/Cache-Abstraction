// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

namespace CcAcca.CacheAbstraction
{
    using System;

    /// <summary>
    /// Base class for creating concreate decorator classes that will add (decorate) additional caching
    /// behaviours to implementations of <see cref="ICache"/>
    /// </summary>
    public abstract class CacheDecorator : ICache
    {
        #region Constructors

        protected CacheDecorator(ICache decoratedCache)
        {
            DecoratedCache = decoratedCache;
        }

        #endregion


        #region Properties

        public ICache DecoratedCache { get; protected internal set; }

        #endregion


        #region ICache Members

        public virtual void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            DecoratedCache.AddOrUpdate(key, value, cachePolicy);
        }

        public virtual void AddOrUpdate<T>(string key, T addValue, Func<string, T, T> updateFactory, object cachePolicy = null)
        {
            DecoratedCache.AddOrUpdate(key, addValue, updateFactory, cachePolicy);
        }

        public virtual T As<T>() where T : class, ICache
        {
            var self = this as T;
            return self ?? DecoratedCache.As<T>();
        }


        public virtual bool Contains(string key)
        {
            return DecoratedCache.Contains(key);
        }


        public virtual int? Count
        {
            get { return DecoratedCache.Count; }
        }

        public virtual CacheIdentity Id
        {
            get { return DecoratedCache.Id; }
        }


        public virtual void Flush()
        {
            DecoratedCache.Flush();
        }

        
        public virtual CacheItem<T> GetCacheItem<T>(string key)
        {
            return DecoratedCache.GetCacheItem<T>(key);
        }


        public virtual object LockKey
        {
            get { return DecoratedCache.LockKey; }
        }


        public virtual ICache StartOfDecoratorChain
        {
            get
            {
                CacheDecorator cacheDecorator = this;
                while (cacheDecorator.DecoratedCache is CacheDecorator)
                {
                    cacheDecorator = (CacheDecorator) cacheDecorator.DecoratedCache;
                }
                return cacheDecorator.DecoratedCache;
            }
        }


        public virtual void Remove(string key)
        {
            DecoratedCache.Remove(key);
        }

        #endregion
    }
}