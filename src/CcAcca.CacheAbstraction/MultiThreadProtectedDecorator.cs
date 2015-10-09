// Copyright (c) 2014 Christian Crowhurst.  All rights reserved.
// see LICENSE

using System;

namespace CcAcca.CacheAbstraction
{
    public class MultiThreadProtectedDecorator : CacheDecorator, IMultiThreadProtectedCache
    {
        public MultiThreadProtectedDecorator(ICache decoratedCache) : base(decoratedCache) {}

        public override void AddOrUpdate<T>(string key, T value, object cachePolicy = null)
        {
            base.AddOrUpdate(key, new Lazy<object>(() => value), cachePolicy);
        }

        public override void AddOrUpdate<T>(string key, T addValue, Func<string, T, T> updateFactory, object cachePolicy = null)
        {
            base.AddOrUpdate(key, new Lazy<object>(() => addValue), (k, existingValue) => new Lazy<object>(() => updateFactory(k, (T)existingValue.Value)), cachePolicy);
        }

        public override CacheItem<T> GetCacheItem<T>(string key)
        {
            CacheItem<Lazy<object>> item = base.GetCacheItem<Lazy<object>>(key);
            if (item == null) return null;

            return new CacheItem<T>((T)item.Value.Value);
        }

        public virtual T GetOrAdd<T>(string key, Func<string, T> constructor, object cachePolicy = null)
        {
            var wrappedCtor = new Lazy<object>(() => constructor(key));
            Lazy<object> result = CacheExtensions.GetOrAddImpl(DecoratedCache, key, _ => wrappedCtor, cachePolicy);
            return (T)result.Value;
        }
    }
}