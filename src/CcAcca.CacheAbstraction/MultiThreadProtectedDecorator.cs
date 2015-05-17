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
            base.AddOrUpdate(key, new Lazy<T>(() => value), cachePolicy);
        }

        public override CacheItem<T> GetCacheItem<T>(string key)
        {
            CacheItem<Lazy<T>> item = base.GetCacheItem<Lazy<T>>(key);
            if (item == null) return null;

            return new CacheItem<T>(item.Value.Value);
        }

        public T GetOrAdd<T>(string key, Func<string, T> constructor, object cachePolicy = null)
        {
            var wrappedCtor = new Lazy<T>(() => constructor(key));
            Lazy<T> result = CacheExtensions.GetOrAddImpl(DecoratedCache, key, _ => wrappedCtor, cachePolicy);
            return result.Value;
        }
    }
}