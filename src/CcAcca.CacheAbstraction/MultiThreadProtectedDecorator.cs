// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;

namespace CcAcca.CacheAbstraction
{
    /// <summary>
    /// See <see cref="IMultiThreadProtectedCache"/>
    /// </summary>
    public class MultiThreadProtectedDecorator : CacheDecorator, IMultiThreadProtectedCache
    {
        public MultiThreadProtectedDecorator(ICache decoratedCache) : base(decoratedCache) {}

        public override void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            base.AddOrUpdate(key, new Lazy<object>(() => value), cachePolicy);
        }

        public override void AddOrUpdate<T>(string key, T addValue, Func<string, T, T> updateValueFactory, object cachePolicy = null)
        {
            base.AddOrUpdate<object>(key,
                addValue,
                (k, v) => {
                    var lazyValue = v as Lazy<object>;
                    return updateValueFactory(k, (T)(lazyValue == null ? v : lazyValue.Value));
                }, 
                cachePolicy);
        }

        public override CacheItem<T> GetCacheItem<T>(string key)
        {
            var rawItem = base.GetCacheItem<object>(key);
            if (rawItem == null)
            {
                return null;
            }

            var lazyValue = rawItem.Value as Lazy<object>;
            return lazyValue == null ? new CacheItem<T>((T)rawItem.Value) : new CacheItem<T>((T)lazyValue.Value);
        }

        public virtual T GetOrAdd<T>(string key, Func<string, T> constructor, object cachePolicy = null)
        {
            var wrappedCtor = new Lazy<object>(() => {
                var value = constructor(key);
                if (value == null)
                {
                    // ReSharper disable once NotResolvedInText
                    throw new ArgumentNullException("value");
                }
                return value;
            });
            var rawValue = CacheExtensions.GetOrAddImpl<object>(DecoratedCache, key, _ => wrappedCtor, cachePolicy);
            var lazyValue = rawValue as Lazy<object>;
            return (T)(lazyValue == null ? rawValue : lazyValue.Value);
        }
    }
}